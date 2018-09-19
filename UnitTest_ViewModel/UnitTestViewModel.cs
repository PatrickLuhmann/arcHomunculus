using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Homunculus_ViewModel;
using System.Collections.Generic;
using Moq;
using Homunculus_Model;
using System.Collections.ObjectModel;

namespace UnitTest_ViewModel
{
	[TestClass]
	public class UnitTestViewModel
	{
		SplitsViewModel TestViewModel;
		Mock<IUserSettings> mockSettings;
		Mock<IHomunculusModel> mockModel;
		List<string> mockModelChallengeList = new List<string>();

		private void CreateBasicChallenge(string ChallengeName)
		{
			List<string> splits = new List<string>
			{
				"split 1",
				"split 2",
				"split 3",
				"split 4",
				"split 5"
			};

			// Setup the model mock to return the appropriate list.
			mockModelChallengeList.Add(ChallengeName);
			mockModel.Setup(m => m.GetChallenges())
				.Returns(mockModelChallengeList);

			// Prepare the list of Split objects that the mock Model
			// will return during CreateChallenge().
			List<Split> modelSplits = new List<Split>();
			foreach (var s in splits)
			{
				modelSplits.Add(new Split { Handle = 0, Name = s });
			}
			mockModel.Setup(m => m.GetSplits(ChallengeName))
				.Returns(modelSplits);

			// Return an empty run list for this challenge.
			mockModel.Setup(m => m.GetRuns(ChallengeName))
				.Returns(new List<Run>());

			// Create the challenge.
			TestViewModel.CreateChallenge(ChallengeName, splits);

		}

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

			// There is no current challenge.
			Assert.AreEqual("", TestViewModel.CurrentChallenge);
		}

		[TestMethod]
		[ExpectedException(typeof(System.InvalidOperationException))]
		public void SuccessProc_RunNotInProgress()
		{
			// ARRANGE
			string challengeName = "new challenge";
			CreateBasicChallenge(challengeName);

			// ACT
			TestViewModel.SuccessProc();

			// ASSERT
		}

