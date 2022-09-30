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
        public class Expression
        {
            private List<Cell> cells;
            public String Text;
            public Color BackColor = new Color();
            public Color ForeColor = new Color();
            public int Offset;
            public int Skip;

            public Expression(String expression, List<Cell> cells)
            {
                this.Text = expression;
                this.cells = cells;
            }

            public void AddCell(int row, int col)
            {
                Cell c = new Cell();

                c.row = row;
                c.col = col;

                this.cells.Add(c);
            }

            public void AddCells(List<Cell> cells)
            {
                this.cells.AddRange(cells);
            }

            public List<Cell> GetCells()
            {
                return this.cells;
            }
        }
    }
}
