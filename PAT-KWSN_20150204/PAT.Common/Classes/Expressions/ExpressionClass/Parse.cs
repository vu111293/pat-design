namespace Dike.ModelChecking.Expression
{
	
	public class Parse
	{
		public static Expression fromFileName(System.String s, System.String filename)
		{
			
			System.String content = "";
			Expression result;
			
			try
			{
				
				//UPGRADE_TODO: Constructor 'java.io.FileInputStream.FileInputStream' was converted to 'System.IO.FileStream.FileStream' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioFileInputStreamFileInputStream_javalangString'"
				System.IO.FileStream infile = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
				//UPGRADE_TODO: The differences in the expected value  of parameters for constructor 'java.io.BufferedReader.BufferedReader'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
				//UPGRADE_WARNING: At least one expression was used more than once in the target code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1181'"
				System.IO.StreamReader filereader = new System.IO.StreamReader(new System.IO.StreamReader(infile, System.Text.Encoding.Default).BaseStream, new System.IO.StreamReader(infile, System.Text.Encoding.Default).CurrentEncoding);
				
				System.String nextline = "";
				while (nextline != null)
				{
					content = content + "\n" + nextline;
					nextline = filereader.ReadLine();
				}
			}
			catch (System.Exception e)
			{
				System.Console.Out.WriteLine("error while reading input");
				result = new Variable("error");
			}
			
			try
			{
				parser p = new parser(new scanner(new System.IO.StringReader(s + content)));
				result = (Expression) p.parse().value_Renamed;
			}
			catch (System.Exception e)
			{
				System.Console.Out.WriteLine("parse error");
				result = new Variable("error");
			}
			return result;
		}
		
		public static Expression fromString(System.String s)
		{
			Expression result;
			try
			{
				parser p = new parser(new scanner(new System.IO.StringReader(s)));
				result = (Expression) p.parse().value_Renamed;
			}
			catch (System.Exception e)
			{
				System.Console.Out.WriteLine("parse error");
				result = new Variable("error");
			}
			return result;
		}
	}
}