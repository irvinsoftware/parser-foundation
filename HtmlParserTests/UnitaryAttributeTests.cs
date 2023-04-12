using FluentAssertions;
using Irvin.Parser.Html;
using NUnit.Framework;

namespace HtmlParserTests
{
    [TestFixture]
    public class UnitaryAttributeTests : ParsingTest
    {
        [Test]
        public void MostBasicTest()
        {
            Attribute actual = Attribute.Build(From("zack"));

            actual.Should().NotBeNull();
            actual.Should().BeOfType<UnitaryAttribute>();
            actual.Name.Should().Be("zack");
        }
    }
}