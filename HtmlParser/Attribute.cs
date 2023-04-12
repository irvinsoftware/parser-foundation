namespace Irvin.Parser.Html
{
    public abstract class Attribute : Element
    {
        public string Name { get; set; }

        public static Attribute Build(TokenCollection source)
        {
            Token firstToken = source.Current;
            if (firstToken.IsSubGroup)
            {
                return new KeyValueAttribute(source);
            }

            source.SetCheckpoint();
            source.MoveNextSkippingSpaces();
            if (source.Current.Content == "=")
            {
                source.Rewind();
                return new KeyValueAttribute(source);
            }

            string name = firstToken.Content;
            var attribute = new UnitaryAttribute();
            attribute.Append(name);
            attribute.Name = name.Trim();
            return attribute;
        }
    }
}