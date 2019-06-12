using Homunculus_ModelCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Homunculus_WPFCore.ViewModels
{
	public class ChallengeVM
	{
		//public string Name { get; set; }
		public int NumRuns { get; set; }
		public ChallengeCore Challenge { get; set; }

		public ChallengeVM()
		{
			//Name = "Default name";
			NumRuns = 0;
		}
	}
}
