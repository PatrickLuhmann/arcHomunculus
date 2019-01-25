using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homunculus_Model
{
	public class Count
	{
		public UInt32 CountId { get; set; }

		// TODO: Should Value be -1 for a split that hasn't been reached yet?
		public int Value { get; set; }

		public Run Run { get; set; }
		public Split Split { get; set; }
	}
}
