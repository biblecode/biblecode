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
    public partial class FormSearchTwo : Form
    {
        public FormSearchTwo()
        {
            InitializeComponent();

            //focus
            this.comboBox1.Focus();
        }

        private void HebrewAdd(String str2add)
        {
            int cursor;
            if (this.comboBox1.Focused)
            {
                cursor = this.comboBox1.SelectionStart;
                this.comboBox1.Text =
                    this.comboBox1.Text.Substring(0, this.comboBox1.SelectionStart) +
                    str2add +
                    this.comboBox1.Text.Substring(this.comboBox1.SelectionStart + this.comboBox1.SelectionLength);
                this.comboBox1.SelectionStart = cursor + str2add.Length;
            }
            if (this.comboBox2.Focused)
            {
                cursor = this.comboBox2.SelectionStart;
                this.comboBox2.Text =
                    this.comboBox2.Text.Substring(0, this.comboBox2.SelectionStart) +
                    str2add +
                    this.comboBox2.Text.Substring(this.comboBox2.SelectionStart + this.comboBox2.SelectionLength);
                this.comboBox2.SelectionStart = cursor + str2add.Length;
            }
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

        private void comboBox1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.button1_Click(sender, e);
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            String ok = "";

            for (int i = 0; i < this.comboBox1.Text.Length; i++)
            {
                if (this.IsLetter(this.comboBox1.Text.ToCharArray()[i]))
                {
                    ok += this.comboBox1.Text.ToCharArray()[i];
                }
            }

            this.comboBox1.Text = ok;
            this.comboBox1.Select(ok.Length, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String tmp = this.comboBox1.Text;
            this.comboBox1.Text = this.comboBox2.Text;
            this.comboBox2.Text = tmp;
        }

        private void FormSearchTwo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                if (this.comboBox1.Text.Length == 0)
                {
                    e.Cancel = true;
                    MessageBox.Show
                    (
                        "Expression 1 should contain at least 1 character.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
            
        }

        private void comboBox1_TextChanged_1(object sender, EventArgs e)
        {
            String ok = "";
            int cursor = this.comboBox1.SelectionStart;
            int originalLength = this.comboBox1.Text.Length;

            for (int i = 0; i < this.comboBox1.Text.Length; i++)
            {
                if (this.IsLetter(this.comboBox1.Text.ToCharArray()[i]))
                {
                    ok += this.comboBox1.Text.ToCharArray()[i];
                }
            }

            ok = ok.Replace('ך', 'כ');
            ok = ok.Replace('ם', 'מ');
            ok = ok.Replace('ן', 'נ');
            ok = ok.Replace('ף', 'פ');
            ok = ok.Replace('ץ', 'צ');

            this.comboBox1.Text = ok;

            if (originalLength - ok.Length == 0)
            {
                this.comboBox1.Select(cursor, 0);
            }
            else if (originalLength - ok.Length == 1)
            {
                this.comboBox1.Select(cursor - 1, 0);
            }
            else
            {
                this.comboBox1.Select(ok.Length - 1, 0);
            }
        }

        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            String ok = "";
            int cursor = this.comboBox2.SelectionStart;
            int originalLength = this.comboBox2.Text.Length;

            for (int i = 0; i < this.comboBox2.Text.Length; i++)
            {
                if (this.IsLetter(this.comboBox2.Text.ToCharArray()[i]))
                {
                    ok += this.comboBox2.Text.ToCharArray()[i];
                }
            }

            ok = ok.Replace('ך', 'כ');
            ok = ok.Replace('ם', 'מ');
            ok = ok.Replace('ן', 'נ');
            ok = ok.Replace('ף', 'פ');
            ok = ok.Replace('ץ', 'צ');

            this.comboBox2.Text = ok;

            if (originalLength - ok.Length == 0)
            {
                this.comboBox2.Select(cursor, 0);
            }
            else if (originalLength - ok.Length == 1)
            {
                this.comboBox2.Select(cursor - 1, 0);
            }
            else
            {
                this.comboBox2.Select(ok.Length - 1, 0);
            }
        }
    }
}
