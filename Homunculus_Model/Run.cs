using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homunculus_Model
{
	public class Run
	{
		public List<int> SplitCounts;
		public int CurrentSplit;
		public bool Closed;
		public bool PB;

		public Run()
		{
			SplitCounts = new List<int>();
			CurrentSplit = -1; // run not yet started
			Closed = false;
			PB = false;
		}
	}
}
