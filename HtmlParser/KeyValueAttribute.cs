using System;
using System.Linq;

namespace Irvin.Parser.Html
{
    public class KeyValueAttribute : Attribute
    {
        internal KeyValueAttribute(TokenCollection source)
        {
            Token firstToken = source.Current;
            Append(firstToken.Content);
            if (firstToken.IsSubGroup)
            {
                NameDelimiter = firstToken.Content.First();
                Name = firstToken.Content.Substring(1, firstToken.Content.Length - 2);
            }
            else
            {
                Name = firstToken.Content;
            }
            
            MoveNextAppendingSpaces(source);
            if (source.Current.Content == "=")
            {
                Append(source.Current.Content);
                MoveNextAppendingSpaces(source);
            }
            else
            {
                throw new FormatException(
                    $"Expected: '=', but found '{source.Current.Content}' (position {source.Current.StartPosition})");
            }

            Token lastToken = source.Current;
            Append(lastToken.Content);
            if (lastToken.IsSubGroup)
            {
                ValueDelimiter = lastToken.Content.First();
                Value = lastToken.Content.Substring(1, lastToken.Content.Length - 2);
            }
            else
            {
                Value = lastToken.Content;
            }
            
            MoveNextAppendingSpaces(source);
        }

        public KeyValueAttribute(string key, string value)
        {
            Name = key;
            Value = value;
        }

        public string Value { get; set; }
        public char? NameDelimiter { get; set; }
        public char? ValueDelimiter { get; set; }
    }
}