using LeagueClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace LeagueClassLibrary.DataAccess
{
    public static class ChampionData
    {
        public static DataTable DataTableChampions { get; set; }

        private static Random r = new Random();

        public static void LoadCsv(string padNaarCsv)
        {
            //Does file exist?
            if (!File.Exists(@padNaarCsv))
            {
                throw new Exception($"No file found on location @'{padNaarCsv}'");
            }

            //Initialize/reset DataTable
            DataTableChampions = new DataTable();

            //Laad de DataTableChampions met het opgegeven pad
            using (StreamReader reader = new StreamReader(@padNaarCsv))
            {
                //Check if it has any content
                if (reader.EndOfStream)
                {
                    throw new Exception("No content available to read. File is empty!");
                }

                //Get the headers first
                string headerLine = reader.ReadLine();
                string[] headerParts = headerLine.Split(';');
                foreach (string part in headerParts)
                {
                    DataTableChampions.Columns.Add(part, typeof(string));
                }

                //Read the next lines to fill in the table
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] parts = line.Split(';');
                    DataTableChampions.Rows.Add(parts);
                }
            }
        }

        public static DataView GetDataView()
        {
            return new DataView(DataTableChampions);
        }

        public static DataView GetDataViewChampionsByPosition(string position)
        {
            DataView view = new DataView(DataTableChampions);
            view.RowFilter = $"ChampionPosition1 = '{position}' OR ChampionPosition2 = '{position}' OR ChampionPosition3 = '{position}'";

            return view;
        }

        public static DataView GetDataViewChampionsBestToWorst()
        {
            /*
             * Deze methode sorteert de rijen in DataTableChampions op de volgende criteria:
                ▪ Op het jaar dat ze zijn uitgekomen.Meest recent naar oud.
                ▪ Vervolgens op hoeveel posities een champion heeft. Meer posities naar minder.
                ▪ Tot slot op de alfabetische volgorde van de naam.
            **De tweede sort criteria word niet uitgevoerd ok
            */
            DataView sortedView = new DataView(DataTableChampions);
            
            sortedView.Sort = "ReleaseYear DESC, " +
                "ChampionName ASC";

            return sortedView;
        }

        public static Champion GetRandomChampionByPosition(string position)
        {
            //Get dataview, filter by position
            DataView dView = GetDataViewChampionsByPosition(position);

            //Select a random row uit of table
            int index = r.Next(0, dView.Count);
            DataRowView row = dView[index];

            //Retrieve all positions (for filling an attribute of the champion class)
            List<string> positions = new List<string>();
            if (!string.IsNullOrEmpty(Convert.ToString(row["ChampionPosition1"])))
            {
                positions.Add(Convert.ToString(row["ChampionPosition1"]));
            }
            if (!string.IsNullOrEmpty(Convert.ToString(row["ChampionPosition2"])))
            {
                positions.Add(Convert.ToString(row["ChampionPosition2"]));
            }
            if (!string.IsNullOrEmpty(Convert.ToString(row["ChampionPosition3"])))
            {
                positions.Add(Convert.ToString(row["ChampionPosition3"]));
            }


            //Create Champion object
            Champion champ = new Champion
            (
                Convert.ToString(row["ChampionName"]),
                Convert.ToString(row["ChampionTitle"]),
                Convert.ToString(row["ChampionClass"]),
                Convert.ToInt16(row["ReleaseYear"].ToString()),
                AbilityData.GetAbilitiesByChampionName((string)row["ChampionName"]),
                positions,
                Convert.ToString(row["ChampionIcon"]),
                Convert.ToString(row["ChampionBanner"]),
                Convert.ToInt16(row["ChampionIPCost"]),
                Convert.ToInt16(row["ChampionRPCost"])
             );


            return champ;
        }
    }
}
