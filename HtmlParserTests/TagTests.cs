using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Irvin.Parser.Html;
using NUnit.Framework;

namespace HtmlParserTests
{
    [TestFixture]
    [Timeout(10000)]
    public class TagTests : ParsingTest
    {
        [Test]
        public void DocTypeTagParseTest()
        {
            string input = "<!DOCTYPE html>";
            
            Tag actual = Tag.Build(From(input));

            actual.Should().BeOfType<DocTypeTag>();
            actual.Attributes.Count.Should().Be(1);
            actual.Attributes.First().Should().BeOfType<UnitaryAttribute>();
            actual.Attributes.First().Name.Should().Be("html");
            actual.ToString().Should().Be(input);
        }

        [Test]
        public void CustomTagNameTest()
        {
            string input = "<platform-footer></platform-footer>";
            
            Tag actual = Tag.Build(From(input));

            actual.Should().NotBeNull();
            actual.Name.Should().Be("platform-footer");
            actual.ToString().Should().Be(input);
        }

        [TestCase("\r")]
        [TestCase("\n")]
        [TestCase("\r\n")]
        public void StyleTagAttributesTest(string newLineCharacter)
        {
            string input = @"<style ""type""=""text/css"" ""zumba-class""=""cancelled"">
                @media (hover:hover){.iEKiLt:hover{color:#212225;-webkit-text-decoration:none;text-decoration:none;}}/*!sc*/
            </style>"
                .Replace(Environment.NewLine, newLineCharacter);
            
            Tag actual = Tag.Build(From(input));

            var actualTag = actual as StylesheetTag;
            actualTag.Should().NotBeNull();
            actualTag.Attributes.Count.Should().Be(2);
            IEnumerable<KeyValueAttribute> actualAttributes = actualTag.Attributes.Cast<KeyValueAttribute>();
            actualAttributes.First().Name.Should().Be("type");
            actualAttributes.First().Value.Should().Be("text/css");
            actualAttributes.Last().Name.Should().Be("zumba-class");
            actualAttributes.Last().Value.Should().Be("cancelled");
            actualTag.StylesheetContent.Trim().Should().Be("@media (hover:hover){.iEKiLt:hover{color:#212225;-webkit-text-decoration:none;text-decoration:none;}}/*!sc*/");
            //actual.ToString().Should().Be(input);
        }

        [TestCase("\r")]
        [TestCase("\n")]
        [TestCase("\r\n")]
        public void PolyglotHtmlDocumentParseTest(string newLineCharacter)
        {
            string input = @"
                <html>
                    <head>
                    <style type='text/css'>
                        .zoom { thing-property: italic; }
                    </style>
                    </head>
                    <body>
                        Hi there I am a potato.
                        <script type=""text/javascript"">
                            if !(zap < style) {
                                craig /= 0;
                            }
                        </script>
                    </body>
                </html>
            "
                .Replace(Environment.NewLine, newLineCharacter);
            
            Tag actual = Tag.Build(From(input));

            Document actualDocument = actual as Document;
            actualDocument.Should().NotBeNull();
            actual.Children.Count.Should().Be(2);
            actualDocument.Body.Should().NotBeNull();
            actualDocument.Header.Should().NotBeNull();
            //actual.ToString().Should().Be(input);
        }
    }
}