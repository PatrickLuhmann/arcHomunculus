using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Homunculus_Model;
using System.Collections.Generic;
using System.Data;
using System.Collections.ObjectModel;

namespace UnitTest_Homunculus_Model
{
	[TestClass]
	public class UnitTestModel
	{
		ModelXml TestModel;
		static List<string> SplitNamesBefore = new List<string>();
		string DefaultFilename = "homtestdefault.xml";

		[ClassInitialize]
		public static void ClassInit(TestContext tc)
		{
			SplitNamesBefore.Add("one");
			SplitNamesBefore.Add("two");
			SplitNamesBefore.Add("three");
			SplitNamesBefore.Add("four");
		}

		[TestInitialize]
		public void Init()
		{
			// Start with a clean Model and database.
			TestModel = new ModelXml();
			if (System.IO.File.Exists(DefaultFilename))
				System.IO.File.Delete(DefaultFilename);
			TestModel.CreateDatabase(DefaultFilename);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void CreateDatabase_NullFilename()
		{
			TestModel.CreateDatabase(null);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentException))]
		public void CreateDatabase_FileAlreadyExists()
		{
			string filename = "notme.txt";
			if (System.IO.File.Exists(filename))
				System.IO.File.Delete(filename);

			// Create the file.
			using (System.IO.StreamWriter outFile = new System.IO.StreamWriter(filename))
			{
				outFile.WriteLine("This is the first line.");
				outFile.WriteLine("This is the second line.");
				outFile.WriteLine("This is the third line.");
			}

			// Try to create a new database using the known-to-exist filename.
			TestModel.CreateDatabase(filename);
		}

