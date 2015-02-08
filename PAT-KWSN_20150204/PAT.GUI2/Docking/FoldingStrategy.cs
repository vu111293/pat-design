using System.Collections.Generic;
using ICSharpCode.TextEditor.Document;

namespace PAT.GUI.Docking
{
    public class FoldingStrategy : IFoldingStrategy
    {
        public List<FoldMarker> GenerateFoldMarkers(IDocument document, string fileName, object parseInformation)
        {
            List<FoldMarker> list = new List<FoldMarker>();

            //Stack<int> startLinesMoscript = new Stack<int>();
            Stack<int> startLinesBraces = new Stack<int>();
            Stack<int> startColumnBraces = new Stack<int>();

            // Create foldmarkers for the whole document, enumerate through every line.
            for (int i = 0; i < document.TotalNumberOfLines; i++)
            {
                var seg = document.GetLineSegment(i);
                string textline = document.GetText(seg);

                int offs, end = document.TextLength;
                char c;
                for (offs = seg.Offset; offs < end && ((c = document.GetCharAt(offs)) == ' ' || c == '\t'); offs++)
                {
                }
                if (offs == end)
                    break;
                //int spaceCount = offs - seg.Offset;

                //// now offs points to the first non-whitespace char on the line
                //if (document.GetCharAt(offs) == '<')
                //{
                //    string text = document.GetText(offs, seg.Length - spaceCount);
                //    if (text.StartsWith("<moscript>", StringComparison.CurrentCultureIgnoreCase))
                //    {
                //        startLinesMoscript.Push(i);
                //    }
                //    if (text.StartsWith("</moscript>", StringComparison.CurrentCultureIgnoreCase) &&
                //        startLinesMoscript.Count > 0)
                //    {
                //        // Add a new FoldMarker to the list.
                //        if (startLinesMoscript.Count > 0)
                //        {
                //            int start = startLinesMoscript.Pop();
                //            list.Add(new FoldMarker(document, start,
                //                                    document.GetLineSegment(start).Length - "<moscript>".Length, i,
                //                                    spaceCount + "</moscript>".Length, FoldType.Region, "MoScript", true));
                //        }
                //    }
                //}

                int index = textline.IndexOfAny("{}".ToCharArray(), 0);
                while (index != -1)
                {
                    if (textline[index] == '{')
                    {
                        startLinesBraces.Push(i);
                        startColumnBraces.Push(index);
                    }
                    else if (textline[index] == '}')
                    {
                        if (startLinesBraces.Count > 0)
                        {
                            int startL = startLinesBraces.Pop();
                            int startC = startColumnBraces.Pop();
                            if (startL != i)
                            {
                                list.Add(new FoldMarker(document, startL, startC, i, index + 1));
                                    //, FoldType.Unspecified, "...", true    
                            }
                        }
                    }

                    index = textline.IndexOfAny("{}".ToCharArray(), index + 1);
                }

                //if (text.StartsWith("{", StringComparison.CurrentCultureIgnoreCase))
                //{
                //    startLinesBraces.Push(i);
                //}
                //if (text.StartsWith("}", StringComparison.CurrentCultureIgnoreCase) && startLinesBraces.Count > 0)
                //{
                //    // Add a new FoldMarker to the list.
                //    int start = startLinesBraces.Pop();
                //    if(start != i)
                //    {
                //        list.Add(new FoldMarker(document, start, document.GetLineSegment(start).Length - "{".Length, i, spaceCount + "}".Length));    //, FoldType.Unspecified, "...", true
                //    }

                //}

            }

            return list;
        }
    }
}
