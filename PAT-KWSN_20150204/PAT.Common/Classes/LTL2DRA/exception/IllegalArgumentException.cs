namespace PAT.Common.Classes.LTL2DRA.exception
{
    public class IllegalArgumentException : PAT.Common.ParsingException
    {
        public IllegalArgumentException(string msg) : base(msg, 0, 0, "")
        {

        }
    }

        public class IndexOutOfBoundsException : PAT.Common.ParsingException
    {
            public IndexOutOfBoundsException(string msg)
                : base(msg, 0, 0, "")
        {

        }
    }

        public class LimitReachedException : PAT.Common.ParsingException
    {
            public LimitReachedException(string msg)
                : base(msg, 0, 0, "")
        {

        }
    }
    
    
}
