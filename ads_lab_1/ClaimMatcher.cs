using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ads_lab_1
{
	
	internal class ClaimMatcher
	{
		private List<Regex> claimMatcher=new();

		public ClaimMatcher(IEnumerable<Regex>orderedRegexClaims)
		{
			claimMatcher.AddRange(orderedRegexClaims);
		}

		public IEnumerable<string> getMatches(string code)
		{
			var matches = claimMatcher[0].Matches(code).Select(x => x.Value);
			for(int i =1;i<claimMatcher.Count;i++)
			{
				matches = matches.Where(x => claimMatcher[i].IsMatch(x)).ToList(); // strange error without ToList
			}
			return matches;
		}

		//public struct MatchIndex
		//{
		//	public int Start;
		//	public int Count;
		//}
		//public IEnumerable<MatchIndex>getMatchesIndexes(string code)
		//{
		//	var matches = claimMatcher[0].Matches(code).Select(x => (x.Index,x.Value));
		//	for (int i = 1; i < claimMatcher.Count; i++)
		//	{
		//		matches = matches.Where(x => claimMatcher[i].IsMatch(x.Value)).ToList();
		//	}
		//	return matches.Select(x=> new MatchIndex { Start=x.Index,Count=x.Value.Length });
		//}

	}
}
