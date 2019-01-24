using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homunculus_Model
{
	public class Run
	{
		public UInt32 RunId { get; set; }

		public ObservableCollection<Count> Counts { get; set; }
		public Challenge Challenge { get; set; }
		public DateTime StartDateTime { get; set; }
		public DateTime EndDateTime { get; set; }
		public TimeSpan Duration { get; set; }

		public Run()
		{
			RunId = 0;
			Counts = new ObservableCollection<Count>();
			Challenge = null;
			StartDateTime = new DateTime(0);
			EndDateTime = new DateTime(0);
			Duration = new TimeSpan(0);
		}
	}
}
