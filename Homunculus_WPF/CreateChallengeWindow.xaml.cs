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

			// Create the text list of splits.
			// TODO: Think about this some more to see if there is a more elegant
			// way to do this. My first attempt with bindings didn't work for
			// some reason. Then again, I eventually want to move to a List
			// that has rows that can be moved up/down via the mouse, so maybe
			// there is no point in trying to improve the text box approach.
			splitsTextBox.Text = ((SplitsViewModel)DataContext).SplitTextList;
		}

		private void addButton_Click(object sender, RoutedEventArgs e)
		{
			((SplitsViewModel)DataContext).AddSplitProc(splitsListBox.SelectedIndex);
		}

		private void okButton_Click(object sender, RoutedEventArgs e)
		{
			//((SplitsViewModel)DataContext).SetSplits(splitsTextBox.Text);

			string name = challengeName.Text;
			List<string> splits = new List<string>();
			//((SplitsViewModel)DataContext).CreateChallenge(name, splits);

			// NOTE: This causes the window to close automatically.
			DialogResult = true;
		}
	}
}
