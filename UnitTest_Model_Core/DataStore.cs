using Homunculus_ModelCore;
using System;
using System.Collections.Generic;
using Xunit;

namespace UnitTest_Model_Core
{
	public class DataStore_UT
	{
		[Fact]
		public void AddCreateAndDeleteChallenges()
		{
			// ASSEMBLE
			DataStoreDynamic ds = new DataStoreDynamic();
			ChallengeCore chall1 = new ChallengeCore { Name = "Chall1" };
			ChallengeCore chall2 = new ChallengeCore { Name = "Chall2" };
			ChallengeCore chall3 = new ChallengeCore { Name = "Chall3" };

			// ACT
			ds.AddChallenge(chall1);
			ds.AddChallenge(chall2);
			ds.AddChallenge(chall3);

			// ASSERT
			List<ChallengeCore> challenges = ds.GetChallenges();
			Assert.Equal(3, challenges.Count);
			Assert.Equal(chall1, challenges[0]);
			Assert.Same(chall1, challenges[0]);
			Assert.Equal(chall2, challenges[1]);
			Assert.Same(chall2, challenges[1]);
			Assert.Equal(chall3, challenges[2]);
			Assert.Same(chall3, challenges[2]);

			// ACT
			ds.DeleteChallenge(chall2);
			ChallengeCore chall4 = ds.CreateChallenge();

			// ASSERT
			challenges = ds.GetChallenges();
			Assert.Equal(3, challenges.Count);
			Assert.Equal(chall1, challenges[0]);
			Assert.Same(chall1, challenges[0]);
			Assert.Equal(chall3, challenges[1]);
			Assert.Same(chall3, challenges[1]);
			Assert.Equal(chall4, challenges[2]);
			Assert.Same(chall4, challenges[2]);

			// ACT
			ds.DeleteChallenge(chall3);
			ds.DeleteChallenge(chall1);
			ds.DeleteChallenge(chall4);

			// ASSERT
			challenges = ds.GetChallenges();
			Assert.Equal(0, challenges.Count);
		}
	}
}
