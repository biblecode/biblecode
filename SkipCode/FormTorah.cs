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
    public partial class FormTorah : Form
    {
        private Torah torah = new Torah();
        private Torah.Versions version = Torah.Versions.Original;
        
        public FormTorah()
        {
            InitializeComponent();

            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 0;
            this.comboBox3.SelectedIndex = 0;
            this.comboBox4.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex >= 0)
            {
                this.comboBox2.Items.Clear();

                List<String> chapters = this.torah.GetChapterNumbers
                (
                    this.comboBox1.SelectedIndex + 1,
                    this.version
                );

                List<String>.Enumerator en = chapters.GetEnumerator();
                while(en.MoveNext())
                {
                    this.comboBox2.Items.Add(en.Current);
                }

                if (sender.Equals(this.comboBox1))
                {
                    this.comboBox2.SelectedIndex = 0;
                }
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox2.SelectedIndex >= 0)
            {
                this.comboBox3.Items.Clear();

                List<String> verses = this.torah.GetVerseNumbers
                (
                    this.comboBox1.SelectedIndex + 1,
                    int.Parse(this.comboBox2.SelectedItem.ToString()),
                    this.version
                );

                List<String>.Enumerator en = verses.GetEnumerator();

                while (en.MoveNext())
                {
                    this.comboBox3.Items.Add(en.Current);
                }

                if(sender.Equals(this.comboBox2))
                {
                    this.comboBox3.SelectedIndex = 0;
                }
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox3.SelectedIndex >= 0)
            {
                Torah.Verse verse = this.torah.GetVerse
                (
                    this.comboBox1.SelectedIndex + 1,
                    int.Parse(this.comboBox2.SelectedItem.ToString()),
                    int.Parse(this.comboBox3.SelectedItem.ToString()),
                    this.version
                );

                this.textBox.Text = verse.Text;
            }
        }

        private void FormTorah_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox4.SelectedIndex == 0)   
            {
                this.version = Torah.Versions.Original;
                this.textBox.TextAlign = HorizontalAlignment.Right;
            }
            else if (comboBox4.SelectedIndex == 1)
            {
                this.version = Torah.Versions.KingJames;
                this.textBox.TextAlign = HorizontalAlignment.Left;
            }
            else if (comboBox4.SelectedIndex == 2)
            {
                this.version = Torah.Versions.Sirach;
                this.textBox.TextAlign = HorizontalAlignment.Left;
            }

            this.comboBox3_SelectedIndexChanged(sender, e);
        }
    }
}
