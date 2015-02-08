﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3672 $</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor.Util
{
	/// <summary>
	/// This class implements a keyword map. It implements a digital search trees (tries) to find
	/// a word.
	/// </summary>
	public class LookupTable
	{
		Node root = new Node(null, null,null);
		bool casesensitive;
		int  length;
		
		/// <value>
		/// The number of elements in the table
		/// </value>
		public int Count {
			get {
				return length;
			}
		}

        public List<Node> GetAllWords()
        {
            return root.GetAllWords();
        }
		
		/// <summary>
		/// Get the object, which was inserted under the keyword (line, at offset, with length length),
		/// returns null, if no such keyword was inserted.
		/// </summary>
		public object this[IDocument document, LineSegment line, int offset, int length] {
			get {
				if(length == 0) {
					return null;
				}
				Node next = root;
				
				int wordOffset = line.Offset + offset;
				if (casesensitive) {
					for (int i = 0; i < length; ++i) {
						int index = ((int)document.GetCharAt(wordOffset + i)) % 256;
						next = next[index];
						
						if (next == null) {
							return null;
						}
						
						if (next.color != null && TextUtility.RegionMatches(document, wordOffset, length, next.word)) {
							return next.color;
						}
					}
				} else {
					for (int i = 0; i < length; ++i) {
						int index = ((int)Char.ToUpper(document.GetCharAt(wordOffset + i))) % 256;
						
						next = next[index];
						
						if (next == null) {
							return null;
						}
						
						if (next.color != null && TextUtility.RegionMatches(document, casesensitive, wordOffset, length, next.word)) {
							return next.color;
						}
					}
				}
				return null;
			}
		}
		
		/// <summary>
		/// Inserts an object in the tree, under keyword
		/// </summary>
		public object this[string keyword, string des] {
			set {
				Node node = root;
				Node next = root;
				if (!casesensitive) {
					keyword = keyword.ToUpper();
				}
				++length;
				
				// insert word into the tree
				for (int i = 0; i < keyword.Length; ++i) {
					int index = ((int)keyword[i]) % 256; // index of curchar
					bool d = keyword[i] == '\\';
					
					next = next[index];             // get node to this index
					
					if (next == null) { // no node created -> insert word here
                        node[index] = new Node(value, keyword, des);
						break;
					}
					
					if (next.word != null && next.word.Length != i) { // node there, take node content and insert them again
						string tmpword  = next.word;                  // this word will be inserted 1 level deeper (better, don't need too much
						object tmpcolor = next.color;                 // string comparisons for finding.)
						next.color = next.word = null;
                        this[tmpword, next.description] = tmpcolor;
					}
					
					if (i == keyword.Length - 1) { // end of keyword reached, insert node there, if a node was here it was
						next.word = keyword;       // reinserted, if it has the same length (keyword EQUALS this word) it will be overwritten
						next.color = value;
						break;
					}
					
					node = next;
				}
			}
		}
		
		/// <summary>
		/// Creates a new instance of <see cref="LookupTable"/>
		/// </summary>
		public LookupTable(bool casesensitive)
		{
			this.casesensitive = casesensitive;
		}
		
		public class Node
		{
			public Node(object color, string word, string des)
			{
				this.word  = word;
				this.color = color;
			    this.description = des;
			}
            public string description;
			public string word;
			public object color;
			
			// Lazily initialize children array. Saves 200 KB of memory for the C# highlighting
			// because we don't have to store the array for leaf nodes.
			public Node this[int index] {
				get { 
					if (children != null)
						return children[index];
					else
						return null;
				}
				set {
					if (children == null)
						children = new Node[256];
					children[index] = value;
				}
			}
			
			private Node[] children;

            public List<Node> GetAllWords()
            {
                List<Node> result = new List<Node>();
                if(word != null && word.ToString().Length > 0 && Char.IsLetter(word.ToString()[0]) )
                {
                    result.Add(this);
                }
                
                if(children != null)
                {
                    foreach (LookupTable.Node list in children)
                    {
                        if (list != null)
                        {
                            result.AddRange(list.GetAllWords());
                        }
                    }
                }
                return result;
            }

            public override bool Equals(object obj)
            {
                return this.word == (obj as Node).word;
            }
		}
	}
}
