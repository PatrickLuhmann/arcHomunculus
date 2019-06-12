using System;
using System.Collections.Generic;
using System.Text;

namespace Homunculus_ModelCore
{
	public static class DataStoreStatic
	{
		public static List<ChallengeCore> GetChallenges()
		{
			List<ChallengeCore> chall = new List<ChallengeCore>();

			// POC - one challenge.
			chall.Add(new ChallengeCore { Name = "Default Challenge" });

			return chall;
		}
	}

	public class DataStoreDynamic
	{
		private List<ChallengeCore> Challenges;
		public List<ChallengeCore> GetChallenges()
		{
			return Challenges;
		}
		public void AddChallenge(ChallengeCore Challenge)
		{
			Challenges.Add(Challenge);
		}
		public ChallengeCore CreateChallenge()
		{
			ChallengeCore chall = new ChallengeCore();
			chall.Name = "Default Challenge Name";
			Challenges.Add(chall);
			return chall;
		}
		public void DeleteChallenge(ChallengeCore Challenge)
		{
			Challenges.Remove(Challenge);
		}
		public DataStoreDynamic()
		{
			Challenges = new List<ChallengeCore>();
		}
	}
}
