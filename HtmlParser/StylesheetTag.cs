namespace Irvin.Parser.Html
{
    public class StylesheetTag : Tag
    {
        public StylesheetTag(TokenCollection source)
        {
            StylesheetContent = LanguageWrapperParse(source);
        }

        public override bool IsLanguageWrapper => true;

        public string StylesheetContent { get; set; }
    }
}