namespace Irvin.Parser.Html
{
    public class DocTypeTag : Tag
    {
        internal DocTypeTag(TokenCollection source)
        {
            Name = source.Current.Content.Substring(2);
            Append(source.Current.Content);
            MoveNextAppendingSpaces(source);

            AddAttribute(source);
            MoveNextAppendingSpaces(source);
            Append(source.Current.Content);
            if (source.HasNext())
            {
                source.MoveNext();
            }
        }
    }
}