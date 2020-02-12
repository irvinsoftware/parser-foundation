using System;
using System.Collections.Generic;
using System.Linq;

namespace Irvin.Parser
{
    public class ParserSettings
    {
        private readonly List<Delimiter> _delimiters;
        private readonly List<string> _escapeCharacters;

        public ParserSettings()
        {
            _delimiters = new List<Delimiter>();
            _escapeCharacters = new List<string>();
            Subgroups = new List<SubgroupSettings>();
        }

        public IEnumerable<Delimiter> Delimiters
        {
            get { return _delimiters; }
        }

        public IEnumerable<string> EscapeCharacters
        {
            get { return _escapeCharacters; }
        }

        public List<SubgroupSettings> Subgroups { get; private set; }

        public IEnumerable<Delimiter> DelimitersByPrecedence
        {
            get { return _delimiters.OrderByDescending(x => x.ExpressionLength); }
        }

        public void AddDelimiter(string delimiter)
        {
            if (!string.IsNullOrEmpty(delimiter))
            {
                AddToList(_delimiters, delimiter);
                if (delimiter == Environment.NewLine)
                {
                    AddToList(_delimiters, delimiter.Last().ToString());
                }
            }
        }

        public void AddEscapeCharacter(string escapeCharacter)
        {
            AddToList(_escapeCharacters, escapeCharacter);
        }

        private static void AddToList<T>(List<T> list, T item)
        {
            if (item != null && !list.Contains(item))
            {
                list.Add(item);
            }
        }

        public void AddDelimiterExpression(string expression)
        {
            _delimiters.Add(new RegexDelimiter(expression));
        }
    }
}