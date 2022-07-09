using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper
{
    public class SkcPaperScale
    {
        public string Name = "";
        public double Numerator = 1.0;
        public double GetScale() => Numerator < 0 ? 1 / (-Numerator) : Numerator;

        public override string ToString()
        {
            return $"NAME(\"{Name}\")NUMERATOR({Numerator})";
        }
    }
}
