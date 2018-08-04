using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Homunculus_ViewModel;
using System.Collections.Generic;
using Moq;

namespace UnitTest_ViewModel
{
	[TestClass]
	public class UnitTestViewModel
	{
		SplitsViewModel TestViewModel;
		Mock<IUserSettings> mockSettings;

		[TestInitialize]
		public void Setup()
		{
			// All tests start without a database, simulating the first
			// time the app is launched.
			if (System.IO.File.Exists("homunculus.xml"))
				System.IO.File.Delete("homunculus.xml");

			// Mock the user settings database because we don't want the
			// unit tests using the real thing, which will screw up what
			// the user might be doing on their own.
			mockSettings = new Mock<IUserSettings>();
			// First launch means there is no value for LastUsedChallenge.
			mockSettings.Setup(us => us.GetUserSetting("LastUsedChallenge"))
				.Returns("");

			TestViewModel = new SplitsViewModel(mockSettings.Object, null);
		}

		[TestMethod]
		public void Constructor()
		{
			// The split list will exist and it will be empty.
			Assert.IsNotNull(TestViewModel.SplitList);
			Assert.AreEqual(0, TestViewModel.SplitList.Count);

			// The split list, text version will be empty.
			Assert.AreEqual("", TestViewModel.SplitTextList);

			// There is no current challenge.
			Assert.AreEqual("", TestViewModel.CurrentChallenge);
		}

