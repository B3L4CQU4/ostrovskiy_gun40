using System.Collections.Generic;
using TacticalPrototype.Controllers.Rules;
using TacticalPrototype.Units;
using UnityEngine;

namespace TacticalPrototype.Controllers.Commands
{
    public sealed class TacticalGameplayCommand : IGameplayCommand
    {
        private readonly BattleController battleController;
        private readonly Battlefield battlefield;
        private readonly PlayerController playerController;

        private IRuleset ruleset;
        private Team activeTeam = Team.White;
        private Unit selectedUnit;
        private Unit forcedUnit;
        private MoveOption pendingMove;
        private List<MoveOption> selectedMoves = new List<MoveOption>();

        public TacticalGameplayCommand(BattleController battleController, Battlefield battlefield, PlayerController playerController)
        {
            this.battleController = battleController;
            this.battlefield = battlefield;
            this.playerController = playerController;
        }

        public Team ActiveTeam { get { return activeTeam; } }

        public void Configure(IRuleset valueRuleset, Team firstTeam)
        {
            ruleset = valueRuleset;
            activeTeam = firstTeam;
            selectedUnit = null;
            forcedUnit = null;
            pendingMove = null;
            selectedMoves.Clear();
            battleController.SetPhase(TurnPhase.WaitingForSelection);
            UpdateHud();
            RefreshForcedHighlights();
        }

        public void Interact(Cell cell)
        {
            if (cell == null || ruleset == null || playerController.IsBusy)
            {
                return;
            }

            TurnPhase phase = battleController.Phase;
            if (phase == TurnPhase.WaitingForMode || phase == TurnPhase.Animating || phase == TurnPhase.PromotionChoice)
            {
                return;
            }

            MoveOption destinationMove = FindMoveByDestination(cell);
            if (destinationMove != null)
            {
                if (pendingMove == destinationMove)
                {
                    Confirm();
                    return;
                }

                pendingMove = destinationMove;
                cell.SetHighlight(destinationMove.IsCapture ? CellHighlight.Attack : CellHighlight.Move);
                battleController.SetStatus("Destination selected: " + BoardState.CoordToName(cell.Coordinate) + ". Press Space to confirm.");
                return;
            }

            Unit unit = cell.CurrentUnit;
            if (unit != null && unit.Team == activeTeam && (forcedUnit == null || forcedUnit == unit))
            {
                SelectUnit(unit);
                return;
            }

            if (selectedUnit == null)
            {
                battleController.SetStatus("Select a " + activeTeam + " unit.");
            }
        }

        public void Cancel()
        {
            if (playerController.IsBusy || battleController.Phase == TurnPhase.PromotionChoice)
            {
                return;
            }

            pendingMove = null;

            if (forcedUnit != null)
            {
                SelectUnit(forcedUnit);
                battleController.SetStatus("Capture chain is locked. Choose the next capture.");
                return;
            }

            selectedUnit = null;
            selectedMoves.Clear();
            battlefield.ClearHighlights();
            battleController.SetPhase(TurnPhase.WaitingForSelection);
            UpdateHud();
            RefreshForcedHighlights();
        }

        public void Confirm()
        {
            if (pendingMove == null || playerController.IsBusy)
            {
                return;
            }

            MoveOption move = pendingMove;
            pendingMove = null;
            battlefield.ClearHighlights();
            battleController.SetPhase(TurnPhase.Animating);

            if (move.RequiresPromotionChoice)
            {
                battleController.SetPhase(TurnPhase.PromotionChoice);
                battleController.RequestPromotionChoice(delegate(UnitType choice)
                {
                    ExecuteMove(move, choice == UnitType.None ? UnitType.Queen : choice);
                });
                return;
            }

            ExecuteMove(move, UnitType.None);
        }

