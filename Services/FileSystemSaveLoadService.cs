using System;
using System.IO;

namespace CasinoGame.Services {
    public class FileSystemSaveLoadService : ISaveLoadService<string> {
        private readonly string _directoryPath;

        public FileSystemSaveLoadService(string directoryPath) {
            if (string.IsNullOrWhiteSpace(directoryPath)) {
                throw new ArgumentException("Directory path cannot be empty.", nameof(directoryPath));
            }

            _directoryPath = directoryPath;

            if (!Directory.Exists(_directoryPath)) {
                Directory.CreateDirectory(_directoryPath);
            }
        }

        public void SaveData(string data, string id) {
            if (string.IsNullOrWhiteSpace(id)) {
                throw new ArgumentException("File identifier cannot be empty.", nameof(id));
            }

            string filePath = Path.Combine(_directoryPath, $"{id}.txt");

            try {
                using (StreamWriter writer = new StreamWriter(filePath, false)) {
                    writer.Write(data);
                }
            } catch (Exception ex) {
                Console.WriteLine($"Error while saving data: {ex.Message}");
            }
        }

        public string LoadData(string id) {
            if (string.IsNullOrWhiteSpace(id)) {
                throw new ArgumentException("File identifier cannot be empty.", nameof(id));
            }

            string filePath = Path.Combine(_directoryPath, $"{id}.txt");

            if (!File.Exists(filePath)) {
                return string.Empty;
            }

            try {
                using (StreamReader reader = new StreamReader(filePath)) {
                    return reader.ReadToEnd();
                }
            } catch (Exception ex) {
                Console.WriteLine($"Error while loading data: {ex.Message}");
                return string.Empty;
            }
        }
    }
}