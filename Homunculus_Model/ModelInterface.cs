using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homunculus_Model
{
	public interface IHomunculusModel
	{
		void CreateDatabase(string Filename);
		void LoadDatabase(string Filename);
		Challenge CreateChallenge(string ChallengeName, List<string> Splits);
		void ModifyChallenge(string ChallengeName, List<Split> Splits, string NewChallengeName);
		void DeleteChallenge(string ChallengeName);
		List<Challenge> GetChallenges();
		List<Split> GetSplits(string ChallengeName);
		List<Run> GetRuns(string ChallengeName);
		//void StartNewRun(string ChallengeName);
		void Success(string ChallengeName);
		void Failure(string ChallengeName);
		void EndRun(string ChallengeName);


		Run CreateRun(Challenge Challenge);

		// TODO: experimental
		void Update();
		void UpdateChallenge(Challenge Chall, Boolean DoSplits, Boolean DoRuns);
	}
}