		[TestMethod]
		public void CreateDatabase_SecondOne()
		{
			// We already have the default database loaded; now try
			// to load a different one to replace it. But first,
			// add a challenge to the default so that there is a
			// difference between the two.
			TestModel.CreateChallenge("challenge1", SplitNamesBefore);

			string filename = "homtest.xml";
			if (System.IO.File.Exists(filename))
				System.IO.File.Delete(filename);

			// Main thing is to check for exceptions thrown during creation.
			TestModel.CreateDatabase(filename);

			List<Challenge> challenges = TestModel.GetChallenges();
			Assert.AreEqual(0, challenges.Count);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void LoadDatabase_NullFilename()
		{
			TestModel.LoadDatabase(null);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentException))]
		public void LoadDatabase_FileDoesntExist()
		{
			string filename = "homtest.xml";
			if (System.IO.File.Exists(filename))
				System.IO.File.Delete(filename);

			TestModel.LoadDatabase(filename);
		}

		[TestMethod]
		[ExpectedException(typeof(System.IO.FileFormatException))]
		public void LoadDatabase_BadFile()
		{
			// Create a known-bad file.
			string filename = "notme.txt";
			if (System.IO.File.Exists(filename))
				System.IO.File.Delete(filename);

			// Create the file.
			using (System.IO.StreamWriter outFile = new System.IO.StreamWriter(filename))
			{
				outFile.WriteLine("This is the first line.");
				outFile.WriteLine("This is the second line.");
				outFile.WriteLine("This is the third line.");
			}

			// Try to load it.
			TestModel.LoadDatabase(filename);
		}

		[TestMethod]
		public void LoadDatabase_LoadSameFileTwice()
		{
			// Load the same file twice (should be a no-op)
			TestModel.LoadDatabase(DefaultFilename);
			TestModel.LoadDatabase(DefaultFilename);

			// A new Model object should be able to do the same thing.
			ModelXml NewTestModel = new ModelXml();
			NewTestModel.LoadDatabase(DefaultFilename);
			NewTestModel.LoadDatabase(DefaultFilename);

			// Now load a different file (although the contents are the same).
			string newFilename = "test1.xml";
			if (System.IO.File.Exists(newFilename))
				System.IO.File.Delete(newFilename);
			System.IO.File.Copy(DefaultFilename, newFilename);

			TestModel.LoadDatabase(newFilename);
			TestModel.LoadDatabase(newFilename);
		}

		[TestMethod]
		[ExpectedException(typeof(System.NullReferenceException))]
		public void CreateChallenge_NoDatabaseLoaded()
		{
			// Use a new Model for this test.
			ModelXml newTestModel = new ModelXml();

			newTestModel.CreateChallenge("new challenge", SplitNamesBefore);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void CreateChallenge_NullName()
		{
			// Create a new challenge.
			TestModel.CreateChallenge(null, SplitNamesBefore);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void CreateChallenge_NullSplitList()
		{
			// Create a new challenge.
			TestModel.CreateChallenge("new challenge", null);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentException))]
		public void CreateChallenge_EmptySplitList()
		{
			// Create a new challenge.
			TestModel.CreateChallenge("new challenge", new List<string>());
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentException))]
		public void CreateChallenge_ChallengeAlreadyExists()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// Try to create it again.
			TestModel.CreateChallenge(challengeName, SplitNamesBefore);
		}

		[TestMethod]
		public void CreateChallenge_Basic()
		{
			// ARRANGE
			string name = "new challenge";

			// ACT
			TestModel.CreateChallenge("blah blah blah", SplitNamesBefore);
			Challenge NewChal = TestModel.CreateChallenge(name, SplitNamesBefore);

			// ASSERT
			Assert.IsTrue(NewChal.ChallengeId > 0);
			Assert.AreEqual(name, NewChal.Name);

			// Verify split names.
			ObservableCollection<Split> SplitsAfter = NewChal.Splits;
			Assert.AreEqual(4, SplitsAfter.Count);
			Assert.AreEqual("one", SplitsAfter[0].Name);
			Assert.AreEqual("two", SplitsAfter[1].Name);
			Assert.AreEqual("three", SplitsAfter[2].Name);
			Assert.AreEqual("four", SplitsAfter[3].Name);

			// We don't know the implementation details, except that
			// SplitId must be unique between different Splits.
			Assert.AreNotEqual(SplitsAfter[0].SplitId, SplitsAfter[1].SplitId);
			Assert.AreNotEqual(SplitsAfter[0].SplitId, SplitsAfter[2].SplitId);
			Assert.AreNotEqual(SplitsAfter[0].SplitId, SplitsAfter[3].SplitId);
			Assert.AreNotEqual(SplitsAfter[1].SplitId, SplitsAfter[2].SplitId);
			Assert.AreNotEqual(SplitsAfter[1].SplitId, SplitsAfter[3].SplitId);
			Assert.AreNotEqual(SplitsAfter[2].SplitId, SplitsAfter[3].SplitId);

			Assert.AreEqual(0, NewChal.Runs.Count);
			Assert.AreEqual(-1, NewChal.CurrentSplitIndex);
			Assert.AreEqual(-1, NewChal.PBIndex);
			Assert.IsNull(NewChal.PBRun);
		}

		[TestMethod]
		public void GetChallenges_None()
		{
			List<Challenge> challenges = TestModel.GetChallenges();

			Assert.AreEqual(0, challenges.Count);
		}

		[TestMethod]
		public void GetChallenges_Basic()
		{
			// ARRANGE
			// Create new challenges.
			Challenge c1 = TestModel.CreateChallenge("challenge1", SplitNamesBefore);
			Challenge c2 = TestModel.CreateChallenge("challenge2", SplitNamesBefore);
			Challenge c3 = TestModel.CreateChallenge("challenge3", SplitNamesBefore);
			Challenge c4 = TestModel.CreateChallenge("challenge4", SplitNamesBefore);

			// ACT
			List<Challenge> challenges = TestModel.GetChallenges();

			// ASSERT
			Assert.AreEqual(4, challenges.Count);
			Challenge test;
			test = challenges.Find(c => c.ChallengeId == c1.ChallengeId);
			Assert.AreEqual("challenge1", test.Name);
			test = challenges.Find(c => c.ChallengeId == c2.ChallengeId);
			Assert.AreEqual("challenge2", test.Name);
			test = challenges.Find(c => c.ChallengeId == c3.ChallengeId);
			Assert.AreEqual("challenge3", test.Name);
			test = challenges.Find(c => c.ChallengeId == c4.ChallengeId);
			Assert.AreEqual("challenge4", test.Name);

		}

		[TestMethod]
		public void DatabaseFileOperations()
		{
			// Add some content to the existing default database.
			TestModel.CreateChallenge("challenge-ldb-1", SplitNamesBefore);
			TestModel.CreateChallenge("challenge-ldb-2", SplitNamesBefore);
			TestModel.CreateChallenge("challenge-ldb-3", SplitNamesBefore);

			// Create a new Model instance.
			ModelXml NewTestModel = new ModelXml();

			// Load the existing database file into the new Model.
			NewTestModel.LoadDatabase(DefaultFilename);

			// Verify the contents of the database.
			List<Challenge> challenges = NewTestModel.GetChallenges();
			Assert.AreEqual(3, challenges.Count);
			// TODO: Can we assume the order in the list and/or in the XML file?
			Assert.AreEqual("challenge-ldb-1", challenges[0].Name);
			Assert.AreEqual("challenge-ldb-2", challenges[1].Name);
			Assert.AreEqual("challenge-ldb-3", challenges[2].Name);

			// TODO: Do I have to check everything here, or can I assume other tests
			// will provide coverage?
		}

		[TestMethod]
		public void ChangeChallengeName()
		{
			Assert.Fail();
		}

		[TestMethod]
		public void StartNewRun_Basic()
		{
			// ARRANGE
			string challengeName = "StartNewRun_Basic";
			Challenge NewChallenge = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// ACT
			// TODO: Should this be a method in the service, or in Challenge itself?
			Run NewRun = NewChallenge.StartNewRun2();

			// ASSERT

			// Verify that the Run object returned is correct.
			Assert.AreEqual(NewChallenge, NewRun.Challenge);
			TimeSpan deltaStart = DateTime.Now - NewRun.StartDateTime;
			TimeSpan thresholdTime = new TimeSpan(0, 1, 0);
			Assert.IsTrue(deltaStart < thresholdTime);
			Assert.AreEqual(DateTime.MinValue, NewRun.EndDateTime);
			Assert.AreEqual(TimeSpan.Zero, NewRun.Duration);
			Assert.AreEqual(SplitNamesBefore.Count, NewRun.Counts.Count);
			foreach (var count in NewRun.Counts)
			{
				Assert.AreEqual(0, count.Value);
			}

			// Then, verify that our Challenge object has been updated correctly.
			// Otherwise we would need to re-read everything from the data store,
			// which would not be efficient.
			Assert.AreEqual(1, NewChallenge.Runs.Count);
			Assert.AreEqual(0, NewChallenge.CurrentSplitIndex);
			Assert.AreEqual(-1, NewChallenge.PBIndex); // no change
			Assert.IsNull(NewChallenge.PBRun); // no change
			Assert.AreSame(NewRun, NewChallenge.Runs[0]);

			// Now, verify that the data store has been updated correctly.

			// Create a new Model instance.
			ModelXml AfterTestModel = new ModelXml();

			// Load the existing database file into the new Model.
			AfterTestModel.LoadDatabase(DefaultFilename);

			// Get the challenges.
			List<Challenge> AfterChallenges = AfterTestModel.GetChallenges();

			// Find NewChallenge.
			Challenge AfterNewChallenge = AfterChallenges.Find(c => c.ChallengeId == NewChallenge.ChallengeId);

			Assert.AreEqual(challengeName, AfterNewChallenge.Name);
			Assert.AreEqual(0, AfterNewChallenge.CurrentSplitIndex);
			Assert.AreEqual(-1, AfterNewChallenge.PBIndex); // no change
			Assert.IsNull(AfterNewChallenge.PBRun); // no change

			// Verify the run.
			Assert.AreEqual(1, AfterNewChallenge.Runs.Count);
			ObservableCollection<Run> runList = AfterNewChallenge.Runs;
			Assert.AreEqual(1, runList.Count);
			Assert.AreEqual(SplitNamesBefore.Count, runList[0].Counts.Count);
			foreach (var count in runList[0].Counts)
			{
				Assert.AreEqual(0, count.Value);
			}
		}

