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

        private string splitTextList;
        public string SplitTextList
        {
            get
            {
#if true
                splitTextList = "";
                foreach(var split in splitList)
                {
                    string text = split.SplitName;
                    splitTextList += text + Environment.NewLine;
                }
#endif
                return splitTextList;
            }

            set
            {
                // Grab the contents of the text box.
                splitTextList = value;

                // Extract the individual lines, ignoring empty lines.
                string[] lines = splitTextList.Split(new string[] { Environment.NewLine },
                    StringSplitOptions.RemoveEmptyEntries);

#if false
                // Recreate the split text list.
                splitTextList = "";
                foreach(string s in lines)
                {
                    splitTextList += s + Environment.NewLine;
                }
#endif
                // Make a new splitList.
                splitList = new ObservableCollection<SplitVM>();
                foreach (string s in lines)
                {
                    splitList.Add(new SplitVM { SplitName = s,
                        CurrentValue = 0,
                        DiffValue = 0,
                        CurrentPbValue = 0 });
                }

                // For now, a new split list means starting over from the beginning.
                CurrentSplit = 0;

                // Manually trigger a UI update because the property is not us.
                // TODO: Is there a better way to do this? I don't want to make the property
                // writable, do I? If it was writable, wouldn't it trigger a UI update every
                // time I did an Add in the loop above? That doesn't sound good.
                PropertyChanged(this, new PropertyChangedEventArgs("SplitList"));
            }
        }

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

            // TODO: Placeholder data.
            splitTextList = "This string should never be seen.";

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
