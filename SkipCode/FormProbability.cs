using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SkipCode
{
    public partial class FormProbability : Form
    {
        public int textLenght = 0;
        public int matrixWidth = 0;
        public int matrixHeight = 0;
        
        public FormProbability()
        {
            InitializeComponent();

            //focus
            this.textBox1.Focus();
        }

        private void HebrewAdd(String str2add)
        {
            int cursor = this.textBox1.SelectionStart;
            this.textBox1.Text =
                this.textBox1.Text.Substring(0, this.textBox1.SelectionStart) +
                str2add +
                this.textBox1.Text.Substring(this.textBox1.SelectionStart + this.textBox1.SelectionLength);
            this.textBox1.SelectionStart = cursor + str2add.Length;
        }

        private bool IsLetter(char c)
        {
            String abeceda = "aábcčdďeéěfghiíjklľmnňoópqrřsštťuúůvwxyýzž";
            abeceda += abeceda.ToUpper();

            abeceda += "אבגדהוזחטיכךלמםנןסעפףצץקרשת";

            return abeceda.Contains(c);
        }

        private void label9_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("א");
        }

        private void label8_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("ב");
        }

        private void label7_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("ג");
        }

        private void label6_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("ד");
        }

        private void label5_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("ה");
        }

        private void label4_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("ו");
        }

        private void label15_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("ז");
        }

        private void label14_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("ח");
        }

        private void label13_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("ט");
        }

        private void label12_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("י");
        }

        private void label11_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("כ");
        }

        private void label10_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("ל");
        }

        private void label21_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("מ");
        }

        private void label20_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("נ");
        }

        private void label19_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("ס");
        }

        private void label18_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("ע");
        }

        private void label17_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("פ");
        }

        private void label16_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("צ");
        }

        private void label22_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("ק");
        }

        private void label23_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("ר");
        }

        private void label24_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("ש");
        }

        private void label25_Click(object sender, EventArgs e)
        {
            this.HebrewAdd("ת");
        }

        private void textBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.button1_Click(sender, e);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            String ok = "";
            int cursor = this.textBox1.SelectionStart;
            int originalLength = this.textBox1.Text.Length;

            for (int i = 0; i < this.textBox1.Text.Length; i++)
            {
                if (this.IsLetter(this.textBox1.Text.ToCharArray()[i]))
                {
                    ok += this.textBox1.Text.ToCharArray()[i];
                }
            }

            ok = ok.Replace('ך', 'כ');
            ok = ok.Replace('ם', 'מ');
            ok = ok.Replace('ן', 'נ');
            ok = ok.Replace('ף', 'פ');
            ok = ok.Replace('ץ', 'צ');

            this.textBox1.Text = ok;

            if (originalLength - ok.Length == 0)
            {
                this.textBox1.Select(cursor, 0);
            }
            else if (originalLength - ok.Length == 1)
            {
                this.textBox1.Select(cursor - 1, 0);
            }
            else
            {
                this.textBox1.Select(ok.Length - 1, 0);
            }

            if (textBox1.Text.Length > 0)
            {
                this.textBox5.Text = Probability.CountProbabiltyOfFindingExpression(this.textBox1.Text).ToString("0.000000E+00");
                this.textBox2.Text = Probability.CountSkipsAndOffsets(this.textBox1.Text.Length, this.textLenght, (int)this.numericUpDown2.Value).ToString("0.000000E+00");
                this.textBox3.Text = Probability.CountForAllCells(this.textBox1.Text.Length, this.matrixWidth, this.matrixHeight).ToString();
                this.textBox4.Text = Probability.EstimateNumberOfFindingExpressionInText(this.textBox1.Text, this.textLenght, (int)this.numericUpDown2.Value).ToString("0.000000E+00");
                this.textBox6.Text = Probability.EstimateNumberOfFindingExpressionInMatrix(this.textBox1.Text, this.matrixWidth, this.matrixHeight).ToString("0.000000E+00");
            }
            else
            {
                this.textBox5.Text = "";
                this.textBox2.Text = "";
                this.textBox3.Text = "";
                this.textBox4.Text = "";
                this.textBox6.Text = "";
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void FormProbability_Load(object sender, EventArgs e)
        {
            this.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                this.textBox2.Text = Probability.CountSkipsAndOffsets(this.textBox1.Text.Length, this.textLenght, (int)this.numericUpDown2.Value).ToString();
                this.textBox4.Text = Probability.EstimateNumberOfFindingExpressionInText(this.textBox1.Text, this.textLenght, (int)this.numericUpDown2.Value).ToString();
            }
        }
    }
}