#if false
		[TestMethod]
		[ExpectedException(typeof(System.ArgumentException))]
		public void StartNewRun_UnknownChallengeName()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

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
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// Start a new run.
			TestModel.StartNewRun(challengeName);

			// Now try to start another run.
			TestModel.StartNewRun(challengeName);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void EndRun_NullChallengeName()
		{
			// ARRANGE
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// Start a new run and give it some results.
			TestModel.StartNewRun(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Success(challengeName);

			// End the run.
			TestModel.EndRun(null);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentException))]
		public void EndRun_UnknownChallengeName()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// Start a new run and give it some results.
			TestModel.StartNewRun(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Success(challengeName);

			// End the run.
			TestModel.EndRun("unknown challenge name");
		}

		[TestMethod]
		[ExpectedException(typeof(System.InvalidOperationException))]
		public void EndRun_RunNotActive()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// End the run.
			TestModel.EndRun(challengeName);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void DeleteChallenge_NullName()
		{
			TestModel.DeleteChallenge(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void DeleteChallenge_UnknownChallengeName()
		{
			TestModel.DeleteChallenge("this challenge does not exist");
		}

		[TestMethod]
		public void DeleteChallenge_NoRuns()
		{
			TestModel.CreateChallenge("name 1", SplitNamesBefore);

			// This check is redundant when CreateChallenge is validated, but
			// I like to have it anyway so that I can focus on this test
			// and not worry about anything else.
			List<Challenge> challenges = TestModel.GetChallenges();
			Assert.AreEqual(1, challenges.Count);
			Assert.IsTrue(challenges.Contains("name 1"));
			List<Split> splits = TestModel.GetSplits("name 1");
			Assert.AreEqual(4, splits.Count);

			TestModel.DeleteChallenge("name 1");

			challenges = TestModel.GetChallenges();
			Assert.AreEqual(0, challenges.Count);

			// TODO: Is there a better way to verify that the splits have been deleted?
			DataSet verifyDb = new DataSet("verifyDb");
			verifyDb.ReadXml(DefaultFilename);
			Assert.AreEqual(0, verifyDb.Tables["Challenges"].Rows.Count);
			Assert.AreEqual(0, verifyDb.Tables["Splits"].Rows.Count);
			Assert.AreEqual(0, verifyDb.Tables["Runs"].Rows.Count);
			Assert.AreEqual(0, verifyDb.Tables["Counts"].Rows.Count);
		}

		[TestMethod]
		public void DeleteChallenge_RunsAndCounts()
		{
			// Create several challenges, only one of which is to be deleted.
			TestModel.CreateChallenge("name 0", SplitNamesBefore);
			TestModel.CreateChallenge("name 1", SplitNamesBefore);
			TestModel.CreateChallenge("name 2", SplitNamesBefore);
			TestModel.CreateChallenge("name 3", SplitNamesBefore);
			TestModel.CreateChallenge("name 4", SplitNamesBefore);

			// Add three runs to the doomed challenge, along with runs for
			// some of the other challenges.
			TestModel.StartNewRun("name 4");
			TestModel.EndRun("name 4");
			TestModel.StartNewRun("name 4");
			TestModel.EndRun("name 4");
			TestModel.StartNewRun("name 1");
			TestModel.EndRun("name 1");
			TestModel.StartNewRun("name 1");
			TestModel.EndRun("name 1");
			TestModel.StartNewRun("name 1");
			TestModel.EndRun("name 1");
			TestModel.StartNewRun("name 0");
			TestModel.EndRun("name 0");
			TestModel.StartNewRun("name 0");
			TestModel.EndRun("name 0");
			TestModel.StartNewRun("name 2");
			TestModel.EndRun("name 2");

			// This check is redundant when CreateChallenge is validated, but
			// I like to have it anyway so that I can focus on this test
			// and not worry about anything else.
			List<Challenge> challenges = TestModel.GetChallenges();
			Assert.AreEqual(5, challenges.Count);
			Assert.IsTrue(challenges.Contains("name 1"));
			List<Split> splits = TestModel.GetSplits("name 1");
			Assert.AreEqual(4, splits.Count);

			TestModel.DeleteChallenge("name 1");

			challenges = TestModel.GetChallenges();
			Assert.AreEqual(4, challenges.Count);

			// TODO: Is there a better way to verify that the splits, runs, and counts have been deleted?
			DataSet verifyDb = new DataSet("verifyDb");
			verifyDb.ReadXml(DefaultFilename);
			Assert.AreEqual(4, verifyDb.Tables["Challenges"].Rows.Count);
			Assert.AreEqual(4 * 4, verifyDb.Tables["Splits"].Rows.Count);
			Assert.AreEqual(2 + 2 + 1, verifyDb.Tables["Runs"].Rows.Count);
			Assert.AreEqual(4 * (2 + 2 + 1), verifyDb.Tables["Counts"].Rows.Count);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Success_NullChallengeName()
		{
			// ARRANGE
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// ACT
			TestModel.Success(null);
		}

		[TestMethod]
		[ExpectedException(typeof(System.IndexOutOfRangeException))]
		public void Success_UnknownChallengeName()
		{
			// ARRANGE
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// ACT
			TestModel.Success("unknown challenge name");
		}

		[TestMethod]
		[ExpectedException(typeof(System.IndexOutOfRangeException))]
		public void Success_NoRunForThisChallenge()
		{
			// ARRANGE
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// ACT
			TestModel.Success(challengeName);
		}

		[TestMethod]
		[ExpectedException(typeof(System.InvalidOperationException))]
		public void Success_RunNotActive()
		{
			// ARRANGE
			// Create a new challenge.
			string challengeName = "new challenge";
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// Create a new run and then end it.
			TestModel.StartNewRun(challengeName);
			TestModel.EndRun(challengeName);

			// ACT
			TestModel.Success(challengeName);
		}

		[TestMethod]
		public void Success_FirstSplitFirstValue()
		{
			// ARRANGE
			// Create a new challenge.
			string challengeName = "new challenge";
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// Create a new run.
			TestModel.StartNewRun(challengeName);

			// ACT
			TestModel.Success(challengeName);

			// ASSERT
			// Just look at the first split of the first run;
			// GetRuns has its own unit tests.
			List<Run> runs = TestModel.GetRuns(challengeName);
			Assert.AreEqual(0, runs[0].SplitCounts[0]);
			Assert.AreEqual(0, runs[0].SplitCounts[1]);
			Assert.AreEqual(1, runs[0].CurrentSplit);
			Assert.AreEqual(false, runs[0].Closed);
			Assert.AreEqual(false, runs[0].PB);
		}

		[TestMethod]
		public void Success_CompletesTheRun_FirstCompletedRun()
		{
			// ARRANGE
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// Start a new run but end it before it is completed.
			TestModel.StartNewRun(challengeName);
			TestModel.Failure(challengeName);
			TestModel.EndRun(challengeName);

			// ACT

			// Start a new run and give it some values before completing it.
			TestModel.StartNewRun(challengeName);
			// Split 0
			TestModel.Failure(challengeName);
			TestModel.Success(challengeName);
			// Split 1
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Success(challengeName);
			// Split 2
			TestModel.Success(challengeName);
			// Split 3
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Success(challengeName);

			// ASSERT
			List<Run> runs = TestModel.GetRuns(challengeName);
			Assert.AreEqual(2, runs.Count);
			Assert.AreEqual(4, runs[0].SplitCounts.Count);
			Assert.AreEqual(1, runs[0].SplitCounts[0]);
			Assert.AreEqual(0, runs[0].SplitCounts[1]);
			Assert.AreEqual(0, runs[0].SplitCounts[2]);
			Assert.AreEqual(0, runs[0].SplitCounts[3]);
			Assert.AreEqual(0, runs[0].CurrentSplit);
			Assert.AreEqual(true, runs[0].Closed);
			Assert.AreEqual(false, runs[0].PB);
			Assert.AreEqual(4, runs[1].SplitCounts.Count);
			Assert.AreEqual(1, runs[1].SplitCounts[0]);
			Assert.AreEqual(6, runs[1].SplitCounts[1]);
			Assert.AreEqual(0, runs[1].SplitCounts[2]);
			Assert.AreEqual(12, runs[1].SplitCounts[3]);
			Assert.AreEqual(4, runs[1].CurrentSplit);
			Assert.AreEqual(true, runs[1].Closed);
			Assert.AreEqual(true, runs[1].PB);
		}

		[TestMethod]
		public void Success_CompletesTheRun_NotFirstPB()
		{
			// ARRANGE
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// Start a new run and complete it.
			TestModel.StartNewRun(challengeName);
			// Split 0
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Success(challengeName);
			// Split 1
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Success(challengeName);
			// Split 2
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Success(challengeName);
			// Split 3
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Success(challengeName);

			// ACT

			// Start a new run and give it some values before completing it.
			TestModel.StartNewRun(challengeName);
			// Split 0
			TestModel.Failure(challengeName);
			TestModel.Success(challengeName);
			// Split 1
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Success(challengeName);
			// Split 2
			TestModel.Failure(challengeName);
			TestModel.Success(challengeName);
			// Split 3
			TestModel.Success(challengeName);

			// ASSERT
			List<Run> runs = TestModel.GetRuns(challengeName);
			Assert.AreEqual(2, runs.Count);
			Assert.AreEqual(4, runs[0].SplitCounts.Count);
			Assert.AreEqual(5, runs[0].SplitCounts[0]);
			Assert.AreEqual(6, runs[0].SplitCounts[1]);
			Assert.AreEqual(2, runs[0].SplitCounts[2]);
			Assert.AreEqual(6, runs[0].SplitCounts[3]);
			Assert.AreEqual(4, runs[0].CurrentSplit);
			Assert.AreEqual(true, runs[0].Closed);
			Assert.AreEqual(false, runs[0].PB);
			Assert.AreEqual(4, runs[1].SplitCounts.Count);
			Assert.AreEqual(1, runs[1].SplitCounts[0]);
			Assert.AreEqual(2, runs[1].SplitCounts[1]);
			Assert.AreEqual(1, runs[1].SplitCounts[2]);
			Assert.AreEqual(0, runs[1].SplitCounts[3]);
			Assert.AreEqual(4, runs[1].CurrentSplit);
			Assert.AreEqual(true, runs[1].Closed);
			Assert.AreEqual(true, runs[1].PB);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Failure_NullChallengeName()
		{
			// ARRANGE
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// ACT
			TestModel.Failure(null);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentException))]
		public void Failure_UnknownChallengeName()
		{
			// ARRANGE
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// ACT
			TestModel.Failure("unknown challenge name");
		}

		[TestMethod]
		[ExpectedException(typeof(System.IndexOutOfRangeException))]
		public void Failure_NoRunForThisChallenge()
		{
			// ARRANGE
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// ACT
			TestModel.Failure(challengeName);
		}

		[TestMethod]
		[ExpectedException(typeof(System.InvalidOperationException))]
		public void Failure_RunNotActive()
		{
			// ARRANGE
			// Create a new challenge.
			string challengeName = "new challenge";
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// Create a new run and then end it.
			TestModel.StartNewRun(challengeName);
			TestModel.EndRun(challengeName);

			// ACT
			TestModel.Failure(challengeName);
		}

		[TestMethod]
		public void Failure_FirstSplitFirstValue()
		{
			// ARRANGE
			// Create a new challenge.
			string challengeName = "new challenge";
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// Create a new run.
			TestModel.StartNewRun(challengeName);

			// ACT
			TestModel.Failure(challengeName);

			// ASSERT
			// Just look at the first split of the first run;
			// GetRuns has its own unit tests.
			List<Run> runs = TestModel.GetRuns(challengeName);
			Assert.AreEqual(1, runs[0].SplitCounts[0]);
			Assert.AreEqual(0, runs[0].SplitCounts[1]);
			Assert.AreEqual(0, runs[0].CurrentSplit);
			Assert.AreEqual(false, runs[0].Closed);
			Assert.AreEqual(false, runs[0].PB);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void ModifyChallenge_NullChallengeName()
		{
			// ARRANGE
			List<Split> SplitList = new List<Split>();
			SplitList.Add(new Split { Handle = 1, Name = "split 1" });

			// ACT
			TestModel.ModifyChallenge(null, SplitList, "new challenge name");
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void ModifyChallenge_NullChanges()
		{
			// ARRANGE

			// ACT
			TestModel.ModifyChallenge("test challenge", null, null);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentException))]
		public void ModifyChallenge_EmptySplitList()
		{
			// ARRANGE
			List<Split> SplitList = new List<Split>();

			// ACT
			TestModel.ModifyChallenge("test challenge", SplitList, null);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentException))]
		public void ModifyChallenge_UnknownChallengeName()
		{
			// ARRANGE
			List<Split> SplitList = new List<Split>();
			SplitList.Add(new Split { Handle = 1, Name = "split 1" });

			// ACT
			TestModel.ModifyChallenge("this challenge does not exist", SplitList, "new challenge name");
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentException))]
		public void ModifyChallenge_ChallengeAlreadyExists()
		{
			// ARRANGE
			TestModel.CreateChallenge("challenge 1", SplitNamesBefore);
			TestModel.CreateChallenge("challenge 2", SplitNamesBefore);

			// ACT
			TestModel.ModifyChallenge("challenge 1", null, "challenge 2");
		}

		[TestMethod]
		[ExpectedException(typeof(System.InvalidOperationException))]
		public void ModifyChallenge_RunActive()
		{
			// ARRANGE
			string challengeName = "new challenge";
			TestModel.CreateChallenge(challengeName, SplitNamesBefore);
			TestModel.StartNewRun(challengeName);

			// ACT
			TestModel.ModifyChallenge(challengeName, null, "different name");
		}

		[TestMethod]
		public void ModifyChallenge_RenameSplit()
		{
			// ARRANGE

			// Create a new challenge.
			string challengeName = "new challenge";
			TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// Get the splits.
			List<Split> SplitsAfter = TestModel.GetSplits(challengeName);

			// ACT
			string newSplitName = "this is a new name";
			SplitsAfter[0].Name = newSplitName;
			TestModel.ModifyChallenge(challengeName, SplitsAfter, null);

			// ASSERT
			ModelXml VerifyModel = new ModelXml();
			VerifyModel.LoadDatabase(DefaultFilename);

			List<Split> SplitsAfterAfter = VerifyModel.GetSplits(challengeName);

			// Verify split names.
			Assert.AreEqual(SplitsAfter.Count, SplitsAfterAfter.Count);
			Assert.AreEqual(newSplitName, SplitsAfterAfter[0].Name);
			Assert.AreEqual("two", SplitsAfterAfter[1].Name);
			Assert.AreEqual("three", SplitsAfterAfter[2].Name);
			Assert.AreEqual("four", SplitsAfterAfter[3].Name);

			// Verify that Handle didn't change.
			Assert.AreEqual(SplitsAfter[0].Handle, SplitsAfterAfter[0].Handle);
			Assert.AreEqual(SplitsAfter[1].Handle, SplitsAfterAfter[1].Handle);
			Assert.AreEqual(SplitsAfter[2].Handle, SplitsAfterAfter[2].Handle);
			Assert.AreEqual(SplitsAfter[3].Handle, SplitsAfterAfter[3].Handle);
		}

		[TestMethod]
		public void ModifyChallenge_RenameAllSplits()
		{
			// ARRANGE

			// Create a new challenge.
			string challengeName = "new challenge";
			TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// Get the splits.
			List<Split> SplitsAfter = TestModel.GetSplits(challengeName);

			// ACT
			string newSplitName = "this is a new name";
			foreach (var split in SplitsAfter)
				split.Name = newSplitName;
			TestModel.ModifyChallenge(challengeName, SplitsAfter, null);

			// ASSERT
			ModelXml VerifyModel = new ModelXml();
			VerifyModel.LoadDatabase(DefaultFilename);

			List<Split> SplitsAfterAfter = VerifyModel.GetSplits(challengeName);

			// Verify split names.
			Assert.AreEqual(SplitsAfter.Count, SplitsAfterAfter.Count);
			Assert.AreEqual(newSplitName, SplitsAfterAfter[0].Name);
			Assert.AreEqual(newSplitName, SplitsAfterAfter[1].Name);
			Assert.AreEqual(newSplitName, SplitsAfterAfter[2].Name);
			Assert.AreEqual(newSplitName, SplitsAfterAfter[3].Name);

			// Verify that Handle didn't change.
			Assert.AreEqual(SplitsAfter[0].Handle, SplitsAfterAfter[0].Handle);
			Assert.AreEqual(SplitsAfter[1].Handle, SplitsAfterAfter[1].Handle);
			Assert.AreEqual(SplitsAfter[2].Handle, SplitsAfterAfter[2].Handle);
			Assert.AreEqual(SplitsAfter[3].Handle, SplitsAfterAfter[3].Handle);
		}