		[TestMethod]
		[ExpectedException(typeof(System.InvalidOperationException))]
		public void FailureProc_RunNotInProgress()
		{
			// ARRANGE
			string challengeName = "new challenge";
			CreateBasicChallenge(challengeName);

			// ACT
			TestViewModel.FailureProc();

			// ASSERT
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

			// Return an empty run list for the new challenge.
			mockModel.Setup(m => m.GetRuns(challengeName))
				.Returns(new List<Run>());

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
			Assert.AreEqual(9999, TestViewModel.SplitList[0].DiffValue);
			Assert.AreEqual(9999, TestViewModel.SplitList[0].CurrentPbValue);
			Assert.AreEqual("split 2", TestViewModel.SplitList[1].SplitName);
			Assert.AreEqual(0, TestViewModel.SplitList[1].CurrentValue);
			Assert.AreEqual(9999, TestViewModel.SplitList[1].DiffValue);
			Assert.AreEqual(9999, TestViewModel.SplitList[1].CurrentPbValue);
			Assert.AreEqual("split 3", TestViewModel.SplitList[2].SplitName);
			Assert.AreEqual(0, TestViewModel.SplitList[2].CurrentValue);
			Assert.AreEqual(9999, TestViewModel.SplitList[2].DiffValue);
			Assert.AreEqual(9999, TestViewModel.SplitList[2].CurrentPbValue);
			Assert.AreEqual("split 4", TestViewModel.SplitList[3].SplitName);
			Assert.AreEqual(0, TestViewModel.SplitList[3].CurrentValue);
			Assert.AreEqual(9999, TestViewModel.SplitList[3].DiffValue);
			Assert.AreEqual(9999, TestViewModel.SplitList[3].CurrentPbValue);
			Assert.AreEqual("split 5", TestViewModel.SplitList[4].SplitName);
			Assert.AreEqual(0, TestViewModel.SplitList[4].CurrentValue);
			Assert.AreEqual(9999, TestViewModel.SplitList[4].DiffValue);
			Assert.AreEqual(9999, TestViewModel.SplitList[4].CurrentPbValue);

			// Has CurrentSplit been set correctly?
			Assert.AreEqual(-1, TestViewModel.CurrentSplit);

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

			// Return an empty run list for any challenge.
			mockModel.Setup(m => m.GetRuns(It.IsAny<string>()))
				.Returns(new List<Run>());

			// Specify the last used challenge.
			mockSettings.Setup(us => us.GetUserSetting("LastUsedChallenge"))
				.Returns("challenge 2");

			// For this we need our own object.
			SplitsViewModel mySvm = new SplitsViewModel(mockSettings.Object, mockModel.Object);

			// ACT

			// The first time does real work.
			mySvm.CurrentChallenge = newCurrentName;

			// The second time should be a nop.
			mySvm.CurrentChallenge = newCurrentName;

			// ASSERT
			Assert.AreEqual(newCurrentName, mySvm.CurrentChallenge);
			Assert.AreEqual(-1, mySvm.CurrentSplit);
			Assert.AreEqual(2, mySvm.SplitList.Count);
			SplitVM testSplit = new SplitVM
			{
				SplitName = "split 1",
				CurrentValue = 0,
				CurrentPbValue = 9999,
				DiffValue = 9999
			};
			Assert.AreEqual<SplitVM>(testSplit, mySvm.SplitList[0]);
			testSplit.SplitName = "split 2";
			Assert.AreEqual<SplitVM>(testSplit, mySvm.SplitList[1]);
			mockSettings.Verify(us => us.SetUserSetting("LastUsedChallenge", newCurrentName));

			// Verify efficient operation.
			mockModel.Verify(mm => mm.GetSplits(newCurrentName), Times.AtMostOnce());
			mockModel.Verify(mm => mm.GetRuns(newCurrentName), Times.AtMostOnce());
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

			// Return an empty run list for any challenge.
			mockModel.Setup(m => m.GetRuns(It.IsAny<string>()))
				.Returns(new List<Run>());

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

		[TestMethod]
		public void Aggregate_FailureAndSuccessOnRun()
		{
			// ARRANGE
			string challengeName = "new challenge";
			CreateBasicChallenge(challengeName);

			// ACT

			// Start the run.
			TestViewModel.StartNewRun();

			// Split 1 has no failures.
			TestViewModel.SuccessProc();

			// Split 2 has 2 failures.
			TestViewModel.FailureProc();
			TestViewModel.FailureProc();
			TestViewModel.SuccessProc();

			// Split 3 has 7 failures.
			TestViewModel.FailureProc();
			TestViewModel.FailureProc();
			TestViewModel.FailureProc();
			TestViewModel.FailureProc();
			TestViewModel.FailureProc();
			TestViewModel.FailureProc();
			TestViewModel.FailureProc();
			TestViewModel.SuccessProc();

			// Split 4 has 1 failure.
			TestViewModel.FailureProc();
			TestViewModel.SuccessProc();

			// Split 5 has no failures.
			TestViewModel.SuccessProc();


			// ASSERT
			Assert.AreEqual(false, TestViewModel.RunInProgress);
			Assert.AreEqual(5, TestViewModel.SplitList.Count);
			Assert.AreEqual(0, TestViewModel.SplitList[0].CurrentValue);
			Assert.AreEqual(9999, TestViewModel.SplitList[0].DiffValue);
			Assert.AreEqual(0, TestViewModel.SplitList[0].CurrentPbValue);
			Assert.AreEqual(2, TestViewModel.SplitList[1].CurrentValue);
			Assert.AreEqual(9997, TestViewModel.SplitList[1].DiffValue);
			Assert.AreEqual(2, TestViewModel.SplitList[1].CurrentPbValue);
			Assert.AreEqual(7, TestViewModel.SplitList[2].CurrentValue);
			Assert.AreEqual(9992, TestViewModel.SplitList[2].DiffValue);
			Assert.AreEqual(7, TestViewModel.SplitList[2].CurrentPbValue);
			Assert.AreEqual(1, TestViewModel.SplitList[3].CurrentValue);
			Assert.AreEqual(9998, TestViewModel.SplitList[3].DiffValue);
			Assert.AreEqual(1, TestViewModel.SplitList[3].CurrentPbValue);
			Assert.AreEqual(0, TestViewModel.SplitList[4].CurrentValue);
			Assert.AreEqual(9999, TestViewModel.SplitList[4].DiffValue);
			Assert.AreEqual(0, TestViewModel.SplitList[4].CurrentPbValue);
			mockModel.Verify(mm => mm.Failure(challengeName), Times.Exactly(10));
			mockModel.Verify(mm => mm.Success(challengeName), Times.Exactly(5));

			// ACT II - One More Time, With Feeling!

			// Start the run.
			TestViewModel.StartNewRun();

			// Split 1 has no failures.
			TestViewModel.SuccessProc();

			// Split 2 has no failures.
			TestViewModel.SuccessProc();

			// Split 3 has no failures.
			TestViewModel.SuccessProc();

			// Split 4 has no failures.
			TestViewModel.SuccessProc();

			// Split 5 has 1 failure. Dang!
			TestViewModel.FailureProc();
			TestViewModel.SuccessProc();


			// ASSERT
			Assert.AreEqual(false, TestViewModel.RunInProgress);
			Assert.AreEqual(5, TestViewModel.SplitList.Count);
			Assert.AreEqual(0, TestViewModel.SplitList[0].CurrentValue);
			Assert.AreEqual(0, TestViewModel.SplitList[0].DiffValue);
			Assert.AreEqual(0, TestViewModel.SplitList[0].CurrentPbValue);
			Assert.AreEqual(0, TestViewModel.SplitList[1].CurrentValue);
			Assert.AreEqual(2, TestViewModel.SplitList[1].DiffValue);
			Assert.AreEqual(0, TestViewModel.SplitList[1].CurrentPbValue);
			Assert.AreEqual(0, TestViewModel.SplitList[2].CurrentValue);
			Assert.AreEqual(7, TestViewModel.SplitList[2].DiffValue);
			Assert.AreEqual(0, TestViewModel.SplitList[2].CurrentPbValue);
			Assert.AreEqual(0, TestViewModel.SplitList[3].CurrentValue);
			Assert.AreEqual(1, TestViewModel.SplitList[3].DiffValue);
			Assert.AreEqual(0, TestViewModel.SplitList[3].CurrentPbValue);
			Assert.AreEqual(1, TestViewModel.SplitList[4].CurrentValue);
			Assert.AreEqual(-1, TestViewModel.SplitList[4].DiffValue);
			Assert.AreEqual(1, TestViewModel.SplitList[4].CurrentPbValue);
			mockModel.Verify(mm => mm.Failure(challengeName), Times.Exactly(10 + 1));
			mockModel.Verify(mm => mm.Success(challengeName), Times.Exactly(5 + 5));

		}

		[TestMethod]
		[Ignore]
		public void SwitchToChallengeWithRunInProgress()
		{
			// Create two challenges. Give the second challenge a run in progress.
			// Start with the first challenge, then move to the second challenge.
			// Verify CurrentSplit, among other things.
		}

		[TestMethod]
		public void MoveUpSplitProc_Basic()
		{
			// ARRANGE
			string challengeName = "new challenge";
			CreateBasicChallenge(challengeName);

			// ACT
			TestViewModel.MoveUpSplitProc(1);

			// ASSERT
			Assert.AreEqual(5, TestViewModel.SplitList.Count);
			Assert.AreEqual("split 2", TestViewModel.SplitList[0].SplitName);
			Assert.AreEqual("split 1", TestViewModel.SplitList[1].SplitName);
			Assert.AreEqual("split 3", TestViewModel.SplitList[2].SplitName);
			Assert.AreEqual("split 4", TestViewModel.SplitList[3].SplitName);
			Assert.AreEqual("split 5", TestViewModel.SplitList[4].SplitName);

			// ACT II
			TestViewModel.MoveUpSplitProc(4);
			TestViewModel.MoveUpSplitProc(3);

			// ASSERT
			Assert.AreEqual(5, TestViewModel.SplitList.Count);
			Assert.AreEqual("split 2", TestViewModel.SplitList[0].SplitName);
			Assert.AreEqual("split 1", TestViewModel.SplitList[1].SplitName);
			Assert.AreEqual("split 5", TestViewModel.SplitList[2].SplitName);
			Assert.AreEqual("split 3", TestViewModel.SplitList[3].SplitName);
			Assert.AreEqual("split 4", TestViewModel.SplitList[4].SplitName);

			// ACT III
			// Invalid values are ignored; no exception is thrown.
			TestViewModel.MoveUpSplitProc(-1);
			TestViewModel.MoveUpSplitProc(5);

			// ASSERT
			Assert.AreEqual(5, TestViewModel.SplitList.Count);
			Assert.AreEqual("split 2", TestViewModel.SplitList[0].SplitName);
			Assert.AreEqual("split 1", TestViewModel.SplitList[1].SplitName);
			Assert.AreEqual("split 5", TestViewModel.SplitList[2].SplitName);
			Assert.AreEqual("split 3", TestViewModel.SplitList[3].SplitName);
			Assert.AreEqual("split 4", TestViewModel.SplitList[4].SplitName);
		}

		[TestMethod]
		public void MoveDownSplitProc_Basic()
		{
			// ARRANGE
			string challengeName = "new challenge";
			CreateBasicChallenge(challengeName);

			// ACT
			TestViewModel.MoveDownSplitProc(3);

			// ASSERT
			Assert.AreEqual(5, TestViewModel.SplitList.Count);
			Assert.AreEqual("split 1", TestViewModel.SplitList[0].SplitName);
			Assert.AreEqual("split 2", TestViewModel.SplitList[1].SplitName);
			Assert.AreEqual("split 3", TestViewModel.SplitList[2].SplitName);
			Assert.AreEqual("split 5", TestViewModel.SplitList[3].SplitName);
			Assert.AreEqual("split 4", TestViewModel.SplitList[4].SplitName);

			// ACT II
			TestViewModel.MoveDownSplitProc(0);
			TestViewModel.MoveDownSplitProc(1);

			// ASSERT
			Assert.AreEqual(5, TestViewModel.SplitList.Count);
			Assert.AreEqual("split 2", TestViewModel.SplitList[0].SplitName);
			Assert.AreEqual("split 3", TestViewModel.SplitList[1].SplitName);
			Assert.AreEqual("split 1", TestViewModel.SplitList[2].SplitName);
			Assert.AreEqual("split 5", TestViewModel.SplitList[3].SplitName);
			Assert.AreEqual("split 4", TestViewModel.SplitList[4].SplitName);

			// ACT III
			// Invalid values are ignored; no exception is thrown.
			TestViewModel.MoveDownSplitProc(-1);
			TestViewModel.MoveDownSplitProc(5);

			// ASSERT
			Assert.AreEqual(5, TestViewModel.SplitList.Count);
			Assert.AreEqual("split 2", TestViewModel.SplitList[0].SplitName);
			Assert.AreEqual("split 3", TestViewModel.SplitList[1].SplitName);
			Assert.AreEqual("split 1", TestViewModel.SplitList[2].SplitName);
			Assert.AreEqual("split 5", TestViewModel.SplitList[3].SplitName);
			Assert.AreEqual("split 4", TestViewModel.SplitList[4].SplitName);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void RearrangeChallenge_NullChallengeName()
		{
			// ACT
			TestViewModel.RearrangeChallenge(null, new ObservableCollection<SplitVM>());
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentNullException))]
		public void RearrangeChallenge_NullSplitList()
		{
			// ACT
			TestViewModel.RearrangeChallenge("test challenge", null);
		}

