using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SakraCadHelper
{
    internal class SkcTokenizer
    {
        public enum TokenKind
        {
            Unknown,
            Eof,
            Number,
            Identifier,
            LPar,
            RPar,
            Comma,
            String,
            Symbol,
        }
        public class Token
        {
            public TokenKind Kind;
            public string Value;
            public bool IsEof => Kind == TokenKind.Eof;

            public Token()
            {
                Kind = TokenKind.Eof;
                Value = "";
            }

            public Token(TokenKind kind, string value)
            {
                Kind = kind;
                Value = value;
            }

            public string GetString() => Value;
            public double GetDouble() => double.Parse(GetString());
            public int GetInt() => int.Parse(GetString());
            public override string ToString() => $"{Kind}::{GetString()}";
            public static readonly Token EofToken = new Token(TokenKind.Eof, "");
        }


        TextReader mReader;
        Stack<int> mCharStack = new();
        Stack<Token> mTokeenStack = new();
        Token CurrentToken = new();
        public SkcTokenizer(TextReader reader)
        {
            mReader = reader;
        }



        public Token GetNextToken()
        {
            if(mTokeenStack.Count > 0)  return mTokeenStack.Pop();
            Skip();
            var c = GetChar();
            if (c < 0)
            {
                CurrentToken = Token.EofToken;
                return CurrentToken;
            }
            switch ((char)c)
            {
                case ',':
                    CurrentToken = new Token(TokenKind.Comma, ",");
                    return CurrentToken;
                case '(':
                    CurrentToken = new Token(TokenKind.LPar, "(");
                    return CurrentToken;
                case ')':
                    CurrentToken = new Token(TokenKind.RPar, ")");
                    return CurrentToken;
                case '"':
                    CurrentToken = ReadString();
                    return CurrentToken;
                default:
                    if (char.IsLetter((char)c))
                    {
                        var sb = new StringBuilder();
                        sb.Append((char)c);
                        while (true)
                        {
                            c = GetChar();
                            if (!char.IsLetterOrDigit((char)c)) break;
                            sb.Append((char)c);
                        }
                        PushChar(c);
                        var s = sb.ToString();
                        CurrentToken = new Token(TokenKind.Identifier, s);
                        return CurrentToken;
                    }
                    if (char.IsDigit((char)c) || (char)c == '-' || (char)c == '+' || (char)c == '.')
                    {
                        var sb = new StringBuilder();
                        sb.Append((char)c);
                        while (true)
                        {
                            c = GetChar();
                            if (char.IsDigit((char)c) ||
                                (char)c == 'e' || (char)c == 'E' ||
                                (char)c == '.' || (char)c == '+' || (char)c == '-')
                            {
                                sb.Append((char)c);
                            }
                            else
                            {
                                break;
                            }
                        }
                        PushChar(c);
                        var s = sb.ToString();
                        if (!Double.TryParse(s, out double d))
                        {
                            CurrentToken = new Token(TokenKind.Unknown, s);
                            return CurrentToken;
                        }
                        CurrentToken = new Token(TokenKind.Number, s);
                        return CurrentToken;
                    }
                    CurrentToken = new Token(TokenKind.Unknown, ((char)c).ToString());
                    return CurrentToken;
            }
        }

        public void PushToken(Token tok)
        {
            mTokeenStack.Push(tok);
        }

        void PushChar(int c)
        {
            mCharStack.Push(c);
        }

        int GetChar()
        {
            if (mCharStack.Count > 0) return mCharStack.Pop();
            return mReader.Read(); 
        }


        void Skip()
        {
            while (true)
            {
                var c = GetChar();
                    if (c < 0 || !Char.IsWhiteSpace((char)c))
                    {
                        PushChar(c);
                        break;
                    }
            }
        }

        Token ReadString()
        {
            var sb = new StringBuilder();
            while (true)
            {
                var c = GetChar();
                if (c < 0) throw new Exception($"Unexpected eof was found in string : {sb}");
                if ((char)c == '"')
                {
                    c = GetChar();
                    if (c < 0) throw new Exception($"Unexpected eof was found in string : {sb}");
                    if ((char)c != '"')
                    {
                        PushChar(c);
                        break;
                    }
                }
                sb.Append((char)c);
            }
            return new Token(TokenKind.String, sb.ToString());
        }
    }
}
