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
	/// <summary>
	/// Interaction logic for CreateChallengeWindow.xaml
	/// </summary>
	public partial class CreateChallengeWindow : Window
	{
		public CreateChallengeWindow(SplitsViewModel svm)
		{
			InitializeComponent();

			DataContext = svm;
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
	}
}
