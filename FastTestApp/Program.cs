using ads_lab_1;
using static ads_lab_1.StringEvaluator;

internal class Program
{
	private static void Main(string[] args)
	{
		var worker = new StringEvaluator();
		//Console.WriteLine(worker.Eval2(@"-19.06887600314356+2"));
		Console.WriteLine(worker.Eval2(@"1+log10(-tan(22*cos(122)+2)+pow(22,-sin(122)+2))/3"));
	}
}