using System.Collections.Generic;

namespace University.ARTQ
{
    /// <summary>
    /// Класс разбора выражений.
    /// </summary>
    public sealed class Lexer 
    {
        #region Fields and Properties

        /// <summary>
        /// Текст содержащий булеву алгебру.
        /// </summary>
        public string AlgebraText { get; set; }
        
        /// <summary>
        /// Конечный запрос разбитый на строки.
        /// </summary>
        public List<string> SqlText { get; }

        public List<RelationalAlgebraToken> AlgebraTokens { get; }
        public List<SqlToken> SqlTokens { get; }

        #endregion

        #region Constructors

        public Lexer()
        {
            SqlText = new List<string>();
            AlgebraTokens = new List<RelationalAlgebraToken>();
            SqlTokens = new List<SqlToken>();
        }

        #endregion

        #region Methods

        public bool WorkAll(string textAlgebra)
        {
            Start(textAlgebra);
            if (SimpleAn() != "ok") return false;
            if (BlockedText() != "ok") return false;
            PostfixFormat();
            if (Parser() != "ok") return false;
            return true;
        }
        
        // получаем строку и разбиваем ее на токены
        public bool Start(string originalText)
        {
            AlgebraText = originalText;
            SqlText.Clear();
            AlgebraTokens.Clear();
            SqlTokens.Clear();

            for (int i = 0; i < originalText.Length; i++)
            {
                if (originalText[i] == ' ')
                    continue;

                var symbol = originalText[i];
                
                if (symbol == '(') AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.SeparatorOpen, "("));
                else if (symbol == '[') AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.SeparatorOpen, "["));
                else if (symbol == '{') AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.SeparatorOpen, "{"));
                else if (symbol == ')') AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.SeparatorClose, ")"));
                else if (symbol == ']') AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.SeparatorClose, "]"));
                else if (symbol == '}') AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.SeparatorClose, "}"));
                else if (symbol == '\u2229') AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Operator, "INTERSECT"));
                else if (symbol == '\u222A') AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Operator, "UNION"));
                else if (symbol == '\u2212') AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Operator, "MINUS"));
                else if (symbol == '\u220F') AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Operator, "PI"));
                else if (symbol == '\u03c3') AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Operator, "SIGMA"));
                else if (symbol == '\u22c8') AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Operator, "JOIN"));
                else if (symbol == '\u2264') AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Logic, "\u2264")); // <=
                else if (symbol == '\u2265') AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Logic, "\u2265")); // >=
                else if (symbol == '\u2260') AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Logic, "\u2260")); // !=
                else if (symbol == '>') AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Logic, ">"));
                else if (symbol == '<') AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Logic, "<"));
                else if (symbol == '=') AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Logic, "="));
                else if (symbol == ',') AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Separator, ","));
                else
                {
                    int h = originalText.IndexOfAny((" <,>=(){}[]\u2229\u222A\u220F\u03c3\u22c8\u2264\u2265\u2260\u2212").ToCharArray(), i);
                    if (h == -1)
                    {
                        AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Table, originalText.Substring(i, originalText.Length - i)));
                        break;
                    }
                    
                    AlgebraTokens.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Table, originalText.Substring(i, h - i)));
                    i = h - 1;
                }
            }

            foreach (var t in AlgebraTokens)
            {
                if (t.Type is RelationalAlgebraTokenType.Table or RelationalAlgebraTokenType.Unknown)
                {
                    string tokenText = t.Text.ToUpper();
                    if (tokenText is "INTERSECT" or "UNION" or "PI" or "SIGMA" or "MINUS" or "JOIN")
                    {
                        t.Text = tokenText;
                        t.Type = RelationalAlgebraTokenType.Operator;
                    }
                    else if (tokenText is "OR" or "AND")
                    {
                        t.Text = tokenText;
                        t.Type = RelationalAlgebraTokenType.Logic;
                    }
                }
            }

            return true;
        }

        // Простые проверки на корректность выражения
        public string SimpleAn()
        {
            if (AlgebraTokens.Count < 1) 
                return "Нет токенов";
            
            if (AlgebraTokens[0].Type != RelationalAlgebraTokenType.Table && AlgebraTokens[0].Type != RelationalAlgebraTokenType.Operator && AlgebraTokens[0].Type != RelationalAlgebraTokenType.SeparatorOpen)
                return "Запись должна начинаться с оператора, отношения или открывающей скобки";

            return "ok";
        }

        // группируем токкены в блоки
        // например, объединяем все токены, принадлежащие к одному оператору Proj, в один блок - SELECT
        public string BlockedText()
        {
            SqlTokens.Clear();

            for (int i = 0; i < AlgebraTokens.Count; i++)
            {
                if (AlgebraTokens[i].Type == RelationalAlgebraTokenType.Operator)
                {
                    if (AlgebraTokens[i].Text == "PI")
                    {
                        string zStr = "";
                        if (i + 1 < AlgebraTokens.Count)
                        {
                            if (AlgebraTokens[i + 1].Type != RelationalAlgebraTokenType.SeparatorOpen)
                            {
                                return "Ошибка в синтаксисе оператора проекции.\n === === ===\nPI ([аргументы])(отношение)\n === === ===\nСкобки для аргументов не опознаны";
                            }
                        }
                        else
                        {
                            return "Ошибка в синтаксисе оператора проекции.\n === === ===\nPI ([аргументы])(отношение)\n === === ===\nСкобки для аргументов не опознаны";
                        }

                        bool zFindResult = false;
                        int zCountOpens = 0;
                        int j;
                        for (j = i + 2; j < AlgebraTokens.Count; j++)
                        {
                            if (AlgebraTokens[j].Type == RelationalAlgebraTokenType.SeparatorClose)
                            {
                                if (zCountOpens > 0)
                                {
                                    zCountOpens--;
                                    zStr += ")";
                                }
                                else
                                {
                                    if (!zFindResult) zStr += " * ";
                                    break;
                                }
                            }
                            else if (AlgebraTokens[j].Type == RelationalAlgebraTokenType.SeparatorOpen)
                            {
                                zCountOpens++;
                                zStr += " (";
                            }
                            else
                            {
                                zFindResult = true;
                                zStr += " " + AlgebraTokens[j].Text;
                            }
                        }

                        i = j;
                        SqlTokens.Add(new SqlToken(SqlTokenType.Select, zStr));
                    }
                    else if (AlgebraTokens[i].Text == "SIGMA")
                    {
                        string zStr = "";
                        if (i + 1 < AlgebraTokens.Count)
                        {
                            if (AlgebraTokens[i + 1].Type != RelationalAlgebraTokenType.SeparatorOpen)
                            {
                                return "Ошибка в синтаксисе оператора выборки.\n === === ===\nSIGMA ([аргументы])(отношение)\n === === ===\nСкобки для аргументов не опознаны";
                            }
                        }
                        else
                        {
                            return "Ошибка в синтаксисе оператора выборки.\n === === ===\nSIGMA ([аргументы])(отношение)\n === === ===\nСкобки для аргументов не опознаны";
                        }

                        bool zFindResult = false;
                        int zCountOpens = 0;
                        int j;
                        for (j = i + 2; j < AlgebraTokens.Count; j++)
                        {
                            if (AlgebraTokens[j].Type == RelationalAlgebraTokenType.SeparatorClose)
                            {
                                if (zCountOpens > 0)
                                {
                                    zCountOpens--;
                                    zStr += ")";
                                }
                                else
                                {
                                    if (!zFindResult) zStr += " * ";
                                    break;
                                }
                            }
                            else if (AlgebraTokens[j].Type == RelationalAlgebraTokenType.SeparatorOpen)
                            {
                                zCountOpens++;
                                zStr += " (";
                            }
                            else
                            {
                                zFindResult = true;
                                zStr += " " + AlgebraTokens[j].Text;
                            }
                        }

                        i = j;
                        SqlTokens.Add(new SqlToken(SqlTokenType.Where, zStr));
                    }
                    else
                    {
                        SqlTokens.Add(new SqlToken(SqlTokenType.Operator, AlgebraTokens[i].Text));
                    }
                }
                else if (AlgebraTokens[i].Type == RelationalAlgebraTokenType.SeparatorOpen)
                {
                    SqlTokens.Add(new SqlToken(SqlTokenType.SeparatorOpen, "("));
                }
                else if (AlgebraTokens[i].Type == RelationalAlgebraTokenType.SeparatorClose)
                {
                    SqlTokens.Add(new SqlToken(SqlTokenType.SeparatorClose, ")"));
                }
                else
                {
                    if (SqlTokens.Count != 0)
                    {
                        if (SqlTokens[SqlTokens.Count - 1].Type == SqlTokenType.From)
                            SqlTokens[SqlTokens.Count - 1].Text += " " + AlgebraTokens[i].Text;
                        else SqlTokens.Add(new SqlToken(SqlTokenType.From, AlgebraTokens[i].Text));
                    }
                    else SqlTokens.Add(new SqlToken(SqlTokenType.From, AlgebraTokens[i].Text));
                }
            }

            return "ok";
        }

        // Получение приоритета блока в записи. 
        int GetPr(SqlToken bl)
        {
            switch (bl.Type)
            {
                case SqlTokenType.Select: return 5;
                case SqlTokenType.From: return 6;
                case SqlTokenType.Where: return 4;
                case SqlTokenType.Operator:
                    if (bl.Text == "UNION") return 2;
                    else if (bl.Text == "MINUS") return 2;
                    else if (bl.Text == "INTERSECT") return 3;
                    else if (bl.Text == "JOIN") return 3;
                    else return 2;
                case SqlTokenType.SeparatorOpen: return 0;
                case SqlTokenType.SeparatorClose: return 1;
                case SqlTokenType.Unknown: return 7;
            }

            return 8;
        }

        // преобразуем последовательность блоков согласно польской записи, для дальнейших вычислений-преобразований
        public void PostfixFormat()
        {
            List<SqlToken> zBlock = new List<SqlToken>();
            Stack<SqlToken> zStack = new Stack<SqlToken>();

            for (int i = 0; i < SqlTokens.Count; i++)
            {
                if (zStack.Count > 0 && SqlTokens[i].Type != SqlTokenType.SeparatorOpen)
                {
                    if (SqlTokens[i].Type == SqlTokenType.SeparatorClose)
                    {
                        SqlToken s = zStack.Pop();
                        while (s.Type != SqlTokenType.SeparatorOpen)
                        {
                            zBlock.Add(s);
                            s = zStack.Pop();
                        }
                    }
                    else
                    {
                        if (GetPr(SqlTokens[i]) >= GetPr(zStack.Peek()))
                            zStack.Push(SqlTokens[i]);
                        else
                        {
                            while (zStack.Count > 0 && GetPr(SqlTokens[i]) < GetPr(zStack.Peek()))
                                zBlock.Add(zStack.Pop());

                            zStack.Push(SqlTokens[i]);
                        }
                    }
                }
                else zStack.Push(SqlTokens[i]);
            }

            if (zStack.Count > 0)
            {
                foreach (SqlToken c in zStack)
                    zBlock.Add(c);
            }

            SqlTokens.Clear();

            foreach (SqlToken c in zBlock)
                SqlTokens.Add(c);
        }

        // по последовательности блоков (в польской записи) создаем SQL запрос
        public string Parser()
        {
            SqlText.Clear();

            var zStrWhere = "";
            var zStrSelect = " * ";
            var zStrFrom = "";

            List<int> todeletelist = new List<int>();
            for (int i = 0; i < SqlTokens.Count; i++)
            {
                if (SqlTokens[i].Type == SqlTokenType.From)
                {
                    int kk = i;
                    zStrFrom = SqlTokens[i].Text;
                    i++;
                    while (i < SqlTokens.Count &&
                           SqlTokens[i].Type != SqlTokenType.Operator &&
                           SqlTokens[i].Type != SqlTokenType.From)
                    {
                        if (SqlTokens[i].Type == SqlTokenType.Select)
                        {
                            zStrSelect = SqlTokens[i].Text;
                            todeletelist.Add(i);
                        }
                        else if (SqlTokens[i].Type == SqlTokenType.Where)
                        {
                            zStrWhere = SqlTokens[i].Text;
                            todeletelist.Add(i);
                        }

                        i++;
                    }

                    SqlTokens[kk].Text = (" SELECT " + zStrSelect + " FROM " + zStrFrom +
                                                   ((zStrWhere.Length > 0) ? (" WHERE " + zStrWhere) : (""))
                        );

                    zStrWhere = "";
                    zStrSelect = " * ";
                    zStrFrom = "";
                    i = kk;
                }
            }

            for (int i = todeletelist.Count - 1; i >= 0; i--)
            {
                SqlTokens.RemoveAt(todeletelist[i]);
            }

            bool findwhere = false, findselect = false;
            Stack<SqlToken> simstack23 = new Stack<SqlToken>();
            
            foreach (SqlToken sss in SqlTokens)
            {
                if (sss.Type == SqlTokenType.Operator)
                {
                    if (simstack23.Count < 2)
                        return "неверен синтаксис оператора " + sss.Text + " (не хватает аргументов)";

                    SqlToken sr = simstack23.Pop();
                    SqlToken sl = simstack23.Pop();

                    if (sss.Text == "JOIN") sss.Text = "NATURAL JOIN";

                    simstack23.Push(new SqlToken(SqlTokenType.From,
                        " ( " + sl.Text + " " + sss.Text + " " + sr.Text + " ) "));

                    findwhere = false;
                    findselect = false;
                }
                else if (sss.Type == SqlTokenType.Where)
                {
                    if (simstack23.Count == 0)
                        return "неверен синтаксис оператора " + sss.Text + " (не хватает аргументов)";

                    SqlToken sr = simstack23.Pop();
                    findwhere = true;
                    string kkj;

                    if (findselect)
                    {
                        kkj = sr.Text;
                        kkj += " WHERE " + sss.Text + " ";
                    }
                    else
                    {
                        kkj = "SELECT * FROM ( ";
                        kkj += sr.Text;
                        kkj += " ) WHERE " + sss.Text + " ";
                    }

                    simstack23.Push(new SqlToken(SqlTokenType.From, kkj));
                }
                else if (sss.Type == SqlTokenType.Select)
                {
                    if (simstack23.Count == 0)
                        return "неверен синтаксис оператора " + sss.Text + " (не хватает аргументов)";

                    SqlToken sr = simstack23.Pop();
                    string kkj = sr.Text;
                    if (findwhere)
                    {
                        kkj = "SELECT " + sss.Text + " ";
                        kkj += sr.Text.Substring(9);
                    }
                    else
                    {
                        kkj = "SELECT " + sss.Text + " FROM ( ";
                        kkj += sr.Text;
                        kkj += " ) ";
                    }

                    simstack23.Push(new SqlToken(SqlTokenType.From, kkj));
                    findselect = true;
                }
                else
                {
                    simstack23.Push(sss);
                }
            }

            SqlTokens.Clear();
            foreach (SqlToken st in simstack23)
                SqlTokens.Add(st);

            for (int i = 0; i < SqlTokens.Count; i++)
            {
                if (SqlTokens[i].Type == SqlTokenType.From)
                {
                    SqlText.Add(SqlTokens[i].Text);
                }
                else
                {
                    SqlText.Add(SqlTokens[i].Text);
                }
            }

            return "ok";
        }
        
        public static int CountWords(string s, string s0)
        {
            return (s.Length - s.Replace(s0, string.Empty).Length) / s0.Length;
        }
        
        public static string ParseLine(string str)
        {
            var result = System.Text.RegularExpressions.Regex.Replace(str, " +", " ");

            result = System.Text.RegularExpressions.Regex.Replace(result, @"\s+", " ");
            result = System.Text.RegularExpressions.Regex.Replace(result, @"[;:.&^%$#@!?№~|\/]", string.Empty);

            return result.Trim();
        }

        #endregion
    }
}