#if false
		[TestMethod]
		public void ModifyChallenge_AddOneSplitAtEnd()
		{
			// ARRANGE

			// Create a new challenge.
			string challengeName = "new challenge";
			TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// Get the splits.
			List<Split> SplitsAfter = TestModel.GetSplits(challengeName);

			// ACT
			string newSplitName = "this is a new name";
			SplitsAfter.Add(new Split { Name = newSplitName });
			TestModel.ModifyChallenge(challengeName, SplitsAfter);

			// ASSERT
			List<Split> SplitsAfterAfter = TestModel.GetSplits(challengeName);

			// Verify split names.
			Assert.AreEqual(5, SplitsAfterAfter.Count);
			Assert.AreEqual("one", SplitsAfterAfter[0].Name);
			Assert.AreEqual("two", SplitsAfterAfter[1].Name);
			Assert.AreEqual("three", SplitsAfterAfter[2].Name);
			Assert.AreEqual("four", SplitsAfterAfter[3].Name);
			Assert.AreEqual(newSplitName, SplitsAfterAfter[4].Name);

			// Verify that Handle didn't change.
			Assert.AreEqual(SplitsAfter[0].Handle, SplitsAfterAfter[0].Handle);
			Assert.AreEqual(SplitsAfter[1].Handle, SplitsAfterAfter[1].Handle);
			Assert.AreEqual(SplitsAfter[2].Handle, SplitsAfterAfter[2].Handle);
			Assert.AreEqual(SplitsAfter[3].Handle, SplitsAfterAfter[3].Handle);
			Assert.AreNotEqual(0, SplitsAfterAfter[4].Handle);
		}
