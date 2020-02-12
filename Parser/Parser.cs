using System;
using System.Linq;
using System.Text;

namespace Irvin.Parser
{
    public abstract class Parser
    {
        protected abstract ParserSettings GetSettings();

        public TokenCollection Parse(string content, StringComparison compareOption = StringComparison.CurrentCultureIgnoreCase)
        {
            TokenCollection elements = new TokenCollection();
            ParserSettings settings = GetSettings();

            int i = 0;
            int bufferBegin = 0;
            StringBuilder buffer = new StringBuilder();
            SubgroupSettings applicableSubgroup = null;
            while (i < content.Length)
            {
                char thisChar = content[i];
                buffer.Append(thisChar);
                string realizedBuffer = buffer.ToString();

                if (applicableSubgroup == null)
                {
                    applicableSubgroup = settings.Subgroups.Find(x => x.StartSymbol.Equals(realizedBuffer, compareOption));
                }

                if (applicableSubgroup != null)
                {
                    string contentSegment = applicableSubgroup.EscapeSymbol != null
                        ? content.Substring(i, applicableSubgroup.EscapeSymbol.Length)
                        : string.Empty;

                    if (contentSegment.Equals(applicableSubgroup.EscapeSymbol, compareOption))
                    {
                        buffer.Append(contentSegment);
                        i += applicableSubgroup.EscapeSymbol.Length - 1;
                    }
                    else if (
                        realizedBuffer.EndsWith(applicableSubgroup.EndSymbol, compareOption) &&
                        buffer.Length > 1)
                    {
                        int subContentLength = realizedBuffer.Length -
                                               applicableSubgroup.EndSymbol.Length -
                                               applicableSubgroup.StartSymbol.Length;
                        string subContent = realizedBuffer.Substring(applicableSubgroup.StartSymbol.Length,
                            subContentLength);

                        elements.Add(new Token
                        {
                            Content = realizedBuffer,
                            SubContent = subContent,
                            StartPosition = bufferBegin,
                        });

                        applicableSubgroup = null;
                        buffer.Clear();
                        bufferBegin = i + 1;
                    }
                }
                else
                {
                    foreach (Delimiter delimiter in settings.DelimitersByPrecedence)
                    {
                        DelimiterMatch match = delimiter.Matches(realizedBuffer, compareOption);
                        if(match != null)
                        {
                            string usedEscape = null;
                            foreach (string escapeCharacter in settings.EscapeCharacters)
                            {
                                string escapedDelimiter = escapeCharacter + match.Delimiter;
                                if (realizedBuffer.EndsWith(escapedDelimiter, compareOption))
                                {
                                    usedEscape = escapeCharacter;
                                }
                            }
                            if (usedEscape != null)
                            {
                                buffer.Remove(buffer.Length - match.Delimiter.Length - usedEscape.Length, usedEscape.Length);
                            }
                            else
                            {
                                string relevantBufferSection;

                                //preceeding content
                                if (buffer.Length > match.Delimiter.Length)
                                {
                                    relevantBufferSection = realizedBuffer.Substring(0, buffer.Length - match.Delimiter.Length);
                                    elements.Add(new Token
                                    {
                                        Content = relevantBufferSection,
                                        StartPosition = bufferBegin,
                                    });
                                }

                                //delimiter
                                int delimiterStartIndex = buffer.Length - match.Delimiter.Length;
                                relevantBufferSection = realizedBuffer.Substring(delimiterStartIndex, match.Delimiter.Length);
                                elements.Add(new Token
                                {
                                    Content = relevantBufferSection,
                                    StartPosition = bufferBegin + delimiterStartIndex,
                                    IsDelimiter = true
                                });

                                buffer.Clear();
                                bufferBegin = i + 1;
                                break;
                            }
                        }
                    }
                }

                i++;
            }

            if (buffer.Length > 0)
            {
                elements.Add(new Token
                {
                    Content =  buffer.ToString(),
                    StartPosition = bufferBegin
                });
            }

            return elements;
        }
    }
}