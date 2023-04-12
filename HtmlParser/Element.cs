using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irvin.Parser.Html
{
    public abstract class Element
    {
        protected Element()
        {
            Content = new StringBuilder();
        }
        
        private StringBuilder Content { get; }

        internal void MoveNextAppendingSpaces(TokenCollection source)
        {
            List<Token> output = new List<Token>();
            source.MoveNextSkippingSpaces(output);
            foreach (Token spaceTokens in output)
            {
                Append(spaceTokens.Content);
            }
        }

        internal IEnumerable<string> Append(IEnumerable<Token> tokens)
        {
            List<string> enumerable = new List<string>();

            foreach (Token token in tokens)
            {
                string append = Append(token.Content);
                enumerable.Add(append);
            }

            return enumerable;
        }
        
        internal string Append(string chunk)
        {
            Content.Append(chunk);
            return chunk;
        }
        
        public override string ToString()
        {
            return Content.ToString();
        }
    }
}