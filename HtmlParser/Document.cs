namespace Irvin.Parser.Html
{
    public class Document : Tag
    {
        internal Document(TokenCollection source)
            : base(source)
        {
        }

        public Header Header { get; set; }
        public Body Body { get; set; }

        protected override void AddChild(Tag childTag)
        {
            base.AddChild(childTag);
            
            Header header = childTag as Header;
            if (header != null)
            {
                Header = header;
            }
            
            Body body = childTag as Body;
            if (body != null)
            {
                Body = body;
            }
        }
    }
}