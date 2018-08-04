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

		/// <summary>
		/// Provide the list of splits as defined by the user.
		/// </summary>
		/// <param name="splits">List of splits, as newline-separated values.</param>
		public void SetSplits(string splits)
		{
			if (null == splits)
				splits = "";

			// Grab the contents of the input.
			splitTextList = splits;

			// Extract the individual lines, ignoring empty lines.
			string[] lines = splitTextList.Split(new string[] { Environment.NewLine },
				StringSplitOptions.RemoveEmptyEntries);

			// Make a new splitList.
			splitList = new ObservableCollection<SplitVM>();
			foreach (string s in lines)
			{
				// Only add non-empty strings
				if (s.Trim() != "")
					splitList.Add(new SplitVM
					{
						SplitName = s.Trim(),
						CurrentValue = 0,
						DiffValue = 0,
						CurrentPbValue = 0
					});
			}

			// For now, a new split list means starting over from the beginning.
			CurrentSplit = 0;

			// Manually trigger a UI update because the property is not us.
			// TODO: Is there a better way to do this? I don't want to make the property
			// writable, do I? If it was writable, wouldn't it trigger a UI update every
			// time I did an Add in the loop above? That doesn't sound good.
			//PropertyChanged(this, new PropertyChangedEventArgs("SplitList"));
			// NOTE: I found a code sample that calls NotifyPropertyChanged(). It works
			// in the GUI, but has a different result when this function is unit tested.
			// I do not know the difference and I don't know which is better. This one
			// doesn't create a new object so I am going to use it.
			NotifyPropertyChanged("SplitList");
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

			splitList = new ObservableCollection<SplitVM>();
			foreach (string splitName in Splits)
			{
				splitList.Add(new SplitVM
				{
					SplitName = splitName,
					CurrentValue = 0,
					DiffValue = 0,
					CurrentPbValue = 0
				});
			}

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
		#endregion

		// Break the tight coupling between the app and the data stores.
		// This allows for unit testing via a mock.
		private IUserSettings UserSettings;
		private IHomunculusModel Challenges;

		private int CurrentSplit = 0;

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

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}

	public class SplitVM : INotifyPropertyChanged
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
