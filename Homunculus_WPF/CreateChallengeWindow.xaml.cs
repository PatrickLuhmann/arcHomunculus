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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Homunculus_ViewModel;

namespace Homunculus_WPF
{
	public enum EditMode { Rearrange = 0, Clone };

	public class HackViewModel : INotifyPropertyChanged
	{
		private ObservableCollection<SplitVM> hackSplitList;
		public ObservableCollection<SplitVM> HackSplitList
		{
			get { return hackSplitList; }
			set { hackSplitList = value; NotifyPropertyChanged("HackSplitList"); }
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

	/// <summary>
	/// Interaction logic for CreateChallengeWindow.xaml
	/// </summary>
	public partial class CreateChallengeWindow : Window
	{
		EditMode Mode;
		HackViewModel myHVM;
		SplitsViewModel Svm;

		public CreateChallengeWindow(SplitsViewModel svm, EditMode mode)
		{
			InitializeComponent();

			Svm = svm;
			myHVM = (HackViewModel)DataContext;
			myHVM.HackSplitList = new ObservableCollection<SplitVM>();
			foreach (var s in svm.SplitList)
			{
				myHVM.HackSplitList.Add(s);
			}

			// Set window behavior based on mode.
			Mode = mode;
			if (Mode == EditMode.Rearrange)
			{
				this.Title = "Rearrange Challenge";
				challengeName.Text = svm.CurrentChallenge;

				// Only allow Up, Down, and Rename.
				addButton.IsEnabled = false;
				deleteButton.IsEnabled = false;
				moveUpButton.IsEnabled = true;
				moveDownButton.IsEnabled = true;
				// TODO: Implement Rename feature.
			}
			else if (Mode == EditMode.Clone)
			{
				this.Title = "Create Challenge";
				challengeName.Text = "New Challenge";

				// All features are available.
				addButton.IsEnabled = true;
				deleteButton.IsEnabled = true;
				moveUpButton.IsEnabled = true;
				moveDownButton.IsEnabled = true;
				// TODO: Implement Rename feature.
			}
			else
			{
				this.Title = "ERROR: Unknown edit mode";
			}
		}

		private void addButton_Click(object sender, RoutedEventArgs e)
		{
			//((SplitsViewModel)DataContext).AddSplitProc(splitsListBox.SelectedIndex);
			int idx = splitsListBox.SelectedIndex;
			if (idx < 0)
				idx = 0;
			myHVM.HackSplitList.Insert(idx, new SplitVM { Handle = 0, SplitName = "New Split", CurrentValue = 0, CurrentPbValue = 0 });
		}

		private void okButton_Click(object sender, RoutedEventArgs e)
		{
			if (Mode == EditMode.Clone)
			{
				if (challengeName.Text != "")
				{
					// Extract the names of the splits and put them into a List.
					List<string> splitNames = new List<string>();
					foreach (SplitVM split in splitsListBox.ItemsSource)
					{
						string name = split.SplitName;
						splitNames.Add(name);
					}
					if (splitNames.Count > 0)
					{
						try
						{
							Svm.CreateChallenge(challengeName.Text, splitNames);
							DialogResult = true;
						}
						catch (ArgumentException)
						{
							// Inform the user that they need to change the challenge name.
							MessageBox.Show("ERROR: The name of the challenge must be unique.", "Bad Challenge Name", MessageBoxButton.OK, MessageBoxImage.Error);
						}
					}
					else
						MessageBox.Show("ERROR: The challenge must have at least one split.", "Empty Split List", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				else
					MessageBox.Show("ERROR: The challenge must have a name.", "Empty Challenge Name", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			else
			{
				// TODO: Implement Edit.
				try
				{
					Svm.RearrangeChallenge(challengeName.Text, myHVM.HackSplitList);

					// NOTE: This causes the window to close automatically.
					DialogResult = true;
				}
				catch (ArgumentException)
				{
					// Inform the user that they need to change the challenge name.
					MessageBox.Show("ERROR: The name of the challenge must be unique.", "Bad Challenge Name", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void deleteButton_Click(object sender, RoutedEventArgs e)
		{
			if (splitsListBox.SelectedIndex != -1)
			{
				//((SplitsViewModel)DataContext).DeleteSplitProc(splitsListBox.SelectedIndex);
			}
		}

		private void splitsListBoxItem_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			ListBoxItem item = sender as ListBoxItem;
			item.IsSelected = true;
		}

		private void moveUpButton_Click(object sender, RoutedEventArgs e)
		{
			if (splitsListBox.SelectedIndex > 0)
			{
				// Need to save the index because changing the list resets it to -1.
				int index = splitsListBox.SelectedIndex;
#if false
				((SplitsViewModel)DataContext).MoveUpSplitProc(splitsListBox.SelectedIndex);
#else
				SplitVM item = myHVM.HackSplitList[index];
				myHVM.HackSplitList.RemoveAt(index);
				myHVM.HackSplitList.Insert(index - 1, item);
				//NotifyPropertyChanged("SplitList");
#endif

				splitsListBox.SelectedIndex = index - 1;
			}
		}

		private void moveDownButton_Click(object sender, RoutedEventArgs e)
		{
			if (splitsListBox.SelectedIndex < (splitsListBox.Items.Count - 1))
			{
				// Need to save the index because changing the list resets it to -1.
				int index = splitsListBox.SelectedIndex;
				//((SplitsViewModel)DataContext).MoveDownSplitProc(splitsListBox.SelectedIndex);
				SplitVM item = myHVM.HackSplitList[index];
				myHVM.HackSplitList.RemoveAt(index);
				myHVM.HackSplitList.Insert(index + 1, item);

				splitsListBox.SelectedIndex = index + 1;
			}
		}
	}
}
