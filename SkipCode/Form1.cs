using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Windows.Input;
using System.Threading;
using System.Collections;

namespace SkipCode
{
    public partial class Form1 : Form
    {
        private Matrix DataGridMatrix = new Matrix(30, 30);
        private Torah torah = new Torah();
        private FormTorah formTorah = new FormTorah();

        private String fileText = "RIPSEXPLAINEDTHATEACHCODEISACASEOFADDINGEEVERYFOURTHORTWELFTHORFIFTIETHLETTERTOFORMAWORD";

        private bool searchedInTorah = true;

        private List<Searching.Result> found = new List<Searching.Result>();
        private int foundIndex = 0;

        private ListViewColumnSorter listViewColumnSorter1;
        private ListViewColumnSorter listViewColumnSorter2;

        private Searching.ResultsSorter resultsSorter1;
        private Searching.ResultsSorter resultsSorter2;

        private List<String> lastSearchExpression1 = new List<String>();
        private List<String> lastSearchExpression2 = new List<String>();
        private String lastSearchExpression2Related;
        private bool lastSearchInFile = false;
        private bool lastSearchDeleteLastTwo = true;
        private bool lastSearchDeleteLastMatrix = true;
        private int lastSearchMaxSkip = 2000;
        private int lastSearchWidth = 30;
        private int lastSearchHeight = 30;
        private int lastSearchExpression1Found = -1;
        private int lastSearchExpression2Found = -1;
        private bool lastSearchExpression1Only = true;
        private FileInfo lastSearchFileInfo;
        private bool lastSearchExportMatrix = false;

        private WebBrowser print = new WebBrowser();
        
        public Form1()
        {
            InitializeComponent();

            //list of find codes
            this.listViewColumnSorter1 = new ListViewColumnSorter();
            this.listView1.ListViewItemSorter = this.listViewColumnSorter1;
            this.resultsSorter1 = new Searching.ResultsSorter();

            //related findings
            this.listViewColumnSorter2 = new ListViewColumnSorter();
            this.listView2.ListViewItemSorter = this.listViewColumnSorter2;
            this.resultsSorter2 = new Searching.ResultsSorter();

            //don't search without nothing to search in
            this.searchInMatrixToolStripMenuItem.Enabled = false;
        }

        private void openFileToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (this.lastSearchFileInfo != null)
            {
                this.openFileDialog1.InitialDirectory = this.lastSearchFileInfo.DirectoryName;
            }
            else
            {
                this.openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
            }
            if (this.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.Cursor = System.Windows.Forms.Cursors.WaitCursor;
                this.lastSearchExportMatrix = false;

                this.lastSearchFileInfo = new FileInfo(this.openFileDialog1.FileName);
                StreamReader sr = new StreamReader(this.openFileDialog1.FileName, true);

                this.fileText = "";

                EscapeKey.StartListening();

                int c;
                for (int i = 0; (c = sr.Read()) != -1; i++)
                {
                    if (this.IsLetter((char)c))
                    {
                        this.fileText += (char)c;
                    }

                    if (EscapeKey.Pressed)
                    {
                        EscapeKey.StopListening();

                        this.fileText = this.fileText.ToUpper();

                        this.toolStripProgressBar1.Value = 0;
                        this.Cursor = System.Windows.Forms.Cursors.Default;

                        return;
                    }

                    this.toolStripProgressBar1.Value = i * 100 / (int)sr.BaseStream.Length;
                }
                sr.Close();

                EscapeKey.StopListening();

                //hebrejský text
                this.fileText = this.fileText.Replace('ך', 'כ');
                this.fileText = this.fileText.Replace('ם', 'מ');
                this.fileText = this.fileText.Replace('ן', 'נ');
                this.fileText = this.fileText.Replace('ף', 'פ');
                this.fileText = this.fileText.Replace('ץ', 'צ');

                //latinský text
                this.fileText = this.fileText.ToUpper();

                FileInfo fi = new FileInfo(this.openFileDialog1.FileName);
                
                StreamWriter sw = new StreamWriter("adapted\\" + fi.Name.Replace(fi.Extension, ".dat"), false, Encoding.UTF8);
                sw.Write(this.fileText);
                sw.Close();

                this.toolStripProgressBar1.Value = 0;
                this.Cursor = System.Windows.Forms.Cursors.Default;

                this.lastSearchInFile = true;

                this.Text = fi.Name + " - SkipCode";
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column > 2)
            {
                return;
            }
            
