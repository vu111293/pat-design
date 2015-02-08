using System;

namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    /// <summary>
    /// Define the rumtime Exception for the internal error happend in the model.
    /// </summary>
    public class RuntimeException :Exception
    {
        public string InnerStackTrace;
        public string Action;
        public RuntimeException(string msg) : base(msg)
        {
            if (msg.Contains("WildConstant"))
            {
                Action = "WildConstant is not supported in the explicity model checking. Please use symbolic model checking using BDD option!";
            }
        }
    }

    /// <summary>
    /// Thrown to indicate that an index of some sort (such as to an array, to a string, or to a vector) is out of range.
    /// </summary>
    public sealed class IndexOutOfBoundsException : RuntimeException
    {
        public IndexOutOfBoundsException(string msg)
            : base("IndexOutOfBoundsException: " + msg)
        {
        }
    }

    /// <summary>
    /// One specific run time exception: Thrown if an application tries to create an array with negative size. 
    /// </summary>
    public sealed class NegativeArraySizeException : RuntimeException
    {
        public NegativeArraySizeException(string msg)
            : base("NegativeArraySizeException: " + msg)
        {
        }
    }

    /// <summary>
    /// Thrown when an exceptional arithmetic condition has occurred. For example, an integer "divide by zero" throws an instance of this class.
    /// </summary>
    public sealed class ArithmeticException : RuntimeException
    {
        public ArithmeticException(string msg) : base("ArithmeticException: " + msg)
        {
        }
    }

    public sealed class EvaluatingException : RuntimeException
    {
        public EvaluatingException(string message) : base("EvaluatingException: " + message)
        {

        }
    }

    public sealed class VariableValueOutOfRangeException : RuntimeException
    {
        public VariableValueOutOfRangeException(string message)
            : base("VariableValueOutOfRangeException: " + message)
        {

        }
    }

    public sealed class OutOfMemoryException : RuntimeException
    {
        public OutOfMemoryException(string message): base("OutOfMemoryException")
        {

        }
    }

    public sealed class TypeConversionException : RuntimeException
    {
        public TypeConversionException(string message)
            : base("TypeConversionException: " + message)
        {

        }
    }
}