		[TestMethod]
		public void SetSplits_Multiple()
		{
			string splits = "one\r\ntwo\r\nthree";
			TestViewModel.SetSplits(splits);
			Assert.AreEqual(3, TestViewModel.SplitList.Count);
			Assert.AreEqual("one", TestViewModel.SplitList[0].SplitName);
			Assert.AreEqual(0, TestViewModel.SplitList[0].CurrentValue);
			Assert.AreEqual(0, TestViewModel.SplitList[0].DiffValue);
			Assert.AreEqual(0, TestViewModel.SplitList[0].CurrentPbValue);
			Assert.AreEqual("two", TestViewModel.SplitList[1].SplitName);
			Assert.AreEqual(0, TestViewModel.SplitList[1].CurrentValue);
			Assert.AreEqual(0, TestViewModel.SplitList[1].DiffValue);
			Assert.AreEqual(0, TestViewModel.SplitList[1].CurrentPbValue);
			Assert.AreEqual("three", TestViewModel.SplitList[2].SplitName);
			Assert.AreEqual(0, TestViewModel.SplitList[2].CurrentValue);
			Assert.AreEqual(0, TestViewModel.SplitList[2].DiffValue);
			Assert.AreEqual(0, TestViewModel.SplitList[2].CurrentPbValue);
			Assert.AreEqual("one\r\ntwo\r\nthree\r\n", TestViewModel.SplitTextList);

			splits = "";
			TestViewModel.SetSplits(splits);
			Assert.AreEqual(0, TestViewModel.SplitList.Count);
			Assert.AreEqual("", TestViewModel.SplitTextList);

			splits = "four\r\nfive\r\nthis is the sixth\r\nand seventh";
			TestViewModel.SetSplits(splits);
			Assert.AreEqual(4, TestViewModel.SplitList.Count);
			Assert.AreEqual("four\r\nfive\r\nthis is the sixth\r\nand seventh\r\n", TestViewModel.SplitTextList);

			splits = "dude\r\nclean\r\n\r\nthese splits\r\n \r\nup\r\n\r\n\r\n";
			TestViewModel.SetSplits(splits);
			Assert.AreEqual(4, TestViewModel.SplitList.Count);
			Assert.AreEqual("dude\r\nclean\r\nthese splits\r\nup\r\n", TestViewModel.SplitTextList);

			splits = "  leading whitespace\r\ntrailing whitespace   \r\n both at once  ";
			TestViewModel.SetSplits(splits);
			Assert.AreEqual(3, TestViewModel.SplitList.Count);
			Assert.AreEqual("leading whitespace\r\ntrailing whitespace\r\nboth at once\r\n", TestViewModel.SplitTextList);

			splits = "\t  what\r\nabout\t\r\n \t tabs? \t \t  ";
			TestViewModel.SetSplits(splits);
			Assert.AreEqual(3, TestViewModel.SplitList.Count);
			Assert.AreEqual("what\r\nabout\r\ntabs?\r\n", TestViewModel.SplitTextList);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentOutOfRangeException))]
		public void SuccessProc_NoSplits()
		{
			// It is not allowed to proc success or failure when there are
			// no splits defined.
			TestViewModel.SuccessProc();
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentOutOfRangeException))]
		public void FailureProc_NoSplits()
		{
			// It is not allowed to proc success or failure when there are
			// no splits defined.
			TestViewModel.FailureProc();
		}

		[TestMethod]
		public void MixedSuccessAndFailureOnSplits()
		{
			// Create several splits.
			string splits = "one\r\ntwo\r\nthree";
			TestViewModel.SetSplits(splits);

			// Hit the first split with a couple of failures.
			TestViewModel.FailureProc();
			TestViewModel.FailureProc();
			Assert.AreEqual(2, TestViewModel.SplitList[0].CurrentValue);
			Assert.AreEqual(0, TestViewModel.SplitList[1].CurrentValue);
			Assert.AreEqual(0, TestViewModel.SplitList[2].CurrentValue);

			// Succeed the second split.
			TestViewModel.SuccessProc();
			TestViewModel.SuccessProc();
			Assert.AreEqual(2, TestViewModel.SplitList[0].CurrentValue);
			Assert.AreEqual(0, TestViewModel.SplitList[1].CurrentValue);
			Assert.AreEqual(0, TestViewModel.SplitList[2].CurrentValue);

			// Fail the third split once.
			TestViewModel.FailureProc();
			Assert.AreEqual(2, TestViewModel.SplitList[0].CurrentValue);
			Assert.AreEqual(0, TestViewModel.SplitList[1].CurrentValue);
			Assert.AreEqual(1, TestViewModel.SplitList[2].CurrentValue);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void CreateChallenge_NullChallengeName()
		{
			List<string> splits = new List<string>();
			splits.Add("split 1");
			TestViewModel.CreateChallenge(null, splits);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void CreateChallenge_NullSplitList()
		{
			TestViewModel.CreateChallenge("new challenge", null);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentException))]
		public void CreateChallenge_EmptySplitList()
		{
			List<string> splits = new List<string>();
			TestViewModel.CreateChallenge("new challenge", splits);
		}

		[TestMethod]
		public void CreateChallenge_Basic()
		{
			List<string> splits = new List<string>();
			splits.Add("split 1");
			splits.Add("split 2");
			splits.Add("split 3");
			splits.Add("split 4");
			splits.Add("split 5");

			TestViewModel.CreateChallenge("new challenge", splits);

			Assert.AreEqual(1, TestViewModel.ChallengeList.Count);
			Assert.AreEqual("new challenge", TestViewModel.ChallengeList[0]);
			Assert.AreEqual("new challenge", TestViewModel.CurrentChallenge);
			mockSettings.Verify(us => us.SetUserSetting("LastUsedChallenge", "new challenge"));
		}

		[TestMethod]
		public void CorruptedUserSettingsFile()
		{
			// Need to use our own test objects for this one.
			Mock<IUserSettings> myMockSettings = new Mock<IUserSettings>();

			// Inject an invalid value for all setting names.
			mockSettings.Setup(us => us.GetUserSetting(It.IsAny<string>()))
				.Returns("always return an invalid value in this test");

			// All tests start without a database, simulating the first
			// time the app is launched.
			if (System.IO.File.Exists("homunculus.xml"))
				System.IO.File.Delete("homunculus.xml");

			SplitsViewModel mySvm = new SplitsViewModel(mockSettings.Object, null);

			// The split list will exist and it will be empty.
			Assert.IsNotNull(TestViewModel.SplitList);
			Assert.AreEqual(0, TestViewModel.SplitList.Count);

			// The split list, text version will be empty.
			Assert.AreEqual("", TestViewModel.SplitTextList);

			// There is no current challenge.
			Assert.AreEqual("", TestViewModel.CurrentChallenge);
		}
	}
}
