using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class DictionaryTask {

    private readonly Dictionary<string, int> keyValuePairs;

    private bool AddStudent() {
        while (true) {
            string? _input = InputHandler.ReadLine("Введите Имя и оценку (от 2 до 5) допустимые разделители \"=\", \":\"");

            // Проверка на выход
            if (_input == null) return false;

            // Проверка ввода через регулярку 
            Match match = Regex.Match(_input, @"^([a-zA-Zа-яА-ЯёЁ]+)\s*[=:]\s*([2-5])$");
            if (match.Success) {
                string name = match.Groups[1].Value;
                int grade = int.Parse(match.Groups[2].Value);
                // Если успешно пробуем добавить 
                bool added = keyValuePairs.TryAdd(name, grade);
                if (added) {
                    return true;
                } else {
                    Console.WriteLine("Имя студента уже существует");
                }
            } else {
                Console.WriteLine("Неверный формат данных. Используйте: Имя=Оценка или Имя:Оценка");
                Console.WriteLine("Пример: Иван=5 или Анна:4");
            }
        }
    }

    private bool FindStudent() {
        while (true) {
            string? _input = InputHandler.ReadLine("Поиск оценки. Введите Имя");
            // Проверка на выход
            if (_input == null) return false;
            // Убираем пробелы
            string selectedName = _input.Trim();

            // Если найден отдаёт оценку
            if (keyValuePairs.TryGetValue(selectedName, out int selectedGrade)) {
                Console.WriteLine($"Оценка студента {_input} : {selectedGrade}");
                return true;
            } else {
                Console.WriteLine("Имя не найдено");
            }
        }
    }
    public DictionaryTask() {
        keyValuePairs = new Dictionary<string, int> { };
    }

    public void TaskLoop() {
        Console.WriteLine("Задание 2");

        while (true) {
            if (!AddStudent()) break;
            if (!FindStudent()) break;
        }
    }
}
