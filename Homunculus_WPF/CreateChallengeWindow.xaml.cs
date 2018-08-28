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
			// Extract the names of the splits and put them into a List.
			List<string> splitNames = new List<string>();
			foreach (SplitVM split in splitsListBox.ItemsSource)
			{
				string name = split.SplitName;
				splitNames.Add(name);
			}
			((SplitsViewModel)DataContext).CreateChallenge(challengeName.Text, splitNames);

			// NOTE: This causes the window to close automatically.
			DialogResult = true;
		}
	}
}
