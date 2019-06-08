using System;
using System.Collections.Generic;
using System.Text;

namespace Homunculus_WPFCore.ViewModels
{
	public class SplitVM
	{
		public string Name { get; set; }
		public int CurrentValue { get; set; }

		public SplitVM()
		{
			Name = "Default split name";
			CurrentValue = 17;
		}
	}
}
