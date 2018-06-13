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
		public void CreateChallenge_GetSplits()
		{
			// Create a new challenge.
			List<string> NamesBefore = new List<string>();
			NamesBefore.Add("one");
			NamesBefore.Add("two");
			NamesBefore.Add("three");
			NamesBefore.Add("four");
			TestModel.CreateChallenge("new challenge", NamesBefore);

			// Verify
			Assert.AreEqual(1, TestModel.NumChallenges);
			List<string> NamesAfter = TestModel.GetSplits("new challenge");
			Assert.AreEqual("one", NamesAfter[0]);
			Assert.AreEqual("two", NamesAfter[1]);
			Assert.AreEqual("three", NamesAfter[2]);
			Assert.AreEqual("four", NamesAfter[3]);

			// Create a new run and go through the splits.
			Run CurrRun = TestModel.NewRun("new challenge");
		}
	}
}
