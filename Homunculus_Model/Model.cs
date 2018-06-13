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

			// Load the data file. If it doesn't exist, create it.
			FileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\homunculus.xml";

			if (System.IO.File.Exists(FileName))
			{
				ChallengeRuns = new DataSet();
				ChallengeRuns.ReadXml(FileName);
				System.Diagnostics.Debug.WriteLine("Model: data file loaded - " + FileName);
			}
			else
			{
				ChallengeRuns = new DataSet("Homunculus");

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

				ChallengeRuns.WriteXml(FileName, XmlWriteMode.WriteSchema);

				System.Diagnostics.Debug.WriteLine("Model: data file created - " + FileName);
			}
		}

		public int NumChallenges
		{
			get
			{
				// There is 1 non-challenge table in the set.
				return ChallengeRuns.Tables.Count - 1;
			}
		}

		// Public interface methods???
		public bool Load() { return false; }
		public bool Save() { return false; }
		public bool Create() { return false; }

		// Create a new challenge
		public void CreateChallenge(string ChallengeName, List<string> Splits)
		{
			// Create a new DataTable.
			DataTable Challenge = new DataTable(ChallengeName);

			// Create the split columns.
			foreach (string Name in Splits)
			{
				DataColumn Col = new DataColumn(Name);
				Challenge.Columns.Add(Col);
			}

			// Create the bookkeeping columns.
			Challenge.Columns.Add("ID", typeof(UInt32));
			Challenge.PrimaryKey = new DataColumn[] { Challenge.Columns["ID"] };
			Challenge.Columns["ID"].AutoIncrement = true;
			Challenge.Columns["ID"].AutoIncrementSeed = 1;
			Challenge.Columns["ID"].AutoIncrementStep = 1;

			// The current split (0-based). -1 means the split has not
			// started. When the run is completed, this will be set to
			// the number of splits (i.e. one past the last split due
			// to 0-based counting).
			Challenge.Columns.Add("State", typeof(int));
			Challenge.Columns["State"].DefaultValue = -1;

			// Only one run can be true; the rest must be false.
			Challenge.Columns.Add("PB", typeof(bool));
			Challenge.Columns["PB"].DefaultValue = false;

			ChallengeRuns.Tables.Add(Challenge);

			// TODO: Save db here?
		}

		public List<string> GetSplits(string ChallengeName)
		{
			DataTable Challenge = ChallengeRuns.Tables[ChallengeName];
			if (Challenge == null)
				return null;

			List<string> Splits = new List<string>();
			foreach (DataColumn Col in Challenge.Columns)
			{
				Splits.Add(Col.ColumnName);
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
}
