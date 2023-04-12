using System;

namespace Irvin.Parser.Html
{
    public class Body : Tag
    {
        internal Body(TokenCollection source)
        {
            CaptureTagStart(source);

            if (source.Current.Content == ">")
            {
                Append(source.Current.Content);

                bool done = false;

                while (!done)
                {
                    var content = source.MoveUntil(t => t.Content.StartsWith("<"));
                    //now on </body> or <child
                
                    //body content
                    Append(content);
                
                    Tag nextTag = Build(source);
                    if (nextTag != null)  //capture <child
                    {
                        //added <child
                        AddChild(nextTag);
                        MoveNextAppendingSpaces(source);
                    }
                    else
                    {
                        done = true;
                        Append(source.MoveUntil(t => t.Content.StartsWith(">")));  //</body>
                        source.MoveNext();
                    }
                }
            }
            else
            {
                throw new FormatException();
            }
        }
    }
}