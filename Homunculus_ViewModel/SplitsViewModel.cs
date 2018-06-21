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
		private ObservableCollection<string> challengeList;
		public ObservableCollection<string> ChallengeList
		{
			get { return challengeList; }
			set { challengeList = value; NotifyPropertyChanged(); }
		}

		private string currentChallenge;
		public string CurrentChallenge
		{
			get { return currentChallenge; }
			set { currentChallenge = value; NotifyPropertyChanged(); }
		}

		private ObservableCollection<SplitVM> splitList;
		public ObservableCollection<SplitVM> SplitList
		{
			get { return splitList; }
			set { splitList = value; NotifyPropertyChanged(); }
		}

#if false
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
#endif

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

#if false
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
#endif

		public int AddNewChallenge(string Name, List<string> Splits)
		{
			// Challenge names must be unique.
			if (challengeList.Contains(Name))
				return -1;

			// There must be at least one split.
			if (Splits.Count == 0)
				return -2;

			List<Split> modelSplits = Challenges.CreateChallenge(Name, Splits);

			// Update the bindable properties.
			CurrentChallenge = Name;
			ChallengeList = new ObservableCollection<string>(Challenges.GetChallenges());
			SplitList.Clear();
			foreach (var split in modelSplits)
			{
				SplitList.Add(new SplitVM
				{
					SplitName = split.Name,
					Handle = split.Handle,
					CurrentPbValue = 0,
					CurrentValue = 0,
					DiffValue = 0
				});
			}

			return 0;
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
		#endregion

		#region Private Data Members
		Model Challenges = new Model();
		private int CurrentSplit = 0;
		#endregion

		public SplitsViewModel()
		{
			CurrentChallenge = "No challenge selected";

			// Get the challenges.
			challengeList = new ObservableCollection<string>(Challenges.GetChallenges());

			// Get the splits.
			// TODO: Placeholder data???
			// TODO: Does the Model hold the instantiation as well as the schema???
			splitList = new ObservableCollection<SplitVM>();
#if false
			splitList.Add(new SplitVM { SplitName = "Last Giant", CurrentValue = 0, DiffValue = 0, CurrentPbValue = 0 });
			splitList.Add(new SplitVM { SplitName = "Pursuer", CurrentValue = 0, DiffValue = 0, CurrentPbValue = 0 });
			splitList.Add(new SplitVM { SplitName = "Last Giant", CurrentValue = 0, DiffValue = 0, CurrentPbValue = 0 });
			splitList.Add(new SplitVM { SplitName = "Pursuer", CurrentValue = 0, DiffValue = 0, CurrentPbValue = 0 });
			splitList.Add(new SplitVM { SplitName = "Last Giant", CurrentValue = 0, DiffValue = 0, CurrentPbValue = 0 });
			splitList.Add(new SplitVM { SplitName = "Pursuer", CurrentValue = 0, DiffValue = 0, CurrentPbValue = 0 });

			// TODO: Placeholder data.
			splitTextList = "This string should never be seen.";
#endif
			SuccessButtonText = "Success";
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
		public UInt32 Handle; // Not a property.

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
}
