namespace Irvin.Parser
{
    public class HtmlParser : Parser
    {
        protected override ParserSettings GetSettings()
        {
            ParserSettings settings = new ParserSettings();
            settings.AddDelimiter('<'.ToString());
            settings.AddDelimiter('>'.ToString());
            settings.AddDelimiter(' '.ToString());
            settings.AddDelimiter('='.ToString());
            settings.AddDelimiter('/'.ToString());
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
            return settings;
        }
    }
}