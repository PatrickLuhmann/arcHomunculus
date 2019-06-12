using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Homunculus_ModelCore;
using Homunculus_WPFCore.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Homunculus_WPFCore.ViewModels
{
	public class MainVM : ViewModelBase
	{
		public static string TheNameOfTheDataStore;
		public static DataStoreDynamic TheDynamicDataStore;

		public string Salutation { get; set; }

		public UserControl ViewForChallenges { get; set; }

		public RelayCommand RelayButtonLoadDataStore { get; private set; }
		private void LoadDataStore()
		{
			TheDynamicDataStore = new DataStoreDynamic();

			// POC - add a random number of challenges to simulate a data store with stuff in it.
			Random rng = new Random();
			int qty = rng.Next(10);
			for (int i = 0; i < qty; i++)
			{
				ChallengeCore challenge = new ChallengeCore();
				challenge.Name = $"Challenge {i}";
				TheDynamicDataStore.AddChallenge(challenge);
			}

			ViewForChallenges = new ChallengesView();
			RaisePropertyChanged("ViewForChallenges");
		}

		public RelayCommand RelayButtonNewDataStore { get; private set; }
		private void NewDataStore()
		{
			TheDynamicDataStore = new DataStoreDynamic();

			ViewForChallenges = new ChallengesView();
			RaisePropertyChanged("ViewForChallenges");
		}

		public MainVM()
		{
			TheNameOfTheDataStore = "Homunculuses R Us";

			// Relay commands
			RelayButtonLoadDataStore = new RelayCommand(LoadDataStore);
			RelayButtonNewDataStore = new RelayCommand(NewDataStore);

			Salutation = "Greetings and felicitations to Homunculus [Core]";
		}
	}
}
