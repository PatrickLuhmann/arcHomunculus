using GalaSoft.MvvmLight;
using Homunculus_ModelCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Homunculus_WPFCore.ViewModels
{
	public class MainVM : ViewModelBase
	{
		public static string TheNameOfTheDataStore;
		public static DataStoreDynamic TheDynamicDataStore;

		public string Salutation { get; set; }

		public MainVM()
		{
			TheNameOfTheDataStore = "Homunculuses R Us";

			TheDynamicDataStore = new DataStoreDynamic();

			Salutation = "Greetings and felicitations to Homunculus [Core]";
		}
	}
}
