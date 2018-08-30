using System;
using System.Collections.Generic;
using System.Linq;
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
	public enum EditMode { Edit = 0, Clone };

	/// <summary>
	/// Interaction logic for CreateChallengeWindow.xaml
	/// </summary>
	public partial class CreateChallengeWindow : Window
	{
		public CreateChallengeWindow(SplitsViewModel svm, EditMode mode)
		{
			InitializeComponent();

			DataContext = svm;

			// Set window behavior based on mode.
			if (mode == EditMode.Edit)
			{
				this.Title = "Edit Challenge";
				challengeName.Text = ((SplitsViewModel)DataContext).CurrentChallenge;

				// Only allow Up, Down, and Rename.
				addButton.IsEnabled = false;
				deleteButton.IsEnabled = false;
				moveUpButton.IsEnabled = true;
				moveDownButton.IsEnabled = true;
				// TODO: Implement Rename feature.
			}
			else if (mode == EditMode.Clone)
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
			((SplitsViewModel)DataContext).AddSplitProc(splitsListBox.SelectedIndex);
		}

		private void okButton_Click(object sender, RoutedEventArgs e)
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
					((SplitsViewModel)DataContext).CreateChallenge(challengeName.Text, splitNames);
			}
			// TODO: Inform the user if the challenge wasn't created?

			// NOTE: This causes the window to close automatically.
			DialogResult = true;
		}

		private void deleteButton_Click(object sender, RoutedEventArgs e)
		{
			if (splitsListBox.SelectedIndex != -1)
				((SplitsViewModel)DataContext).DeleteSplitProc(splitsListBox.SelectedIndex);
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
				((SplitsViewModel)DataContext).MoveUpSplitProc(splitsListBox.SelectedIndex);
				splitsListBox.SelectedIndex = index - 1;
			}
		}

		private void moveDownButton_Click(object sender, RoutedEventArgs e)
		{
			if (splitsListBox.SelectedIndex < (splitsListBox.Items.Count - 1))
			{
				// Need to save the index because changing the list resets it to -1.
				int index = splitsListBox.SelectedIndex;
				((SplitsViewModel)DataContext).MoveDownSplitProc(splitsListBox.SelectedIndex);
				splitsListBox.SelectedIndex = index + 1;
			}
		}
	}
}
