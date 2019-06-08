using System;
using System.Collections.Generic;
using System.Text;

namespace Homunculus_WPFCore.ViewModels
{
	public class MainVM
	{
		public string Salutation { get; set; }

		public MainVM()
		{
			Salutation = "Greetings and felicitations to Homunculus [Core]";
		}
	}
}
