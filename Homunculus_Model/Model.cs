using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homunculus_Model
{
	public class Model
	{
		// All data is stored in a single DataSet.
		private DataSet ChallengeRuns;
		//private string FileName;

		public Model()
		{
			System.Diagnostics.Debug.WriteLine("Enter Model constructor");

#if false
			// Load the data file. If it doesn't exist, create it.
			FileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\homunculus.xml";

			if (System.IO.File.Exists(FileName))
			{
				ChallengeRuns = new DataSet();
				ChallengeRuns.ReadXml(FileName);
				System.Diagnostics.Debug.WriteLine("Model: data file loaded - " + FileName);
			}
			else
#endif
			{
				ChallengeRuns = new DataSet("Homunculus");

				// TABLE: Challenges
				DataTable challenges = ChallengeRuns.Tables.Add("Challenges");
				challenges.Columns.Add("ID", typeof(UInt32));
				challenges.PrimaryKey = new DataColumn[] { challenges.Columns["ID"] };
				challenges.Columns["ID"].AutoIncrement = true;
				challenges.Columns["ID"].AutoIncrementSeed = 1;
				challenges.Columns["ID"].AutoIncrementStep = 1;
				challenges.Columns.Add("Name", typeof(string));

				// TABLE: Splits
				DataTable splits = ChallengeRuns.Tables.Add("Splits");
				splits.Columns.Add("ID", typeof(UInt32));
				splits.PrimaryKey = new DataColumn[] { splits.Columns["ID"] };
				splits.Columns["ID"].AutoIncrement = true;
				splits.Columns["ID"].AutoIncrementSeed = 1;
				splits.Columns["ID"].AutoIncrementStep = 1;
				splits.Columns.Add("Name", typeof(string));
				splits.Columns.Add("ChallengeID", typeof(UInt32));
				splits.Columns.Add("IndexWithinChallenge", typeof(UInt32));

				// TABLE: Runs
				DataTable runs = ChallengeRuns.Tables.Add("Runs");
				runs.Columns.Add("ID", typeof(UInt32));
				runs.PrimaryKey = new DataColumn[] { runs.Columns["ID"] };
				runs.Columns["ID"].AutoIncrement = true;
				runs.Columns["ID"].AutoIncrementSeed = 1;
				runs.Columns["ID"].AutoIncrementStep = 1;
				runs.Columns.Add("ChallengeID", typeof(UInt32));
				runs.Columns.Add("Closed", typeof(bool));
				runs.Columns["Closed"].DefaultValue = false;
				runs.Columns.Add("PB", typeof(bool));
				runs.Columns["PB"].DefaultValue = false;

				// TABLE: Counts
				DataTable counts = ChallengeRuns.Tables.Add("Counts");
				counts.Columns.Add("ID", typeof(UInt32));
				counts.PrimaryKey = new DataColumn[] { counts.Columns["ID"] };
				counts.Columns["ID"].AutoIncrement = true;
				counts.Columns["ID"].AutoIncrementSeed = 1;
				counts.Columns["ID"].AutoIncrementStep = 1;
				counts.Columns.Add("RunID", typeof(UInt32));
				counts.Columns.Add("SplitID", typeof(UInt32));
				counts.Columns.Add("Value", typeof(Int32));

				// TABLE: ConfigParams
				DataTable configParameters = ChallengeRuns.Tables.Add("ConfigParams");
				configParameters.Columns.Add("Name", typeof(String));
				configParameters.PrimaryKey = new DataColumn[] { configParameters.Columns["Name"] };
				configParameters.Columns.Add("Value", typeof(String));

				DataRow row;
				row = configParameters.NewRow();
				row["Name"] = "AppName";
				row["Value"] = "Homunculus";
				configParameters.Rows.Add(row);

				row = configParameters.NewRow();
				row["Name"] = "SchemaVersion";
				row["Value"] = "1";
				configParameters.Rows.Add(row);

				//ChallengeRuns.WriteXml(FileName, XmlWriteMode.WriteSchema);
				//System.Diagnostics.Debug.WriteLine("Model: data file created - " + FileName);
			}
		}

		// Public interface methods???
		public bool Load() { return false; }
		public bool Save() { return false; }
		public bool Create() { return false; }

		/// <summary>
		/// Create a new challenge in the database.
		/// </summary>
		/// <param name="ChallengeName">The name of the challenge (must be unique).</param>
		/// <param name="Splits">Ordered list of the splits for the challenge (must not be null or empty).</param>
		/// <returns>Ordered list of the splits, with Handle filled in for future use.</returns>
		public List<Split> CreateChallenge(string ChallengeName, List<Split> Splits)
		{
			// Check for bad parameters.
			if (ChallengeName == null || Splits == null)
				throw new ArgumentNullException();

			if (Splits.Count == 0)
				throw new ArgumentException();

			// Create a new entry in the Challenges table for the challenge.
			DataTable Challenges = ChallengeRuns.Tables["Challenges"];
			DataRow row = Challenges.NewRow();
			row["Name"] = ChallengeName;
			UInt32 challengeID = Convert.ToUInt32(row["ID"].ToString());
			Challenges.Rows.Add(row);

			// Create the split entries in the Splits table.
			DataTable dt = ChallengeRuns.Tables["Splits"];
			DataRow dr;
			for (int i = 0; i < Splits.Count; i++)
			{
				dr = dt.NewRow();
				dr["Name"] = Splits[i].Name;
				dr["ChallengeID"] = challengeID;
				dr["IndexWithinChallenge"] = i;
				dt.Rows.Add(dr);
				Splits[i].Handle = Convert.ToUInt32(dr["ID"]);
			}

			// TODO: Save db here?

			return Splits;
		}

		/// <summary>
		/// Get a list of the challenges in the database.
		/// </summary>
		/// <returns>List of the challenge names (no particular order).</returns>
		public List<string> GetChallenges()
		{
			List<string> challenges = new List<string>();
			foreach (DataRow row in ChallengeRuns.Tables["Challenges"].Rows)
			{
				challenges.Add(row["Name"].ToString());
			}
			return challenges;
		}

		/// <summary>
		/// Get an ordered list of the splits in the given challenge.
		/// </summary>
		/// <param name="ChallengeName">Name of the challenge.</param>
		/// <returns>Ordered list of the splits in the given challenge.</returns>
		public List<Split> GetSplits(string ChallengeName)
		{
			// Get the Challenge ID.
			DataRow[] dr = ChallengeRuns.Tables["Challenges"]
										.Select("Name = '" + ChallengeName + "'");

			UInt32 challengeID = Convert.ToUInt32(dr[0]["ID"]);

			// Get the splits that go with this challenge.
			dr = ChallengeRuns.Tables["Splits"]
							  .Select("ChallengeID = " + challengeID.ToString(),
									  "IndexWithinChallenge ASC");

			List<Split> splits = new List<Split>();
			foreach (var r in dr)
			{
				splits.Add(new Split
				{
					Name = r["Name"].ToString(),
					Handle = Convert.ToUInt32(r["ID"])
				});
			}

			return splits;
		}

		public void StartNewRun(string ChallengeName)
		{
			if (ChallengeName == null)
				throw new ArgumentNullException();

			// Get the Challenge ID.
			DataRow[] dr = ChallengeRuns.Tables["Challenges"]
										.Select("Name = '" + ChallengeName + "'");
			UInt32 challengeID = Convert.ToUInt32(dr[0]["ID"]);

			// Make sure there isn't already an active run for this challenge.
			DataRow[] runRows = ChallengeRuns.Tables["Runs"]
				.Select("ChallengeID = " + challengeID.ToString(),
				"ID DESC");
			if (runRows.Length != 0)
			{
				if (Convert.ToBoolean(runRows[0]["Closed"]) == false)
					throw new InvalidOperationException();
			}

			// Create a new Run for this Challenge.
			DataRow runRow = ChallengeRuns.Tables["Runs"].NewRow();
			runRow["ChallengeID"] = challengeID;
			UInt32 runId = Convert.ToUInt32(runRow["ID"].ToString());
			ChallengeRuns.Tables["Runs"].Rows.Add(runRow);

			// Create the Counts for this new Run.

			// Get the splits that go with this challenge.
			dr = ChallengeRuns.Tables["Splits"]
							  .Select("ChallengeID = " + challengeID.ToString(),
									  "IndexWithinChallenge ASC");

			// For each split, create a row in the Counts table.
			foreach (var sr in dr)
			{
				// Create the new count row.
				DataRow countRow = ChallengeRuns.Tables["Counts"].NewRow();

				// Set the SplitID.
				UInt32 splitId = Convert.ToUInt32(sr["ID"]);
				countRow["SplitID"] = splitId;

				// Set the RunID.
				countRow["RunID"] = runId;

				// Set the default Value.
				countRow["Value"] = 0;

				// Put the row into the table.
				ChallengeRuns.Tables["Counts"].Rows.Add(countRow);
			}
		}

		public void UpdateRun(string ChallengeName, List<int> SplitValues)
		{
			// Check for bad parameters.
			if (ChallengeName == null || SplitValues == null)
				throw new ArgumentNullException();

			// Get the Challenge ID.
			DataRow[] dr = ChallengeRuns.Tables["Challenges"]
										.Select("Name = '" + ChallengeName + "'");
			UInt32 challengeID = Convert.ToUInt32(dr[0]["ID"]);

			// Get the active run for this challenge.
			DataRow[] rowRuns = ChallengeRuns.Tables["Runs"]
				.Select("ChallengeID = " + challengeID.ToString(),
				"ID DESC");
			if (Convert.ToBoolean(rowRuns[0]["Closed"]) == true)
				throw new InvalidOperationException();
			UInt32 runID = Convert.ToUInt32(rowRuns[0]["ID"]);

			// Get the splits that go with this challenge.
			// Make sure they are in the correct order.
			DataRow[] rowSplits = ChallengeRuns.Tables["Splits"]
				.Select("ChallengeID = " + challengeID.ToString(),
				"IndexWithinChallenge ASC");

			if (rowSplits.Length != SplitValues.Count)
				throw new ArgumentException();

			for (int i = 0; i < rowSplits.Length; i++)
			{
				DataRow[] rowCounts = ChallengeRuns.Tables["Counts"]
					.Select("RunID = " + runID.ToString() + " AND SplitID = " + rowSplits[i]["ID"].ToString());
				rowCounts[0]["Value"] = SplitValues[i];
			}
		}

		public void EndRun(string ChallengeName)
		{
			if (ChallengeName == null)
				throw new ArgumentNullException();

			// Get the Challenge ID.
			DataRow[] dr = ChallengeRuns.Tables["Challenges"]
										.Select("Name = '" + ChallengeName + "'");
			UInt32 challengeID = Convert.ToUInt32(dr[0]["ID"]);

			// Make sure there is an active run for this challenge.
			DataRow[] runRows = ChallengeRuns.Tables["Runs"]
				.Select("ChallengeID = " + challengeID.ToString(),
				"ID DESC");
			if ((runRows.Length == 0) ||
				(Convert.ToBoolean(runRows[0]["Closed"]) == true))
				throw new InvalidOperationException();

			// Close the run.
			runRows[0]["Closed"] = true;

			// Check to see if this run is a new PB.
		}

		public List<List<int>> GetRuns(string ChallengeName)
		{
			if (ChallengeName == null)
				throw new ArgumentNullException();

			// Get the Challenge ID.
			DataRow[] dr = ChallengeRuns.Tables["Challenges"]
										.Select("Name = '" + ChallengeName + "'");
			UInt32 challengeID = Convert.ToUInt32(dr[0]["ID"]);

			// Get the splits that go with this challenge.
			DataRow[] rowSplits = ChallengeRuns.Tables["Splits"]
				.Select("ChallengeID = " + challengeID.ToString(),
				"IndexWithinChallenge ASC");

			// Get the runs that go with this challenge.
			DataRow[] rowRuns = ChallengeRuns.Tables["Runs"]
				.Select("ChallengeID = " + challengeID.ToString(),
				"ID ASC");

			List<List<int>> runs = new List<List<int>>();
			foreach (var rr in rowRuns)
			{
				// Need a new list for each Run.
				List<int> runCounts = new List<int>();

				// Get the ID of this run.
				UInt32 runId = Convert.ToUInt32(rr["ID"]);

				// The splits need to be ordered for this to work.
				foreach (var sr in rowSplits)
				{
					UInt32 splitId = Convert.ToUInt32(sr["ID"]);
					DataRow[] count = ChallengeRuns.Tables["Counts"]
						.Select("RunID = " + runId + " AND SplitID = " + splitId);

					runCounts.Add(Convert.ToInt32(count[0]["Value"]));
				}

				// Add the newly-created list of split values to the main list.
				runs.Add(runCounts);
			}
			return runs;
		}
	}

	public class Split
	{
		public UInt32 Handle = 0;
		public string Name;
	}
}
