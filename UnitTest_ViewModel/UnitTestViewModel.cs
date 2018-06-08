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
		public void SetSplits_Multiple()
		{
			string splits = "one\r\ntwo\r\nthree";
			vm.SetSplits(splits);
			Assert.AreEqual(3, vm.SplitList.Count);
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
		public void FailureProc_FirstSplitOfSeveral()
		{
			// Create several splits.

		}
	}
}
