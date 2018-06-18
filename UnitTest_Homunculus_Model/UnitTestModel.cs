using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Homunculus_Model;
using System.Collections.Generic;

namespace UnitTest_Homunculus_Model
{
	[TestClass]
	public class UnitTestModel
	{
		Model TestModel;

		[TestInitialize]
		public void Init()
		{
			TestModel = new Model();
		}

		[TestMethod]
		public void CreateChallenge_Basic()
		{
			// Create a new challenge.
			List<Split> SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "one" });
			SplitsBefore.Add(new Split { Name = "two" });
			SplitsBefore.Add(new Split { Name = "three" });
			SplitsBefore.Add(new Split { Name = "four" });
			List<Split> SplitsAfter = TestModel.CreateChallenge("new challenge", SplitsBefore);

			// Verify split names.
			Assert.AreEqual(4, SplitsAfter.Count);
			Assert.AreEqual("one", SplitsAfter[0].Name);
			Assert.AreEqual("two", SplitsAfter[1].Name);
			Assert.AreEqual("three", SplitsAfter[2].Name);
			Assert.AreEqual("four", SplitsAfter[3].Name);

			// We don't know the implementation details, except that
			// Handle must be unique between different Splits.
			Assert.AreNotEqual(SplitsAfter[0].Handle, SplitsAfter[1].Handle);
			Assert.AreNotEqual(SplitsAfter[0].Handle, SplitsAfter[2].Handle);
			Assert.AreNotEqual(SplitsAfter[0].Handle, SplitsAfter[3].Handle);
			Assert.AreNotEqual(SplitsAfter[1].Handle, SplitsAfter[2].Handle);
			Assert.AreNotEqual(SplitsAfter[1].Handle, SplitsAfter[3].Handle);
			Assert.AreNotEqual(SplitsAfter[2].Handle, SplitsAfter[3].Handle);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void CreateChallenge_NullName()
		{
			// Create a new challenge.
			List<Split> SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "one" });
			SplitsBefore.Add(new Split { Name = "two" });
			SplitsBefore.Add(new Split { Name = "three" });
			SplitsBefore.Add(new Split { Name = "four" });
			List<Split> SplitsAfter = TestModel.CreateChallenge(null, SplitsBefore);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void CreateChallenge_NullSplitList()
		{
			// Create a new challenge.
			List<Split> SplitsAfter = TestModel.CreateChallenge("new challenge", null);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentException))]
		public void CreateChallenge_EmptySplitList()
		{
			// Create a new challenge.
			List<Split> SplitsBefore = new List<Split>();
			List<Split> SplitsAfter = TestModel.CreateChallenge("new challenge", SplitsBefore);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentException))]
		public void CreateChallenge_ChallengeAlreadyExists()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsBefore = new List<Split>();
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitsBefore);

			// Try to create it again.
			SplitsAfter = TestModel.CreateChallenge(challengeName, SplitsBefore);
		}

		[TestMethod]
		public void GetChallenges_Basic()
		{
			// Create a new challenge.
			List<Split> SplitsBefore;
			SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "one" });
			SplitsBefore.Add(new Split { Name = "two" });
			SplitsBefore.Add(new Split { Name = "three" });
			SplitsBefore.Add(new Split { Name = "four" });
			TestModel.CreateChallenge("challenge1", SplitsBefore);
			SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "seven" });
			SplitsBefore.Add(new Split { Name = "six" });
			SplitsBefore.Add(new Split { Name = "five" });
			TestModel.CreateChallenge("challenge2", SplitsBefore);
			SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "just this one split" });
			TestModel.CreateChallenge("challenge3", SplitsBefore);
			SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "barrels" });
			SplitsBefore.Add(new Split { Name = "fireballs" });
			SplitsBefore.Add(new Split { Name = "springs" });
			SplitsBefore.Add(new Split { Name = "cement" });
			TestModel.CreateChallenge("challenge4", SplitsBefore);

			List<string> challenges = TestModel.GetChallenges();

			Assert.AreEqual(4, challenges.Count);
			Assert.IsTrue(challenges.Contains("challenge1"));
			Assert.IsTrue(challenges.Contains("challenge2"));
			Assert.IsTrue(challenges.Contains("challenge3"));
			Assert.IsTrue(challenges.Contains("challenge4"));

		}
		[TestMethod]
		public void GetSplits_Basic()
		{
			// Create a new challenge.
			List<Split> SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "one" });
			SplitsBefore.Add(new Split { Name = "two" });
			SplitsBefore.Add(new Split { Name = "three" });
			SplitsBefore.Add(new Split { Name = "four" });
			SplitsBefore.Add(new Split { Name = "five" });
			TestModel.CreateChallenge("new challenge", SplitsBefore);

			// Get the splits.
			List<Split> SplitsAfter = TestModel.GetSplits("new challenge");

			// Verify split names.
			Assert.AreEqual(5, SplitsAfter.Count);
			Assert.AreEqual("one", SplitsAfter[0].Name);
			Assert.AreEqual("two", SplitsAfter[1].Name);
			Assert.AreEqual("three", SplitsAfter[2].Name);
			Assert.AreEqual("four", SplitsAfter[3].Name);
			Assert.AreEqual("five", SplitsAfter[4].Name);

			// We don't know the implementation details, except that
			// Handle must be unique between different Splits.
			Assert.AreNotEqual(SplitsAfter[0].Handle, SplitsAfter[1].Handle);
			Assert.AreNotEqual(SplitsAfter[0].Handle, SplitsAfter[2].Handle);
			Assert.AreNotEqual(SplitsAfter[0].Handle, SplitsAfter[3].Handle);
			Assert.AreNotEqual(SplitsAfter[0].Handle, SplitsAfter[4].Handle);
			Assert.AreNotEqual(SplitsAfter[1].Handle, SplitsAfter[2].Handle);
			Assert.AreNotEqual(SplitsAfter[1].Handle, SplitsAfter[3].Handle);
			Assert.AreNotEqual(SplitsAfter[1].Handle, SplitsAfter[4].Handle);
			Assert.AreNotEqual(SplitsAfter[2].Handle, SplitsAfter[3].Handle);
			Assert.AreNotEqual(SplitsAfter[2].Handle, SplitsAfter[4].Handle);
			Assert.AreNotEqual(SplitsAfter[3].Handle, SplitsAfter[4].Handle);
		}

		[TestMethod]
		public void StartNewRun_Basic()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "one" });
			SplitsBefore.Add(new Split { Name = "two" });
			SplitsBefore.Add(new Split { Name = "three" });
			SplitsBefore.Add(new Split { Name = "four" });
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitsBefore);

			// Start a new run.
			TestModel.StartNewRun(challengeName);

			// NOTE: If done correctly, nothing bad happens.
			// Because this test is so simple, it could be combined with another, such
			// as for GetCurrentRun().
		}

		[TestMethod]
		[ExpectedException(typeof(System.IndexOutOfRangeException))]
		public void StartNewRun_UnknownChallengeName()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "one" });
			SplitsBefore.Add(new Split { Name = "two" });
			SplitsBefore.Add(new Split { Name = "three" });
			SplitsBefore.Add(new Split { Name = "four" });
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitsBefore);

			// Start a new run.
			TestModel.StartNewRun("this challenge does not exist");
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void StartNewRun_NullChallengeName()
		{
			// Start a new run.
			TestModel.StartNewRun(null);
		}

		[TestMethod]
		[ExpectedException(typeof(System.InvalidOperationException))]
		public void StartNewRun_RunAlreadyActive()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "one" });
			SplitsBefore.Add(new Split { Name = "two" });
			SplitsBefore.Add(new Split { Name = "three" });
			SplitsBefore.Add(new Split { Name = "four" });
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitsBefore);

			// Start a new run.
			TestModel.StartNewRun(challengeName);

			// Now try to start another run.
			TestModel.StartNewRun(challengeName);
		}

		[TestMethod]
		public void UpdateRun_Basic()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "one" });
			SplitsBefore.Add(new Split { Name = "two" });
			SplitsBefore.Add(new Split { Name = "three" });
			SplitsBefore.Add(new Split { Name = "four" });
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitsBefore);

			// Start a new run.
			TestModel.StartNewRun(challengeName);

			// Create a list of ints for the split values.
			List<int> splitValues = new List<int>();
			for (int i = 0; i < SplitsAfter.Count; i++)
			{
				splitValues.Add(i);
			}

			// Update the run.
			TestModel.UpdateRun(challengeName, splitValues);

			// Need to get the run in order to verify it.
			List<List<int>> runs = TestModel.GetRuns(challengeName);
			Assert.AreEqual(1, runs.Count);
			Assert.AreEqual(4, runs[0].Count);
			Assert.AreEqual(0, runs[0][0]);
			Assert.AreEqual(1, runs[0][1]);
			Assert.AreEqual(2, runs[0][2]);
			Assert.AreEqual(3, runs[0][3]);
		}

		[TestMethod]
		[ExpectedException(typeof(System.IndexOutOfRangeException))]
		public void UpdateRun_UnknownChallengeName()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "one" });
			SplitsBefore.Add(new Split { Name = "two" });
			SplitsBefore.Add(new Split { Name = "three" });
			SplitsBefore.Add(new Split { Name = "four" });
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitsBefore);

			// Start a new run.
			TestModel.StartNewRun(challengeName);

			// Create a list of ints for the split values.
			List<int> splitValues = new List<int>();
			for (int i = 0; i < SplitsAfter.Count; i++)
			{
				splitValues.Add(i);
			}

			// Update the run.
			TestModel.UpdateRun("unknown challenge name", splitValues);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void UpdateRun_NullChallengeName()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "one" });
			SplitsBefore.Add(new Split { Name = "two" });
			SplitsBefore.Add(new Split { Name = "three" });
			SplitsBefore.Add(new Split { Name = "four" });
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitsBefore);

			// Start a new run.
			TestModel.StartNewRun(challengeName);

			// Create a list of ints for the split values.
			List<int> splitValues = new List<int>();
			for (int i = 0; i < SplitsAfter.Count; i++)
			{
				splitValues.Add(i);
			}

			// Update the run.
			TestModel.UpdateRun(null, splitValues);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void UpdateRun_NullSplitValues()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "one" });
			SplitsBefore.Add(new Split { Name = "two" });
			SplitsBefore.Add(new Split { Name = "three" });
			SplitsBefore.Add(new Split { Name = "four" });
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitsBefore);

			// Start a new run.
			TestModel.StartNewRun(challengeName);

			// Update the run.
			TestModel.UpdateRun(challengeName, null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void UpdateRun_WrongNumberOfSplits()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "one" });
			SplitsBefore.Add(new Split { Name = "two" });
			SplitsBefore.Add(new Split { Name = "three" });
			SplitsBefore.Add(new Split { Name = "four" });
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitsBefore);

			// Start a new run.
			TestModel.StartNewRun(challengeName);

			// Create a list of ints for the split values.
			List<int> splitValues = new List<int>();
			for (int i = 0; i < SplitsAfter.Count + 1; i++)
			{
				splitValues.Add(i);
			}

			// Update the run.
			TestModel.UpdateRun(challengeName, splitValues);
		}

		[TestMethod]
		[ExpectedException(typeof(System.InvalidOperationException))]
		public void UpdateRun_RunNotActive()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "one" });
			SplitsBefore.Add(new Split { Name = "two" });
			SplitsBefore.Add(new Split { Name = "three" });
			SplitsBefore.Add(new Split { Name = "four" });
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitsBefore);

			// Start a new run.
			TestModel.StartNewRun(challengeName);

			// Create a list of ints for the split values.
			List<int> splitValues = new List<int>();
			for (int i = 0; i < SplitsAfter.Count; i++)
			{
				splitValues.Add(i);
			}

			// Update the run.
			TestModel.UpdateRun(challengeName, splitValues);

			// End the run.
			TestModel.EndRun(challengeName);

			// Now try to update the same run again.
			TestModel.UpdateRun(challengeName, splitValues);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void EndRun_NullChallengeName()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "one" });
			SplitsBefore.Add(new Split { Name = "two" });
			SplitsBefore.Add(new Split { Name = "three" });
			SplitsBefore.Add(new Split { Name = "four" });
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitsBefore);

			// Start a new run.
			TestModel.StartNewRun(challengeName);

			// Create a list of ints for the split values.
			List<int> splitValues = new List<int>();
			for (int i = 0; i < SplitsAfter.Count; i++)
			{
				splitValues.Add(i);
			}

			// Update the run.
			TestModel.UpdateRun(null, splitValues);

			// End the run.
			TestModel.EndRun(null);
		}

		[TestMethod]
		[ExpectedException(typeof(System.IndexOutOfRangeException))]
		public void EndRun_UnknownChallengeName()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "one" });
			SplitsBefore.Add(new Split { Name = "two" });
			SplitsBefore.Add(new Split { Name = "three" });
			SplitsBefore.Add(new Split { Name = "four" });
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitsBefore);

			// Start a new run.
			TestModel.StartNewRun(challengeName);

			// Create a list of ints for the split values.
			List<int> splitValues = new List<int>();
			for (int i = 0; i < SplitsAfter.Count; i++)
			{
				splitValues.Add(i);
			}

			// Update the run.
			TestModel.UpdateRun(challengeName, splitValues);

			// End the run.
			TestModel.EndRun("unknown challenge name");
		}

		[TestMethod]
		[ExpectedException(typeof(System.InvalidOperationException))]
		public void EndRun_RunNotActive()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "one" });
			SplitsBefore.Add(new Split { Name = "two" });
			SplitsBefore.Add(new Split { Name = "three" });
			SplitsBefore.Add(new Split { Name = "four" });
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitsBefore);

			// End the run.
			TestModel.EndRun(challengeName);
		}

		[TestMethod]
		public void GetRuns_NoRuns()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "one" });
			SplitsBefore.Add(new Split { Name = "two" });
			SplitsBefore.Add(new Split { Name = "three" });
			SplitsBefore.Add(new Split { Name = "four" });
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitsBefore);

			// Do not start any runs.

			// Get the info on the runs for the challenge.
			List<List<int>> runs = TestModel.GetRuns(challengeName);
			Assert.AreEqual(0, runs.Count);
		}
	}
}
