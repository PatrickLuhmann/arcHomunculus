using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Homunculus_ModelCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Homunculus_WPFCore.ViewModels
{
	public class ChallengesVM : ViewModelBase
	{
		public ObservableCollection<ChallengeVM> Challenges { get; set; }
		public RelayCommand RelayButtonAddChallenge { get; private set; }
		private void AddChallenge()
		{
			var chall = MainVM.TheDynamicDataStore.CreateChallenge();
			Challenges.Add(new ChallengeVM { Name = chall.Name, NumRuns = 0 });
			// TODO: Explicit notify?
		}

		public ChallengesVM()
		{
			// Populate relay commands.
			RelayButtonAddChallenge = new RelayCommand(AddChallenge);

			Challenges = new ObservableCollection<ChallengeVM>();

			// POC?
			var challs = DataStoreStatic.GetChallenges();
			foreach (var chall in challs)
			{
				Challenges.Add(new ChallengeVM { Name = chall.Name, NumRuns = 0 });
			}
			Challenges.Add(new ChallengeVM { Name = MainVM.TheNameOfTheDataStore, NumRuns = 420 });

		}
	}
}
