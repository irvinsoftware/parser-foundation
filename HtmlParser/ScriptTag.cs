namespace Irvin.Parser.Html
{
    public class ScriptTag : Tag
    {
        internal ScriptTag(TokenCollection source)
        {
            ScriptDefinition = LanguageWrapperParse(source);
        }

        public string ScriptDefinition { get; set; }

        public override bool IsLanguageWrapper => true;
    }
}