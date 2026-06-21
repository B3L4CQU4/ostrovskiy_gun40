using TacticalPrototype.Controllers.Commands;
using TacticalPrototype.Controllers.Rules;
using TacticalPrototype.Units;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TacticalPrototype.Controllers
{
    [DisallowMultipleComponent]
    public sealed class BattleController : MonoBehaviour
    {
        private static BattleController instance;

        [SerializeField] private Battlefield battlefield;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private GameModeSelectionView gameModeSelectionView;
        [SerializeField] private HudView hudView;
        [SerializeField] private PromotionChoiceView promotionChoiceView;
        [SerializeField, Min(0.1f)] private float restartHoldDuration = 1f;

        private TacticalGameplayCommand gameplayCommand;
        private float restartHoldTimer;
        private bool restartRequested;

        public TurnPhase Phase { get; private set; }

        public static void InstanceSelectCell(Cell cell)
        {
            if (instance != null)
            {
                instance.SelectCell(cell);
            }
        }

        private void Awake()
        {
            instance = this;
            Phase = TurnPhase.WaitingForMode;

            if (battlefield == null)
            {
                battlefield = FindObjectOfType<Battlefield>();
            }

            if (playerController == null)
            {
                playerController = FindObjectOfType<PlayerController>();
            }

            gameplayCommand = new TacticalGameplayCommand(this, battlefield, playerController);
        }

        private void Start()
        {
            battlefield.Initialize();

            if (gameModeSelectionView != null)
            {
                gameModeSelectionView.Show(this);
            }

            if (promotionChoiceView != null)
            {
                promotionChoiceView.Hide();
            }

            UpdateHud(GameKind.None, Team.None);
            SetStatus("Choose game mode.");
        }

        private void Update()
        {
            HandleRestartShortcut();

            if (restartRequested)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                gameplayCommand.Cancel();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                gameplayCommand.Confirm();
            }
        }

        private void HandleRestartShortcut()
        {
            if (!Input.GetKey(KeyCode.Tab))
            {
                restartHoldTimer = 0f;
                return;
            }

            restartHoldTimer += Time.unscaledDeltaTime;

            if (restartHoldTimer >= restartHoldDuration)
            {
                RestartLevel();
            }
        }

        private void RestartLevel()
        {
            if (restartRequested)
            {
                return;
            }

            restartRequested = true;
            SetStatus("Restarting level...");
            Debug.Log("Restarting level by holding Tab.");

            Scene activeScene = SceneManager.GetActiveScene();
            if (activeScene.buildIndex >= 0)
            {
                SceneManager.LoadScene(activeScene.buildIndex);
                return;
            }

            SceneManager.LoadScene(activeScene.name);
        }

        public void StartCheckers()
        {
            StartGame(GameKind.Checkers);
        }

        public void StartChess()
        {
            StartGame(GameKind.Chess);
        }

        public void StartGame(GameKind gameKind)
        {
            battlefield.ActivateGame(gameKind);

            if (gameModeSelectionView != null)
            {
                gameModeSelectionView.Hide();
            }

            IRuleset ruleset = gameKind == GameKind.Checkers
                ? (IRuleset)new CheckersRuleset()
                : new ChessRuleset();

            gameplayCommand.Configure(ruleset, Team.White);
        }

        public void SelectCell(Cell cell)
        {
            gameplayCommand.Interact(cell);
        }

        public void SetPhase(TurnPhase phase)
        {
            Phase = phase;
        }

        public void UpdateHud(GameKind gameKind, Team activeTeam)
        {
            if (hudView != null)
            {
                hudView.SetTurn(gameKind, activeTeam, Phase);
            }
        }

        public void SetStatus(string status)
        {
            if (hudView != null)
            {
                hudView.SetStatus(status);
            }
        }

        public void RequestPromotionChoice(System.Action<UnitType> onChoice)
        {
            if (promotionChoiceView == null)
            {
                onChoice.Invoke(UnitType.Queen);
                return;
            }

            promotionChoiceView.Show(onChoice);
        }
    }
}
