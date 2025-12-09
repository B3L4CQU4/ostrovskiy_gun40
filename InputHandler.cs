using System;

public static class InputHandler {
    public static string? ReadLine(string prompt) {
        /* 
        Вывод строки подсказки и обработка инпута
        :param prompt: Строка подсказка которая будет выведена
        */
        Console.WriteLine($"{prompt} (для выхода введите -q)");
        string input = Console.ReadLine() ?? "";

        // Возвращаем null или инпут
        return input == "-q" ? null : input;
    }
}