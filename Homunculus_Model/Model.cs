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

		// Public interface methods???
		public bool Load() { return false; }
		public bool Save() { return false; }
		public bool Create() { return false; }
		
		// Provides the splits
	}
}