#endif

		[TestMethod]
		public void ModifyChallenge_SwapTwoSplits()
		{
			// ARRANGE

			// Create a new challenge.
			string challengeName = "new challenge";
			TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// Get the splits.
			List<Split> SplitsAfter = TestModel.GetSplits(challengeName);

			// ACT
			List<Split> newSplitOrder = new List<Split>();
			newSplitOrder.Add(new Split { Handle = SplitsAfter[0].Handle, Name = SplitsAfter[0].Name });
			newSplitOrder.Add(new Split { Handle = SplitsAfter[1].Handle, Name = SplitsAfter[1].Name });
			newSplitOrder.Add(new Split { Handle = SplitsAfter[3].Handle, Name = SplitsAfter[3].Name });
			newSplitOrder.Add(new Split { Handle = SplitsAfter[2].Handle, Name = SplitsAfter[2].Name });

			TestModel.ModifyChallenge(challengeName, newSplitOrder, null);

			// ASSERT
			ModelXml VerifyModel = new ModelXml();
			VerifyModel.LoadDatabase(DefaultFilename);

			List<Split> SplitsAfterAfter = VerifyModel.GetSplits(challengeName);

			// Verify split names.
			Assert.AreEqual(newSplitOrder.Count, SplitsAfterAfter.Count);
			Assert.AreEqual(newSplitOrder[0].Name, SplitsAfterAfter[0].Name);
			Assert.AreEqual(newSplitOrder[1].Name, SplitsAfterAfter[1].Name);
			Assert.AreEqual(newSplitOrder[2].Name, SplitsAfterAfter[2].Name);
			Assert.AreEqual(newSplitOrder[3].Name, SplitsAfterAfter[3].Name);

			// Verify that Handle didn't change.
			Assert.AreEqual(newSplitOrder[0].Handle, SplitsAfterAfter[0].Handle);
			Assert.AreEqual(newSplitOrder[1].Handle, SplitsAfterAfter[1].Handle);
			Assert.AreEqual(newSplitOrder[2].Handle, SplitsAfterAfter[2].Handle);
			Assert.AreEqual(newSplitOrder[3].Handle, SplitsAfterAfter[3].Handle);
		}

		[TestMethod]
		public void ModifyChallenge_RenameChallenge()
		{
			string oldName = "challenge 1";
			string newName = "challenge 2";
			// ARRANGE
			List<Split> SplitsAfter = TestModel.CreateChallenge(oldName, SplitNamesBefore);

			// ACT
			TestModel.ModifyChallenge(oldName, null, newName);

			// ASSERT
			ModelXml VerifyModel = new ModelXml();
			VerifyModel.LoadDatabase(DefaultFilename);

			List<Challenge> challenges = VerifyModel.GetChallenges();
			Assert.AreEqual(1, challenges.Count);
			Assert.AreEqual(newName, challenges[0]);

			List<Split> SplitsAfterAfter = VerifyModel.GetSplits(newName);

			// Verify split names.
			Assert.AreEqual(SplitsAfter.Count, SplitsAfterAfter.Count);
			Assert.AreEqual(SplitsAfter[0].Name, SplitsAfterAfter[0].Name);
			Assert.AreEqual(SplitsAfter[1].Name, SplitsAfterAfter[1].Name);
			Assert.AreEqual(SplitsAfter[2].Name, SplitsAfterAfter[2].Name);
			Assert.AreEqual(SplitsAfter[3].Name, SplitsAfterAfter[3].Name);

			// Verify that Handle didn't change.
			Assert.AreEqual(SplitsAfter[0].Handle, SplitsAfterAfter[0].Handle);
			Assert.AreEqual(SplitsAfter[1].Handle, SplitsAfterAfter[1].Handle);
			Assert.AreEqual(SplitsAfter[2].Handle, SplitsAfterAfter[2].Handle);
			Assert.AreEqual(SplitsAfter[3].Handle, SplitsAfterAfter[3].Handle);
		}
