using System;

public class LinkedListTask {
    private class Node {
        public string Data { get; set; }
        public Node? Next { get; set; }
        public Node? Prev { get; set; }

        public Node(string data) {
            Data = data;
            Next = null;
            Prev = null;
        }
    }

    private Node? head;
    private Node? tail;

    // Добавление элемента в конец списка
    private void Add(string data) {
        Node newNode = new Node(data);

        if (head == null) {
            head = newNode;
            tail = newNode;
        } else {
            tail!.Next = newNode;
            newNode.Prev = tail;
            tail = newNode;
        }
    }

    // Вывод списка в прямом порядке
    private void PrintForward() {
        Console.Write("Список в прямом порядке: ");
        Node? current = head;
        while (current != null) {
            Console.Write(current.Data);
            if (current.Next != null) Console.Write(" <-> ");
            current = current.Next;
        }
        Console.WriteLine();
    }

    // Вывод списка в обратном порядке
    private void PrintBackward() {
        Console.Write("Список в обратном порядке: ");
        Node? current = tail;
        while (current != null) {
            Console.Write(current.Data);
            if (current.Prev != null) Console.Write(" <-> ");
            current = current.Prev;
        }
        Console.WriteLine();
    }

    // Проверка количества элементов
    private bool IsValidCount(string[] elements) {
        int count = elements.Length;
        return count >= 3 && count <= 6;
    }

    // Очистка списка
    private void Clear() {
        head = null;
        tail = null;
    }

    public LinkedListTask() {
        head = null;
        tail = null;
    }

    public void TaskLoop() {
        Console.WriteLine("Задание 3");
        while (true) {
            string? input = InputHandler.ReadLine("Введите от 3 до 6 элементов через запятую");

            // Проверка на выход
            if (input == null) break;

            // Разделение строки по запятым
            string[] elements = input.Split(',', StringSplitOptions.RemoveEmptyEntries);

            // Удаление лишних пробелов вокруг каждого элемента
            for (int i = 0; i < elements.Length; i++) {
                elements[i] = elements[i].Trim();
            }

            // Проверка количества элементов
            if (!IsValidCount(elements)) {
                Console.WriteLine("Ошибка: нужно ввести от 3 до 6 элементов.");
                Console.WriteLine($"Вы ввели {elements.Length} элементов.");
                continue;
            }

            Clear();

            // Добавляем элементы в список
            foreach (var element in elements) {
                Add(element);
            }

            Console.WriteLine("\nРезультат:");
            PrintForward();
            PrintBackward();

            Console.WriteLine("\n");
        }
    }
}