        private void SelectUnit(Unit unit)
        {
            BoardState state = battlefield.CreateState();
            IReadOnlyList<MoveOption> legalMoves = GetSelectableMovesForUnit(state, unit);

            if (legalMoves.Count == 0)
            {
                battleController.SetStatus("Selected unit has no legal moves.");
                return;
            }

            selectedUnit = unit;
            selectedMoves = new List<MoveOption>(legalMoves);
            pendingMove = null;
            battlefield.HighlightSelection(selectedUnit, selectedMoves);
            battleController.SetPhase(TurnPhase.WaitingForDestination);
            battleController.SetStatus("Choose destination for " + activeTeam + " " + unit.UnitType + ".");
            UpdateHud();
        }

        private IReadOnlyList<MoveOption> GetSelectableMovesForUnit(BoardState state, Unit unit)
        {
            if (forcedUnit != null)
            {
                return ruleset.GetLegalMovesForUnit(state, unit, activeTeam, forcedUnit);
            }

            if (ruleset.GameKind != GameKind.Checkers)
            {
                return ruleset.GetLegalMovesForUnit(state, unit, activeTeam, null);
            }

            IReadOnlyList<MoveOption> teamMoves = ruleset.GetLegalMoves(state, activeTeam, null);
            List<MoveOption> filtered = new List<MoveOption>();

            foreach (MoveOption move in teamMoves)
            {
                if (move.Unit == unit)
                {
                    filtered.Add(move);
                }
            }

            return filtered;
        }

        private MoveOption FindMoveByDestination(Cell destination)
        {
            foreach (MoveOption move in selectedMoves)
            {
                if (move.MatchesDestination(destination))
                {
                    return move;
                }
            }

            return null;
        }

        private void ExecuteMove(MoveOption move, UnitType promotionChoice)
        {
            battleController.SetPhase(TurnPhase.Animating);
            playerController.PlayMove(move, promotionChoice, delegate
            {
                battlefield.RebuildOccupants();
                BoardState state = battlefield.CreateState();
                ruleset.OnMoveApplied(state, move, promotionChoice);

                if (ruleset.GameKind == GameKind.Checkers && move.IsCapture)
                {
                    IReadOnlyList<MoveOption> nextCaptures = ruleset.GetLegalMovesForUnit(battlefield.CreateState(), move.Unit, activeTeam, move.Unit);
                    if (nextCaptures.Count > 0)
                    {
                        forcedUnit = move.Unit;
                        selectedUnit = move.Unit;
                        selectedMoves = new List<MoveOption>(nextCaptures);
                        battlefield.HighlightSelection(selectedUnit, selectedMoves);
                        battleController.SetPhase(TurnPhase.WaitingForDestination);
                        battleController.SetStatus("Continue capture with the same checker.");
                        UpdateHud();
                        return;
                    }
                }

                forcedUnit = null;
                selectedUnit = null;
                selectedMoves.Clear();
                activeTeam = BoardState.Opponent(activeTeam);
                battleController.SetPhase(TurnPhase.WaitingForSelection);
                UpdateHud();
                RefreshForcedHighlights();
            });
        }

        private void RefreshForcedHighlights()
        {
            if (ruleset == null)
            {
                return;
            }

            IReadOnlyList<MoveOption> teamMoves = ruleset.GetLegalMoves(battlefield.CreateState(), activeTeam, null);
            bool hasCaptures = false;
            foreach (MoveOption move in teamMoves)
            {
                if (move.IsCapture)
                {
                    hasCaptures = true;
                    break;
                }
            }

            if (hasCaptures && ruleset.GameKind == GameKind.Checkers)
            {
                battlefield.HighlightForcedUnits(teamMoves);
                battleController.SetStatus(activeTeam + " must capture.");
            }
            else
            {
                battleController.SetStatus(activeTeam + " turn. Select a unit.");
            }
        }

        private void UpdateHud()
        {
            battleController.UpdateHud(ruleset != null ? ruleset.GameKind : GameKind.None, activeTeam);
        }
    }
}
