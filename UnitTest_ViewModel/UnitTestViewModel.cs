using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Homunculus_ViewModel;
using System.Collections.Generic;

namespace UnitTest_ViewModel
{
	[TestClass]
	public class UnitTestViewModel
	{
		SplitsViewModel vm;

		[TestInitialize]
		public void Setup()
		{
			// Everyone will use an instance of the class.
			vm = new SplitsViewModel();
		}

		[TestMethod]
		public void Constructor()
		{
			// The list of challenge names will exist and it will be empty.
			// TODO: This should change when persistent storage is added.
			Assert.IsNotNull(vm.ChallengeList);

			// The name of the current challenge will have a default, meaningless value.
			// TODO: This should change when persistent storage is added.
			Assert.AreEqual("No challenge selected", vm.CurrentChallenge);

			// The list of split objects will exist and it will be empty.
			Assert.IsNotNull(vm.SplitList);
			Assert.AreEqual(0, vm.SplitList.Count);
		}

#if false
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
#endif

		[TestMethod]
		public void AddNewChallenge_Multiple()
		{
			string name = "challenge1";
			List<string> splits = new List<string>();
			splits.Add("one");
			splits.Add("two");
			splits.Add("three");

			int ret = vm.AddNewChallenge(name, splits);

			Assert.AreEqual(0, ret);
			Assert.AreEqual("challenge1", vm.CurrentChallenge);
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
		[Ignore]
		public void MixedSuccessAndFailureOnSplits()
		{
			// Create a new challenge with several splits.
			List<string> splitNames = new List<string>();
			splitNames.Add("one");
			splitNames.Add("two");
			splitNames.Add("three");
			vm.AddNewChallenge("test challenge", splitNames);

			// Start a new run.
			// TODO: This code does not yet exist.

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
	}
}
