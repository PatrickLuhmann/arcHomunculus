﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Homunculus_ViewModel;
using System.Collections.Generic;
using Moq;
using Homunculus_Model;

namespace UnitTest_ViewModel
{
	[TestClass]
	public class UnitTestViewModel
	{
		SplitsViewModel TestViewModel;
		Mock<IUserSettings> mockSettings;
		Mock<IHomunculusModel> mockModel;

		[TestInitialize]
		public void Setup()
		{
			// Mock the user settings database because we don't want the
			// unit tests using the real thing, which will screw up what
			// the user might be doing on their own.
			mockSettings = new Mock<IUserSettings>();
			// First launch means there is no value for LastUsedChallenge.
			mockSettings.Setup(us => us.GetUserSetting("LastUsedChallenge"))
				.Returns("");

			// Mock the model so that we are not dependent on a database.
			mockModel = new Mock<IHomunculusModel>();
			// With no database, the challenge list will be empty.
			mockModel.Setup(m => m.GetChallenges())
				.Returns(new List<string>());

			TestViewModel = new SplitsViewModel(mockSettings.Object, mockModel.Object);
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
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void CreateChallenge_NullChallengeName()
		{
			// ACT
			TestViewModel.CreateChallenge(null, new List<string>());

			// ASSERT - expect exception.
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void CreateChallenge_NullSplitList()
		{
			// ACT
			TestViewModel.CreateChallenge("new challenge", null);

			// ASSERT - expect exception.
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentException))]
		public void CreateChallenge_EmptySplitList()
		{
			// ACT
			TestViewModel.CreateChallenge("new challenge", new List<string>());

			// ASSERT - expect exception.
		}

		[TestMethod]
		public void CreateChallenge_Basic()
		{
			// ARRANGE
			string challengeName = "new challenge";
			List<string> splits = new List<string>
			{
				"split 1",
				"split 2",
				"split 3",
				"split 4",
				"split 5"
			};

			// Setup the model mock to return the appropriate list.
			List<string> mockModelChallengeList = new List<string> { challengeName };
			mockModel.Setup(m => m.GetChallenges())
				.Returns(mockModelChallengeList);

			// Prepare the list of Split objects that the mock Model
			// will return during CreateChallenge().
			List<Split> modelSplits = new List<Split>();
			foreach (var s in splits)
			{
				modelSplits.Add(new Split { Handle = 0, Name = s });
			}
			mockModel.Setup(m => m.GetSplits(challengeName))
				.Returns(modelSplits);

			// ACT
			TestViewModel.CreateChallenge(challengeName, splits);

			// ASSERT
			// Was the Model informed correctly?
			mockModel.Verify(m => m.CreateChallenge(challengeName, splits));

			// Has ChallengeList been updated correctly?
			Assert.AreEqual(1, TestViewModel.ChallengeList.Count);
			Assert.AreEqual(challengeName, TestViewModel.ChallengeList[0]);

			// Has CurrentChallenge been set correctly?
			Assert.AreEqual(challengeName, TestViewModel.CurrentChallenge);

			// Has SplitList been updated correctly?
			Assert.AreEqual(5, TestViewModel.SplitList.Count);
			Assert.AreEqual("split 1", TestViewModel.SplitList[0].SplitName);
			Assert.AreEqual(0, TestViewModel.SplitList[0].CurrentValue);
			Assert.AreEqual(5, TestViewModel.SplitList[0].DiffValue);
			Assert.AreEqual(7, TestViewModel.SplitList[0].CurrentPbValue);
			Assert.AreEqual("split 2", TestViewModel.SplitList[1].SplitName);
			Assert.AreEqual(0, TestViewModel.SplitList[1].CurrentValue);
			Assert.AreEqual(5, TestViewModel.SplitList[1].DiffValue);
			Assert.AreEqual(7, TestViewModel.SplitList[1].CurrentPbValue);
			Assert.AreEqual("split 3", TestViewModel.SplitList[2].SplitName);
			Assert.AreEqual(0, TestViewModel.SplitList[2].CurrentValue);
			Assert.AreEqual(5, TestViewModel.SplitList[2].DiffValue);
			Assert.AreEqual(7, TestViewModel.SplitList[2].CurrentPbValue);
			Assert.AreEqual("split 4", TestViewModel.SplitList[3].SplitName);
			Assert.AreEqual(0, TestViewModel.SplitList[3].CurrentValue);
			Assert.AreEqual(5, TestViewModel.SplitList[3].DiffValue);
			Assert.AreEqual(7, TestViewModel.SplitList[3].CurrentPbValue);
			Assert.AreEqual("split 5", TestViewModel.SplitList[4].SplitName);
			Assert.AreEqual(0, TestViewModel.SplitList[4].CurrentValue);
			Assert.AreEqual(5, TestViewModel.SplitList[4].DiffValue);
			Assert.AreEqual(7, TestViewModel.SplitList[4].CurrentPbValue);

			// Has the user setting LastUsedChallenge been updated correctly?
			mockSettings.Verify(us => us.SetUserSetting("LastUsedChallenge", challengeName));
		}

