using System;

namespace Irvin.Parser.Html
{
    public class HtmlParser : Parser
    {
        protected override ParserSettings GetSettings()
        {
            ParserSettings settings = new ParserSettings();
            
            settings.Subgroups.Add(new SubgroupSettings
            {
                StartSymbol = "<style",
                EndSymbol = "</style>"
            });
            settings.Subgroups.Add(new SubgroupSettings
            {
                StartSymbol = "<script",
                EndSymbol = "</script>"
            });
            
            settings.AddDelimiterExpression(@"^<[abcdefghijklmnopqrtuvwxyz][a-z0-9\-]{0,}\s");
            
            settings.Subgroups.Add(new SubgroupSettings
            {
                StartSymbol =  @"""",
                EndSymbol = @""""
            });
            settings.Subgroups.Add(new SubgroupSettings
            {
                StartSymbol = @"'",
                EndSymbol = @"'"
            });

            settings.AddDelimiter("</");
            settings.AddDelimiter("/>");
            settings.AddDelimiter('>'.ToString());
            settings.AddDelimiter(' '.ToString());
            settings.AddDelimiter('='.ToString());

            return settings;
        }

        public Document ParseDocument(string html)
        {
            Document document;
            
            TokenCollection parsed = Parse(html);

            parsed.MoveNext();
            Tag tag = Tag.Build(parsed);
            DocTypeTag docTypeTag = tag as DocTypeTag;
            if (docTypeTag == null)
            {
                if (!(tag is Document))
                {
                    throw new FormatException();
                }
                document = (Document)tag;
            }
            else
            {
                document = (Document)Tag.Build(parsed);
            }
            
            return document;
        }
    }
}