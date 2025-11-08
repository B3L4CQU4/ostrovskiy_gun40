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
            int[] fibonacciArray = new int[8];

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
            string[] monthArray = new string[12];

            for (int i = 0; i < monthArray.Length; i++) {
                monthArray[i] = ((Month)(i + 1)).ToString();
            }

            Console.WriteLine("Задание 2");
            Console.WriteLine(string.Join(", ", monthArray));

            // Задание 3
            int[] array = new int[3] { 2, 3, 4 };
            int[,] matrixArray = new int[3, 3];

            for (int i = 0; i < matrixArray.GetLength(0); i++) {
                for (int j = 0; j < matrixArray.GetLength(1); j++) {
                    matrixArray[i, j] = (int)Math.Pow(array[j], i + 1);
                }
            }

            Console.WriteLine("Задание 3");
            for (int i = 0; i < matrixArray.GetLength(0); i++) {
                for (int j = 0; j < matrixArray.GetLength(1); j++) {
                    Console.Write(matrixArray[i, j] + " ");
                }
                Console.WriteLine();
            }

            // Задание 4
            double[][] jaggedArray = new double[3][];

            // числа от 1 до 5
            jaggedArray[0] = new double[5];
            for (int i = 0; i < 5; i++) {
                jaggedArray[0][i] = i + 1;
            }

            // константы e и pi
            jaggedArray[1] = new double[2];
            jaggedArray[1][0] = Math.E;
            jaggedArray[1][1] = Math.PI;

            // логарифмы по основанию 10
            jaggedArray[2] = new double[4];
            jaggedArray[2][0] = Math.Log10(1);
            jaggedArray[2][1] = Math.Log10(10);
            jaggedArray[2][2] = Math.Log10(100);
            jaggedArray[2][3] = Math.Log10(1000);

            Console.WriteLine("Задание 4");
            foreach (double[] innerArray in jaggedArray) {
                foreach (double value in innerArray) {
                    Console.Write(value + " ");
                }
                Console.WriteLine();
            }

            int[] array1 = { 1, 2, 3, 4, 5 };
            int[] array2 = { 7, 8, 9, 10, 11, 12, 13 };

            // Задание 5
            Array.Copy(
                sourceArray: array1,
                sourceIndex: 0,
                destinationArray: array2,
                destinationIndex: 0,
                length: 3
            );
            Console.WriteLine("Задание 5");
            Console.WriteLine(string.Join(", ", array2));

            // Задание 6
            Array.Resize(ref array1, array1.Length * 2);
            Console.WriteLine("Задание 6");
            Console.WriteLine(string.Join(", ", array1));
        }
    }
}