#endif

		// These will never be used again because they are now in the Challenge object.
#if false
		[TestMethod]
		public void GetSplits_Basic()
		{
			// Create a new challenge.
			TestModel.CreateChallenge("new challenge", SplitNamesBefore);

			// Get the splits.
			List<Split> SplitsAfter = TestModel.GetSplits("new challenge");

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
		[ExpectedException(typeof(System.ArgumentException))]
		public void GetRuns_UnknownChallengeName()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// Get the info on the runs for the challenge.
			List<Run> runs = TestModel.GetRuns("unknown challenge name");
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void GetRuns_NullChallengeName()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// Get the info on the runs for the challenge.
			List<Run> runs = TestModel.GetRuns(null);
		}

		[TestMethod]
		public void GetRuns_NoRuns()
		{
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// Do not start any runs.

			// Get the info on the runs for the challenge.
			List<Run> runs = TestModel.GetRuns(challengeName);
			Assert.AreEqual(0, runs.Count);
		}

		[TestMethod]
		public void GetRuns_Basic()
		{
			// ARRANGE
			string challengeName = "new challenge";

			// Create a new challenge.
			List<Split> SplitsAfter = TestModel.CreateChallenge(challengeName, SplitNamesBefore);

			// Start a new run then end it immediately.
			TestModel.StartNewRun(challengeName);
			TestModel.EndRun(challengeName);

			// Start a new run and give it some values before completing it.
			TestModel.StartNewRun(challengeName);
			// Split 0
			TestModel.Success(challengeName);
			// Split 1
			TestModel.Failure(challengeName);
			TestModel.Success(challengeName);
			// Split 2
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Success(challengeName);
			// Split 3
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Success(challengeName);

			// Start a new run, give it some values, but don't end or complete it.
			TestModel.StartNewRun(challengeName);
			// Split 0
			TestModel.Failure(challengeName);
			TestModel.Success(challengeName);
			// Split 1
			TestModel.Success(challengeName);
			// Split 2
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);
			TestModel.Failure(challengeName);

			// ACT
			List<Run> runs = TestModel.GetRuns(challengeName);

			// ASSERT
			Assert.AreEqual(3, runs.Count);
			Assert.AreEqual(4, runs[0].SplitCounts.Count);
			Assert.AreEqual(0, runs[0].SplitCounts[0]);
			Assert.AreEqual(0, runs[0].SplitCounts[1]);
			Assert.AreEqual(0, runs[0].SplitCounts[2]);
			Assert.AreEqual(0, runs[0].SplitCounts[3]);
			Assert.AreEqual(0, runs[0].CurrentSplit);
			Assert.AreEqual(true, runs[0].Closed);
			Assert.AreEqual(false, runs[0].PB);
			Assert.AreEqual(4, runs[1].SplitCounts.Count);
			Assert.AreEqual(0, runs[1].SplitCounts[0]);
			Assert.AreEqual(1, runs[1].SplitCounts[1]);
			Assert.AreEqual(2, runs[1].SplitCounts[2]);
			Assert.AreEqual(3, runs[1].SplitCounts[3]);
			Assert.AreEqual(4, runs[1].CurrentSplit);
			Assert.AreEqual(true, runs[1].Closed);
			Assert.AreEqual(true, runs[1].PB);
			Assert.AreEqual(4, runs[2].SplitCounts.Count);
			Assert.AreEqual(1, runs[2].SplitCounts[0]);
			Assert.AreEqual(0, runs[2].SplitCounts[1]);
			Assert.AreEqual(5, runs[2].SplitCounts[2]);
			Assert.AreEqual(0, runs[2].SplitCounts[3]);
			Assert.AreEqual(2, runs[2].CurrentSplit);
			Assert.AreEqual(false, runs[2].Closed);
			Assert.AreEqual(false, runs[2].PB);
		}

#endif
	}
}
