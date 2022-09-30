using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkipCode
{
    public partial class Torah
    {
        public class Verse
        {
            public String BookName;
            public int BookNumber;
            public int ChapterNumber;
            public int VerseNumber;
            public String Text;

            public String GetFull()
            {
                if (this.BookName.Length > 0)
                {
                    return this.BookName + " " + this.ChapterNumber + ":" + this.VerseNumber + " " + this.Text;
                }

                return "";
            }

            public String GetAddress()
            {
                if (this.BookName.Length > 0)
                {
                    return this.BookName + " " + this.ChapterNumber + ":" + this.VerseNumber;
                }

                return "";
            }
        }
    }
}
