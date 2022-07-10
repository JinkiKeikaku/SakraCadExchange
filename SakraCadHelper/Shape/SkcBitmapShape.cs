using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper.Shape
{
    public class SkcBitmapShape : SkcShape
    {
        public SkcPoint P0 = new();
        public double Width = 20.0;
        public double Height = 20.0;
        public SkcDib Dib = new ();

        public override string Name => "BITMAP";
        public override SkcShape Create() => new SkcBitmapShape();

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
                { "DIB", (reader)=> Dib.Read(reader) },
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
            w.WriteObject("DIB", false, w =>
            {
                Dib.Write(w);
            });
        }
    }
    public class SkcDib
    {
        public byte[]? Header;
        public byte[]? Image;

        public SkcDib()
        {

        }

        public SkcDib(byte[] header, byte[] image)
        {
            Header = header;
            Image = image;
        }

        public void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                { "HEADER", (reader)=> Header = reader.ReadBytes() },
                { "IMAGE", (reader)=> Image = reader.ReadCompressBytes() },
            }); ;
        }
        public void Write(SkcWriter w)
        {
            if (Header == null) throw new Exception("SkcDib::Write() Haeder is null.");
            if (Image == null) throw new Exception("SkcDib::Write() Image is null.");
            w.WriteBytes("HEADER", Header);
            w.WriteCompressBytes("IMAGE", Image);
        }

    }
}
