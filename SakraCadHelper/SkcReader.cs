using SakraCadHelper.Shape;
using System.IO.Compression;
using System.Text;

namespace SakraCadHelper
{
    internal class SkcReader
    {
        SkcTokenizer mTokenizer = null!;

        /// <inheritdoc/>
        public SkcReader(TextReader r)
        {
            mTokenizer = new SkcTokenizer(r);
        }

        public void SkipTag()
        {
            while (true)
            {
                var tok = mTokenizer.GetNextToken();
                if (tok.Kind == SkcTokenizer.TokenKind.Eof || tok.Kind == SkcTokenizer.TokenKind.RPar)
                {
                    mTokenizer.PushToken(tok);
                    break;
                }
                if (tok.Kind == SkcTokenizer.TokenKind.LPar)
                {
                    SkipTag();
                    tok = mTokenizer.GetNextToken();
                    if (tok.Kind == SkcTokenizer.TokenKind.Eof)
                    {
                        mTokenizer.PushToken(tok);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// TAG(...)の中身を[action]で読み込みます。TAGが[tag]でない場合はfalseが返ります。
        /// 主にTAGの出現順が決まっているセクションの読み込みで使います。
        public bool ReadTag(string tag, Action<SkcReader> action)
        {
            var tok = mTokenizer.GetNextToken();
            if (tok.Kind != SkcTokenizer.TokenKind.Identifier)
            {
                throw new Exception($"Could not find tag:{tag}(found token is {tok})");
            }
            if (tok.Value != tag)
            {
                mTokenizer.PushToken(tok);
                return false;
            }
            tok = mTokenizer.GetNextToken();
            if (tok.Kind != SkcTokenizer.TokenKind.LPar)
            {
                throw new Exception($"Could not find '(':{tag}(found token is {tok})");
            }
            action(this);
            tok = mTokenizer.GetNextToken();
            if (tok.Kind != SkcTokenizer.TokenKind.RPar)
            {
                throw new Exception($"Could not find ')':{tag}(found token is {tok})");
            }
            return true;
        }

        /// <summary>
        /// 複数のタグを読み込みます。[tagAction]にタグ名と処理を設定します。[eleAction]には[tagAction]にないタグを
        /// 処理する場合に設定します。[eleAction]がnullの場合、そのタグは無視（Skip）されます。
        /// </summary>
        /// <param name="tagAction"></param>
        /// <param name="elseAction"></param>
        /// <exception cref="Exception"></exception>
        public void ReadTags(Dictionary<string, Action<SkcReader>> tagAction, Action<SkcReader, string>? elseAction = null)
        {
            while (true)
            {
                var tok = mTokenizer.GetNextToken();
                if (tok.Kind == SkcTokenizer.TokenKind.Eof || tok.Kind == SkcTokenizer.TokenKind.RPar)
                {
                    mTokenizer.PushToken(tok);
                    break;
                }
                if (tok.Kind != SkcTokenizer.TokenKind.Identifier)
                {
                    throw new Exception($"Could not find identifier.(found token is {tok})");
                }
                var tag = tok.Value;
                tok = mTokenizer.GetNextToken();
                if (tok.Kind != SkcTokenizer.TokenKind.LPar)
                {
                    throw new Exception($"Could not find '(':{tag}(found token is {tok})");
                }
                if (tagAction.TryGetValue(tag, out var action))
                {
                    action(this);
                }
                else
                {
                    if (elseAction != null)
                    {
                        elseAction(this, tag);
                    }
                    else
                    {
                        SkipTag();
                    }
                }
                tok = mTokenizer.GetNextToken();
                if (tok.Kind != SkcTokenizer.TokenKind.RPar)
                {
                    throw new Exception($"Could not find ')':{tag}(found token is {tok})");
                }
            }
        }

        /// <summary>
        /// タグの値が文字列の時使います。
        /// </summary>
        public string ReadString()
        {
            var tok = mTokenizer.GetNextToken();
            if (tok.Kind != SkcTokenizer.TokenKind.String) throw new Exception($"ReadString::token is not String : {tok}");
            return tok.GetString();
        }

        /// <summary>
        /// タグの値がNUMBERの時、値をintで返します。
        /// </summary>
        public int ReadInt()
        {
            var tok = mTokenizer.GetNextToken();
            if (tok.Kind != SkcTokenizer.TokenKind.Number) throw new Exception($"ReadInt::token is not Number : {tok}");
            return tok.GetInt();
        }

        /// <summary>
        /// タグの値がNUMBERの時、値をdoubleで返します。
        /// </summary>
        public double ReadDouble()
        {
            var tok = mTokenizer.GetNextToken();
            if (tok.Kind != SkcTokenizer.TokenKind.Number) throw new Exception($"ReadDouble::token is not Number : {tok}");
            return tok.GetDouble();
        }

        /// <summary>
        /// タグをSkcPointで読みます。
        /// </summary>
        public SkcPoint ReadPoint()
        {
            var tok = mTokenizer.GetNextToken();
            if (tok.Kind != SkcTokenizer.TokenKind.Number) throw new Exception($"ReadPoint::token is not Number(1st) : {tok}");
            var x = tok.GetDouble();
            tok = mTokenizer.GetNextToken();
            if (tok.Kind != SkcTokenizer.TokenKind.Comma) throw new Exception($"ReadPoint::token is not Comma : {tok}");
            tok = mTokenizer.GetNextToken();
            if (tok.Kind != SkcTokenizer.TokenKind.Number) throw new Exception($"ReadPoint::token is not Number(2nd) : {tok}");
            var y = tok.GetDouble();
            return new SkcPoint(x, y);
        }
        public void SkipComma()
        {
            var tok = mTokenizer.GetNextToken();
            if (tok.Kind != SkcTokenizer.TokenKind.Comma) throw new Exception($"ReadPoint::token is not Comma : {tok}");
        }

        public byte[] ReadBytes()
        {
            var tok = mTokenizer.GetNextToken();
            if (tok.Kind != SkcTokenizer.TokenKind.Number) throw new Exception($"ReadBytes::token is not Number(1st) : {tok}");
            var size = tok.GetInt();
            tok = mTokenizer.GetNextToken();
            if (tok.Kind != SkcTokenizer.TokenKind.Comma) throw new Exception($"ReadBytes::token is not Comma : {tok}");
            var sb = new StringBuilder();
            while (true)
            {
                tok = mTokenizer.GetNextToken();
                if(tok.Kind != SkcTokenizer.TokenKind.String)
                {
                    mTokenizer.PushToken(tok);
                    break;
                }
                sb.Append(tok.GetString());
                tok = mTokenizer.GetNextToken();
                if (tok.Kind != SkcTokenizer.TokenKind.Comma)
                {
                    mTokenizer.PushToken(tok);
                    break;
                }
            }
            return Convert.FromBase64String(sb.ToString());
        }
        public byte[] ReadCompressBytes()
        {
            var tok = mTokenizer.GetNextToken();
            if (tok.Kind != SkcTokenizer.TokenKind.Number) throw new Exception($"ReadBytes::token is not Number(1st) : {tok}");
            var size = tok.GetInt();
            tok = mTokenizer.GetNextToken();
            if (tok.Kind != SkcTokenizer.TokenKind.Comma) throw new Exception($"ReadBytes::token is not Comma : {tok}");
            tok = mTokenizer.GetNextToken();
            if (tok.Kind != SkcTokenizer.TokenKind.String || tok.GetString() != "ZLIB") throw new Exception($"ReadBytes::token is not \"ZLIB\" : {tok}");
            tok = mTokenizer.GetNextToken();
            if (tok.Kind != SkcTokenizer.TokenKind.Comma) throw new Exception($"ReadBytes::token is not Comma : {tok}");
            var sb = new StringBuilder();
            while (true)
            {
                tok = mTokenizer.GetNextToken();
                if (tok.Kind != SkcTokenizer.TokenKind.String)
                {
                    mTokenizer.PushToken(tok);
                    break;
                }
                sb.Append(tok.GetString());
                tok = mTokenizer.GetNextToken();
                if (tok.Kind != SkcTokenizer.TokenKind.Comma)
                {
                    mTokenizer.PushToken(tok);
                    break;
                }
            }
            var m = Convert.FromBase64String(sb.ToString());
            using var bs = new MemoryStream(m);
            using var ds = new ZLibStream(bs, CompressionMode.Decompress);
            var buf = new byte[size];
            var offset = 0;
            while (offset < buf.Length)
            {
                var readSize = ds.Read(buf, offset, buf.Length - offset);
                if (readSize == 0) break;
                offset += readSize;
            }
//            ds.Read(buf, 0, size);
            ds.Close();
            return buf;
        }
    }
}