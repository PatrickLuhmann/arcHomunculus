using Homunculus_ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Homunculus_WPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			// The ViewModel wants to know when the app closes.
			Closing += ((SplitsViewModel)DataContext).OnClosing;

			splitsListView.SelectedIndex = 0;

			// TODO: What if the last challenge has an active run?
			statusText.Text = "Run not active";
		}

		private void failureButton_Click(object sender, RoutedEventArgs e)
		{
			// Let the ViewModel do its thing.
			((SplitsViewModel)DataContext).FailureProc();

			// TODO: Do we do things with color here, or should that be
			// somewhere else? Eventually the user will be able to load
			// an in-progress run, and that list will need to be colored
			// before the button is pressed, so probably not here.
		}

		private void successButton_Click(object sender, RoutedEventArgs e)
		{
			// Let the ViewModel do its thing.
			((SplitsViewModel)DataContext).SuccessProc();

			// Advance the selected row in the ListView.
			// TODO: This should be fancier, such as with a star at the end.
			splitsListView.SelectedIndex++;
			if (splitsListView.SelectedIndex == splitsListView.Items.Count)
				splitsListView.SelectedIndex = -1;
		}

		private void createChallengeButton_Click(object sender, RoutedEventArgs e)
		{
			// Display Create Challenge window.
			var wnd = new CreateChallengeWindow((SplitsViewModel)DataContext, EditMode.Clone);
			Nullable<bool> ret = wnd.ShowDialog(); // ShowDialog is blocking.

			if (ret == true)
			{
				// For now, editing the splits will reset the current run. This is likely
				// not what the user wants, but we need to start somewhere.
				splitsListView.SelectedIndex = 0;
			}
		}

		private void challengesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ListView lv = (sender as ListView);
			if (lv.SelectedItem != null)
				((SplitsViewModel)DataContext).CurrentChallenge = lv.SelectedItem.ToString();

			// TODO: What if this challenge has an active run?
		}

		private void deleteChallengeButton_Click(object sender, RoutedEventArgs e)
		{
			// Get the challenge that is currently selected.
			string challengeName = challengesListView.SelectedItem as string;

			// Confirm that the user really wants to do this.
			MessageBoxResult result = MessageBox.Show("Do you want to delete challenge " + challengeName,
				"Confirm Delete Challenge", MessageBoxButton.YesNo, MessageBoxImage.Warning);

			if (result == MessageBoxResult.Yes)
			{
				// Invoke the Delete method on it.
				(DataContext as SplitsViewModel).DeleteChallenge(challengeName);
			}
		}

		private void startRunButton_Click(object sender, RoutedEventArgs e)
		{
			// Inform the ViewModel.
			(DataContext as SplitsViewModel).StartNewRun();

			// Reset the index of the split window.
			splitsListView.SelectedIndex = 0;

			statusText.Text = "Run In Progress";

		}

		private void endRunButton_Click(object sender, RoutedEventArgs e)
		{
			// Inform the ViewModel.
			(DataContext as SplitsViewModel).EndRun();

			// TODO: This is fine for explicit end, but what about when
			// the run ends naturally?
			statusText.Text = "Run not active";
		}

		/// <summary>
		/// Triggered when CurrentChallenge is updated.
		/// </summary>
		/// This is needed when the challenge changes due to something other than
		/// the user clicking it in the challenge list. That includes:
		/// - app launch
		/// - after new challenge is created
		/// - after a challenge is deleted
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TextBlock_SourceUpdated(object sender, DataTransferEventArgs e)
		{
			// Update the selected index.
			challengesListView.SelectedItem = ((SplitsViewModel)DataContext).CurrentChallenge;
		}

		/// <summary>
		/// Triggered when splitsListView is updated via its binding.
		/// </summary>
		/// We need to do more than just change the items in the list. We also
		/// need to set the selected index and the status text to reflect the
		/// new run.
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void splitsListView_TargetUpdated(object sender, DataTransferEventArgs e)
		{
			// Update the selected index.
			splitsListView.SelectedIndex = ((SplitsViewModel)DataContext).CurrentSplit;

			if (((SplitsViewModel)DataContext).RunInProgress)
				statusText.Text = "Run In Progress";
			else
				statusText.Text = "Run not active";
		}

		private void editChallengeButton_Click(object sender, RoutedEventArgs e)
		{
			// Display Edit Challenge window.
			var wnd = new CreateChallengeWindow((SplitsViewModel)DataContext, EditMode.Rearrange);
			wnd.SizeToContent = SizeToContent.WidthAndHeight;
			Nullable<bool> ret = wnd.ShowDialog(); // ShowDialog is blocking.

			if (ret == true)
			{
				// For now, editing the splits will reset the current run. This is likely
				// not what the user wants, but we need to start somewhere.
				splitsListView.SelectedIndex = 0;
			}
		}
	}
}
