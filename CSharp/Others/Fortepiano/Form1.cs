using System;
using System.Windows.Forms;

namespace University.Fortepiano
{
    public struct PianoKeyInfo
    {
        public int CurrentTime;
        public int MoveSpeed;
        public bool IsRun;
    }

    public partial class Form1 : Form
    {
        #region Fields and properties

        private const int Speed = 10;
        private const int KeyCount = 16;
        private Button[] buttons;
        private PianoKeyInfo[] keyInfos;

        #endregion

        #region Constructors

        public Form1()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        private void Form1_Load(object sender, EventArgs e)
        {
            keyInfos = new PianoKeyInfo[KeyCount];
            buttons = new Button[KeyCount];
            
            for (int i = 0; i < KeyCount; i++)
            {
                keyInfos[i].CurrentTime = 0;
                keyInfos[i].IsRun = false;
                keyInfos[i].MoveSpeed = -Speed;
            
                buttons[i] = new Button();
                buttons[i].SetBounds(20 + i * 40, 95, 30, 50);
                buttons[i].Text = "I";
                Controls.Add(buttons[i]);
            }

            Focus();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < KeyCount; i++)
            {
                if (keyInfos[i].IsRun)
                {
                    keyInfos[i].CurrentTime++;
                    buttons[i].SetBounds(buttons[i].Location.X, 
                        buttons[i].Location.Y + keyInfos[i].MoveSpeed, 
                        buttons[i].Size.Width,
                        buttons[i].Size.Height);
                    
                    if (buttons[i].Location.Y > 100)
                        keyInfos[i].MoveSpeed = -Speed;
                    else if (buttons[i].Location.Y < 80) 
                        keyInfos[i].MoveSpeed = Speed;

                    if (keyInfos[i].CurrentTime > 40)
                    {
                        keyInfos[i].CurrentTime = 0;
                        keyInfos[i].IsRun = false;
                    }
                }
            }     
        }

        private void button1_KeyDown(object sender, KeyEventArgs e)
        {
            var random = new Random();
            var r = random.Next(0, KeyCount - 1);
            keyInfos[r].IsRun = true;
            keyInfos[r].MoveSpeed = -Speed;
        }

        #endregion
    }
}