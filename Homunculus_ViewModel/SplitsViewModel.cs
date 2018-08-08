using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Homunculus_Model;

namespace Homunculus_ViewModel
{
	public class SplitsViewModel : INotifyPropertyChanged
	{
		#region View properties
		private List<string> challengeList;
		public List<string> ChallengeList {  get { return challengeList; } }

		private string currentChallenge;
		public string CurrentChallenge {
			get { return currentChallenge; }
			set
			{
				// Grab the name of the challenge.
				if (value == null)
					currentChallenge = "";
				else
				{
					// Make sure that the challenge is in the database.
					if (challengeList.Contains(value))
//					if (challengeList.Exists(s => s.Equals(value)))
						currentChallenge = value;
					else
						currentChallenge = "";

				}

				if (currentChallenge == "")
				{
					// Clear the list of splits.
					// This also covers the case where the app is launched with no
					// challenges defined; we don't want splitList to be null.
					splitList = new ObservableCollection<SplitVM>();
				}
				else
				{
					// Get the split info for this challenge.
					List<Split> mSplits = Challenges.GetSplits(currentChallenge);

					// TODO: Get the PB run for this challenge.

					// Create the list of splits for the View.
					// Assume there isn't an active run for this split,
					// so set CurrentValue to 0.
					// CurrentPbValue comes from the runs.
					splitList = new ObservableCollection<SplitVM>();
					foreach (var split in mSplits)
					{
						splitList.Add(new SplitVM
						{
							SplitName = split.Name,
							CurrentValue = 0,
							DiffValue = 5,
							CurrentPbValue = 7
						});
					}
				}

				// Remember this choice.
				UserSettings.SetUserSetting("LastUsedChallenge", currentChallenge);
				UserSettings.Save();

				// We changed some of our public properties.
				NotifyPropertyChanged("CurrentChallenge");
				NotifyPropertyChanged("SplitList");
			}
		}

		private ObservableCollection<SplitVM> splitList;
		public ObservableCollection<SplitVM> SplitList { get { return splitList; } }

		// TODO: Need to do something better than this.
		private string splitTextList;
		public string SplitTextList
		{
			get
			{
				splitTextList = "";
				foreach (var split in splitList)
				{
					string text = split.SplitName;
					splitTextList += text + Environment.NewLine;
				}
				return splitTextList;
			}
		}

		// TODO: Should these really be properties? What if the View want to use
		// different words, or even a different language?
		public string SuccessButtonText { get; set; }
		public string FailureButtonText { get { return "Failure"; } }
		public string CreateChallengeButtonText { get { return "Create Challenge"; } }
#endregion

#region View methods
		/// <summary>
		/// Indicate success for the current split.
		/// 
		/// There must be at least one split defined before this method can be called. If
		/// this is not the case, ArgumentOutOfRangeException will be thrown.
		/// </summary>
		public void SuccessProc()
		{
			// If the user has not defined a split yet, throw an exception. This
			// should never happen; it is the responsibility of other code to
			// enforce this rule.  The exception is to help debugging.
			if (splitList.Count == 0)
				throw new System.ArgumentOutOfRangeException();

			// Go to the next split.
			CurrentSplit++;

			// If we are past the end, then we have won the game.
			if (CurrentSplit == SplitList.Count)
			{
				// TODO: Handle "game won" condition here.
				CurrentSplit--;
			}
		}

		/// <summary>
		/// Indicate failure for the current split.
		/// 
		/// There must be at least one split defined before this method can be called. If
		/// this is not the case, ArgumentOutOfRangeException will be thrown.
		/// </summary>
		public void FailureProc()
		{
			// Increment the current value of the current split.
			splitList[CurrentSplit].CurrentValue++;

			// Recalculate the difference.
			splitList[CurrentSplit].DiffValue =
				splitList[CurrentSplit].CurrentPbValue - splitList[CurrentSplit].CurrentValue;
		}

		public void CreateChallenge(string Name, List<string> Splits)
		{
			if (Name == null || Splits == null)
				throw new ArgumentNullException();
			if (Splits.Count == 0)
				throw new ArgumentException();

			Challenges.CreateChallenge(Name, Splits);
			challengeList = Challenges.GetChallenges();
			CurrentChallenge = Name;

			// We changed some of our public properties.
			NotifyPropertyChanged("ChallengeList");
			NotifyPropertyChanged("CurrentChallenge");
			NotifyPropertyChanged("SplitList");
		}

