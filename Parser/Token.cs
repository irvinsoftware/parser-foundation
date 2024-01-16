using System;
using System.Collections.Generic;
using System.Linq;
using Irvin.Extensions.Collections;

namespace Irvin.Parser
{
    public class Token
    {
        private string _content;

        public string Content
        {
            get
            {
                return _content;
            }
            internal set
            {
                _content = value;
                ContentUpper = value?.ToUpper();
                ContentLower = value?.ToLower();
            }
        }

        public string ContentUpper { get; private set; }
        public string ContentLower { get; private set; }
        
        public string SubContent { get; set; }
        public int StartPosition { get; internal set; }
        public bool IsDelimiter { get; internal set; }

        public override string ToString()
        {
            return Content;
        }

        public static string Join(IEnumerable<Token> tokens)
        {
            return tokens.Select(x => x.Content).Concatenate();
        }

        public bool IsSpace => Content == " " || Content == "\t";
        public bool IsSpaceOrNewLine => IsSpace || Content == Environment.NewLine;
        public bool IsSpaceOrLineFeed => IsSpace || IsLineFeed;
        public bool IsLineFeed => Content == "\n";

        /// <summary>
        /// The token's text matches the Unicode definition of whitespace.
        /// </summary>
        public bool IsWhitespace => Content.All(char.IsWhiteSpace);
    }
}