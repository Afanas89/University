using System;

namespace University.ARTQ
{
    class Program
    {
        static void Main(string[] args)
        {
            String z_RichText = "A  ∩ (∏(x)(A, B)∪σ(a≠6  AND z >7)(T))";
            Lexer m_Lexer = new Lexer();

            if (z_RichText.Length > 0)
            {
                int z_Count1 = Lexer.CountWords(z_RichText, "(");
                int z_Count2 = Lexer.CountWords(z_RichText, ")");
                int z_Count3 = Lexer.CountWords(z_RichText, "[");
                int z_Count4 = Lexer.CountWords(z_RichText, "]");
                int z_Count5 = Lexer.CountWords(z_RichText, "{");
                int z_Count6 = Lexer.CountWords(z_RichText, "}");
                String z_error = "Скобки: ";
                bool z_bError = false;
                if (z_Count1 != z_Count2)
                {
                    z_bError = true;
                    z_error += "\n()   -  " + z_Count1 + " " + z_Count2;
                }

                if (z_Count3 != z_Count4)
                {
                    z_bError = true;
                    z_error += "\n[]   -  " + z_Count3 + " " + z_Count4;
                }

                if (z_Count5 != z_Count6)
                {
                    z_bError = true;
                    z_error += "\n{}   -  " + z_Count5 + " " + z_Count6;
                }

                if (!z_bError)
                {

                    String z_NormalText = Lexer.ParseLine(z_RichText);
                    System.Console.Write(z_NormalText);
                    m_Lexer.Start(z_NormalText);
                    System.Console.Write("\n\n=============  TOKENS  ============================\n");
                    string[] z_namestype = {"operator", "logic", "devider", "param", "open", "close", "unknown"};
                    for (int i = 0; i < m_Lexer.m_TokenList.Count; i++)
                    {
                        System.Console.Write("[" + z_namestype[(int) m_Lexer.m_TokenList[i].Type] + "] " +
                                             m_Lexer.m_TokenList[i].Text + "\n");
                    }

                    if (m_Lexer.SimpleAn() != "ok") return;


                    System.Console.Write("\n\n=============  BLOCKS  ============================\n");
                    if (m_Lexer.BlockedText() != "ok") return;
                    string[] z_namestypeblc = {"select", "from", "where", "operator", "open", "close", "unknown"};
                    for (int i = 0; i < m_Lexer.m_BlockTokenList.Count; i++)
                    {
                        System.Console.Write("[" + z_namestypeblc[(int) m_Lexer.m_BlockTokenList[i].Type] + "] " +
                                             m_Lexer.m_BlockTokenList[i].Text + "\n");
                    }

                    System.Console.Write("\n\n==POSTFIX==\n");
                    m_Lexer.PostfixFormat();
                    for (int i = 0; i < m_Lexer.m_BlockTokenList.Count; i++)
                    {
                        System.Console.Write("[" + z_namestypeblc[(int) m_Lexer.m_BlockTokenList[i].Type] + "] " +
                                             m_Lexer.m_BlockTokenList[i].Text + "\n");
                    }

                    System.Console.Write("\n\n=============  SQL  ================================\n");
                    if (m_Lexer.Parser() != "ok") return;

                    foreach (String st in m_Lexer.m_SQL_Text)
                        System.Console.Write("\n" + st);

                }
                else System.Console.WriteLine(z_error, "Синтаксис");
            }
            else System.Console.WriteLine("Введите текст", "Синтаксис");

            System.Console.ReadKey();
        }
    }
}