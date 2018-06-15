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
		public void CreateNewRun_Basic()
		{
			// Create a new challenge.
			List<Split> SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "one" });
			SplitsBefore.Add(new Split { Name = "two" });
			SplitsBefore.Add(new Split { Name = "three" });
			SplitsBefore.Add(new Split { Name = "four" });
			List<Split> SplitsAfter = TestModel.CreateChallenge("new challenge", SplitsBefore);

			// Create a new run.
			List<int> RunValues = TestModel.CreateNewRun("new challenge");
			Assert.AreEqual(4, RunValues.Count);
			Assert.AreEqual(-1, RunValues[0]);
			Assert.AreEqual(-1, RunValues[1]);
			Assert.AreEqual(-1, RunValues[2]);
			Assert.AreEqual(-1, RunValues[3]);
		}

		[TestMethod]
		public void GetRuns_Basic()
		{
			// Create a new challenge.
			List<Split> SplitsBefore = new List<Split>();
			SplitsBefore.Add(new Split { Name = "one" });
			SplitsBefore.Add(new Split { Name = "two" });
			SplitsBefore.Add(new Split { Name = "three" });
			SplitsBefore.Add(new Split { Name = "four" });
			List<Split> SplitsAfter = TestModel.CreateChallenge("new challenge", SplitsBefore);

			// Create a new run.
			List<int> RunValues = TestModel.CreateNewRun("new challenge");

			// Get the info on the runs for the challenge.
			List<List<int>> runs = TestModel.GetRuns("new challenge");
			Assert.AreEqual(1, runs.Count);
			Assert.AreEqual(4, runs[0].Count);
			Assert.AreEqual(-1, runs[0][0]);
			Assert.AreEqual(-1, runs[0][1]);
			Assert.AreEqual(-1, runs[0][2]);
			Assert.AreEqual(-1, runs[0][3]);
		}
	}
}
