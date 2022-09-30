using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
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
    public partial class Torah
    {
        private OleDbConnection connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=\"data\\torah.mdb\"");
        private OleDbDataAdapter dataAdapter = new OleDbDataAdapter();
        
        public DataSet DataSet = new DataSet();

        public String[] Books = { null, "Genesis", "Exodus", "Leviticus", "Numeri", "Deuteronomium" };

        public enum Versions : int { Original, Sirach, KingJames };
        private String[] versionTables = { "wlc", "sicher", "kjv" };

        public String Text;

        public Torah()
        {
            StreamReader sr = new StreamReader("data\\wlc.dat");
            this.Text = sr.ReadToEnd();
        }

        public Verse GetVerse(int charOffset)
        {
            this.DataSet.Clear();

            String query = "select * " +
                "from wlc where " +
                "startIndex <= " + charOffset.ToString() + " and " +
                "endIndex >= " + charOffset.ToString();

            this.dataAdapter.SelectCommand = new OleDbCommand(query, connection);

            this.dataAdapter.Fill(this.DataSet);

            this.connection.Close();

            if (DataSet.Tables[0].Rows.Count > 0)
            {
                DataRow row = DataSet.Tables[0].Rows[0];
                Verse v = new Verse();

                v.BookNumber = int.Parse(row["Book"].ToString());
                v.BookName = this.Books[v.BookNumber];
                v.ChapterNumber = int.Parse(row["Chapter"].ToString());
                v.VerseNumber = int.Parse(row["Verse"].ToString());
                v.Text = row["Body"].ToString();

                return v;
            }

            return null;
        }

        public Verse GetVerse(int book, int chapter, int verse, Torah.Versions version)
        {
            this.DataSet.Clear();

            String query = "select * " +
                "from " + this.versionTables[(int)version] + " where " +
                "Book = " + book.ToString() + " and " +
                "Chapter = " + chapter.ToString() + " and " +
                "Verse = " + verse.ToString();

            this.dataAdapter.SelectCommand = new OleDbCommand(query, connection);

            this.dataAdapter.Fill(this.DataSet);

            this.connection.Close();

            if (DataSet.Tables[0].Rows.Count > 0)
            {
                DataRow row = DataSet.Tables[0].Rows[0];
                Verse v = new Verse();

                v.BookNumber = int.Parse(row["Book"].ToString());
                v.BookName = this.Books[v.BookNumber];
                v.ChapterNumber = int.Parse(row["Chapter"].ToString());
                v.VerseNumber = int.Parse(row["Verse"].ToString());
                v.Text = row["Body"].ToString();

                return v;
            }

            return null;
        }

        public List<String> GetChapterNumbers(int bookIndex, Torah.Versions version)
        {
            List<String> list = new List<String>();

            this.DataSet.Clear();

            String query = "select Chapter" +
                " from " + this.versionTables[(int)version] +
                " where Book = " + bookIndex.ToString() +
                " group by Chapter";

            this.dataAdapter.SelectCommand = new OleDbCommand(query, connection);

            this.dataAdapter.Fill(this.DataSet);

            this.connection.Close();

            IEnumerator en = this.DataSet.Tables[0].Rows.GetEnumerator();

            while (en.MoveNext())
            {
                list.Add(((DataRow)en.Current)["Chapter"].ToString());
            }

            return list;
        }

        public List<String> GetVerseNumbers(int bookIndex, int chapterIndex, Torah.Versions version)
        {
            List<String> list = new List<String>();

            this.DataSet.Clear();

            String query = "select Verse" +
                " from " + this.versionTables[(int)version] +
                " where Book = " + bookIndex.ToString() +
                " and Chapter = " + chapterIndex.ToString() +
                " group by Verse";

            this.dataAdapter.SelectCommand = new OleDbCommand(query, connection);

            this.dataAdapter.Fill(this.DataSet);

            this.connection.Close();

            IEnumerator en = this.DataSet.Tables[0].Rows.GetEnumerator();

            while (en.MoveNext())
            {
                list.Add(((DataRow)en.Current)["Verse"].ToString());
            }

            return list;
        }
    }
}
