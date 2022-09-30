using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkipCode
{
    public partial class Searching
    {
        private int maxSkip;

        private enum direction : int { left, right };

        private int lastSkip;
        private int lastOffset;
        private direction currentDirection;

        private char[] expChars;
        private char[] txtChars;

        public List<Searching.Result> Results;

        public static bool Aborted = false;

        public Searching(String expression, String scannedText, int maxSkip)
        {
            this.maxSkip = maxSkip;
            this.expChars = expression.ToUpper().ToCharArray();
            this.txtChars = scannedText.ToCharArray();
            this.maxSkip = maxSkip;
            this.lastSkip = 0;
            this.currentDirection = direction.right;

            this.Results = new List<Result>();

            Searching.Aborted = false;
        }

        public Searching.Result FindNext()
        {
            for (int offset = this.lastOffset; offset < this.txtChars.Length; offset++)
            {
                if (this.txtChars[offset] == expChars[0])
                {
                    if (this.currentDirection == direction.right)
                    {
                        int max = (this.txtChars.Length - offset - 1) / (this.expChars.Length - 1);

                        if (max > this.maxSkip && this.maxSkip != 0)
                        {
                            max = this.maxSkip;
                        }

                        for (int skip = this.lastSkip + 1; skip <= max; skip++)
                        {
                            bool ok = true;

                            for (int i = 1; i < this.expChars.Length; i++)
                            {
                                if (EscapeKey.Pressed)
                                {
                                    Searching.Aborted = true;
                                    return null;
                                }
                                
                                if (expChars[i] != this.txtChars[offset + i * skip])
                                {
                                    ok = false;
                                    break;
                                }
                            }

                            if (ok)
                            {
                                Searching.Result result = new Result();
                                result.Expression = new String(this.expChars);
                                this.lastSkip = result.Skip = skip;
                                this.lastOffset = result.Offset = offset;

                                return result;
                            }
                        }
                        this.lastSkip = 0;
                        this.currentDirection = direction.left;
                    }
                    
                    if(this.currentDirection == direction.left)
                    {
                        int max = (offset) / (this.expChars.Length - 1);

                        if (max > this.maxSkip && this.maxSkip != 0)
                        {
                            max = this.maxSkip;
                        }

                        for (int skip = this.lastSkip + 1; skip <= max; skip++)
                        {
                            bool ok = true;

                            for (int i = 1; i < this.expChars.Length; i++)
                            {
                                if (EscapeKey.Pressed)
                                {
                                    Searching.Aborted = true;
                                    return null;
                                }
                                
                                if (expChars[i] != this.txtChars[offset - i * skip])
                                {
                                    ok = false;
                                    break;
                                }
                            }

                            if (ok)
                            {
                                Searching.Result result = new Result();
                                result.Expression = new String(this.expChars);
                                this.lastSkip = skip;
                                result.Skip = -skip;
                                this.lastOffset = result.Offset = offset;

                                return result;
                            }
                        }
                        this.lastSkip = 0;
                        this.currentDirection = direction.right;
                    }
                }
            }
            
            return null;
        }
    }
}
