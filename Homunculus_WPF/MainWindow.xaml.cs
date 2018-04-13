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

			splitsListView.SelectedIndex = 0;
		}

		private void failureButton_Click(object sender, RoutedEventArgs e)
		{
			// TODO: I don't like this implementation. I don't want the View to 
			// know the name of the class of the ViewModel because they should be
			// separate. However, this same class name is given in the XAML in
			// the DataContext section, so what is the big deal?
			((SplitsViewModel)DataContext).FailureProc();
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
	}
}
