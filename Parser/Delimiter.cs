using System;

namespace Irvin.Parser
{
    public class Delimiter
    {
        protected readonly string _delimiterExpression;

        public Delimiter(string delimiterExpression)
        {
            _delimiterExpression = delimiterExpression;
        }

        public int ExpressionLength
        {
            get { return _delimiterExpression.Length; }
        }

        public virtual DelimiterMatch Matches(string input, StringComparison compareOption)
        {
            if(input.EndsWith(_delimiterExpression, compareOption))
            {
                int delimiterStartIndex = Math.Max(input.Length - _delimiterExpression.Length, 0);
                return new DelimiterMatch
                {
                    Input = input,
                    Delimiter = input.Substring(delimiterStartIndex)
                };
            }
            return null;
        }

        public override bool Equals(object other)
        {
            if (other != null)
            {
                Delimiter delimiter = other as Delimiter;
                return delimiter != null && 
                       _delimiterExpression.Equals(delimiter._delimiterExpression);
            }
            return false;
        }

        public static implicit operator Delimiter(string s)
        {
            return new Delimiter(s);
        }
    }
}