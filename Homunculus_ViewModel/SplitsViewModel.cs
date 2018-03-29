using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homunculus_ViewModel
{
    public class SplitsViewModel
    {
		public SplitsViewModel()
		{
			// TODO: Placeholder data???
			splitList = new List<SplitVM>();
			splitList.Add(new SplitVM { SplitName = "Last Giant", CurrentValue = "0", DiffValue = "0", PbValue = "0 (0)" });
			splitList.Add(new SplitVM { SplitName = "Pursuer", CurrentValue = "0", DiffValue = "0", PbValue = "1 (1)" });
		}
		private List<SplitVM> splitList;
		public List<SplitVM> SplitList { get { return splitList; } }
    }

	public class SplitVM
	{
		public string SplitName { get; set; }
		public string CurrentValue { get; set; }
		public string DiffValue { get; set; }
		public string PbValue { get; set; }
	}
}
