using System.Collections.Generic;

namespace University.ARTQ
{
    /// <summary>
    /// Класс разбора выражений.
    /// </summary>
    public sealed class Lexer 
    {
        #region Fields and Properties
        // пересечение
        private const string OperatorIntersect = "\u2229";

        // объединение
        private const string OperatorUnion = "\u222A";

        // минус
        private const string OperatorMinus = "\u2212";

        // PI
        private const string OperatorPI = "\u220F";
        private const string gm_MR = "\u2264";

        // sigma
        private const string OperatorSigma = "\u03c3";
        private const string gm_BR = "\u2265";

        // join
        private const string OperatorJoin = "\u22c8";
        private const string gm_NR = "\u2260";
        
        // Query Tree
        private Query MainTreeNode;
        private List<Query> OtherTreeNode;
        
        public List<string> ReservedOperators { get; set; }
        
        public string AlgebraText { get; set; }
        
        public List<string> m_SQL_Text;
        public int m_PrevToken, m_Offset;
        public List<RelationalAlgebraToken> m_TokenList;
        public List<SqlToken> m_BlockTokenList;
        
        
        

        #endregion

        #region Constructors
        public Lexer()
        {
            ReservedOperators = new List<string>()
            {
                OperatorIntersect,
                OperatorUnion,
                OperatorMinus,
                OperatorPI,
                OperatorSigma,
                OperatorJoin,
                gm_MR,
                gm_BR,
                gm_NR,
                ">",
                "<",
                "=",
                " OR ",
                " AND "
            };
            
            m_SQL_Text = new List<string>();
            m_TokenList = new List<RelationalAlgebraToken>();
            m_BlockTokenList = new List<SqlToken>();
        }
        #endregion

        #region Methods
        public bool WorkAll(string text_algebra)
        {
            Start(text_algebra);
            if (SimpleAn() != "ok") return false;
            if (BlockedText() != "ok") return false;
            PostfixFormat();
            if (Parser() != "ok") return false;
            return true;
        }

        public static int CountWords(string s, string s0)
        {
            int count = (s.Length - s.Replace(s0, "").Length) / s0.Length;
            return count;
        }
        public static string ParseLine(string str)
        {
            string ttt = System.Text.RegularExpressions.Regex.Replace(str, " +", " ");
            ttt = System.Text.RegularExpressions.Regex.Replace(ttt, @"\s+", " ");
            ttt = System.Text.RegularExpressions.Regex.Replace(ttt, @"[;:.&^%$#@!?№~|\/]", "");
            return ttt.Trim();
        }

