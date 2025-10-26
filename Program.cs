// See https://aka.ms/new-console-template for more information
using System;

namespace MyApp {
    class Program {
        static void Main(string[] args) {
            try {
                int a = ReadNumber("Enter the first number: ");
                int b = ReadNumber("Enter the second number: ");
                string operation = ReadOperator("Enter the operator &, | or ^ ");
                int result = Calculate(a, b, operation);
                Console.WriteLine(
                    $"Result: in decimal {result} in hexadecimal 0x{result:X} in binary {Convert.ToString(result, 2)}"
                );
            } catch (ApplicationException e) {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            static int ReadNumber(string text) {
                Console.Write(text);
                if (!Int32.TryParse(Console.ReadLine(), out var number)) {
                    throw new ApplicationException("Not a number or number is not an integer");
                }

                return number;
            }

            static bool IsValidOperator(string operation) => operation is "^" or "&" or "|";

            static string ReadOperator(string text) {
                Console.Write(text);
                string operationType = Console.ReadLine() ?? "";
                if (!IsValidOperator(operationType))
                    throw new ApplicationException("Wrong operator");

                return operationType;
            }

            static int Calculate(int a, int b, string operation) {
                switch (operation) {
                    case "^": return a ^ b;
                    case "&": return a & b;
                    case "|": return a | b;
                    default: throw new ApplicationException("Invalid data");
                }
            }
        }
    }
}
