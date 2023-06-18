using LeagueClassLibrary.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueClassLibrary.Entities
{
    public class SummonersRift : Match
    {
        public SummonersRift(string code) : base(code)
        {
            this.Code = code;

            //Initialize teams
            Team1Champions = new List<Champion>();
            Team2Champions = new List<Champion>();
        }

        public override void GenereerTeams()
        {
            string[] poss = { "sup", "mid", "jung", "bot", "top" };

            //Vul lijsten
            foreach (string pos in poss)
            {
                Team1Champions.Add(ChampionData.GetRandomChampionByPosition(pos));
                Team2Champions.Add(ChampionData.GetRandomChampionByPosition(pos));
            }
        }
    }
}