		[TestMethod]
		public void CurrentChallenge_Set()
		{
			// ARRANGE
			// Mock the existence of a database with several challenges in it.
			string newCurrentName = "new current challenge";
			List<string> challengeNames = new List<string>
			{
				"challenge 1",
				"challenge 2",
				"challenge 3",
				newCurrentName,
				"challenge 4"
			};
			mockModel.Setup(m => m.GetChallenges())
				.Returns(challengeNames);

			// Needs some splits for the mentioned challenges.
			// The inital current challenge is not interesting.
			mockModel.Setup(m => m.GetSplits("challenge 2"))
				.Returns(new List<Split>());
			// The new current challenge needs some splits so that we have
			// something to validate.
			List<Split> newSplits = new List<Split>
			{
				new Split { Handle = 0, Name = "split 1" },
				new Split { Handle = 1, Name = "split 2" }
			};
			mockModel.Setup(m => m.GetSplits(newCurrentName))
				.Returns(newSplits);

			// Specify the last used challenge.
			mockSettings.Setup(us => us.GetUserSetting("LastUsedChallenge"))
				.Returns("challenge 2");

			// For this we need our own object.
			SplitsViewModel mySvm = new SplitsViewModel(mockSettings.Object, mockModel.Object);

			// ACT
			mySvm.CurrentChallenge = newCurrentName;

			// ASSERT
			Assert.AreEqual(newCurrentName, mySvm.CurrentChallenge);
			Assert.AreEqual(2, mySvm.SplitList.Count);
			SplitVM testSplit = new SplitVM
			{
				SplitName = "split 1",
				CurrentValue = 0,
				CurrentPbValue = 7,
				DiffValue = 5
			};
			Assert.AreEqual<SplitVM>(testSplit, mySvm.SplitList[0]);
			testSplit.SplitName = "split 2";
			Assert.AreEqual<SplitVM>(testSplit, mySvm.SplitList[1]);
			mockSettings.Verify(us => us.SetUserSetting("LastUsedChallenge", newCurrentName));
		}

		[TestMethod]
		public void CorruptedUserSettingsFile()
		{
			//ARRANGE
			// Need to use our own test object for this one.
			Mock<IUserSettings> myMockSettings = new Mock<IUserSettings>();
			mockSettings.Setup(us => us.GetUserSetting(It.IsAny<string>()))
				.Returns("always return an invalid value in this test");

			// ACT
			SplitsViewModel mySvm = new SplitsViewModel(mockSettings.Object, mockModel.Object);

			// ASSERT
			// The split list will exist and it will be empty.
			Assert.IsNotNull(TestViewModel.SplitList);
			Assert.AreEqual(0, TestViewModel.SplitList.Count);

			// There is no current challenge.
			Assert.AreEqual("", TestViewModel.CurrentChallenge);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void DeleteChallenge_NullChallengeName()
		{
			// ARRANGE
			// If the ViewModel calls DeleteChallenge directly (as opposed
			// to checking the parameter itself) then an exception will be thrown.
			mockModel.Setup(m => m.DeleteChallenge(null))
				.Throws<ArgumentNullException>();

			// ACT
			TestViewModel.DeleteChallenge(null);

			// ASSERT
			// The exception is the only thing we are interested in.
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentException))]
		public void DeleteChallenge_UnknownChallengeName()
		{
			// ARRANGE
			string challengeName = "this challenge does not exist";

			// If the ViewModel calls DeleteChallenge directly (as opposed
			// to manually searching the list returned by GetChallenges)
			// then an exception will be thrown.
			mockModel.Setup(m => m.DeleteChallenge(challengeName))
				.Throws<ArgumentException>();

			// ACT
			TestViewModel.DeleteChallenge(challengeName);

			// ASSERT
			// The exception is the only thing we are interested in.
		}

