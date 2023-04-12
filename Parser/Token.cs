using System;
using System.Collections.Generic;
using System.Linq;

namespace Irvin.Parser
{
    public class Token
    {
        public string Content { get; internal set; }
        public string SubContent { get; internal set; }
        public int StartPosition { get; internal set; }
        public bool IsDelimiter { get; internal set; }
        public bool IsSubGroup { get; internal set; }

        public override string ToString()
        {
            return Content;
        }

        public static string Join(IEnumerable<Token> tokens)
        {
            return String.Join("", tokens.Select(x => x.Content).ToArray());
        }

        public bool IsSpace
        {
            get { return Content == " " || Content == "\t"; }
        }

        public bool IsSpaceOrNewLine
        {
            get
            {
                return IsSpace || Content == Environment.NewLine;
            }
        }
    }
}