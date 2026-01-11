using System;

namespace CasinoGame.Core {
    public class GameLoop {
        private readonly Casino _casino;
        private bool _isRunning = true;

        public GameLoop(Casino casino) {
            _casino = casino;
        }

        public void Run() {
            // без этой проверки при старте с 0 после вызова HandleGameOver 
            // и выбора "no" выводится хелп и игра не завершается корректно 
            if (_casino.ExitRequested) {
                Exit();
                return;
            }
            PrintHelp();

            while (_isRunning) {
                Console.WriteLine("Command or ENTER to play: ");
                string input = Console.ReadLine()?.Trim() ?? string.Empty;
                HandleCommand(input);
            }
        }

        // Оработка команд 
        private void HandleCommand(string command) {
            switch (command) {
                case "":
                    _casino.PlayRound();
                    break;

                case "-h":
                    PrintHelp();
                    break;

                case "-switch":
                    _casino.SwitchGame();
                    break;

                case "-delete profile":
                    _casino.DeleteProfile();
                    break;

                case "-e":
                    Exit();
                    break;

                default:
                    Console.WriteLine("Unknown command. Type -h for help.");
                    break;
            }
            // без этой проверки после проигрыша внутри игры игра не завершается корректно
            if (_casino.ExitRequested) {
                Exit();
            }
        }

        private void Exit() {
            _casino.SaveProfile();
            Console.WriteLine("Goodbye!");
            _isRunning = false;
        }

        private void PrintHelp() {
            Console.WriteLine("=====================================");
            Console.WriteLine("Available commands:");
            Console.WriteLine("ENTER            - play round (or select game if none)");
            Console.WriteLine("-h               - show help");
            Console.WriteLine("-switch          - choose / change game");
            Console.WriteLine("-delete profile  - delete profile");
            Console.WriteLine("-e               - exit");
            Console.WriteLine("=====================================");
        }
    }
}