            if (e.Column == this.listViewColumnSorter1.SortColumn)
            {
                if (this.listViewColumnSorter1.Order == SortOrder.Ascending)
                {
                    this.listViewColumnSorter1.Order = SortOrder.Descending;
                    this.resultsSorter1.Order = SortOrder.Descending;
                }
                else
                {
                    this.listViewColumnSorter1.Order = SortOrder.Ascending;
                    this.resultsSorter1.Order = SortOrder.Ascending;
                }
            }
            else
            {
                this.listViewColumnSorter1.SortColumn = e.Column;
                if (e.Column == 0)
                {
                    this.resultsSorter1.SortingVariable = Searching.ResultsSorter.SortBy.Expression;
                }
                else if (e.Column == 1)
                {
                    this.resultsSorter1.SortingVariable = Searching.ResultsSorter.SortBy.Offset;
                }
                else if (e.Column == 2)
                {
                    this.resultsSorter1.SortingVariable = Searching.ResultsSorter.SortBy.Skip;
                }
                this.listViewColumnSorter1.Order = SortOrder.Ascending;
                this.resultsSorter1.Order = SortOrder.Ascending;
            }

            this.listView1.Sort();
            this.found.Sort(this.resultsSorter1.Compare);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                this.searchInMatrixToolStripMenuItem.Enabled = true;
                this.lastSearchExportMatrix = true;
                
                this.foundIndex = this.listView1.SelectedItems[0].Index;

                this.listView2.Items.Clear();
                for (int i = 0; i < this.found[this.foundIndex].RelatedResults.Count; i++)
                {
                    Searching.Result r = this.found[this.foundIndex].RelatedResults[i];
                    ListViewItem item = new ListViewItem(r.Expression);
                    item.SubItems.Add(r.Offset.ToString());
                    item.SubItems.Add(r.Skip.ToString());

                    if (searchedInTorah)
                    {
                        Torah.Verse v1 = this.torah.GetVerse(r.Offset);
                        if (v1 != null)
                        {
                            item.SubItems.Add(v1.GetAddress());
                        }

                        Torah.Verse v2 = this.torah.GetVerse(r.Offset + (r.Expression.Length - 1) * r.Skip);
                        if (v2 != null)
                        {
                            item.SubItems.Add(v2.GetAddress());
                        }
                    }

                    this.listView2.Items.Add(item);
                }
                this.listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                
                DataTable dt = new DataTable();

                for (int i = 0; i < this.DataGridMatrix.Width; i++)
                {
                    dt.Columns.Add();
                }

                String text;

                if (this.searchedInTorah)
                {
                    text = this.torah.Text;
                }
                else
                {
                    text = this.fileText;
                }

                String exp = this.found[this.foundIndex].Expression;
                int offset = this.found[this.foundIndex].Offset;
                int skip = this.found[this.foundIndex].Skip;

                this.DataGridMatrix.Fill(text, exp, offset, skip);

                for (int i = 0; i < this.DataGridMatrix.Height; i++)
                {
                    String[] row = new String[this.DataGridMatrix.Width];

                    for (int j = 0; j < this.DataGridMatrix.Width; j++)
                    {
                        row[j] = this.DataGridMatrix.GetLetter(i, j);
                    }

                    dt.Rows.Add(row);
                }

                this.dataGridView1.DataSource = dt;

                this.listView3.Items.Clear();

