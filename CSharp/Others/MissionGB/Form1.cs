using System;
using System.Drawing;
using System.Windows.Forms;

namespace University.MissionGB
{
    public partial class Form1 : Form
    {
        #region Fields and properties

        private int money;
        private int addMoney;
        private readonly Random rd;
        private Color[] simpleColor;
        private int koef1;
        private int koef2;
        private int koef3;
        private int koef4;
        private bool bStarted;
        private bool bGet;

        #endregion 

        #region Constructors

        public Form1()
        {
            InitializeComponent();
            rd = new Random();
            money = 100;
            addMoney = 0;
            koef4 = -20;
        }

        #endregion

        #region Methods

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (bGet)
            {
                bGet = false;
                bStarted = false;
            }

            if (bStarted)
            {
                panel1.BackColor = panel2.BackColor = simpleColor[rd.Next(0, 5)];
                panel3.BackColor = panel4.BackColor = panel5.BackColor = panel6.BackColor = simpleColor[rd.Next(0, 5)];
                
                if (bStarted && trackBar1.Value == 10)
                {
                    bStarted = false;
                    bGet = true;
                    trackBar1.Enabled = true;
                    var lisi1 = Convert.ToInt32(listBox1.SelectedItem);
                    var lisi2 = Convert.ToInt32(listBox2.SelectedItem);
                    var lisi3 = Convert.ToInt32(listBox3.SelectedItem);

                    listBox1.Items.Clear();
                    listBox1.Items.Add((lisi1 - 1).ToString());
                    listBox1.Items.Add((lisi1).ToString());
                    listBox1.Items.Add((lisi1 + 1).ToString());

                    listBox2.Items.Clear();
                    listBox2.Items.Add((lisi2 - 1).ToString());
                    listBox2.Items.Add((lisi2).ToString());
                    listBox2.Items.Add((lisi2 + 1).ToString());

                    listBox3.Items.Clear();
                    listBox3.Items.Add((lisi3 - 1).ToString());
                    listBox3.Items.Add((lisi3).ToString());
                    listBox3.Items.Add((lisi3 + 1).ToString());

                    if (lisi1 == lisi2) addMoney += 10;
                    if (lisi2 == lisi3) addMoney += 10;
                    if (lisi1 == lisi3) addMoney += 30;
                    label4.Text = addMoney.ToString();
                    money += addMoney;
                    label2.Text = money.ToString();
                }
                else
                {
                    if (listBox1.SelectedIndex + 1 >= 10) listBox1.SelectedIndex = 1;
                    if (listBox2.SelectedIndex - 1 <= 1) listBox2.SelectedIndex = 10;
                    if (listBox3.SelectedIndex + 1 >= 10) listBox3.SelectedIndex = 1;
                    listBox1.SelectedIndex += koef1;
                    listBox2.SelectedIndex += koef2;
                    listBox3.SelectedIndex += koef3;
                    trackBar1.Value++;
                }
            }
            else
            {
                if (panel1.BackColor.A <= 20) 
                    koef4 = 20;
                else if (panel1.BackColor.A >= 230) 
                    koef4 = -20;
                
                panel1.BackColor = Color.FromArgb(panel1.BackColor.A + koef4, panel1.BackColor.R, panel1.BackColor.G, panel1.BackColor.B);
                panel2.BackColor = Color.FromArgb(panel2.BackColor.A + koef4, panel1.BackColor.R, panel1.BackColor.G, panel1.BackColor.B);
                panel3.BackColor = Color.FromArgb(panel3.BackColor.A + koef4, panel1.BackColor.R, panel1.BackColor.G, panel1.BackColor.B);
                panel4.BackColor = Color.FromArgb(panel4.BackColor.A + koef4, panel1.BackColor.R, panel1.BackColor.G, panel1.BackColor.B);
                panel5.BackColor = Color.FromArgb(panel5.BackColor.A + koef4, panel1.BackColor.R, panel1.BackColor.G, panel1.BackColor.B);
                panel6.BackColor = Color.FromArgb(panel6.BackColor.A + koef4, panel1.BackColor.R, panel1.BackColor.G, panel1.BackColor.B);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bStarted = false;
            bGet = false;
            koef1 = 1;
            koef2 = -1;
            koef3 = 1;
            simpleColor = new Color[6];
            simpleColor[0] = Color.Red;
            simpleColor[1] = Color.Green;
            simpleColor[2] = Color.Blue;
            simpleColor[3] = Color.HotPink;
            simpleColor[4] = Color.Yellow;
            simpleColor[5] = Color.Orange;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (trackBar1.Value == 0 && !bStarted)
            {
                listBox1.Items.Clear();
                listBox1.Items.Add("9");
                listBox1.Items.Add("0");
                listBox1.Items.Add("1");
                listBox1.Items.Add("2");
                listBox1.Items.Add("3");
                listBox1.Items.Add("4");
                listBox1.Items.Add("5");
                listBox1.Items.Add("6");
                listBox1.Items.Add("7");
                listBox1.Items.Add("8");
                listBox1.Items.Add("9");
                listBox1.Items.Add("0");
                listBox2.Items.Clear();
                listBox2.Items.Add("9");
                listBox2.Items.Add("0");
                listBox2.Items.Add("1");
                listBox2.Items.Add("2");
                listBox2.Items.Add("3");
                listBox2.Items.Add("4");
                listBox2.Items.Add("5");
                listBox2.Items.Add("6");
                listBox2.Items.Add("7");
                listBox2.Items.Add("8");
                listBox2.Items.Add("9");
                listBox2.Items.Add("0");
                listBox3.Items.Clear();
                listBox3.Items.Add("9");
                listBox3.Items.Add("0");
                listBox3.Items.Add("1");
                listBox3.Items.Add("2");
                listBox3.Items.Add("3");
                listBox3.Items.Add("4");
                listBox3.Items.Add("5");
                listBox3.Items.Add("6");
                listBox3.Items.Add("7");
                listBox3.Items.Add("8");
                listBox3.Items.Add("9");
                listBox3.Items.Add("0");
                koef1 = rd.Next(1, 3);
                koef2 = rd.Next(-3, -1);
                koef3 = rd.Next(1, 3);
                panel1.BackColor = Color.FromArgb(255, panel1.BackColor.R, panel1.BackColor.G, panel1.BackColor.B);
                panel2.BackColor = Color.FromArgb(255, panel1.BackColor.R, panel1.BackColor.G, panel1.BackColor.B);
                panel3.BackColor = Color.FromArgb(255, panel1.BackColor.R, panel1.BackColor.G, panel1.BackColor.B);
                panel4.BackColor = Color.FromArgb(255, panel1.BackColor.R, panel1.BackColor.G, panel1.BackColor.B);
                panel5.BackColor = Color.FromArgb(255, panel1.BackColor.R, panel1.BackColor.G, panel1.BackColor.B);
                panel6.BackColor = Color.FromArgb(255, panel1.BackColor.R, panel1.BackColor.G, panel1.BackColor.B);
                bStarted = true;
                trackBar1.Enabled = false;
                addMoney = -5;
                label4.Text = addMoney.ToString();
            }
        }

        #endregion
    }
}