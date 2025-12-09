using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Program {
    internal class Program {

        static void Main(string[] args) {

            int task;

            while (true) {
                string? input = InputHandler.ReadLine("Введите 1, 2 или 3 для выбора соответствующего задания");

                if (input == null) return;

                if (int.TryParse(input, out task)) {
                    if (task >= 1 && task <= 3) {
                        break;
                    } else {
                        Console.WriteLine("Некорректная цифра. Допустимы цифры от 1 до 3");
                    }
                } else {
                    Console.WriteLine("Некорректный ввод. Допустимы только цифры");
                }
            }

            switch (task) {
                case 1:
                    CheckTaskFirst();
                    break;
                case 2:
                    CheckTaskSecond();
                    break;
                case 3:
                    CheckTaskThird();
                    break;
            }
        }

        private static void CheckTaskFirst() {
            var listTask = new ListTask();
            listTask.TaskLoop();
        }

        private static void CheckTaskSecond() {
            var dictionaryTask = new DictionaryTask();
            dictionaryTask.TaskLoop();
        }

        private static void CheckTaskThird() {
            var linkedListTask = new LinkedListTask();
            linkedListTask.TaskLoop();
        }
    }
}