using static ads_lab_1.StringEvaluator;

internal class Program
{
	private static void Main(string[] args)
	{
		Console.WriteLine(Eval2(@"(pow(22,cos(122)+2))+(pow(22,cos(122)+2))/3"));
	}
}