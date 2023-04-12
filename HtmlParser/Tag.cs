using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Irvin.Extensions.Collections;

namespace Irvin.Parser.Html
{
    public class Tag : Element
    {
        public Tag()
        {
            Attributes = new List<Attribute>();
            Children = new List<Tag>();
        }

        internal Tag(TokenCollection source)
            : this()
        {
            CaptureTagStart(source);

            if (source.Current.Content == "/>")
            {
                Append(source.Current.Content);
                source.MoveNext();
            }
            else
            {
                Append(source.Current.Content);
                source.MoveNext();

                ushort passes = 0;
                Tag nextTag = Build(source);
                while (nextTag != null)
                {
                    passes++;
                    if (passes == ushort.MaxValue)
                    {
                        throw new StackOverflowException();
                    }
                    
                    AddChild(nextTag);
                    nextTag = Build(source);
                }

                if (source.Current.Content != "</")
                {
                    MoveNextAppendingSpaces(source);
                }

                ReadOnlyCollection<Token> readOnlyCollection = source.MoveUntil(x => x.Content == ">");
                Append(readOnlyCollection);
                Append(source.Current.Content);
                if (source.HasNext())
                {
                    source.MoveNext();
                }
            }
        }

        protected void CaptureTagStart(TokenCollection source)
        {
            if (!source.Current.Content.StartsWith("<"))
            {
                MoveNextAppendingSpaces(source);
            }

            Name = source.Current.Content.Substring(1);
            Append(source.Current.Content);

            MoveNextAppendingSpaces(source);

            AddAttributes(source);
        }

        protected void AddAttributes(TokenCollection source)
        {
            ushort passes = 0;
            while (source.HasNext() && !source.Current.Content.IsOneOf("/>", ">"))
            {
                passes++;
                if (passes == ushort.MaxValue)
                {
                    throw new StackOverflowException();
                }

                AddAttribute(source);
            }
        }

        protected virtual void AddChild(Tag childTag)
        {
            Children.Add(childTag);
        }

        public string Name { get; protected set; }
        public List<Attribute> Attributes { get; set; }
        public List<Tag> Children { get; set; }
        public virtual bool IsLanguageWrapper { get; }
        
        public static Tag Build(TokenCollection source)
        {
            if (source.Current == null)
            {
                source.MoveNext();
            }
            Debug.Assert(source.Current != null);
            
            if (source.Current.IsSpaceOrNewLine)
            {
                source.SetCheckpoint();
                source.MoveNextSkippingSpaces();
            }

            string tagName;
            if (source.Current.IsSubGroup)
            {
                tagName = source.Current.Content.Substring(1, 6);
            }
            else
            {
                tagName = source.Current.Content.Substring(1);
            }
            source.RewindIfPossible();
            
            switch (tagName.ToLower().Trim())
            {
                case "/":
                    return null;
                case "!doctype":
                    return new DocTypeTag(source);
                case "html":
                    return new Document(source);
                case "head":
                    return new Header(source);
                case "style":
                    return new StylesheetTag(source);
                case "script":
                    return new ScriptTag(source);
                case "body":
                    return new Body(source);
                default:
                    return new Tag(source);
            }
        }

        internal void AddAttribute(TokenCollection source)
        {
            Attributes.Add(Attribute.Build(source));
            Append(Attributes.Last().ToString());
        }

        protected string LanguageWrapperParse(TokenCollection source)
        {
            if (source.Current.IsSpaceOrNewLine)
            {
                MoveNextAppendingSpaces(source);
            }
            Append(source.Current.Content);

            HtmlParser subParser = new HtmlParser();
            TokenCollection subSource = subParser.Parse(source.Current.SubContent);
            ReadOnlyCollection<Token> attributeSection = subSource.MoveUntil(t => t.Content == ">");
            TokenCollection attributesSource = new TokenCollection(attributeSection);
            MoveNextAppendingSpaces(attributesSource);
            AddAttributes(attributesSource);

            subSource.MoveNext();
            IEnumerable<Token> remainingContent = subSource.ReadToEnd();
            IEnumerable<string> bitsAsString = Append(remainingContent);
            string innerContent = string.Join(string.Empty, bitsAsString);

            if (source.HasNext())
            {
                source.MoveNext();
            }

            return innerContent;
        }
    }
}