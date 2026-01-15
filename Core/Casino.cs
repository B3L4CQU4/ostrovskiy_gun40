using System;
using CasinoGame.Entities;
using CasinoGame.Games;
using CasinoGame.Services;
using System.Text.RegularExpressions;

namespace CasinoGame.Core {
    public class Casino : IGame {
        private const string SaveFolder = "Saves";
        private const string SaveFileId = "player";

        private readonly ISaveLoadService<string> _saveLoadService;

        private readonly BlackjackGame _blackjackGame;
        private readonly DiceGame _diceGame;

        private CasinoGameBase _currentGame = null!;
        private PlayerProfile _player = null!;

        private int _currentBet;
        public bool IsGameOver { get; private set; }
        public bool ExitRequested { get; private set; }
        private bool _lastRoundWasWin;
        private int _highBankRounds;

        public Casino() {
            _saveLoadService = new FileSystemSaveLoadService(SaveFolder);

            _blackjackGame = new BlackjackGame(cardsCount: 36);
            _diceGame = new DiceGame(diceCount: 3, minValue: 1, maxValue: 6);

            SubscribeToGameEvents(_blackjackGame);
            SubscribeToGameEvents(_diceGame);
        }

        // Первичная инициализация казино (без игрового цикла)
        public void StartGame() {
            LoadOrCreateProfile();
            Console.WriteLine("=====================================");
            Console.WriteLine($"Welcome to Casino {_player.Name}!");
            if (_player.Bank <= 0) {
                IsGameOver = true;
                HandleGameOver("You lost all your money.");
            }
            if (_player.Bank == PlayerProfile.MaxBank) {
                IsGameOver = true;
                HandleGameOver("You won this casino.");
            }
            SaveProfile();
        }

        // игровой раунд
        public void PlayRound() {
            if (_currentGame == null) {
                _currentGame = ChooseGame();
                return;
            }

            MakeBet();
            _currentGame.PlayGame();
            ApplyPostGameRules();
            SaveProfile();
            if (_player.Bank <= 0) {
                IsGameOver = true;
                HandleGameOver("You lost all your money.");
            } else if (_player.Bank == PlayerProfile.MaxBank) {
                IsGameOver = true;
                HandleGameOver("You won this casino.");
            }
        }

        // Смена текущей игры
        public void SwitchGame() {
            _currentGame = ChooseGame();
            Console.WriteLine("=====================================");
            Console.WriteLine("Game selected.");
            Console.WriteLine("=====================================");
        }

        // Сохранение профиля игрока
        public void SaveProfile() {
            string data = $"{_player.Name};{_player.Bank}";
            _saveLoadService.SaveData(data, SaveFileId);
        }

        public void DeleteProfile() {
            Console.WriteLine(
                "WARNING: Your profile will be permanently deleted.\n" +
                "This action cannot be undone.\n" +
                "Type YES to confirm:");

            string input = Console.ReadLine() ?? string.Empty;

            if (!input.Equals("YES", StringComparison.OrdinalIgnoreCase)) {
                Console.WriteLine("Profile deletion cancelled.");
                return;
            }

            _saveLoadService.SaveData(string.Empty, SaveFileId);

            Console.WriteLine("Profile deleted.\nRestarting game...\n");

            RestartGame();
        }

        private void HandleGameOver(string text) {
            Console.WriteLine(text);

            while (true) {
                Console.Write("Do you want to start over? (yes/no): ");
                string input = Console.ReadLine()?.Trim() ?? string.Empty;

                if (input.Equals("yes", StringComparison.OrdinalIgnoreCase)) {
                    DeleteProfileInternal();
                    RestartGame();
                    return;
                }

                if (input.Equals("no", StringComparison.OrdinalIgnoreCase)) {
                    Console.WriteLine("Thanks for playing!");
                    ExitRequested = true;
                    return;
                }

                Console.WriteLine("Invalid input. Please type 'yes' or 'no'.");
            }
        }

        private void DeleteProfileInternal() {
            _saveLoadService.SaveData(string.Empty, SaveFileId);
        }

