using System.Collections.Generic;

namespace PAT.GUI.Forms.GraphDiff
{

    /// <summary>
    /// Data node class
    /// </summary>
    public class DataNode 
    {
        private string m_Name;
        private string m_Comment;
        private bool m_Expanded;
        private int m_Level;
        internal List<DataNode> m_Children;
        
        public DataNode(string nam, string cmt, int lvl, bool stat)
        {
            m_Name = nam;
            m_Comment = cmt;
            m_Level = lvl;
            m_Expanded = stat;
            m_Children = new List<DataNode>(16);
        }

        /// <summary>
        /// Adds a child node.
        /// </summary>
        /// <param name="child">The child to add.</param>
        public void AddChild(DataNode child)
        {
            m_Children.Add(child);
            
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        /// <summary>
        /// Gets the children count.
        /// </summary>
        /// <value>The children count.</value>
        public int CountChildren
        {
            get { return m_Children.Count; }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public List<DataNode> Children
        {
            get { return m_Children; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DataNode"/> is expanded.
        /// </summary>
        /// <value><c>true</c> if expanded; otherwise, <c>false</c>.</value>
        public bool Expanded
        {
            get { return m_Expanded; }
            set { m_Expanded = value; }
        }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>The level.</value>
        public int Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>The comment.</value>
        public string Comment
        {
            get { return m_Comment; }
            set { m_Comment = value; }
        }
    }
}