		/// <summary>
		/// Delete the given challenge from the database.
		/// </summary>
		/// <param name="Name">The name of the challenge.</param>
		public void DeleteChallenge(string Name)
		{
			if (Name == null)
				throw new ArgumentNullException();

			// Remove it from the database.
			Challenges.DeleteChallenge(Name);

			// Update the challenge list.
			challengeList = Challenges.GetChallenges();

			// Set a new current challenge.
			if (challengeList.Count == 0)
				CurrentChallenge = "";
			else
			{
				// TODO: Can we assume that the last item in the list is the newest?
				CurrentChallenge = challengeList.Last();
			}

			// We changed some of our public properties.
			NotifyPropertyChanged("ChallengeList");
			// TODO: These are redundant to what was done in CurrentChallenge.set
			// so they can probably be deleted.
			NotifyPropertyChanged("CurrentChallenge");
			NotifyPropertyChanged("SplitList");
		}

		/// <summary>
		/// NOTE: This might be deprecated, or at least delayed.
		/// </summary>
		/// <param name="selectedSplit"></param>
		public void AddSplitProc(int selectedSplit)
		{
			// TODO: Add the new item at an arbitrary point.
			splitList.Add(new SplitVM { SplitName = "New Split" });
			// TODO: Do I need to do something here to get the list to update in the UI?
		}
		
		public void OnClosing(object s, CancelEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("Enter ViewModel::OnClosing");

			// TODO: What do we need to do when the app is closing?
		}

		public SplitsViewModel(IUserSettings Settings, IHomunculusModel Model)
		{
			// If the caller is not supplying a user settings
			// interface object, just make our own.
			if (Settings == null)
			{
				UserSettings = new StandardUserSettings();
			}
			else
				UserSettings = Settings;

			// If the caller is not supplying a model object,
			// just make our own.
			if (Model == null)
			{
				Challenges = new ModelXml();
				if (System.IO.File.Exists("homunculus.xml"))
				{
					Challenges.LoadDatabase("homunculus.xml");
				}
				else
				{
					Challenges.CreateDatabase("homunculus.xml");
				}
			}
			else
			{
				Challenges = Model;
			}

			// Get the challenges and set one to current, if available.
			challengeList = Challenges.GetChallenges();
			CurrentChallenge = (string)UserSettings.GetUserSetting("LastUsedChallenge");
			
			SuccessButtonText = "Success";
		}

		public SplitsViewModel() : this(null, null)
		{
		}
#endregion

#region Private data members
		// Break the tight coupling between the app and the data stores.
		// This allows for unit testing via a mock.
		private IUserSettings UserSettings;
		private IHomunculusModel Challenges;

		private int CurrentSplit = 0;
		private bool RunInProgress = false;
#endregion

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}

	public class SplitVM : INotifyPropertyChanged, IEquatable<SplitVM>
	{
		public string SplitName { get; set; }

		private int currentValue;
		public int CurrentValue
		{
			get
			{
				return currentValue;
			}
			set
			{
				currentValue = value;
				NotifyPropertyChanged();
			}
		}

		private int diffValue;
		public int DiffValue
		{
			get { return diffValue; }
			set
			{
				diffValue = value;
				NotifyPropertyChanged();
			}
		}

		private int currentPbValue;
		public int CurrentPbValue
		{
			get
			{
				return currentPbValue;
			}
			set
			{
				currentPbValue = value;
				NotifyPropertyChanged();
			}
		}

		public bool Equals(SplitVM other)
		{
			if (other == null)
				return false;

			return SplitName == other.SplitName &&
				currentValue == other.currentValue &&
				diffValue == other.diffValue &&
				currentPbValue == other.currentPbValue;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			SplitVM SplitVMObj = obj as SplitVM;
			if (SplitVMObj == null)
				return false;
			else
				return Equals(SplitVMObj);
		}

		// TODO: Do I need to do something better with this?
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(SplitVM person1, SplitVM person2)
		{
			if (((object)person1) == null || ((object)person2) == null)
				return Object.Equals(person1, person2);

			return person1.Equals(person2);
		}

		public static bool operator !=(SplitVM person1, SplitVM person2)
		{
			if (((object)person1) == null || ((object)person2) == null)
				return !Object.Equals(person1, person2);

			return !(person1.Equals(person2));
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}

	public interface IUserSettings
	{
		object GetUserSetting(string name);
		void SetUserSetting(string name, object value);
		void Save();
	}

	public class StandardUserSettings : IUserSettings
	{
		public object GetUserSetting(string name)
		{
			// Access the user.config file for the requested value.
			return Properties.Settings.Default[name];
		}

		public void SetUserSetting(string name, object value)
		{
			Properties.Settings.Default[name] = value;
		}

		public void Save()
		{
			Properties.Settings.Default.Save();
		}
	}
}
