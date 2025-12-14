using System;
using System.Text;

namespace HomeWork {
    internal class Program {
        // Задание 1
        public static string ConcatenateStrings(string str1, string str2) {
            return str1 + str2;
        }

        // Задание 2
        public static string GreetUser(string name, int age) {
            return $"Hello, {name}!\nYou are {age} years old.";
        }

        // Задание 3
        public static string GetStringInfo(string input) {
            int length = input.Length;
            string upper = input.ToUpper();
            string lower = input.ToLower();

            return $"Длина строки: {length}\n" +
                   $"Верхний регистр: {upper}\n" +
                   $"Нижний регистр: {lower}";
        }

        // Задание 4
        public static string GetFirstFiveCharacters(string input) {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return input.Length <= 5 ? input : input.Substring(0, 5);
        }

        // Задание 5
        public static StringBuilder BuildSentenceFromArray(string[] words) {
            StringBuilder sb = new StringBuilder();

            foreach (string word in words) {
                sb.Append(word);
                sb.Append(' ');
            }

            // Удаляем последний лишний пробел
            if (sb.Length > 0)
                sb.Length--;

            return sb;
        }

        // Задание 6
        public static string ReplaceWords(string inputString, string wordToReplace, string replacementWord) {
            return inputString.Replace(wordToReplace, replacementWord);
        }

        static void Main(string[] args) {
            Console.WriteLine("Тесты\n");

            // Тест 1
            Console.WriteLine("Задание 1:");
            Console.WriteLine($"Тест: {ConcatenateStrings("Один ", "два")}\n");

            // Тест 2
            Console.WriteLine("Задание 2:");
            Console.WriteLine($"Тест:\n{GreetUser("Bob", 30)}\n");

            // Тест 3
            Console.WriteLine("Задание 3:");
            Console.WriteLine($"Тест: {GetStringInfo("Тестовая строка")}\n");

            // Тест 4
            Console.WriteLine("Задание 4:");
            string firstFive = GetFirstFiveCharacters("Hello World!");
            Console.WriteLine($"Результат: {firstFive}");
            Console.WriteLine($"Тест 2 (короткая строка): {GetFirstFiveCharacters("Hi")}");
            Console.WriteLine($"Тест 3 (пустая строка): '{GetFirstFiveCharacters("")}'\n");

            // Тест 5
            Console.WriteLine("Задание 5:");
            string[] words = { "Это", "был", "список", "слов" };
            StringBuilder sentence = BuildSentenceFromArray(words);
            Console.WriteLine($"Тест: {sentence}");

            string[] words2 = { "Второй", "список", "слов", "который", "больше", "чем", "первый" };
            Console.WriteLine($"Тест 2: {BuildSentenceFromArray(words2)}\n");

            // Тест 6
            Console.WriteLine("Задание 6:");
            string replaced = ReplaceWords("Hello world, world is beautiful", "world", "universe");
            Console.WriteLine($"Тест: {replaced}");
        }
    }
}