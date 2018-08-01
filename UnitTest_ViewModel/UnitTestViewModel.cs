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
		SplitsViewModel vm;
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

			vm = new SplitsViewModel(mockSettings.Object);
		}

		[TestMethod]
		public void Constructor()
		{
			// The split list will exist and it will be empty.
			Assert.IsNotNull(vm.SplitList);
			Assert.AreEqual(0, vm.SplitList.Count);

			// The split list, text version will be empty.
			Assert.AreEqual("", vm.SplitTextList);

			// There is no current challenge.
			Assert.AreEqual("", vm.CurrentChallenge);
		}

		[TestMethod]
		public void SetSplits_Multiple()
		{
			string splits = "one\r\ntwo\r\nthree";
			vm.SetSplits(splits);
			Assert.AreEqual(3, vm.SplitList.Count);
			Assert.AreEqual("one", vm.SplitList[0].SplitName);
			Assert.AreEqual(0, vm.SplitList[0].CurrentValue);
			Assert.AreEqual(0, vm.SplitList[0].DiffValue);
			Assert.AreEqual(0, vm.SplitList[0].CurrentPbValue);
			Assert.AreEqual("two", vm.SplitList[1].SplitName);
			Assert.AreEqual(0, vm.SplitList[1].CurrentValue);
			Assert.AreEqual(0, vm.SplitList[1].DiffValue);
			Assert.AreEqual(0, vm.SplitList[1].CurrentPbValue);
			Assert.AreEqual("three", vm.SplitList[2].SplitName);
			Assert.AreEqual(0, vm.SplitList[2].CurrentValue);
			Assert.AreEqual(0, vm.SplitList[2].DiffValue);
			Assert.AreEqual(0, vm.SplitList[2].CurrentPbValue);
			Assert.AreEqual("one\r\ntwo\r\nthree\r\n", vm.SplitTextList);

			splits = "";
			vm.SetSplits(splits);
			Assert.AreEqual(0, vm.SplitList.Count);
			Assert.AreEqual("", vm.SplitTextList);

			splits = "four\r\nfive\r\nthis is the sixth\r\nand seventh";
			vm.SetSplits(splits);
			Assert.AreEqual(4, vm.SplitList.Count);
			Assert.AreEqual("four\r\nfive\r\nthis is the sixth\r\nand seventh\r\n", vm.SplitTextList);

			splits = "dude\r\nclean\r\n\r\nthese splits\r\n \r\nup\r\n\r\n\r\n";
			vm.SetSplits(splits);
			Assert.AreEqual(4, vm.SplitList.Count);
			Assert.AreEqual("dude\r\nclean\r\nthese splits\r\nup\r\n", vm.SplitTextList);

			splits = "  leading whitespace\r\ntrailing whitespace   \r\n both at once  ";
			vm.SetSplits(splits);
			Assert.AreEqual(3, vm.SplitList.Count);
			Assert.AreEqual("leading whitespace\r\ntrailing whitespace\r\nboth at once\r\n", vm.SplitTextList);

			splits = "\t  what\r\nabout\t\r\n \t tabs? \t \t  ";
			vm.SetSplits(splits);
			Assert.AreEqual(3, vm.SplitList.Count);
			Assert.AreEqual("what\r\nabout\r\ntabs?\r\n", vm.SplitTextList);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentOutOfRangeException))]
		public void SuccessProc_NoSplits()
		{
			// It is not allowed to proc success or failure when there are
			// no splits defined.
			vm.SuccessProc();
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentOutOfRangeException))]
		public void FailureProc_NoSplits()
		{
			// It is not allowed to proc success or failure when there are
			// no splits defined.
			vm.FailureProc();
		}

		[TestMethod]
		public void MixedSuccessAndFailureOnSplits()
		{
			// Create several splits.
			string splits = "one\r\ntwo\r\nthree";
			vm.SetSplits(splits);

			// Hit the first split with a couple of failures.
			vm.FailureProc();
			vm.FailureProc();
			Assert.AreEqual(2, vm.SplitList[0].CurrentValue);
			Assert.AreEqual(0, vm.SplitList[1].CurrentValue);
			Assert.AreEqual(0, vm.SplitList[2].CurrentValue);

			// Succeed the second split.
			vm.SuccessProc();
			vm.SuccessProc();
			Assert.AreEqual(2, vm.SplitList[0].CurrentValue);
			Assert.AreEqual(0, vm.SplitList[1].CurrentValue);
			Assert.AreEqual(0, vm.SplitList[2].CurrentValue);

			// Fail the third split once.
			vm.FailureProc();
			Assert.AreEqual(2, vm.SplitList[0].CurrentValue);
			Assert.AreEqual(0, vm.SplitList[1].CurrentValue);
			Assert.AreEqual(1, vm.SplitList[2].CurrentValue);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void CreateChallenge_NullChallengeName()
		{
			List<string> splits = new List<string>();
			splits.Add("split 1");
			vm.CreateChallenge(null, splits);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void CreateChallenge_NullSplitList()
		{
			vm.CreateChallenge("new challenge", null);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentException))]
		public void CreateChallenge_EmptySplitList()
		{
			List<string> splits = new List<string>();
			vm.CreateChallenge("new challenge", splits);
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

			vm.CreateChallenge("new challenge", splits);

			Assert.AreEqual(1, vm.ChallengeList.Count);
			Assert.AreEqual("new challenge", vm.ChallengeList[0]);
			Assert.AreEqual("new challenge", vm.CurrentChallenge);
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

			SplitsViewModel mySvm = new SplitsViewModel(mockSettings.Object);

			// The split list will exist and it will be empty.
			Assert.IsNotNull(vm.SplitList);
			Assert.AreEqual(0, vm.SplitList.Count);

			// The split list, text version will be empty.
			Assert.AreEqual("", vm.SplitTextList);

			// There is no current challenge.
			Assert.AreEqual("", vm.CurrentChallenge);
		}
	}
}