		[TestMethod]
		public void DeleteChallenge_Current_NotOnlyOne()
		{
			//ARRANGE
			string nameChallengeToDelete = "challenge to be deleted";
			string nameNewestChallenge = "this is the newest challenge";

			// Provide the challenge list, after the deletion. This assumes
			// that the MUT doesn't call GetChallenges at the beginning to
			// refresh its list or something. This feels like having too
			// much implementation knowledge, but on the other hand we want
			// the MUT to be efficient.
			List<string> afterChallengeList = new List<string>();
			afterChallengeList.Add("old 1");
			afterChallengeList.Add("old 2");
			afterChallengeList.Add("old 3");
			afterChallengeList.Add(nameNewestChallenge);
			mockModel.Setup(m => m.GetChallenges())
				.Returns(afterChallengeList);

			// The splits are irrelevent in this test but an object must
			// be provided because CurrentChallenge looks at it.
			mockModel.Setup(m => m.GetSplits(It.IsAny<string>()))
				.Returns(new List<Split>());

			// ACT
			// Delete the challenge.
			TestViewModel.DeleteChallenge(nameChallengeToDelete);

			// ASSERT
			// The ViewModel must call DeleteChallenge with the proper name.
			mockModel.Verify(m => m.DeleteChallenge(nameChallengeToDelete));

			// Verify it set CurrentChallenge to be the last one in the list.
			Assert.AreEqual(nameNewestChallenge, TestViewModel.CurrentChallenge);
			Assert.AreEqual(afterChallengeList.Count, TestViewModel.ChallengeList.Count);

			// At this point we know that the MUT invoked CurrentChallenge with the
			// correct challenge name. Because we have unit tests specifically for
			// the set accessor for CurrentChallenge, we do not need to do any additional
			// check here (e.g. for the split list).
		}
		[TestMethod]
		public void DeleteChallenge_OnlyOne()
		{
			//ARRANGE
			string nameChallengeToDelete = "challenge to be deleted";

			// Provide the challenge list, after the deletion. This assumes
			// that the MUT doesn't call GetChallenges at the beginning to
			// refresh its list or something. This feels like having too
			// much implementation knowledge, but on the other hand we want
			// the MUT to be efficient.
			List<string> afterChallengeList = new List<string>();
			mockModel.Setup(m => m.GetChallenges())
				.Returns(afterChallengeList);

			// The splits are irrelevent in this test but an object must
			// be provided because CurrentChallenge looks at it.
			mockModel.Setup(m => m.GetSplits(It.IsAny<string>()))
				.Returns(new List<Split>());

			// ACT
			// Delete the challenge.
			TestViewModel.DeleteChallenge(nameChallengeToDelete);

			// ASSERT
			// The ViewModel must call DeleteChallenge with the proper name.
			mockModel.Verify(m => m.DeleteChallenge(nameChallengeToDelete));

			// Verify it set CurrentChallenge to be the empty string.
			Assert.AreEqual("", TestViewModel.CurrentChallenge);
			Assert.AreEqual(afterChallengeList.Count, TestViewModel.ChallengeList.Count);

			// At this point we know that the MUT invoked CurrentChallenge with the
			// correct challenge name. Because we have unit tests specifically for
			// the set accessor for CurrentChallenge, we do not need to do any additional
			// check here (e.g. for the split list).
		}

		[TestMethod]
		[ExpectedException(typeof(System.InvalidOperationException))]
		public void StartNewRun_NoCurrentChallenge()
		{
			// ARRANGE

			// The test starts with no challenges. Thus, don't do anything.

			// ACT
			TestViewModel.StartNewRun();
		}
	}
}
