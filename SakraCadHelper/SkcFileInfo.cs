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

        internal void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                {"ENCODING",  reader => EncodingName = reader.ReadString()},
                {"GENERATOR",  reader => GeneratorName = reader.ReadString()},
                {"SKCVERSION",  reader => SkcVersion = reader.ReadString()},
            });
        }

        internal void Write(SkcWriter writer)
        {
            writer.WriteString("ENCODING", EncodingName);
            writer.WriteString("GENERATOR", GeneratorName);
            writer.WriteString("SKCVERSION", SkcVersion);
        }


        public override string ToString()
        {
            return $"ENCODING(\"{EncodingName}\")GENERATOR(\"{GeneratorName}\")SKCVERSION(\"{SkcVersion}\")";
        }
    }
}