        // получаем строку и разбиваем ее на токены
        public bool Start(string originalText)
        {
            AlgebraText = originalText;

            m_PrevToken = 0;
            RelationalAlgebraToken ErrorToken = new ();

            m_Offset = 0;
            m_TokenList.Clear();

            for (int i = 0; i < originalText.Length; i++)
            {
                if (originalText[i] == ' ')
                    continue;

                char symbol = originalText[i];


                if (symbol == '(') m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.SeparatorOpen, "("));
                else if (symbol == '[') m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.SeparatorOpen, "["));
                else if (symbol == '{') m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.SeparatorOpen, "{"));
                else if (symbol == ')') m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.SeparatorClose, ")"));
                else if (symbol == ']') m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.SeparatorClose, "]"));
                else if (symbol == '}') m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.SeparatorClose, "}"));
                else if (symbol == '\u2229') m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Operator, "INTERSECT"));
                else if (symbol == '\u222A') m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Operator, "UNION"));
                else if (symbol == '\u2212') m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Operator, "MINUS"));
                else if (symbol == '\u220F') m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Operator, "PI"));
                else if (symbol == '\u03c3') m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Operator, "SIGMA"));
                else if (symbol == '\u22c8') m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Operator, "JOIN"));
                else if (symbol == '\u2264') m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Logic, "\u2264")); // <=
                else if (symbol == '\u2265') m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Logic, "\u2265")); // >=
                else if (symbol == '\u2260') m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Logic, "\u2260")); // !=
                else if (symbol == '>') m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Logic, ">"));
                else if (symbol == '<') m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Logic, "<"));
                else if (symbol == '=') m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Logic, "="));
                else if (symbol == ',') m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Separator, ","));
                else
                {
                    int h = originalText.IndexOfAny((" <,>=(){}[]\u2229\u222A\u220F\u03c3\u22c8\u2264\u2265\u2260\u2212").ToCharArray(), i);
                    if (h == -1)
                    {
                        m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Table, originalText.Substring(i, originalText.Length - i)));
                        break;
                    }
                    else
                    {
                        m_TokenList.Add(new RelationalAlgebraToken(RelationalAlgebraTokenType.Table, originalText.Substring(i, h - i)));
                        i = h - 1;
                    }
                }
                
            }

            for (int i = 0; i < m_TokenList.Count; i++)
            {
                if (m_TokenList[i].Type == RelationalAlgebraTokenType.Table || m_TokenList[i].Type == RelationalAlgebraTokenType.Unknown)
                {
                    string z_ts = m_TokenList[i].Text.ToUpper();
                    if (z_ts == "INTERSECT" || z_ts == "UNION" || z_ts == "PI" || z_ts == "SIGMA" || z_ts == "MINUS" || z_ts == "JOIN")
                    {
                        m_TokenList[i].Text = z_ts;
                        m_TokenList[i].Type = RelationalAlgebraTokenType.Operator;
                    }
                    else if (z_ts == "OR" || z_ts == "AND")
                    {
                        m_TokenList[i].Text = z_ts;
                        m_TokenList[i].Type = RelationalAlgebraTokenType.Logic;
                    }
                }
            }
            return true;
        }

        // Простые проверки на корректность выражения
        public string SimpleAn()
        {
            if (m_TokenList.Count < 1) return "Нет токенов";
            if (m_TokenList[0].Type != RelationalAlgebraTokenType.Table && m_TokenList[0].Type != RelationalAlgebraTokenType.Operator && m_TokenList[0].Type != RelationalAlgebraTokenType.SeparatorOpen)
                return "Запись должна начинаться с оператора, отношения или открывающей скобки";
            return "ok";
        }

        // группируем токкены в блоки
        // например, объединяем все токены, принадлежащие к одному оператору Proj, в один блок - SELECT
        public string BlockedText()
        {
            MainTreeNode = new Query();
            OtherTreeNode = new List<Query>();
            m_BlockTokenList.Clear();

            for (int i = 0; i < m_TokenList.Count; i++)
            {
                if (m_TokenList[i].Type == RelationalAlgebraTokenType.Operator)
                {
                    if (m_TokenList[i].Text == "PI")
                    {
                        string z_str = "";
                        if (i + 1 < m_TokenList.Count)
                        {
                            if (m_TokenList[i + 1].Type != RelationalAlgebraTokenType.SeparatorOpen)
                            {
                                return "Ошибка в синтаксисе оператора проекции.\n === === ===\nPI ([аргументы])(отношение)\n === === ===\nСкобки для аргументов не опознаны";
                            }
                        }
                        else
                        {
                            return "Ошибка в синтаксисе оператора проекции.\n === === ===\nPI ([аргументы])(отношение)\n === === ===\nСкобки для аргументов не опознаны";
                        }

                        bool z_FindResult = false;
                        int z_CountOpens = 0;
                        int j;
                        for (j = i + 2; j < m_TokenList.Count; j++)
                        {
                            if (m_TokenList[j].Type == RelationalAlgebraTokenType.SeparatorClose)
                            {
                                if (z_CountOpens > 0)
                                {
                                    z_CountOpens--;
                                    z_str += ")";
                                }
                                else
                                {
                                    if (!z_FindResult) z_str += " * ";
                                    break;
                                }
                            }
                            else if (m_TokenList[j].Type == RelationalAlgebraTokenType.SeparatorOpen)
                            {
                                z_CountOpens++;
                                z_str += " (";
                            }
                            else
                            {
                                z_FindResult = true;
                                z_str += " " + m_TokenList[j].Text;
                            }
                        }
                        i = j;
                        m_BlockTokenList.Add(new SqlToken(SqlTokenType.Select, z_str));
                    }
                    else if (m_TokenList[i].Text == "SIGMA")
                    {
                        string z_str = "";
                        if (i + 1 < m_TokenList.Count)
                        {
                            if (m_TokenList[i + 1].Type != RelationalAlgebraTokenType.SeparatorOpen)
                            {
                                return "Ошибка в синтаксисе оператора выборки.\n === === ===\nSIGMA ([аргументы])(отношение)\n === === ===\nСкобки для аргументов не опознаны";
                            }
                        }
                        else
                        {
                            return "Ошибка в синтаксисе оператора выборки.\n === === ===\nSIGMA ([аргументы])(отношение)\n === === ===\nСкобки для аргументов не опознаны";
                        }

                        bool z_FindResult = false;
                        int z_CountOpens = 0;
                        int j;
                        for (j = i + 2; j < m_TokenList.Count; j++)
                        {
                            if (m_TokenList[j].Type == RelationalAlgebraTokenType.SeparatorClose)
                            {
                                if (z_CountOpens > 0)
                                {
                                    z_CountOpens--;
                                    z_str += ")";
                                }
                                else
                                {
                                    if (!z_FindResult) z_str += " * ";
                                    break;

                                }
                            }
                            else if (m_TokenList[j].Type == RelationalAlgebraTokenType.SeparatorOpen)
                            {
                                z_CountOpens++;
                                z_str += " (";
                            }
                            else
                            {
                                z_FindResult = true;
                                z_str += " " + m_TokenList[j].Text;
                            }
                        }
                        i = j;
                        m_BlockTokenList.Add(new SqlToken(SqlTokenType.Where, z_str));
                    }
                    else
                    {
                        m_BlockTokenList.Add(new SqlToken(SqlTokenType.Operator, m_TokenList[i].Text));
                    }
                }
                else if (m_TokenList[i].Type == RelationalAlgebraTokenType.SeparatorOpen)
                {
                    m_BlockTokenList.Add(new SqlToken(SqlTokenType.SeparatorOpen, "("));
                }
                else if (m_TokenList[i].Type == RelationalAlgebraTokenType.SeparatorClose)
                {
                    m_BlockTokenList.Add(new SqlToken(SqlTokenType.SeparatorClose, ")"));
                }
                else
                {
                    if (m_BlockTokenList.Count != 0)
                    {
                        if (m_BlockTokenList[m_BlockTokenList.Count - 1].Type == SqlTokenType.From)
                            m_BlockTokenList[m_BlockTokenList.Count - 1].Text += " " + m_TokenList[i].Text;
                        else m_BlockTokenList.Add(new SqlToken(SqlTokenType.From, m_TokenList[i].Text));
                    }
                    else m_BlockTokenList.Add(new SqlToken(SqlTokenType.From, m_TokenList[i].Text));
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
            List<SqlToken> z_Block = new List<SqlToken>();
            Stack<SqlToken> z_Stack = new Stack<SqlToken>();

            for (int i = 0; i < m_BlockTokenList.Count; i++)
            {
                if (z_Stack.Count > 0 && m_BlockTokenList[i].Type != SqlTokenType.SeparatorOpen)
                {
                    if (m_BlockTokenList[i].Type == SqlTokenType.SeparatorClose)
                    {
                        SqlToken s = z_Stack.Pop();
                        while (s.Type != SqlTokenType.SeparatorOpen)
                        {
                            z_Block.Add(s);
                            //if (z_Stack.Count == 0) { return; }
                            s = z_Stack.Pop();
                        }
                    }
                    else
                    {
                        if (GetPr(m_BlockTokenList[i]) >= GetPr(z_Stack.Peek()))
                            z_Stack.Push(m_BlockTokenList[i]);
                        else
                        {
                            while (z_Stack.Count > 0 && GetPr(m_BlockTokenList[i]) < GetPr(z_Stack.Peek()))
                                z_Block.Add(z_Stack.Pop());
                            z_Stack.Push(m_BlockTokenList[i]);
                        }
                    }
                }
                else z_Stack.Push(m_BlockTokenList[i]);
            }
            if (z_Stack.Count > 0)
            {
                foreach (SqlToken c in z_Stack)
                    z_Block.Add(c);
            }

            m_BlockTokenList.Clear();

            foreach (SqlToken c in z_Block)
                m_BlockTokenList.Add(c);
        }

        // по последовательности блоков (в польской записи) создаем SQL запрос
        public string Parser()
        {
            m_SQL_Text.Clear();

            string zSTRWhere, zSTRSelect, zSTRFrom;

            zSTRWhere = "";
            zSTRSelect = " * ";
            zSTRFrom = "";

            List<int> todeletelist = new List<int>();
            for (int i = 0; i < m_BlockTokenList.Count; i++)
            {

                if (m_BlockTokenList[i].Type == SqlTokenType.From)
                {
                    int kk = i;
                    zSTRFrom = m_BlockTokenList[i].Text;
                    i++;
                    while (i < m_BlockTokenList.Count &&
                           m_BlockTokenList[i].Type != SqlTokenType.Operator &&
                           m_BlockTokenList[i].Type != SqlTokenType.From)
                    {
                        if (m_BlockTokenList[i].Type == SqlTokenType.Select)
                        {
                            zSTRSelect = m_BlockTokenList[i].Text;
                            todeletelist.Add(i);
                        }
                        else if (m_BlockTokenList[i].Type == SqlTokenType.Where)
                        {
                            zSTRWhere = m_BlockTokenList[i].Text;
                            todeletelist.Add(i);
                        }

                        i++;
                    }

                    m_BlockTokenList[kk].Text = (" SELECT " + zSTRSelect + " FROM " + zSTRFrom +
                                                   ((zSTRWhere.Length > 0) ? (" WHERE " + zSTRWhere) : (""))
                        );
                    zSTRWhere = "";
                    zSTRSelect = " * ";
                    zSTRFrom = "";
                    i = kk;
                }
            }

            for (int i = todeletelist.Count - 1; i >= 0; i--)
            {
                m_BlockTokenList.RemoveAt(todeletelist[i]);
            }

            bool findwhere = false, findselect = false;
            Stack<SqlToken> simstack23 = new Stack<SqlToken>();
            List<string> outst23 = new List<string>();
            foreach (SqlToken sss in m_BlockTokenList)
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

            m_BlockTokenList.Clear();
            foreach (SqlToken st in simstack23)
                m_BlockTokenList.Add(st);

            for (int i = 0; i < m_BlockTokenList.Count; i++)
            {

                if (m_BlockTokenList[i].Type == SqlTokenType.From)
                {
                    m_SQL_Text.Add(m_BlockTokenList[i].Text);
                }
                else
                {
                    m_SQL_Text.Add(m_BlockTokenList[i].Text);
                }
            }

            return "ok";
        }
        #endregion

        #region Data structs
        private struct Query
        {
            private string SelectText;
            private string FromText;
            private string WhereText;
        };
        #endregion
    }
}