		[TestMethod]
		[ExpectedException(typeof(System.ArgumentException))]
		public void RearrangeChallenge_NewNameNotUnique()
		{
			// ARRANGE
			string targetChal = "target challenge";
			CreateBasicChallenge(targetChal);
			string existChal = "existing challenge";
			CreateBasicChallenge(existChal);
			TestViewModel.CurrentChallenge = targetChal;

			// ACT
			TestViewModel.RearrangeChallenge(existChal, new ObservableCollection<SplitVM>());
		}

		private bool FunkyTest(ObservableCollection<SplitVM> sourceList, List<Split> testList)
		{
			// IF the counts are different then can't possibly be correct.
			if (testList.Count != sourceList.Count)
				return false;

			// Check that the names are correct.
			for (int idx = 0; idx < sourceList.Count; idx++)
			{
				if (testList[idx].Name != sourceList[idx].SplitName)
					return false;
			}

			return true;
		}

		[TestMethod]
		public void RearrangeChallenge_SplitNames()
		{
			// ARRANGE
			string testChal = "test challenge";
			CreateBasicChallenge(testChal);

			ObservableCollection<SplitVM> origSplits = new ObservableCollection<SplitVM>();
			ObservableCollection<SplitVM> testSplits = new ObservableCollection<SplitVM>();
			foreach (var s in TestViewModel.SplitList)
			{
				// Save a copy of the splits as they were. Must be a true new object,
				// otherwise it will see the change to be made and the asserts
				// will not work correctly.
				origSplits.Add(new SplitVM
				{
					Handle = s.Handle,
					SplitName = s.SplitName,
					CurrentValue = s.CurrentValue,
					DiffValue = s.DiffValue,
					CurrentPbValue = s.CurrentPbValue
				});

				// Create the new split list we will be passing to the MUT.
				testSplits.Add(new SplitVM
				{
					Handle = s.Handle,
					// Change the name.
					SplitName = s.SplitName + "this is a test",
					CurrentValue = s.CurrentValue,
					DiffValue = s.DiffValue,
					CurrentPbValue = s.CurrentPbValue
				});
			}

			// Prepare the list of Split objects that the mock Model
			// will return during RearrangeChallenge().
			List<Split> modelSplits = new List<Split>();
			foreach (var s in testSplits)
			{
				modelSplits.Add(new Split { Handle = 0, Name = s.SplitName });
			}
			mockModel.Setup(mm => mm.GetSplits(testChal))
				.Returns(modelSplits);

			// ACT
			TestViewModel.RearrangeChallenge(testChal, testSplits);

			// ASSERT
			mockModel.Verify(mm => mm.ModifyChallenge(testChal,
				It.Is<List<Split>>(l => FunkyTest(testSplits, l)), testChal));

			// Property: ChallengeList - no change
			Assert.AreEqual(1, TestViewModel.ChallengeList.Count);
			Assert.AreEqual(testChal, TestViewModel.ChallengeList[0]);

			// Property: CurrentChallenge - no change
			Assert.AreEqual(testChal, TestViewModel.CurrentChallenge);

			// Property: SplitList - verify new names
			ObservableCollection<SplitVM> actSplits = TestViewModel.SplitList;
			Assert.AreEqual(testSplits.Count, actSplits.Count);
			for (int i = 0; i < actSplits.Count; i++)
			{
				// The name is different.
				Assert.AreEqual(origSplits[i].SplitName + "this is a test", actSplits[i].SplitName);
				// The rest is the same.
				Assert.AreEqual(origSplits[i].Handle, actSplits[i].Handle);
				Assert.AreEqual(origSplits[i].CurrentValue, actSplits[i].CurrentValue);
				Assert.AreEqual(origSplits[i].DiffValue, actSplits[i].DiffValue);
				Assert.AreEqual(origSplits[i].CurrentPbValue, actSplits[i].CurrentPbValue);
			}

			// TODO: Other properties?
		}

