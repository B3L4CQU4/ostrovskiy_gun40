// See https://aka.ms/new-console-template for more information
using System;


namespace MyApp {
    public enum Month {
        January = 1, February, March, April,
        May, June, July, August, September,
        October, November, December
    }
    class Program {
        static void Main(string[] args) {
            // Задание 1
            int[] fibonacciArray = new int[10];

            for (int i = 0; i < fibonacciArray.Length; i++) {
                fibonacciArray[i] = i switch {
                    0 => 0,
                    1 => 1,
                    _ => fibonacciArray[i] = fibonacciArray[i - 1] + fibonacciArray[i - 2]
                };

            }
            Console.WriteLine("Задание 1");
            Console.WriteLine(string.Join(", ", fibonacciArray));

            // Задание 2
            Console.WriteLine("Задание 2");
            for (int i = 0; i <= 20; i++) {
                if (i % 2 == 0 & i != 0) {
                    Console.Write($"{i} ");
                }
            }
            Console.WriteLine();

            // Задание 3
            Console.WriteLine("Задание 3");
            for (int i = 1; i <= 5; i++) {
                for (int j = 1; j <= 5; j++) {
                    Console.Write($"{i} * {j} = {i * j}\t");
                }
                Console.WriteLine();
            }

            // Задание 4
            Console.WriteLine("Задание 4");
            string password = "qwerty";
            string userInput;
            do {
                Console.WriteLine("Введите пароль: ");
                userInput = Console.ReadLine() ?? "";
                if (userInput != password) {
                    Console.WriteLine("Неверный пароль");
                }

            } while (userInput != password);
            Console.WriteLine("Пароль верный");

        }
    }
}
