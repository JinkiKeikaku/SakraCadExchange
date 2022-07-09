using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper
{
    public class SkcWriter
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
    }
}
