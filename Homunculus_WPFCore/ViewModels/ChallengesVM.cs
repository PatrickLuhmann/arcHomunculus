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
//		public ObservableCollection<ChallengeVM> Challenges { get; set; }
		public List<ChallengeVM> Challenges { get; set; }
		private ChallengeVM selectedChallenge;
		public ChallengeVM SelectedChallenge
		{
			get
			{
				return selectedChallenge;
			}
			set
			{
				selectedChallenge = value;
				RaisePropertyChanged("SelectedChallenge");
			}
		}
		public RelayCommand RelayButtonAddChallenge { get; private set; }
		public RelayCommand RelayButtonDeleteChallenge { get; private set; }
		private void AddChallenge()
		{
			var chall = MainVM.TheDynamicDataStore.CreateChallenge();
			Challenges.Add(new ChallengeVM { Challenge = chall, NumRuns = 0 });
			// TODO: Explicit notify?
		}
		private void DeleteChallenge()
		{
			if (selectedChallenge != null)
			{
				// Remove from the data store.
				MainVM.TheDynamicDataStore.DeleteChallenge(selectedChallenge.Challenge);

				// Remove from the list.
				Challenges.Remove(selectedChallenge);

				//RaisePropertyChanged("Challenges");
			}
		}

		public ChallengesVM()
		{
			// Populate relay commands.
			RelayButtonAddChallenge = new RelayCommand(AddChallenge);
			RelayButtonDeleteChallenge = new RelayCommand(DeleteChallenge);

			Challenges = new List<ChallengeVM>();

			// POC?
			var challs = MainVM.TheDynamicDataStore.GetChallenges();
			foreach (var chall in challs)
			{
				Challenges.Add(new ChallengeVM { Challenge = chall, NumRuns = 0 });
			}
			//Challenges.Add(new ChallengeVM { Name = MainVM.TheNameOfTheDataStore, NumRuns = 420 });

		}
	}
}
