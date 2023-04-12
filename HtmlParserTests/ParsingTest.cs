using Irvin.Parser;
using HtmlParser = Irvin.Parser.Html.HtmlParser;

namespace HtmlParserTests
{
    public class ParsingTest
    {
        protected static TokenCollection From(string input)
        {
            HtmlParser parser = new HtmlParser();
            TokenCollection tokenCollection = parser.Parse(input);
            tokenCollection.MoveNext();
            return tokenCollection;
        }
    }
}