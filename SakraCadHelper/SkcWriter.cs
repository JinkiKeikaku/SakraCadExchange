using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper
{
    internal class SkcWriter
    {
        TextWriter mWriter;
        int mIndent = 0;
        bool mNewLined = true; 
        public SkcWriter(TextWriter w)
        {
            mWriter = w;
        }

        public void NewLine()  {
            mWriter.WriteLine();
            mNewLined = true;
        }

        void Intent()
        {
            if (mNewLined) {
                for (var i = 0; i < mIndent; i++) mWriter.Write(' ');
                mNewLined = false;
            }
        }

        public void Write<T>(string tag, T x, bool endLine = false)
        {
            Intent();
            mWriter.Write($"{tag}({x})");
            if (endLine) NewLine();// mWriter.WriteLine();
        }
        public void Write<T>(string tag, T x, T defaultValue, bool endLine = false)
        {
            if (x?.Equals(defaultValue) != true)
            {
                Intent();
                mWriter.Write($"{tag}({x})");
                if (endLine) NewLine();// mWriter.WriteLine();
            }
        }

        public void WriteString(string tag, string text, bool endLine = false)
        {
            var s = text.Replace("\"", "\"\"");
            Intent();
            mWriter.Write($"{tag}(\"{s}\")");
        }

        public void WriteObject(string tag, bool newLine, Action<SkcWriter> action, bool endLine = false)
        {
            Intent();
            mWriter.Write($"{tag}(");
            mIndent++;
            if (newLine) NewLine();// mWriter.WriteLine();
            action(this);
            mIndent--;
            Intent();
            mWriter.Write($")");
            if (endLine) NewLine();// mWriter.WriteLine();
        }

        public void WriteObjects<T>(string tag, IEnumerable<T> objects,  Action<SkcWriter, T> action, bool endLine = false)
        {
            WriteObject(tag, true, w =>
            {
                foreach (var obj in objects)
                {
                    action(this, obj);
                    NewLine();
                }
            }, endLine);
        }
        public void WriteBytes(string tag, byte[] bytes)
        {
            Intent();
            mWriter.Write($"{tag}({bytes.Length}");
            mIndent++;
            for (var i = 0; i < bytes.Length; i += 54)
            {
                var size = (bytes.Length - i >= 54) ? 54 : bytes.Length - i;
                var s = Convert.ToBase64String(bytes, i, size);
                mWriter.Write(",");
                NewLine();
                Intent();
                mWriter.Write($"\"{s}\"");
            }
            mIndent--;
            Intent();
            mWriter.Write(")");
        }

        public void WriteCompressBytes(string tag, byte[] bytes)
        {
            using var bs = new MemoryStream();
            using var ds = new ZLibStream(bs, CompressionMode.Compress);
            ds.Write(bytes, 0, bytes.Length);
            ds.Close();
            var buf = bs.ToArray();
            Intent();
            mWriter.Write($"{tag}({bytes.Length},\"ZLIB\"");
            mIndent++;
            for (var i = 0; i < buf.Length; i += 54)
            {
                var size = (buf.Length - i >= 54) ? 54 : buf.Length - i;
                var s = Convert.ToBase64String(buf, i, size);
                mWriter.Write(",");
                NewLine();
                Intent();
                mWriter.Write($"\"{s}\"");
            }
            mIndent--;
            Intent();
            mWriter.Write(")");
        }
    }
}
