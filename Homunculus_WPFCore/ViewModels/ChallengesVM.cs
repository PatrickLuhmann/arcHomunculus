using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Homunculus_WPFCore.ViewModels
{
	public class ChallengesVM
	{
		public ObservableCollection<ChallengeVM> Challenges { get; set; }

		public ChallengesVM()
		{
			Challenges = new ObservableCollection<ChallengeVM>();

			// TODO: Remove this POC code.
			Challenges.Add(new ChallengeVM { Name = "first" });
			Challenges.Add(new ChallengeVM { Name = "second" });
		}
	}
}
