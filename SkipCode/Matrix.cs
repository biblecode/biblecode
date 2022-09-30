using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SkipCode
{
    public partial class Matrix
    {
        public int Width = 30;
        public int Height = 30;

        private int skip;

        private Hashtable coorTable = new Hashtable();
        private int[][] indexTable = null;
        private String[][] letterTable = null;

        public List<Expression> Expressions = new List<Expression>();

        public Matrix(int w, int h)
        {
            this.Width = w;
            this.Height = h;

            this.ClearTables();
        }

        private void ClearTables()
        {
            this.indexTable = new int[this.Height][];
            this.letterTable = new String[this.Height][];
            for (int i = 0; i < this.Height; i++)
            {
                this.indexTable[i] = new int[this.Width];
                this.letterTable[i] = new String[this.Width];
            }

            this.coorTable.Clear();

            this.Expressions.Clear();
        }

        public void AddIndex(int index, int row, int col, String letter)
        {
            Cell c = new Cell();
            c.row = row;
            c.col = col;

            if (index >= 0)
            {
                List<Cell> cells = new List<Cell>();
                cells.Add(c);

                if (this.coorTable.ContainsKey(index))
                {
                    List<Cell> exists = (List<Cell>)this.coorTable[index];
                    cells.AddRange(exists);

                    this.coorTable.Remove(index);
                }
                this.coorTable.Add(index, cells);
            }

            this.indexTable[row][col] = index;
            this.letterTable[row][col] = letter;
        }

        public List<Cell> GetCoordinates(int index)
        {
            return (List<Cell>)this.coorTable[index];
        }

        public int GetIndex(int row, int col)
        {
            return this.indexTable[row][col];
        }

        public String GetLetter(int row, int col)
        {
            return this.letterTable[row][col];
        }

        public List<Cell> GetExpressionCells(int expLength, int offset, int skip)
        {
            List<Cell> cells = new List<Cell>();
            
            for (int i = 0; i < expLength; i++)
            {
                cells.AddRange(this.GetCoordinates(offset + i * skip));
            }

            return cells;
        }

        public void Fill(String text, String exp, int offset, int skip)
        {
            this.skip = skip;

            this.ClearTables();

            this.Expressions.Clear();
            this.Expressions.Add(new Expression(exp, new List<Cell>()));
            this.Expressions[0].ForeColor = Color.Black;
            this.Expressions[0].BackColor = Color.LimeGreen;
            this.Expressions[0].Skip = skip;
            this.Expressions[0].Offset = offset;

            char[] textChars = text.ToCharArray();

            if (skip < 0)
            {
                offset = offset + (exp.Length - 1) * skip;
                skip = -skip;
            }

            int rowsHalf = this.Width / 2;
            int rowsBefore = (this.Height - exp.Length) / 2;
            int indexStart = offset - rowsBefore * skip - rowsHalf;

            for (int i = 0; i < this.Height; i++)
            {
                String[] row = new String[this.Width];
                int rowIndexMin = offset - rowsBefore * skip + i * skip - (this.Width / 2);

                this.indexTable[i] = new int[this.Width];

                if (text.Contains("י")) //hebrew
                {
                    for (int j = 0; j < this.Width; j++)
                    {
                        if ((rowIndexMin + this.Width - 1 - j) < 0 || (rowIndexMin + this.Width - 1 - j) > (textChars.Length - 1))
                        {
                            this.AddIndex(-1, i, j, "");
                        }
                        else
                        {
                            int idx = rowIndexMin + this.Width - 1 - j;
                            String letter = textChars[idx].ToString();

                            this.AddIndex(idx, i, j, letter);

                            if ((idx - offset) / skip >= 0 && (idx - offset) / skip < exp.Length && (idx - offset) % skip == 0)
                            {
                                this.Expressions[0].AddCell(i, j);
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < this.Width; j++)
                    {
                        if ((rowIndexMin + j) < 0 || (rowIndexMin + j) > (textChars.Length - 1))
                        {
                            this.AddIndex(-1, i, j, "");
                        }
                        else
                        {
                            int idx = rowIndexMin + j;
                            String letter = textChars[idx].ToString();

                            this.AddIndex(idx, i, j, letter);

                            if ((idx - offset) / skip >= 0 && (idx - offset) / skip < exp.Length && (idx - offset) % skip == 0)
                            {
                                this.Expressions[0].AddCell(i, j);
                            }
                        }
                    }
                }
            }
        }

        public List<Searching.Result> Find(String expression)
        {
            char[] expChars = expression.ToUpper().ToCharArray();
            List<Searching.Result> results = new List<Searching.Result>();

            for (int r1 = 0; r1 < this.Height; r1++)
            {
                for (int c1 = 0; c1 < this.Width; c1++)
                {
                    if (EscapeKey.Pressed)
                    {
                        return results;
                    }
                    
                    if (letterTable[r1][c1].Contains(expChars[0]))
                    {
                        if (expChars.Length == 1)
                        {
                            Searching.Result result = new Searching.Result();
                            result.Offset = indexTable[r1][c1];
                            result.Skip = 0;
                            result.Expression = expression;

                            results.Add(result);
                        }
                        else
                        {
                            for (int r2 = 0; r2 < this.Height; r2++)
                            {
                                for (int c2 = 0; c2 < this.Width; c2++)
                                {
                                    if (EscapeKey.Pressed)
                                    {
                                        return results;
                                    }

                                    if (letterTable[r2][c2].Contains(expChars[1]) && !(r1 == r2 && c1 == c2))
                                    {
                                        int dr = r2 - r1;
                                        int dc = c2 - c1;

                                        bool ok = true;

                                        if (expChars.Length > 2)
                                        {
                                            int r3 = r2;
                                            int c3 = c2;

                                            for (int i = 2; i < expChars.Length; i++)
                                            {
                                                if (EscapeKey.Pressed)
                                                {
                                                    return results;
                                                }
                                                
                                                r3 += dr;
                                                c3 += dc;

                                                if (!(
                                                    r3 < this.Height &&
                                                    r3 >= 0 &&
                                                    c3 < this.Width && 
                                                    c3 >= 0 &&
                                                    letterTable[r3][c3].Contains(expChars[i])
                                                ))
                                                {
                                                    ok = false;
                                                    break;
                                                }
                                            }
                                        }

                                        if (ok)
                                        {
                                            Searching.Result result = new Searching.Result();
                                            result.Offset = indexTable[r1][c1];
                                            result.Skip = indexTable[r2][c2] - indexTable[r1][c1];
                                            result.Expression = expression;

                                            results.Add(result);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return results;
        }
    }
}
