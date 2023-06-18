using LeagueClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows;
using System.Xml.Linq;

namespace LeagueClassLibrary.DataAccess
{
    public static class MatchData
    {
        public static DataTable DataTableMatches { get; set; }

        public static void InitializeDataTableMatches() 
        { 
            DataTableMatches = new DataTable();
            DataTableMatches.Columns.Add("Id", typeof(int));
            DataTableMatches.Columns.Add("Code", typeof(string));
            DataTableMatches.Columns.Add("Winner", typeof (string));

            //Auto increment
            DataTableMatches.Columns["Id"].AutoIncrement = true;
            DataTableMatches.Columns["Id"].AutoIncrementSeed = 1;
            DataTableMatches.Columns["Id"].AutoIncrementStep = 1;
        }

        public static void AddFinishedMatch(Match match)
        {
            DataRow row = DataTableMatches.NewRow();
            row["Code"] = match.Code;
            row["Winner"] = match.Winner == 1 ? "Red" : "Blue";


            DataTableMatches.Rows.Add(row);
        }

        public static DataView GetDataViewMatches()
        {
            return new DataView(DataTableMatches);
        }

        public static void ExportToXML(string path)
        {
            DataTableMatches.TableName = "Matches";
            DataTableMatches.WriteXml(path);
        }

        public static bool IsUniqueCode(string code)
        {
            List<DataRow> matches = new List<DataRow>();

            foreach (DataRow row in DataTableMatches.Rows)
            {
                matches.Add(row);
            }

            if (matches.Any(a => (string)a["Code"] == code))
            {
                return false;
            }

            return true;
        }
    }
}
