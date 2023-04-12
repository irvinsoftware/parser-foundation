using System;
using System.Diagnostics;
using System.Text;

namespace Irvin.Parser
{
    public abstract class Parser
    {
        protected abstract ParserSettings GetSettings();

        public TokenCollection Parse(string content, 
                                     StringComparison compareOption = StringComparison.CurrentCultureIgnoreCase,
                                     TimeSpan? timeout = null)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            timeout = timeout ?? TimeSpan.FromSeconds(5);
            if (Debugger.IsAttached)
            {
                timeout = TimeSpan.MaxValue;
            }
            
            TokenCollection elements = new TokenCollection();
            ParserSettings settings = GetSettings();

            int i = 0;
            int passes = 0;
            int bufferBegin = 0;
            StringBuilder buffer = new StringBuilder();
            SubgroupSettings applicableSubgroup = null;
            while (i < content.Length)
            {
                passes++;
                Debug.Assert(passes <= content.Length);
                Debug.Assert(buffer.Length <= content.Length);
                if (timer.Elapsed.TotalMilliseconds >= timeout.Value.TotalMilliseconds)
                {
                    throw new TimeoutException();
                }
                
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

                        Add(elements, new Token
                        {
                            Content = realizedBuffer,
                            SubContent = subContent,
                            StartPosition = bufferBegin,
                            IsSubGroup = true
                        });

                        applicableSubgroup = null;
                        buffer.Clear();
                        bufferBegin = i + 1;
                    }
                    else
                    {
                        int endSectionStart = content.IndexOf(applicableSubgroup.EndSymbol, bufferBegin + 1, compareOption);
                        int endSectionEnd = endSectionStart + applicableSubgroup.EndSymbol.Length - 1;
                        int substringStart = bufferBegin + applicableSubgroup.StartSymbol.Length;
                        string substring = content.Substring(substringStart, endSectionEnd - substringStart);
                        buffer.Append(substring);
                        i = endSectionEnd - 1;
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

                                //preceding content
                                if (buffer.Length > match.Delimiter.Length)
                                {
                                    relevantBufferSection = realizedBuffer.Substring(0, buffer.Length - match.Delimiter.Length);
                                    Add(elements, new Token
                                    {
                                        Content = relevantBufferSection,
                                        StartPosition = bufferBegin,
                                    });
                                }

                                //delimiter
                                int delimiterStartIndex = buffer.Length - match.Delimiter.Length;
                                relevantBufferSection = realizedBuffer.Substring(delimiterStartIndex, match.Delimiter.Length);
                                Add(elements, new Token
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
                Add(elements, new Token
                {
                    Content =  buffer.ToString(),
                    StartPosition = bufferBegin
                });
            }

            timer.Stop();
            Debug.Print($"Parse time: {timer.Elapsed}");
            return elements;
        }

        private void Add(TokenCollection elements, Token token)
        {
            elements.Add(token);
            //Debug.Print($"{token.StartPosition}: {token}");
        }
    }
}