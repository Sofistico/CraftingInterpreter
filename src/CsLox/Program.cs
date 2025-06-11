using System.Text;

namespace CsLox
{
    public static class Program
    {
        private static bool _hadError = false;

        private static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: CsLox [script]");
                Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                RunFile(args[0]);
            }
            else
            {
                RunPrompt();
            }
        }

        private static void RunFile(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            Run(Encoding.UTF8.GetString(bytes));
            if (_hadError)
                Environment.Exit(65);
        }

        private static void RunPrompt()
        {
            var reader = new StreamReader(Console.OpenStandardInput());

            for (;;)
            {
                Console.Write("> ");
                string? line = reader.ReadLine();
                if (line == null) break;
                Run(line);
                _hadError = false;
            }
        }

        private static void Run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
            _hadError = true;
        }
    }
}
