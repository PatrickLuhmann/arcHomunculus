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
			var wnd = new CreateChallengeWindow((SplitsViewModel)DataContext);
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
			((SplitsViewModel)DataContext).CurrentChallenge = lv.SelectedItem.ToString();
		}
	}
}
