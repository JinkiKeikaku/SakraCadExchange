using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper.Shape
{
    public class SkcOleShape : SkcShape
    {
        public SkcPoint P0 = new();
        public double Width;
        public double Height;
        public SkcOle2Item Ole2Item = new();

        public override string Name => "OLE";
        public override SkcShape Create() => new SkcOleShape();

        public override void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                { "PARAM", (reader)=>
                    reader.ReadTags(new()
                    {
                        { "P0", (reader)=> P0 = reader.ReadPoint()},
                        { "WIDTH", (reader)=> Width = reader.ReadDouble()},
                        { "HEIGHT", (reader)=> Height = reader.ReadDouble()},
                    })
                },
                { "OLECLIENTITEM", (reader)=> Ole2Item.Read(reader) },
            });
        }

        public override void Write(SkcWriter w)
        {
            w.WriteObject("PARAM", false, w =>
            {
                w.Write("P0", P0);
                w.Write("WIDTH", Width);
                w.Write("HEIGHT", Height);
            });
            w.NewLine();
            w.WriteObject("OLECLIENTITEM", false, w =>
            {
                Ole2Item.Write(w);
            });
        }
    }
    public class SkcOle2Item
    {
        public byte[]? Data;

        public SkcOle2Item()
        {
        }

        public SkcOle2Item(byte[] data)
        {
            Data = data;
        }

        public void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                { "OBJECT", (reader)=> Data = reader.ReadCompressBytes() },
            }); ;
        }
        public void Write(SkcWriter w)
        {
            if (Data == null) throw new Exception("SkcOle2Item::Write() Data is null.");
            w.WriteCompressBytes("OBJECT", Data);
        }

    }

}
