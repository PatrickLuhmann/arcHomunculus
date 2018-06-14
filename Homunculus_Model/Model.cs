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
		private string FileName;

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
				runs.Columns.Add("Name", typeof(string));
				runs.Columns.Add("ChallengeID", typeof(UInt32));
				runs.Columns.Add("IndexWithinChallenge", typeof(UInt32));

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

		// Create a new challenge
		public List<Split> CreateChallenge(string ChallengeName, List<Split> Splits)
		{
			// Create a new entry in the Challenges table for the challenge.
			DataTable Challenges = ChallengeRuns.Tables["Challenges"];
			DataRow row = Challenges.NewRow();
			row["Name"] = ChallengeName;
			UInt32 challengeID = Convert.ToUInt32( row["ID"].ToString() );
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
				Splits[i].Handle = Convert.ToUInt32(dr["ID"] );
			}

			// TODO: Save db here?

			return Splits;
		}

		public List<Split> GetSplits(string ChallengeName)
		{
			DataTable Challenge = ChallengeRuns.Tables[ChallengeName];
			if (Challenge == null)
				return null;

			List<Split> Splits = new List<Split>();
			foreach (DataColumn Col in Challenge.Columns)
			{
				Splits.Add(new Split { Name = Col.ColumnName, Handle = 0 });
			}

			return Splits;
		}

		// Provides the splits from the Personal Best run
		public List<RunInfo> GetPBRun(string ChallengeName)
		{
			return null;
		}

		public Run NewRun(string ChallengeName)
		{
			DataTable Challenge = ChallengeRuns.Tables[ChallengeName];
			DataRow Row = Challenge.NewRow();
			Challenge.Rows.Add(Row);

			Run NewRun = new Run();
			NewRun.Handle = (int)Row["ID"];
			NewRun.CurrSplit = 0;
			// TODO: This seems very hacky.
			for (int i = 0; i < Challenge.Columns.Count - 3; i++)
			{
				NewRun.Splits[i] = Challenge.Columns[i].ColumnName;
				NewRun.Counts[i] = 0;
			}
			return NewRun;
		}
	}

	public class RunInfo
	{
		public string SplitName;
		public int HitCount;
	}

	public class Run
	{
		public int Handle; // opaque handle to the user
		public List<string> Splits;
		public List<int> Counts;
		public int CurrSplit;
	}

	public class Split
	{
		public UInt32 Handle = 0;
		public string Name;
	}
}
