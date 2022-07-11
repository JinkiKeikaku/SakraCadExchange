using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper.Shape
{
    public abstract class SkcFaceColor
    {
    }

    public class SkcSolidColor : SkcFaceColor
    {
        public int Color;
        public SkcSolidColor(int c) { Color = c; }
    }

    public class SkcGradationColor : SkcFaceColor
    {
        public class MidColor
        {
            public double Pos;
            public int Color;
            public override string ToString() => $"{Pos},{Color}";
        }
        public int ID;
        public int SC;
        public int EC;
        public double Angle;
        public SkcPoint P0 =new();
        public MidColor? Mid = null;

        internal void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                { "TYPE", (reader)=> ID = reader.ReadInt()},
                { "SC", (reader)=> SC = reader.ReadInt()},
                { "EC", (reader)=> EC = reader.ReadInt()},
                { "ANGLE", (reader)=> Angle = reader.ReadDouble()},
                { "P0", (reader)=> P0 = reader.ReadPoint()},
                { "MID", (reader)=> {
                    Mid = new MidColor();
                    Mid.Pos = reader.ReadDouble();
                    reader.SkipComma();
                    Mid.Color = reader.ReadInt();
                } },
            });
        }
        internal void Write(SkcWriter w)
        {
            w.Write("TYPE", ID);
            w.Write("SC", SC);
            w.Write("EC", EC);
            switch (ID)
            {
                case 0:
                    w.Write("ANGLE", Angle);
                    break;
                case 1:
                    w.Write("ANGLE", Angle);
                    w.Write("P0", P0);
                    break;
                case 2:
                    w.Write("P0", P0);
                    break;
            }
            if(Mid != null)
            {
                w.Write("MID", Mid);
            }
        }
    }

    public class SkcArrowAttribute
    {
        const int ARROWSTYLE_NONE = 0;
        const int ARROWSTYLE_ARROW = 1;
        const int ARROWSTYLE_TRIANGLE = 2;
        const int ARROWSTYLE_SLASH = 3;
        const int ARROWSTYLE_RING = 4;
        const int ARROWSTYLE_DOT = 5;
        const int ARROWSTYLE_UNFILLEDTRIANGLE = 6;
        const int ARROWSTYLE_UNFILLEDBOX = 7;

        public int ID = ARROWSTYLE_NONE;
        public double Size = 0.0;
        internal void Read(SkcReader reader)
        {
            reader.ReadTags(new ()
            {
                { "ID", (reader)=> ID = reader.ReadInt()},
                { "SIZE", (reader)=> Size = reader.ReadDouble()},
            });
        }
        internal void Write(SkcWriter writer)
        {
            writer.Write("ID", ID);
            writer.Write("SIZE", Size);
        }


        public override string ToString() => $"ID({ID})SIZE({Size})";
    }
}