		[TestMethod]
		public void RearrangeChallenge_MoveSplits()
		{
			// ARRANGE
			string testChal = "test challenge";
			CreateBasicChallenge(testChal);

			ObservableCollection<SplitVM> origSplits = new ObservableCollection<SplitVM>();
			ObservableCollection<SplitVM> testSplits = new ObservableCollection<SplitVM>();
			foreach (var s in TestViewModel.SplitList)
			{
				// Save a copy of the splits as they were. Must be a true new object,
				// otherwise it will see the change to be made and the asserts
				// will not work correctly.
				origSplits.Add(new SplitVM
				{
					Handle = s.Handle,
					SplitName = s.SplitName,
					CurrentValue = s.CurrentValue,
					DiffValue = s.DiffValue,
					CurrentPbValue = s.CurrentPbValue
				});

				// Create the new split list we will be passing to the MUT.
				testSplits.Add(new SplitVM
				{
					Handle = s.Handle,
					SplitName = s.SplitName,
					CurrentValue = s.CurrentValue,
					DiffValue = s.DiffValue,
					CurrentPbValue = s.CurrentPbValue
				});
			}

			// Move the testSplits around.
			SplitVM item;
			item = testSplits[0];
			testSplits.RemoveAt(0);
			testSplits.Insert(4, item);
			item = testSplits[0];
			testSplits.RemoveAt(0);
			testSplits.Insert(3, item);
			item = testSplits[0];
			testSplits.RemoveAt(0);
			testSplits.Insert(2, item);
			item = testSplits[0];
			testSplits.RemoveAt(0);
			testSplits.Insert(1, item);

			// Prepare the list of Split objects that the mock Model
			// will return during RearrangeChallenge().
			List<Split> modelSplits = new List<Split>();
			foreach (var s in testSplits)
			{
				modelSplits.Add(new Split { Handle = 0, Name = s.SplitName });
			}
			mockModel.Setup(mm => mm.GetSplits(testChal))
				.Returns(modelSplits);

			// ACT
			TestViewModel.RearrangeChallenge(testChal, testSplits);

			// ASSERT
			mockModel.Verify(mm => mm.ModifyChallenge(testChal,
				It.Is<List<Split>>(l => FunkyTest(testSplits, l)), testChal));

			// Property: ChallengeList - no change
			Assert.AreEqual(1, TestViewModel.ChallengeList.Count);
			Assert.AreEqual(testChal, TestViewModel.ChallengeList[0]);

			// Property: CurrentChallenge - no change
			Assert.AreEqual(testChal, TestViewModel.CurrentChallenge);

			// Property: SplitList - verify new names
			ObservableCollection<SplitVM> actSplits = TestViewModel.SplitList;
			Assert.AreEqual(testSplits.Count, actSplits.Count);
			for (int i = 0; i < actSplits.Count; i++)
			{
				// The name is different.
				Assert.AreEqual(origSplits[4-i].SplitName, actSplits[i].SplitName);
				// The rest is the same.
				Assert.AreEqual(origSplits[4-i].Handle, actSplits[i].Handle);
				Assert.AreEqual(origSplits[4-i].CurrentValue, actSplits[i].CurrentValue);
				Assert.AreEqual(origSplits[4-i].DiffValue, actSplits[i].DiffValue);
				Assert.AreEqual(origSplits[4-i].CurrentPbValue, actSplits[i].CurrentPbValue);
			}

			// TODO: Other properties?
		}

