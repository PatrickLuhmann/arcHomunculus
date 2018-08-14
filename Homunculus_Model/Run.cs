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
		public bool Closed;
		public bool PB;

		public Run()
		{
			SplitCounts = new List<int>();
			Closed = false;
			PB = false;
		}
	}
}
