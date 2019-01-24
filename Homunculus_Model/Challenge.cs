using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homunculus_Model
{
	public class Challenge
	{
		public UInt32 ChallengeId { get; set; }

		public string Name { get; set; }
		public ObservableCollection<Split> Splits { get; set; }
		public ObservableCollection<Run> Runs { get; set; }
		public int CurrentSplitIndex { get; set; }
		public int PBIndex { get; set; }
		public Run PBRun {get;set;}

		public Challenge()
		{
			ChallengeId = 0; // 0 means the object is not in the database.
			Name = null;
			Splits = new ObservableCollection<Split>();
			Runs = new ObservableCollection<Run>();
			CurrentSplitIndex = -1;
			PBIndex = -1;
			PBRun = null;
		}
	}
}
