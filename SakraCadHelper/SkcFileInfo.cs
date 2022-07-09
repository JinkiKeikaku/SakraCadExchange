using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper
{
    public class SkcFileInfo
    {
        public string EncodingName = "SJIS";
        public string GeneratorName = "SakraCadHelper";
        public string SkcVersion = "1.2.0";

        public override string ToString()
        {
            return $"ENCODING(\"{EncodingName}\")GENERATOR(\"{GeneratorName}\")SKCVERSION(\"{SkcVersion}\")";
        }
    }
}
