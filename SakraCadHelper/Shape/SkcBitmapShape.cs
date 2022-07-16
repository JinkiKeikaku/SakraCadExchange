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

        internal override string Name => "BITMAP";
        internal override SkcShape Create() => new SkcBitmapShape();

        internal override void Read(SkcReader reader)
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

        internal override void Write(SkcWriter w)
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
        public byte[]? FileHeader;
        public byte[]? Header;
        public byte[]? Image;

        public SkcDib()
        {

        }

        /// <summary>
        /// dibをinfoheaderとimageから作ります。
        /// </summary>
        public SkcDib(byte[] header, byte[] image)
        {
            Header = header;
            Image = image;
            CalcBitmapFuileHeader();
        }

        /// <summary>
        /// dib(fileheader+infoheader+image)を渡します
        /// </summary>
        public SkcDib(byte[] dib)
        {
            FileHeader = dib[0..14];
            var a = dib[14] + (((int)dib[15]) << 8) + (((int)dib[16]) << 16) + (((int)dib[17]) << 24) + 14;
            Header = dib[14..a];
            Image = dib[a..];
        }


        void CalcBitmapFuileHeader()
        {
            if (Header == null) return;
            var ms = new MemoryStream();
            ms.WriteByte(0x42);//B
            ms.WriteByte(0x4d);//M
            var size = Header.Length + Image.Length + 14;
            byte[] buf = new byte[4];
            buf[0] = (byte)(size & 255);
            buf[1] = (byte)((size >> 8) & 255);
            buf[2] = (byte)((size >> 16) & 255);
            buf[3] = (byte)((size >> 24) & 255);
            ms.Write(buf, 0, 4);
            ms.WriteByte(0);
            ms.WriteByte(0);
            ms.WriteByte(0);
            ms.WriteByte(0);
            size = 14 + Header.Length;
            buf[0] = (byte)(size & 255);
            buf[1] = (byte)((size >> 8) & 255);
            buf[2] = (byte)((size >> 16) & 255);
            buf[3] = (byte)((size >> 24) & 255);
            ms.Write(buf, 0, 4);
            FileHeader = ms.ToArray();
        }

        internal void Read(SkcReader reader)
        {
            reader.ReadTags(new()
            {
                { "HEADER", (reader)=> Header = reader.ReadBytes() },
                { "IMAGE", (reader)=> Image = reader.ReadCompressBytes() },
            }); ;
            CalcBitmapFuileHeader();
        }
        internal void Write(SkcWriter w)
        {
            if (Header == null) throw new Exception("SkcDib::Write() Haeder is null.");
            if (Image == null) throw new Exception("SkcDib::Write() Image is null.");
            w.WriteBytes("HEADER", Header);
            w.WriteCompressBytes("IMAGE", Image);
        }

    }
}
