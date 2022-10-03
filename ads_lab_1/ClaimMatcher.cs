using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ads_lab_1
{
	// Оператор цикла do while и знаки математических операций / и *.
	internal class ClaimMatcher
	{
		private List<Regex> claimMatcher=new();

		public ClaimMatcher()
		{
			claimMatcher.Add(new Regex(@"(?<=(\s|\A))do[\s]*[{]\s*.+\s*[}][\s]*while[(].+[)]\s*[;]"));
			claimMatcher.Add(new Regex(@"[^/][/][^/]"));
			claimMatcher.Add(new Regex(@"[^/][/][^/]"));
		}

		public IEnumerable<string> matches(string code)
		{
			var matches = claimMatcher[0].Matches(code).Select(x => x.Value);
			for(int i =1;i<claimMatcher.Count-1;i++)
			{
				matches = matches.Where(x => claimMatcher[i].IsMatch(x));
			}
			return matches;
		}
	}
}
