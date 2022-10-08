using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ads_lab_1
{

	internal class ClaimMatcher
	{
		private List<Regex> claimMatcher = new();

		public ClaimMatcher(IEnumerable<Regex> orderedRegexClaims)
		{
			claimMatcher.AddRange(orderedRegexClaims);
		}

		public IEnumerable<string> getMatches(string code, out IEnumerable<int> indexes)
		{
			var matches = claimMatcher[0].Matches(code).Select(x => x.Value);
			for (int i = 1; i < claimMatcher.Count; i++)
			{
				matches = matches.Where(x => claimMatcher[i].IsMatch(x)).ToList(); // strange error without ToList
			}
			indexes = matches.SelectMany(x => new Regex(Regex.Escape(x)).Matches(code).Select(x => x.Index));
			return matches;
		}
	}
}
