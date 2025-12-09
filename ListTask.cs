using System;
using System.Collections.Generic;

public class ListTask {
    private readonly List<string> _listOfStrings;
    private void PrintList(List<string> listObject) {
        Console.WriteLine();
        foreach (var item in listObject) {
            Console.WriteLine(item);
        }
        Console.WriteLine();
    }
    public ListTask() {
        _listOfStrings = new List<string> {
            "Первая строка",
            "Вторая строка",
            "Третья строка"
        };
    }

    public void TaskLoop() {
        Console.WriteLine("Задание 1");
        while (true) {
            // Читаем и проверяем инпут через кастомный класс
            string? _input = InputHandler.ReadLine("Введите первую строку");

            // Проверка на выход
            if (_input == null) break;

            if (!string.IsNullOrWhiteSpace(_input))
                _listOfStrings.Add(_input);

            PrintList(_listOfStrings);

            _input = InputHandler.ReadLine("Введите вторую строку");

            // Проверка на выход
            if (_input == null) break;

            if (!string.IsNullOrWhiteSpace(_input))
                _listOfStrings.Insert(_listOfStrings.Count / 2, _input);

            PrintList(_listOfStrings);
        }
    }
}