		[TestMethod]
		public void RearrangeChallenge_ChangeChallengeName()
		{
			// ARRANGE
			string testChal = "test challenge";
			CreateBasicChallenge(testChal);
			string newChallengeName = "new challenge name";

			ObservableCollection<SplitVM> origSplits = new ObservableCollection<SplitVM>();
			ObservableCollection<SplitVM> testSplits = new ObservableCollection<SplitVM>();
			foreach (var s in TestViewModel.SplitList)
			{
				// Save a copy of the splits as they were. Must be a true new object,
				// otherwise it will see the change to be made and the asserts
				// will not work correctly.
				origSplits.Add(new SplitVM
				{
					Handle = s.Handle,
					SplitName = s.SplitName,
					CurrentValue = s.CurrentValue,
					DiffValue = s.DiffValue,
					CurrentPbValue = s.CurrentPbValue
				});

				// Create the new split list we will be passing to the MUT.
				testSplits.Add(new SplitVM
				{
					Handle = s.Handle,
					SplitName = s.SplitName,
					CurrentValue = s.CurrentValue,
					DiffValue = s.DiffValue,
					CurrentPbValue = s.CurrentPbValue
				});
			}

			// Set new list of challenge names. This needs to be a new object.
			List<string> newNameList = new List<string>();
			newNameList.Add(newChallengeName);
			mockModel.Setup(m => m.GetChallenges())
				.Returns(newNameList);

			// Prepare the list of Split objects that the mock Model
			// will return during RearrangeChallenge().
			List<Split> modelSplits = new List<Split>();
			foreach (var s in testSplits)
			{
				modelSplits.Add(new Split { Handle = s.Handle, Name = s.SplitName });
			}
			mockModel.Setup(mm => mm.GetSplits(newChallengeName))
				.Returns(modelSplits);
			
			// Return an empty run list for this challenge.
			mockModel.Setup(m => m.GetRuns(newChallengeName))
				.Returns(new List<Run>());

			// ACT
			TestViewModel.RearrangeChallenge(newChallengeName, testSplits);

			// ASSERT
			mockModel.Verify(mm => mm.ModifyChallenge(testChal,
				It.Is<List<Split>>(l => FunkyTest(testSplits, l)), newChallengeName));

			// Property: ChallengeList - the name has changed
			Assert.AreEqual(1, TestViewModel.ChallengeList.Count);
			Assert.AreEqual(newChallengeName, TestViewModel.ChallengeList[0]);

			// Property: CurrentChallenge - the name has changed
			Assert.AreEqual(newChallengeName, TestViewModel.CurrentChallenge);

			// Property: SplitList - no change
			ObservableCollection<SplitVM> actSplits = TestViewModel.SplitList;
			Assert.AreEqual(origSplits.Count, actSplits.Count);
			for (int i = 0; i < actSplits.Count; i++)
			{
				// The name is different.
				Assert.AreEqual(origSplits[i].SplitName, actSplits[i].SplitName);
				// The rest is the same.
				Assert.AreEqual(origSplits[i].Handle, actSplits[i].Handle);
				Assert.AreEqual(origSplits[i].CurrentValue, actSplits[i].CurrentValue);
				Assert.AreEqual(origSplits[i].DiffValue, actSplits[i].DiffValue);
				Assert.AreEqual(origSplits[i].CurrentPbValue, actSplits[i].CurrentPbValue);
			}

			// TODO: Other properties?
		}
	}
}
