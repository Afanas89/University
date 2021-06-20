using System;
using System.Windows.Forms;

namespace University.ARTQ
{
    public partial class MainForm : Form
    {
        #region Fields and properties

        private Lexer lexer;

        #endregion

        #region Constructors

        public MainForm()
        {
            InitializeComponent(); 
        }

        #endregion

        #region Methods

        private void MainForm_Load(object sender, EventArgs e)
        {
            lexer = new Lexer();

            button3.Text = Operators.OperatorBr;
            button4.Text = Operators.OperatorMr;
            button5.Text = Operators.OperatorNr;

            button8.Text = Operators.OperatorSigma;
            button7.Text = Operators.OperatorPI;
            button6.Text = Operators.OperatorJoin;

            button11.Text = Operators.OperatorUnion;
            button10.Text = Operators.OperatorIntersect;
            button9.Text = Operators.OperatorMinus;
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(richTextBox1.Text))
            {
                MessageBox.Show("Введите текст","Синтаксис", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
                
            string sourceText = richTextBox1.Text;
            
            var count1 = Lexer.CountWords(sourceText, "(");
            var count2 = Lexer.CountWords(sourceText, ")");
            var count3 = Lexer.CountWords(sourceText, "[");
            var count4 = Lexer.CountWords(sourceText, "]");
            var count5 = Lexer.CountWords(sourceText, "{");
            var count6 = Lexer.CountWords(sourceText, "}");
            
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
                MessageBox.Show(errorText, "Синтаксис", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            string parsedText = Lexer.ParseLine(sourceText);
            
            if (MainMenu_IsDebug.Checked)
                richTextBox2.Text = parsedText;
            else 
                richTextBox2.Text = string.Empty;
            
            lexer.Start(parsedText);
            
            if (MainMenu_IsDebug.Checked)
            {
                richTextBox2.Text += "\n\n=============  TOKENS  ============================\n";
                        
                foreach (var token in lexer.AlgebraTokens)
                {
                    Console.WriteLine($"[{token.Type}] {token.Text}");
                }
            }

            string resultSimpleAnalyze = lexer.SimpleAn();
            if (resultSimpleAnalyze != "ok") 
            {
                MessageBox.Show(resultSimpleAnalyze, "Анализ (стадия 1)");
                richTextBox2.Text += $"\n\n=== ОШИБКА! Анализ (стадия 1)\n{resultSimpleAnalyze}\n===\n\n";
                return;
            }

            string resultBlockedText = lexer.BlockedText();
            if (resultBlockedText != "ok") 
            {
                MessageBox.Show(resultBlockedText, "Парсер в блоки");
                richTextBox2.Text += $"\n\n=== ОШИБКА! Парсер в блоки\n{resultBlockedText}\n===\n\n";
                return;
            }
            
            if (MainMenu_IsDebug.Checked)
            {
                richTextBox2.Text += "\n\n=============  BLOCKS  ============================\n";

                foreach (var token in lexer.SqlTokens)
                {
                    Console.WriteLine($"[{token.Type}] {token.Text}");
                }

                richTextBox2.Text += "\n\n==POSTFIX==\n";
            }

            lexer.PostfixFormat();
            if (MainMenu_IsDebug.Checked)
            {
                foreach (var token in lexer.SqlTokens)
                {
                    Console.WriteLine($"[{token.Type}] {token.Text}");
                }

                richTextBox2.Text += "\n\n=============  SQL  ================================\n";
            }
            
            string resultParse = lexer.Parser();
            if (resultParse != "ok")
            {
                MessageBox.Show(resultParse, "Парсер в SQL");
                richTextBox2.Text += $"\n\n=== ОШИБКА! Парсер в SQL\n{resultParse}\n===\n\n";
                return;
            }

            if (MainMenu_IsDebug.Checked)
            {
                foreach (var token in lexer.SqlTokens)
                {
                    Console.WriteLine($"[{token.Type}] {token.Text}");
                }

                richTextBox2.Text += "\n\n=============  SQL  ================================\n";
            }
            
            foreach (var token in lexer.SqlText)
            {
                richTextBox2.Text += $"\n{token}";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }
        
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "A  ∩ (∏(x)(A, B)∪σ(a≠6  AND z >7)(T))";
        }

        private void MainMenu_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void транслироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1_Click(sender, e);
        }

        private void MainMenu_IsUnicode_Click(object sender, EventArgs e)
        {
            if (MainMenu_IsUnicode.Checked)
            {
                button3.Text = Operators.OperatorBr;
                button4.Text = Operators.OperatorMr;
                button5.Text = Operators.OperatorNr;

                button8.Text = Operators.OperatorSigma;
                button7.Text = Operators.OperatorPI;
                button6.Text = Operators.OperatorJoin;

                button11.Text = Operators.OperatorUnion;
                button10.Text = Operators.OperatorIntersect;
                button9.Text = Operators.OperatorMinus;
            }
            else
            {
                button3.Text = ">=";
                button4.Text = "<=";
                button5.Text = "!=";

                button8.Text = "SIGMA";
                button7.Text = "PI";
                button6.Text = "JOIN";

                button11.Text = "UNION";
                button10.Text = "INTERSECT";
                button9.Text = "MINUS";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = "\u2265";
            richTextBox1.Focus();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = "\u2264";
            richTextBox1.Focus();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = "\u2260";
            richTextBox1.Focus();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = "\u03c3";
            richTextBox1.Focus();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = "\u220F";
            richTextBox1.Focus();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = "\u22c8";
            richTextBox1.Focus();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = "\u222A";
            richTextBox1.Focus();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = "\u2229";
            richTextBox1.Focus();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = "\u2212";
            richTextBox1.Focus();
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "у∩∏(г)(ц∩ф∪σ(ы)(в∩а))";
        }

        private void добавитьWhereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button8_Click(sender, e);
        }

        private void добавитьSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button7_Click(sender, e);
        }

        private void добавитьJoinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button6_Click(sender, e);
        }

        private void добавитьUnionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button11_Click(sender, e);
        }

        private void добавитьIntersectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button10_Click(sender, e);
        }

        private void добавитьMinusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button9_Click(sender, e);
        }
    }
}