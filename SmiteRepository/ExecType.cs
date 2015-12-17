using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmiteRepository
{
    internal class ExecType
    {
        public const string Max = "Max";
        public const string Min = "Min";
        public const string Sum = "Sum";
        public const string Count = "Count";
        public const string Avg = "Avg";
        public const string Scalar = "";
        public const string Exists = "TOP 1 ";
    }
}