        private void RestartGame() {
            IsGameOver = false;
            _player = null!;
            _currentGame = null!;

            Console.WriteLine("Starting new game...\n");
            StartGame();
        }

        private void LoadOrCreateProfile() {
            string data = _saveLoadService.LoadData(SaveFileId);

            if (!string.IsNullOrWhiteSpace(data)) {
                string[] parts = data.Split(';');
                _player = new PlayerProfile(parts[0], int.Parse(parts[1]));
                return;
            }
            Console.WriteLine("=====================================");
            Console.WriteLine("Welcome to Casino!");
            Console.WriteLine("=====================================");
            Regex nameRegex = new Regex("^[A-Za-z]{1,10}$");

            while (true) {
                Console.WriteLine("Name rules:");
                Console.WriteLine("- Only Latin letters (A-Z, a-z)");
                Console.WriteLine("- Maximum length: 10 characters");
                Console.Write("Enter your name: ");
                string name = Console.ReadLine()?.Trim() ?? string.Empty;

                if (!nameRegex.IsMatch(name)) {
                    Console.WriteLine("Invalid name. Please follow the rules.");
                    continue;
                }

                _player = new PlayerProfile(name, bank: 1000);
                return;
            }
        }

        private CasinoGameBase ChooseGame() {
            Console.WriteLine("=====================================");
            Console.WriteLine("Choose game:");
            Console.WriteLine("1 - Blackjack");
            Console.WriteLine("2 - Dice");
            Console.WriteLine("=====================================");

            while (true) {
                string input = Console.ReadLine() ?? string.Empty;

                if (input == "1")
                    return _blackjackGame;

                if (input == "2")
                    return _diceGame;

                Console.WriteLine("Invalid choice. Enter 1 or 2.");
            }
        }

        private void MakeBet() {
            while (true) {
                Console.WriteLine("=====================================");
                Console.WriteLine($"Your bank: {_player.Bank}");
                Console.Write("Make a bet: ");

                string input = Console.ReadLine() ?? string.Empty;

                if (!int.TryParse(input, out int bet)) {
                    Console.WriteLine("Invalid input. Please enter an int number.");
                    continue;
                }

                if (bet <= 0) {
                    Console.WriteLine("Bet must be greater than zero.");
                    continue;
                }

                if (bet > _player.Bank) {
                    Console.WriteLine("You don't have enough money for this bet.");
                    continue;
                }

                _currentBet = bet;
                return;
            }
        }

        private void SubscribeToGameEvents(CasinoGameBase game) {
            game.OnWin += () => {
                _player.Bank += _currentBet;
                _lastRoundWasWin = true;
            };

            game.OnLoose += () => {
                _player.Bank -= _currentBet;
                _lastRoundWasWin = false;
            };

            game.OnDraw += () => {
                _lastRoundWasWin = false;
            };
        }

        private void ApplyPostGameRules() {
            // 1. Полный проигрыш только для вывода кастомного текста
            if (_player.Bank <= 0) {
                Console.WriteLine("No money? Kicked!");
                return;
            }

            // 2. Разорение казино — только после большого выигрыша за малое количество шагов
            if (_lastRoundWasWin && _player.Bank > PlayerProfile.MaxBank) {
                int excess = _player.Bank - PlayerProfile.MaxBank;
                _player.Bank = PlayerProfile.MaxBank;
                Console.WriteLine($"You ruined the casino! Excess money: {excess}. A new casino will be built here.");
                Console.WriteLine($"Current bank: {_player.Bank}");
                return;
            }

            // Счётчик ходов когда банк игрока превышает половину от максимального банка
            if (_player.Bank > PlayerProfile.MaxBank / 2) {
                _highBankRounds++;
            } else {
                _highBankRounds = 0;
            }

            // 3. Ивент с баром только если больше 3х ходов банк игрока больше половины максимального 
            if (_highBankRounds >= 3) {
                _player.Bank /= 2;
                _highBankRounds = 0;
                Console.WriteLine("You wasted half of your bank money in casino's bar");
            }

            Console.WriteLine($"Current bank: {_player.Bank}");
        }
    }
}
