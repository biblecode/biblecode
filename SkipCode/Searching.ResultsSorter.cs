using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace SkipCode
{
    public partial class Searching
    {
        public class ResultsSorter : System.Collections.IComparer
        {
            public enum SortBy : int { Expression, Offset, Skip };
            public SortBy SortingVariable;
            private SortOrder OrderOfSort;
            private CaseInsensitiveComparer ObjectCompare;

            public ResultsSorter()
            {
                this.SortingVariable = SortBy.Expression;

                this.OrderOfSort = SortOrder.None;

                this.ObjectCompare = new CaseInsensitiveComparer();
            }

            public int Compare(object x, object y)
            {
                int compareResult = 0;
                Searching.Result resultX, resultY;

                resultX = (Searching.Result)x;
                resultY = (Searching.Result)y;

                if (this.SortingVariable == SortBy.Expression)
                {
                    compareResult = ObjectCompare.Compare(resultX.Expression, resultY.Expression);
                }
                else if (this.SortingVariable == SortBy.Offset)
                {
                    compareResult = resultX.Offset - resultY.Offset;
                }
                else if (this.SortingVariable == SortBy.Skip)
                {
                    compareResult = resultX.Skip - resultY.Skip;
                }

                if (OrderOfSort == SortOrder.Ascending)
                {
                    return compareResult;
                }
                else if (OrderOfSort == SortOrder.Descending)
                {
                    return (-compareResult);
                }
                else
                {
                    return 0;
                }
            }

            public SortOrder Order
            {
                set
                {
                    OrderOfSort = value;
                }
                get
                {
                    return OrderOfSort;
                }
            }
        }
    }
}
