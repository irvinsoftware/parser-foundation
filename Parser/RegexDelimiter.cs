using System;
using System.Text.RegularExpressions;

namespace Irvin.Parser
{
    public class RegexDelimiter : Delimiter
    {
        public RegexDelimiter(string delimiterExpression) 
            : base(delimiterExpression)
        {
        }

        public override DelimiterMatch Matches(string input, StringComparison compareOption)
        {
            RegexOptions options = RegexOptions.Singleline;
            if (compareOption == StringComparison.CurrentCultureIgnoreCase ||
                compareOption == StringComparison.InvariantCultureIgnoreCase)
            {
                options |= RegexOptions.IgnoreCase;
            }

            Match match = Regex.Match(input, _delimiterExpression, options);

            if (match.Success)
            {
                return new DelimiterMatch
                {
                    Input = input,
                    Delimiter = match.Captures[0].Value
                };    
            }

            return null;
        }
    }
}