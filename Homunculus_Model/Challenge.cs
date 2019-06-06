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
		internal IHomunculusModel TheDataService;

		public UInt32 ChallengeId { get; set; }

		public string Name { get; set; }
		public ObservableCollection<Split> Splits { get; set; }
		public ObservableCollection<Run> Runs { get; set; }
		public int CurrentSplitIndex { get; set; }
		public int PBIndex { get; set; } // TODO: What is this for?
		public Run PBRun {get;set;}

		internal Challenge(IHomunculusModel service)
		{
			TheDataService = service;
			ChallengeId = 0; // 0 means the object is not in the database.
			Name = null;
			Splits = new ObservableCollection<Split>();
			Runs = new ObservableCollection<Run>();
			CurrentSplitIndex = -1;
			PBIndex = -1;
			PBRun = null;
		}

		public Challenge() : this(null)
		{
		}

		public Run StartNewRun()
		{
			// Create a new Run for this challenge.
			Run newRun = TheDataService.CreateRun(this);

			Runs.Add(newRun);

			CurrentSplitIndex = 0;
			// TODO: How is this change to the Challenge saved in the db?

			return newRun;
		}

		public Run StartNewRun2()
		{
			// Make all the necessary changes to the in-memory objects.
			Run newRun = new Run();
			newRun.Challenge = this;
			newRun.StartDateTime = DateTime.Now;
			newRun.EndDateTime = DateTime.MinValue;
			newRun.Duration = new TimeSpan(TimeSpan.Zero.Ticks);
			foreach (var split in this.Splits)
			{
				Count newCount = new Count();
				newCount.Value = 0;
				newCount.Run = newRun;
				newCount.Split = split;

				newRun.Counts.Add(newCount);
			}
			this.Runs.Add(newRun);
			this.CurrentSplitIndex = 123;

			// Now update the database.
			//this.Name = "this is a test";
			TheDataService.UpdateChallenge(this, false, false);

			return newRun;
		}

		public void Success()
		{

		}
	}
}
