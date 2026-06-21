# Пошаговая тактика

Unity prototype for an 8x8 turn-based tactics board with two selectable rulesets: checkers and chess.

## Сдача

- Branch: `Tactics`
- Repository URL: fill in before submission.
- Реализованная команда: шашки и шахматы.
- Паттерны: Command (`IGameplayCommand`), Strategy (`IRuleset`), MVC-style split between rules, controllers and scene views.
- Сторонние ассеты: In-game Debug Console `v1.8.7` (`Assets/Plugins/IngameDebugConsole`). Игровой визуал собран из Unity primitives/materials.

## Управление

- Mouse left click: select unit/cell and choose destination.
- Space: confirm selected move.
- Esc: cancel selection. During forced checkers capture chain, cancel keeps the forced capture state.
- Hold Tab for 1 second: restart level.
- BackQuote: toggle in-game debug console.

## Scene Authoring

Main scene: `Assets/Scenes/TacticalPrototype.unity`.

The board, cells, pieces, UI and debug console are editable scene objects/prefab instances. Runtime code does not generate the board or starting pieces; it only validates references, activates the selected ruleset root and links existing `Cell`/`Unit` objects.
