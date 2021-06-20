using System;

namespace University.ARTQ
{
    class Program
    {
        static void Main()
        {
            const string testText = "A  ∩ (∏(x)(A, B)∪σ(a≠6  AND z >7)(T))";
            
            var lexer = new Lexer();
            
            var count1 = Lexer.CountWords(testText, "(");
            var count2 = Lexer.CountWords(testText, ")");
            var count3 = Lexer.CountWords(testText, "[");
            var count4 = Lexer.CountWords(testText, "]");
            var count5 = Lexer.CountWords(testText, "{");
            var count6 = Lexer.CountWords(testText, "}");
            
            var errorText = "Скобки: ";
            var isError = false;
            
            if (count1 != count2)
            {
                isError = true;
                errorText += $"\n()   -  {count1} {count2}";
            }
            
            if (count3 != count4)
            {
                isError = true;
                errorText += $"\n[]   -  {count3} {count4}";
            }
            
            if (count5 != count6)
            {
                isError = true;
                errorText += "\n{}" + $"   -  {count5} {count6}";
            }

            if (isError)
            {
                Console.WriteLine(errorText);
                Console.ReadKey();
                return;
            }
            
            String parsedText = Lexer.ParseLine(testText);
            
            Console.Write(parsedText);
            lexer.Start(parsedText);
            
            Console.Write("\n\n=============  TOKENS  ============================\n");
            
            foreach (var token in lexer.AlgebraTokens)
            {
                Console.WriteLine($"[{token.Type}] {token.Text}");
            }

            string resultSimpleAnalyze = lexer.SimpleAn();
            if (resultSimpleAnalyze != "ok") 
            {
                Console.WriteLine(resultSimpleAnalyze);
                Console.ReadKey();
                return;
            }
            
            Console.Write("\n\n=============  BLOCKS  ============================\n");
            
            string resultBlockedText = lexer.BlockedText();
            if (resultBlockedText != "ok") 
            {
                Console.WriteLine(resultBlockedText);
                Console.ReadKey();
                return;
            }
            
            foreach (var token in lexer.SqlTokens)
            {
                Console.WriteLine($"[{token.Type}] {token.Text}");
            }
            
            Console.Write("\n\n==POSTFIX==\n");
            lexer.PostfixFormat();
            foreach (var token in lexer.SqlTokens)
            {
                Console.WriteLine($"[{token.Type}] {token.Text}");
            }

            Console.Write("\n\n=============  SQL  ================================\n\n");
            
            string resultParse = lexer.Parser();
            if (resultParse != "ok")
            {
                Console.WriteLine(resultParse);
                Console.ReadKey();
                return;
            }

            foreach (var token in lexer.SqlText)
            {
                Console.WriteLine(token);
            }
            
            Console.ReadKey();
        }
    }
}