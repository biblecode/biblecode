using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace SkipCode
{
    public static class Probability
    {
        private static Hashtable frequencyCharacteristic = new Hashtable();
        
        public static void CountFrequencyCharacteristic(String text)
        {
            Probability.frequencyCharacteristic.Clear();
            Hashtable tableOfQuantity = new Hashtable();
            String character;

            for(int i = 0; i < text.Length; i++)
	        {
		        character = text.Substring(i,1);

                if (tableOfQuantity.Contains(character))
			    {
                    tableOfQuantity[character] = ((int)tableOfQuantity[character]) + 1;
			    }
			    else
			    {
                    tableOfQuantity.Add(character, (int)1);
			    }
	        }

            IDictionaryEnumerator en = tableOfQuantity.GetEnumerator();

            while (en.MoveNext())
            {
                Probability.frequencyCharacteristic.Add(en.Key, (float)((float)(int)en.Value / text.Length));
            }
        }

        public static float EstimateNumberOfFindingExpressionInText(String expression, int textLength, int maxSkip)
        {
            float p = 2 * Probability.CountSkipsAndOffsets(expression.Length, textLength, maxSkip);

            return p * Probability.CountProbabiltyOfFindingExpression(expression);
        }
        
        public static float EstimateNumberOfFindingExpressionInMatrix(String expression, int tableWidth, int tableHeight)
        {
            float p = Probability.CountForAllCells(expression.Length, tableWidth, tableHeight);

            return p * Probability.CountProbabiltyOfFindingExpression(expression);
        }

        public static float CountProbabiltyOfFindingExpression(String expression)
        {
            float p = 1;

            expression = expression.ToUpper();

            for (int i = 0; i < expression.Length; i++)
            {
                p *= Probability.GetFrequencyOfChar(expression.Substring(i, 1));
            }

            return p;
        }

        private static float GetFrequencyOfChar(String character)
        {
            float p = 0;

            if (Probability.frequencyCharacteristic.Contains(character))
            {
                p = (float)Probability.frequencyCharacteristic[character];
            }

            return p;
        }

        public static DataTable GetFrequnecyCharacteristic()
        {
            DataTable table = new DataTable();

            table.Columns.Add("Character", typeof(String));
            table.Columns.Add("Quotient", typeof(float));

            IDictionaryEnumerator en = Probability.frequencyCharacteristic.GetEnumerator();
            while (en.MoveNext())
            {
                DataRow r = table.NewRow();
                r["Character"] = (String)en.Key;
                r["Quotient"] = (float)en.Value;
                table.Rows.Add(r);
            }

            return table;
        }
        
        public static int CountForAllCells(int expressionLength, int tableWidth, int tableHeight)
        {
            if (expressionLength == 0)
            {
                return 0;
            }

            if (expressionLength == 1)
            {
                return tableHeight * tableWidth;
            }
            
            int total = 0;

            for (int i = 0; i < tableHeight; i++)
            {
                for (int j = 0; j < tableWidth; j++)
                {
                    total += Probability.CountFromCell(i, j, expressionLength, tableWidth, tableHeight);
                }
            }

            return total;
        }

        private static int CountFromCell(int x, int y, int expressionLength, int tableWidth, int tableHeight)
        {
            int count = 0;

            for (int r = 0; r < tableHeight; r++)
            {
                for (int c = 0; c < tableWidth; c++)
                {
                    int dr = r - x;
                    int dc = c - y;

                    if (dr != 0 || dc != 0)
                    {
                        bool ok = true;

                        for (int i = 1; ok == true; i++)
                        {
                            if (
                            (i >= expressionLength) ||
                            (x + i * dr < 0) ||
                            (x + i * dr >= tableHeight) ||
                            (y + i * dc < 0) ||
                            (y + i * dc >= tableWidth)
                            )
                            {
                                ok = false;
                            }
                            else
                            {
                                if (i == expressionLength - 1)
                                {
                                    count++;
                                }
                            }
                        }
                    }
                }
            }

            return count;
        }

        public static float CountSkipsAndOffsets(int expressionLength, int textLenght, int maxSkip)
        {
            if (expressionLength == 0)
            {
                return 0;
            }

            if (expressionLength == 1)
            {
                return (float)textLenght;
            }

            float count = 0;

            for (int i = 0; i <= (textLenght - expressionLength); i++)
            {
                int maxSkip_ = (textLenght - i - 1) / (expressionLength - 1);

                if (maxSkip_ <= maxSkip || maxSkip == 0)
                {
                    count += (textLenght - i) / expressionLength;
                }
                else
                {
                    count += maxSkip;
                }
            }

            return count;
        }

        private static float Factorial(int n)
        {
            if (n==0)
            {
                return 1;
            }

            return n * Probability.Factorial(n - 1);
        }
    }
}
