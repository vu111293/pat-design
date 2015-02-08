using System;
using System.Collections.Generic;
using System.IO;
using Antlr.Runtime;

namespace PAT.Common
{
    /// <summary>
    /// Exception thrown by parsers
    /// </summary>
    public class ParsingException : Exception
    {
        public static SortedDictionary<int, string[]> FileOffset = new SortedDictionary<int, string[]>();
        public int Line;
        public int CharPositionInLine;
        public string Text;

        /// <summary>
        /// Find the containing definition, here still use parsingException type if for the purpose of nested structures.
        /// </summary>
        public ParsingException ContainingDefinition;

        public string DisplayFileName
        {
            get
            {
                if(string.IsNullOrEmpty(FileName))
                {
                    return "";
                }

                if(File.Exists(FileName))
                {
                    return FileName;
                }
                else
                {
                    string[] files = FileName.Split('\\');
                    if(files.Length > 0)
                    {
                        return files[files.Length - 1];
                    }
                    return "";
                }
            }
        }


        public string FileName;
        public string NodeName = "";

        public ParsingException(string message, IToken token, ParsingException definition) : this(message, token)
        {
            ContainingDefinition = definition;
        }

        public ParsingException(string message, IToken token) : base(message)
        {
            Line = token.Line;
            CharPositionInLine = token.CharPositionInLine;
            Text = token.Text;

            int previousLine = 0;
            foreach(KeyValuePair<int, string[]> pair in FileOffset)
            {
                if(Line <= pair.Key)
                {
                    Line = Line - previousLine;
                    FileName = pair.Value[0];
                    if(pair.Value.Length > 1)
                    {
                        NodeName = pair.Value[1];
                    }
                    return;
                }
                else
                {
                    previousLine = pair.Key;
                }
            }
        }

        public ParsingException(string message, int line, int position, string text, ParsingException definition) : this(message,  line, position, text)
        {
            ContainingDefinition = definition;
        }

        public ParsingException(string message, int line, int position, string text) : base(message)
        {
            Line = line;
            CharPositionInLine = position;
            Text = text;

            int previousLine = 0;
            foreach (KeyValuePair<int, string[]> pair in FileOffset)
            {
                if (Line <= pair.Key)
                {
                    Line = Line - previousLine;
                    FileName = pair.Value[0];
                    if (pair.Value.Length > 1)
                    {
                        NodeName = pair.Value[1];
                    }
                    return;
                }
                else
                {
                    previousLine = pair.Key;
                }
            }
        }

        public static string GetFileNameByLineNumber(int line)
        {
            int previousLine = 0;
            foreach (KeyValuePair<int, string[]> pair in FileOffset)
            {
                if (line <= pair.Key)
                {
                    line = line - previousLine;
                    return pair.Value[0];
                }
                else
                {
                    previousLine = pair.Key;
                }
            }

            return "";
        }


        //public ParsingException(string message, int line, int position, string text, string fileName) : this(message, line, position, text)
        //{
        //    FileName = fileName;
        //    Line = Line - FileOffset[fileName];
        //}

        //public ParsingException(string message, IToken token, string fileName) : this(message, token)
        //{
        //    FileName = fileName;
        //    Line = Line - FileOffset[fileName];
        //}

        public static int CountLinesInFile(string s)
        {
            int count = 1;
            int start = 0;
            while ((start = s.IndexOf('\n', start)) != -1)
            {
                count++;
                start++;
            }
            return count;
        }
    }

    public class ParsingExceptionComparer : IComparer<ParsingException>
    {
        public int Compare(ParsingException x, ParsingException y)
        {
            if(x.Line != y.Line)
            {
                return x.Line.CompareTo(y.Line);
            }
            else
            {
                return x.CharPositionInLine.CompareTo(y.CharPositionInLine);
            }
        }
    }


    /// <summary>
    /// Exception thrown by AsyncUtils.AsyncOperation.Start when an
    /// operation is already in progress.
    /// </summary>
    public sealed class AlreadyRunningException : ApplicationException
    {
        public AlreadyRunningException() : base("Operation already running")
        { }
    }


    public sealed class CancelRunningException : ApplicationException
    {
        public CancelRunningException() : base("Operation is cancelled")
        { }
    }
}