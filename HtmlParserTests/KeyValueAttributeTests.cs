using FluentAssertions;
using Irvin.Parser.Html;
using NUnit.Framework;

namespace HtmlParserTests
{
    [TestFixture]
    public class KeyValueAttributeTests : ParsingTest
    {
        [Test]
        public void AgnosticCreationTest_VerySimple()
        {
            const string input = "zack=pan";

            KeyValueAttribute attribute = Attribute.Build(From(input)) as KeyValueAttribute;

            attribute.Should().NotBeNull();
            attribute.Name.Should().Be("zack");
            attribute.NameDelimiter.Should().BeNull();
            attribute.Value.Should().Be("pan");
            attribute.ValueDelimiter.Should().BeNull();
            attribute.ToString().Should().Be(input);
        }

        [TestCase("\"href\"=\"https://www.panam.com/\"")]
        [TestCase("\"href\" =\"https://www.panam.com/\"")]
        [TestCase("\"href\"= \"https://www.panam.com/\"")]
        public void AgnosticCreationTest_Standard(string input)
        {
            KeyValueAttribute attribute = Attribute.Build(From(input)) as KeyValueAttribute;
            
            attribute.Should().NotBeNull();
            attribute.Name.Should().Be("href");
            attribute.NameDelimiter.Should().Be('"');
            attribute.Value.Should().Be("https://www.panam.com/");
            attribute.ValueDelimiter.Should().Be('"');
            attribute.ToString().Should().Be(input);
        }
        
        [TestCase("'href'='https://www.panam.com/'")]
        [TestCase("'href' ='https://www.panam.com/'")]
        [TestCase("'href'= 'https://www.panam.com/'")]
        public void AgnosticCreationTest_SemiStandard(string input)
        {
            KeyValueAttribute attribute = Attribute.Build(From(input)) as KeyValueAttribute;
            
            attribute.Should().NotBeNull();
            attribute.Name.Should().Be("href");
            attribute.NameDelimiter.Should().Be('\'');
            attribute.Value.Should().Be("https://www.panam.com/");
            attribute.ValueDelimiter.Should().Be('\'');
            attribute.ToString().Should().Be(input);
        }
    }
}