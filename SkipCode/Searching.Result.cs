using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkipCode
{
    public partial class Searching
    {
        public class Result
        {
            public String Expression;
            public int Offset;
            public int Skip;

            public List<Searching.Result> RelatedResults = new List<Result>();
        }
    }
}
