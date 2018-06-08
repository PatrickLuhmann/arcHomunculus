using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Homunculus_ViewModel;

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
			// The split list will exist and it will be empty.
			Assert.IsNotNull(vm.SplitList);
			Assert.AreEqual(0, vm.SplitList.Count);

			// The split list, text version will be empty.
			Assert.AreEqual("", vm.SplitTextList);
		}

		[TestMethod]
		public void SetSplits_NoSplits()
		{
			// It is okay to call SetSplits with an empty string. It is up to
			// the "activate run" code to enforce the "must be at least one
			// split" rule.
			string empty = "";
			vm.SetSplits(empty);

#if false
			// I got this code from https://stackoverflow.com/questions/248989/unit-testing-that-an-event-is-raised-in-c-sharp#249042
			System.Collections.Generic.List<string> receivedEvents = new System.Collections.Generic.List<string>();
			vm.PropertyChanged += delegate (object sender, System.ComponentModel.PropertyChangedEventArgs e)
			{
				receivedEvents.Add(e.PropertyName);
			};
			Assert.AreEqual(1, receivedEvents.Count);
			Assert.AreEqual("SplitList", receivedEvents[0]);
#endif

			Assert.AreEqual(0, vm.SplitList.Count);
			Assert.AreEqual("", vm.SplitTextList);
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
		public void FailureProc_FirstSplitOfSeveral()
		{
			// Create several splits.

		}
	}
}
