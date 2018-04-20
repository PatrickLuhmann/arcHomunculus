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

namespace Homunculus_ViewModel
{
    public class SplitsViewModel : INotifyPropertyChanged
	{
		#region View properties
		private ObservableCollection<SplitVM> splitList;
		public ObservableCollection<SplitVM> SplitList { get { return splitList; } }

		public string SuccessButtonText { get; set; }
		public string FailureButtonText {  get { return "Failure"; } }
		public string CreateChallengeButtonText { get { return "Create Challenge"; } }
		#endregion

		private int CurrentSplit = 0;

		public SplitsViewModel()
		{
			// TODO: Placeholder data???
			// TODO: Does the Model hold the instantiation as well as the schema???
			splitList = new ObservableCollection<SplitVM>();
			splitList.Add(new SplitVM { SplitName = "Last Giant", CurrentValue = 0, DiffValue = 0, CurrentPbValue = 0 });
			splitList.Add(new SplitVM { SplitName = "Pursuer", CurrentValue = 0, DiffValue = 0, CurrentPbValue = 0 });
			splitList.Add(new SplitVM { SplitName = "Last Giant", CurrentValue = 0, DiffValue = 0, CurrentPbValue = 0 });
			splitList.Add(new SplitVM { SplitName = "Pursuer", CurrentValue = 0, DiffValue = 0, CurrentPbValue = 0 });
			splitList.Add(new SplitVM { SplitName = "Last Giant", CurrentValue = 0, DiffValue = 0, CurrentPbValue = 0 });
			splitList.Add(new SplitVM { SplitName = "Pursuer", CurrentValue = 0, DiffValue = 0, CurrentPbValue = 0 });
			
			SuccessButtonText = "Success";
		}


		public void SuccessProc()
		{
			// Go to the next split.
			CurrentSplit++;

			// If we are past the end, then we have won the game.
			if (CurrentSplit == SplitList.Count)
			{
				// TODO: Handle "game won" condition here.
				CurrentSplit--;
			}
		}

		public void FailureProc()
		{
			// Increment the current value of the current split.
			splitList[CurrentSplit].CurrentValue++;

			// Recalculate the difference.
			splitList[CurrentSplit].DiffValue = 
				splitList[CurrentSplit].CurrentPbValue - splitList[CurrentSplit].CurrentValue;

			// TODO: This Add causes the list in MainWindow to be modified and shown, but the
			// CreateChallenge window only shows the original list items.
			splitList.Add(new SplitVM { SplitName = "New Failure Split" });
		}

		public void AddSplitProc(int selectedSplit)
		{
			// TODO: Add the new item at an arbitrary point.
			splitList.Add(new SplitVM { SplitName = "New Split" });
			// TODO: Do I need to do something here to get the list to update in the UI?
		}

		// I copied this from a web page. I do not know what it does.
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
		public int DiffValue { get { return diffValue; }
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

		// I copied this from a web page. I do not know what it does.
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