                List<Matrix.Expression> expressions = this.DataGridMatrix.Expressions;
                for (int i = 0; i < expressions.Count; i++)
                {
                    List<Matrix.Cell> exp1Cells = expressions[i].GetCells();
                    for (int j = 0; j < exp1Cells.Count; j++)
                    {
                        Matrix.Cell cell = exp1Cells[j];
                        this.dataGridView1.Rows[cell.row].Cells[cell.col].Style.BackColor = expressions[i].BackColor;
                        this.dataGridView1.Rows[cell.row].Cells[cell.col].Style.ForeColor = expressions[i].ForeColor;
                    }

                    ListViewItem item = new ListViewItem(expressions[i].Text.ToLower());
                    item.BackColor = expressions[i].BackColor;
                    item.ForeColor = expressions[i].ForeColor;
                    this.listView3.Items.Add(item);
                }
            }
        }    

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(Directory.GetCurrentDirectory() + "\\adapted"))
            {
                this.openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory() + "\\adapted";
            }
            else
            {
                this.openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
            }

            if (this.openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.lastSearchFileInfo = new FileInfo(this.openFileDialog1.FileName);
                StreamReader sr = new StreamReader(this.openFileDialog1.FileName, true);
                this.fileText = sr.ReadToEnd();

                this.lastSearchInFile = true;
                this.lastSearchExportMatrix = false;

                FileInfo fi = new FileInfo(this.openFileDialog1.FileName);
                this.Text = fi.Name + " - SkipCode";
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.searchedInTorah)
            {
                int index = this.DataGridMatrix.GetIndex(e.RowIndex, e.ColumnIndex);

                if (index != -1)
                {
                    Torah.Verse verse = this.torah.GetVerse(index);

                    this.formTorah.comboBox1.SelectedIndex = verse.BookNumber - 1;
                    this.formTorah.comboBox2.SelectedIndex = verse.ChapterNumber - 1;
                    this.formTorah.comboBox3.SelectedIndex = verse.VerseNumber - 1;
                }
            }
        }

        private void searchTwoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSearchTwo form = new FormSearchTwo();
            if (this.lastSearchExpression1.Count > 0)
            {
                form.comboBox1.Text = this.lastSearchExpression1[0];
                form.comboBox1.Items.AddRange(this.lastSearchExpression1.ToArray());
            }
            if (this.lastSearchExpression2.Count > 0)
            {
                form.comboBox2.Text = this.lastSearchExpression2[0];
                form.comboBox2.Items.AddRange(this.lastSearchExpression2.ToArray());
            }
            form.numericUpDown1.Value = this.lastSearchMaxSkip;
            form.numericUpDown2.Value = this.lastSearchHeight;
            form.numericUpDown3.Value = this.lastSearchWidth;
            if (this.lastSearchInFile)
            {
                form.radioButton2.Checked = true;
            }
            else
            {
                form.radioButton1.Checked = true;
            }
            form.checkBox1.Checked = this.lastSearchDeleteLastTwo;
            
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if(form.comboBox1.Text.Length > 0)
                {
                    this.lastSearchExpression1.Insert(0, form.comboBox1.Text);
                }
                if (form.comboBox2.Text.Length > 0)
                {
                    this.lastSearchExpression2Related = form.comboBox2.Text;
                    this.lastSearchExpression2.Insert(0, form.comboBox2.Text);
                    this.lastSearchExpression1Only = false;
                }
                else
                {
                    this.lastSearchExpression1Only = true;
                }
                this.lastSearchMaxSkip = (int)form.numericUpDown1.Value;
                this.lastSearchHeight = (int)form.numericUpDown2.Value;
                this.lastSearchWidth = (int)form.numericUpDown3.Value;
                if (form.radioButton2.Checked)
                {
                    this.lastSearchInFile = true;
                }
                else
                {
                    this.lastSearchInFile = false;
                }
                this.lastSearchDeleteLastTwo = form.checkBox1.Checked;
                
                this.searchInMatrixToolStripMenuItem.Enabled = false;

                this.lastSearchExportMatrix = false;

                if (form.checkBox1.Checked)
                {
                    this.listView1.Items.Clear();
                    this.listView2.Items.Clear();
                    this.found.Clear();
                }
                
                this.DataGridMatrix.Width = (int)form.numericUpDown3.Value;
                this.DataGridMatrix.Height = (int)form.numericUpDown2.Value;
                int maxSkip = (int)form.numericUpDown1.Value;

                if (form.radioButton1.Checked)
                {
                    this.searchedInTorah = true;
                    this.SearchSkipCode(form.comboBox1.Text, form.comboBox2.Text, this.torah.Text, maxSkip);
                }
                else
                {
                    this.searchedInTorah = false;
                    this.SearchSkipCode(form.comboBox1.Text, form.comboBox2.Text, this.fileText, maxSkip);
                }

                this.listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                MessageBox.Show
                (
                    "Found " + this.found.Count.ToString() + " results.",
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void searchInMatrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSearchInMatrix form = new FormSearchInMatrix();
            if (this.lastSearchExpression2.Count > 0)
            {
                form.comboBox1.Text = this.lastSearchExpression2[0];
                form.comboBox1.Items.AddRange(this.lastSearchExpression2.ToArray());
            }
            form.checkBox1.Checked = this.lastSearchDeleteLastMatrix;

            if (form.ShowDialog() == DialogResult.OK)
            {
                this.lastSearchExpression2.Insert(0, form.comboBox1.Text);
                this.lastSearchDeleteLastMatrix = form.checkBox1.Checked;
                
                EscapeKey.StartListening();
                List<Searching.Result> results = this.DataGridMatrix.Find(form.comboBox1.Text);
                EscapeKey.StopListening();

                if (form.checkBox1.Checked)
                {
                    this.listView2.Items.Clear();
                    this.found[this.foundIndex].RelatedResults.Clear();
                }

                this.found[this.foundIndex].RelatedResults.AddRange(results);
                
                for (int i = 0; i < results.Count; i++)
                {
                    ListViewItem item = new ListViewItem(results[i].Expression);
                    item.SubItems.Add(results[i].Offset.ToString());
                    item.SubItems.Add(results[i].Skip.ToString());

                    if (searchedInTorah)
                    {
                        Torah.Verse v1 = this.torah.GetVerse(results[i].Offset);
                        if (v1 != null)
                        {
                            item.SubItems.Add(v1.GetAddress());
                        }

                        Torah.Verse v2 = this.torah.GetVerse(results[i].Offset + (results[i].Expression.Length - 1) * results[i].Skip);
                        if (v2 != null)
                        {
                            item.SubItems.Add(v2.GetAddress());
                        }
                    }

                    this.listView2.Items.Add(item);
                }

                this.listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                MessageBox.Show
                (
                    "Found " + results.Count.ToString() + " results.",
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView2.SelectedItems.Count > 0)
            {
                int index = this.listView2.SelectedItems[0].Index;
                Searching.Result r = this.found[this.foundIndex].RelatedResults[index];

                this.dataGridView1.ClearSelection();
                List<Matrix.Cell> cells = new List<Matrix.Cell>();
                for (int i = 0; i < r.Expression.Length; i++)
                {
                    cells.AddRange(this.DataGridMatrix.GetCoordinates(r.Offset + i * r.Skip));
                }

                for (int i = 0; i < cells.Count; i++)
                {
                    this.dataGridView1.Rows[cells[i].row].Cells[cells[i].col].Selected = true;
                }
            }
        }

        private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column > 2)
            {
                return;
            }

            if (e.Column == this.listViewColumnSorter2.SortColumn)
            {
                if (this.listViewColumnSorter2.Order == SortOrder.Ascending)
                {
                    this.listViewColumnSorter2.Order = SortOrder.Descending;
                    this.resultsSorter2.Order = SortOrder.Descending;
                }
                else
                {
                    this.listViewColumnSorter2.Order = SortOrder.Ascending;
                    this.resultsSorter2.Order = SortOrder.Ascending;
                }
            }
            else
            {
                this.listViewColumnSorter2.SortColumn = e.Column;
                if (e.Column == 0)
                {
                    this.resultsSorter2.SortingVariable = Searching.ResultsSorter.SortBy.Expression;
                }
                else if (e.Column == 1)
                {
                    this.resultsSorter2.SortingVariable = Searching.ResultsSorter.SortBy.Offset;
                }
                else if (e.Column == 2)
                {
                    this.resultsSorter2.SortingVariable = Searching.ResultsSorter.SortBy.Skip;
                }
                this.listViewColumnSorter2.Order = SortOrder.Ascending;
                this.resultsSorter2.Order = SortOrder.Ascending;
            }

            this.listView2.Sort();
            this.found[this.foundIndex].RelatedResults.Sort(this.resultsSorter2.Compare);
        }

        private void probabilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormProbability form = new FormProbability();
            
            int textLenght;
            if (this.lastSearchInFile == false)
            {
                Probability.CountFrequencyCharacteristic(this.torah.Text);
                form.Text = "Torah - Probability";
                textLenght = this.torah.Text.Length;
            }
            else
            {
                Probability.CountFrequencyCharacteristic(this.fileText);
                form.Text = this.lastSearchFileInfo.Name +  " - Probability";
                textLenght = this.fileText.Length;
            }

            form.textLenght = textLenght;
            form.matrixHeight = this.DataGridMatrix.Height;
            form.matrixWidth = this.DataGridMatrix.Width;

            form.listView1.Items.Clear();

            List<String[]> log = this.createLog();
            for (int i = 0; i < log.Count; i++)
            {
                ListViewItem item = new ListViewItem(log[i][0]);
                item.SubItems.Add(log[i][1]);
                form.listView1.Items.Add(item);
            }

            if (form.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                //do nothing
            }
        }

        private bool IsLetter(char c)
        {
            String abeceda = "aábcčdďeéěfghiíjklľmnňoópqrřsštťuúůvwxyýzž";
            abeceda += abeceda.ToUpper();

            abeceda += "אבגדהוזחטיכךלמםנןסעפףצץקרשת";

            return abeceda.Contains(c);
        }

        private void SearchSkipCode(String expression1, String expression2, String text, int maxSkip)
        {
            this.Cursor = System.Windows.Forms.Cursors.WaitCursor;

            EscapeKey.StartListening();

            Searching searching = new Searching(expression1, text, maxSkip);

            Searching.Result r;
            this.lastSearchExpression1Found = 0;
            this.lastSearchExpression2Found = 0;

            while ((r = searching.FindNext()) != null)
            {
                this.lastSearchExpression1Found++;
                
                if (expression2.Length > 0)
                {
                    Matrix tmpMatrix = new Matrix(this.DataGridMatrix.Width, this.DataGridMatrix.Height);
                    tmpMatrix.Fill(text, expression1, r.Offset, r.Skip);
                    r.RelatedResults.AddRange(tmpMatrix.Find(expression2));
                }

                this.lastSearchExpression2Found += r.RelatedResults.Count;

                if (expression2.Length == 0 || r.RelatedResults.Count > 0)
                {
                    this.found.Add(r);

                    ListViewItem item = new ListViewItem(expression1);
                    item.SubItems.Add(r.Offset.ToString());
                    item.SubItems.Add(r.Skip.ToString());

                    if (searchedInTorah)
                    {
                        Torah.Verse v1 = this.torah.GetVerse(r.Offset);
                        if (v1 != null)
                        {
                            item.SubItems.Add(v1.GetAddress());
                        }

                        Torah.Verse v2 = this.torah.GetVerse(r.Offset + (expression1.Length - 1) * r.Skip);
                        if (v2 != null)
                        {
                            item.SubItems.Add(v2.GetAddress());
                        }
                    }

                    this.listView1.Items.Add(item);
                    this.splitContainer1.Panel1.Refresh();
                }

                if (r.Offset > 0)
                {
                    this.toolStripProgressBar1.Value = r.Offset * 100 / text.Length;
                }
            }

            EscapeKey.StopListening();

            this.toolStripProgressBar1.Value = 0;
            this.Cursor = System.Windows.Forms.Cursors.Default;
        }

        private void letterFrequencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormLetterFrequency form = new FormLetterFrequency();

            if (this.lastSearchInFile == false)
            {
                Probability.CountFrequencyCharacteristic(this.torah.Text);
                form.Text = "Torah - Letter Frequency";
            }
            else
            {
                Probability.CountFrequencyCharacteristic(this.fileText);
                form.Text = this.lastSearchFileInfo.Name + " - Letter Frequency";
            }

            DataTable source = Probability.GetFrequnecyCharacteristic();
            form.dataGridView1.DataSource = source;

            if (form.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                //do nothing
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout form = new FormAbout();
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //do nothing
            }
        }

        private void addToMatrixExpressionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listView2.SelectedItems.Count; i++)
            {
                int index = this.listView2.SelectedItems[0].Index;
                Searching.Result r = this.found[this.foundIndex].RelatedResults[index];
                
                ListViewItem item = new ListViewItem(r.Expression);
                this.listView3.Items.Add(r.Expression);

                Matrix.Expression exp = new
                    Matrix.Expression
                    (
                        r.Expression,
                        this.DataGridMatrix.GetExpressionCells
                        (
                            r.Expression.Length,
                            r.Offset,
                            r.Skip
                        )
                    );
                exp.Offset = r.Offset;
                exp.Skip = r.Skip;

                this.DataGridMatrix.Expressions.Add(exp);
            }
        }

        private void changeForeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listView3.SelectedItems.Count > 0)
            {
                if (this.colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    for (int i = 0; i < this.listView3.SelectedItems.Count; i++)
                    {
                        ListViewItem item = this.listView3.SelectedItems[i];
                        item.ForeColor = this.colorDialog1.Color;

                        this.DataGridMatrix.Expressions[item.Index].ForeColor = item.ForeColor;
                        this.DataGridMatrix.Expressions[item.Index].BackColor = item.BackColor;

                        List<Matrix.Cell> expCells = this.DataGridMatrix.Expressions[item.Index].GetCells();
                        for (int j = 0; j < expCells.Count; j++)
                        {
                            Matrix.Cell cell = expCells[j];
                            this.dataGridView1.Rows[cell.row].Cells[cell.col].Style.ForeColor = item.ForeColor;
                        }
                    }
                }
            }
        }

        private void changeBackColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                for (int i = 0; i < this.listView3.SelectedItems.Count; i++)
                {
                    ListViewItem item = this.listView3.SelectedItems[i];
                    item.BackColor = this.colorDialog1.Color;

                    this.DataGridMatrix.Expressions[item.Index].ForeColor = item.ForeColor;
                    this.DataGridMatrix.Expressions[item.Index].BackColor = item.BackColor;

                    List<Matrix.Cell> expCells = this.DataGridMatrix.Expressions[item.Index].GetCells();
                    for (int j = 0; j < expCells.Count; j++)
                    {
                        Matrix.Cell cell = expCells[j];
                        this.dataGridView1.Rows[cell.row].Cells[cell.col].Style.BackColor = item.BackColor;
                    }
                }
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.searchedInTorah)
            {
                int index = this.DataGridMatrix.GetIndex(e.RowIndex, e.ColumnIndex);

                if (index != -1)
                {
                    Torah.Verse verse = this.torah.GetVerse(index);

                    this.formTorah.comboBox1.SelectedIndex = verse.BookNumber - 1;
                    this.formTorah.comboBox2.SelectedIndex = verse.ChapterNumber - 1;
                    this.formTorah.comboBox3.SelectedIndex = verse.VerseNumber - 1;

                    this.formTorah.Show();
                }
            }
        }

        private String exportToHTML()
        {
            String html = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\">\n";
            html += "<html>\n";
            html += "<head>\n";
            html += "<meta http-equiv=\"content-type\" content=\"text/html; charset=utf-8\">\n";
            html += "<meta name=\"generator\" content=\"SkipCode\">\n";
            if (this.lastSearchExportMatrix)
            {
                html += "<title>Skip Code: " + this.listView3.Items[0].Text + "</title>\n";
            }
            else
            {
                html += "<title>SkipCode: no matrix</title>\n";
            }
            html += "<style type=\"text/css\">\n";
            html += "* {\n";
            html += "margin: 0 0 0 0;\n";
            html += "padding: 0 0 0 0;\n";
            html += "border: 0 0 0 0;\n";
            html += "}\n";
            html += "table {\n";
            html += "border-spacing: 2px;\n";
            html += "}\n";
            html += "td {\n";
            html += "width: 15px; \n";
            html += "height: 15px; \n";
            html += "font-family: Tahoma, sans; \n";
            html += "font-size: 12px; \n";
            html += "line-height: 12px; \n";
            html += "vertical align: middle; \n";
            html += "text-align: center; \n";
            html += "border: rgb(220, 220, 220) 1px solid; \n";
            html += "}\n";
            html += "h2 {\n";
            html += "margin-top: 15px;\n";
            html += "margin-bottom: 5px;\n";
            html += "size: 24px;\n";
            html += "}\n";
            html += "</style>\n";
            html += "</head>\n";
            html += "<body>\n";

            html += "<h1>SkipCode</h1>\n";

            if (this.lastSearchExportMatrix)
            {
                html += "<h2>Matrix</h2>\n";

                html += "<table>\n";
                for (int r = 0; r < this.dataGridView1.Rows.Count; r++)
                {
                    if
                    (
                        this.dataGridView1.Rows[r].Cells[0].Value.ToString().Length > 0 ||
                        this.dataGridView1.Rows[r].Cells[this.dataGridView1.Columns.Count - 1].Value.ToString().Length > 0
                    )
                    {
                        html += "<tr>\n";
                        for (int c = 0; c < this.dataGridView1.Columns.Count; c++)
                        {
                            html += "<td style=\"";
                            html += "color: rgb(";
                            if (this.dataGridView1.Rows[r].Cells[c].Style.ForeColor.IsEmpty)
                            {
                                html += "0, 0, 0); ";
                            }
                            else
                            {
                                html += (int)this.dataGridView1.Rows[r].Cells[c].Style.ForeColor.R + ", ";
                                html += (int)this.dataGridView1.Rows[r].Cells[c].Style.ForeColor.G + ", ";
                                html += (int)this.dataGridView1.Rows[r].Cells[c].Style.ForeColor.B + "); ";
                            }
                            html += "background-color: rgb(";
                            if (this.dataGridView1.Rows[r].Cells[c].Style.BackColor.IsEmpty)
                            {
                                html += "255, 255, 255); ";
                            }
                            else
                            {
                                html += (int)this.dataGridView1.Rows[r].Cells[c].Style.BackColor.R + ", ";
                                html += (int)this.dataGridView1.Rows[r].Cells[c].Style.BackColor.G + ", ";
                                html += (int)this.dataGridView1.Rows[r].Cells[c].Style.BackColor.B + "); ";
                            }
                            html += "\">\n";
                            html += this.dataGridView1.Rows[r].Cells[c].Value.ToString() + "\n";
                            html += "</td>\n";
                        }
                        html += "</tr>\n";
                    }
                }
                html += "</table>\n";

                html += "<h2>Legend</h2>\n";
                html += "<table>\n";
                for (int i = 0; i < this.DataGridMatrix.Expressions.Count; i++)
                {
                    html += "<tr>\n";
                    html += "<td style=\"padding: 0 5px 0 5px; ";
                    html += "color: rgb(";
                    if (this.DataGridMatrix.Expressions[i].ForeColor.IsEmpty)
                    {
                        html += "0, 0, 0); ";
                    }
                    else
                    {
                        html += (int)this.DataGridMatrix.Expressions[i].ForeColor.R + ", ";
                        html += (int)this.DataGridMatrix.Expressions[i].ForeColor.G + ", ";
                        html += (int)this.DataGridMatrix.Expressions[i].ForeColor.B + "); ";
                    }
                    html += "background-color: rgb(";
                    if (this.listView3.Items[i].BackColor.IsEmpty)
                    {
                        html += "255, 255, 255); ";
                    }
                    else
                    {
                        html += (int)this.DataGridMatrix.Expressions[i].BackColor.R + ", ";
                        html += (int)this.DataGridMatrix.Expressions[i].BackColor.G + ", ";
                        html += (int)this.DataGridMatrix.Expressions[i].BackColor.B + "); ";
                    }
                    html += "\">\n";
                    html += this.DataGridMatrix.Expressions[i].Text;
                    html += "</td>\n";
                    html += "<td style=\"width: auto; text-align: left; \">\n";
                    html += "(offset:&nbsp;" + this.DataGridMatrix.Expressions[i].Offset + ", ";
                    html += "skip:&nbsp;" + this.DataGridMatrix.Expressions[i].Skip + ")\n";
                    html += "</td>\n";
                    html += "</tr>\n";
                }

                html += "</table>\n";
            }

            html += "<h2>Log</h2>\n";
            html += "<table>\n";
            List<String[]> log = this.createLog();
            for (int i = 0; i < log.Count; i++)
            {
                html += "<tr>\n";
                html += "<td style=\"width: auto; padding: 0 5px 0 5px; text-align: left\">" + log[i][0] + "</td>\n";
                html += "<td style=\"width: auto; padding: 0 5px 0 5px; text-align: right\">" + log[i][1] + "</td>\n";
                html += "</tr>\n";
            }
            html += "</table>\n";

            if (this.lastSearchInFile)
            {
                html += "<h2>File info</h2>\n";
                html += "<table>\n";
                html += "<tr>\n";
                html += "<td style=\"width: auto; padding: 0 5px 0 5px; text-align: left\">Full Name</td>\n";
                html += "<td style=\"width: auto; padding: 0 5px 0 5px; text-align: left\">" + this.lastSearchFileInfo.FullName + "</td>\n";
                html += "</tr>\n";
                html += "<tr>\n";
                html += "<td style=\"width: auto; padding: 0 5px 0 5px; text-align: left\">Modified</td>\n";
                html += "<td style=\"width: auto; padding: 0 5px 0 5px; text-align: left\">" + this.lastSearchFileInfo.LastWriteTime.ToString() + "</td>\n";
                html += "</tr>\n";
                html += "<tr>\n";
                html += "<td style=\"width: auto; padding: 0 5px 0 5px; text-align: left\">Size</td>\n";
                html += "<td style=\"width: auto; padding: 0 5px 0 5px; text-align: left\">" + this.lastSearchFileInfo.Length.ToString() + "B</td>\n";
                html += "</tr>\n";
                html += "</table>\n";
            }

            html += "</body>\n";
            html += "</html>";

            return html;
        }

        private void exportToHTMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
            
            if(String.IsNullOrEmpty(this.saveFileDialog1.FileName) == false)
            {
                try
                {
                    FileInfo fi = new FileInfo(this.saveFileDialog1.FileName);
                    this.saveFileDialog1.InitialDirectory = fi.DirectoryName;
                }
                catch
                {
                    
                }
            }
            
            this.saveFileDialog1.Filter = "html files (*.htm)|*.htm";
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(this.saveFileDialog1.FileName);
                sw.Write(this.exportToHTML());
                sw.Close();
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.print.DocumentText = this.exportToHTML();
            this.print.Print();
        }

        private void printOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.print.DocumentText = this.exportToHTML();
            this.print.ShowPrintDialog();
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.print.DocumentText = this.exportToHTML();
            this.print.ShowPrintPreviewDialog();
        }

        private void listView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private List<String[]> createLog()
        {
            List<String[]> log = new List<String[]>();
            String[] row;

            int textLenght;
            if (this.lastSearchInFile == false)
            {
                textLenght = this.torah.Text.Length;
                Probability.CountFrequencyCharacteristic(this.torah.Text);
            }
            else
            {
                textLenght = this.fileText.Length;
                Probability.CountFrequencyCharacteristic(this.fileText);
            }
            
            row = new String[2];
            row[0] = "text lenght";
            row[1] = textLenght.ToString();
            log.Add(row);

            row = new String[2];
            row[0] = "matrix size";
            row[1] = this.DataGridMatrix.Width + "x" + this.DataGridMatrix.Height;
            log.Add(row);

            float estimated1 = 0;
            if (this.lastSearchExpression1.Count > 0)
            {
                row = new String[2];
                row[0] = "maximal skip";
                row[1] = this.lastSearchMaxSkip.ToString();
                log.Add(row);

                row = new String[2];
                row[0] = "expression 1";
                row[1] = this.lastSearchExpression1[0];
                log.Add(row);

                row = new String[2];
                row[0] = "expression 1 length";
                row[1] = this.lastSearchExpression1[0].Length.ToString();
                log.Add(row);

                row = new String[2];
                row[0] = "finding expression 1 by random choice";
                row[1] = 
                    Probability.CountProbabiltyOfFindingExpression
                    (
                        this.lastSearchExpression1[0]
                    ).ToString("0.000000E+00");
                log.Add(row);

                row = new String[2];
                row[0] = "possible locations of expression 1 in text";
                row[1] = 
                    Probability.CountSkipsAndOffsets
                    (
                        this.lastSearchExpression1[0].Length,
                        textLenght,
                        this.lastSearchMaxSkip
                    ).ToString("0.000000E+00");
                log.Add(row);

                row = new String[2];
                row[0] = "estimated number of expression 1";
                estimated1 = 
                    Probability.EstimateNumberOfFindingExpressionInText
                    (
                        this.lastSearchExpression1[0],
                        textLenght,
                        this.lastSearchMaxSkip
                    );
                row[1] = estimated1.ToString("0.000000E+00");
                log.Add(row);

                row = new String[2];
                row[0] = "actual number of expression 1";
                if (Searching.Aborted == false)
                {
                    row[1] = this.lastSearchExpression1Found.ToString();
                }
                else
                {
                    row[1] = "searching aborted";
                }
                log.Add(row);
            }

            if (this.lastSearchExpression1Only == false)
            {
                row = new String[2];
                row[0] = "expression 2";
                row[1] = this.lastSearchExpression2Related;
                log.Add(row);

                row = new String[2];
                row[0] = "expression 2 length";
                row[1] = this.lastSearchExpression2Related.Length.ToString();
                log.Add(row);

                row = new String[2];
                row[0] = "finding expression 2 by random choice";
                row[1] = 
                    Probability.CountProbabiltyOfFindingExpression
                    (
                        this.lastSearchExpression2Related
                    ).ToString("0.000000E+00");
                log.Add(row);

                row = new String[2];
                row[0] = "possible locations of expression 2 in matrix";
                row[1] = 
                    Probability.CountForAllCells
                    (
                        this.lastSearchExpression2Related.Length,
                        this.DataGridMatrix.Width,
                        this.DataGridMatrix.Height
                    ).ToString();
                log.Add(row);

                row = new String[2];
                row[0] = "estimated number of expression 2 in 1 matrix";
                float estimated2 = 
                    Probability.EstimateNumberOfFindingExpressionInMatrix
                    (
                        this.lastSearchExpression2Related,
                        this.DataGridMatrix.Width,
                        this.DataGridMatrix.Height
                    );
                row[1] = estimated2.ToString("0.000000E+00");
                log.Add(row);

                row = new String[2];
                row[0] = "average number of expression 2 in 1 matrix";
                row[1] =
                    (
                        (float)this.lastSearchExpression2Found / (float)this.lastSearchExpression1Found
                    ).ToString("0.000000E+00");
                log.Add(row);
            }
            
            return log;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.listView3.SelectedItems.Count; i++)
            {
                int index = this.listView3.SelectedItems[i].Index;

                List<Matrix.Cell> expCells = this.DataGridMatrix.Expressions[index].GetCells();
                for (int j = 0; j < expCells.Count; j++)
                {
                    Matrix.Cell cell = expCells[j];
                    this.dataGridView1.Rows[cell.row].Cells[cell.col].Style.ForeColor = new Color();
                    this.dataGridView1.Rows[cell.row].Cells[cell.col].Style.BackColor = new Color();
                }

                this.listView3.Items.RemoveAt(index);
                this.DataGridMatrix.Expressions.RemoveAt(index);
            }
        }
    }
}
