using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Homunculus_WPFCore.ViewModels
{
	public class CurrentChallengeVM
	{
		public ObservableCollection<SplitVM> Splits { get; set; }

		public CurrentChallengeVM()
		{
			Splits = new ObservableCollection<SplitVM>();
		}
	}
}
