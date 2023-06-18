using System;
using System.Collections.Generic;
using System.Linq;

namespace LeagueClassLibrary.Entities
{
    public abstract class Match : IWinnable
    {
        public List<Champion> Team1Champions { get; set; }
        public List<Champion> Team2Champions { get; set; }
        public string Code { get; set; }
        public int Winner { get; set; }

        public Match(string code)
        {
            
        }

        public abstract void GenereerTeams();

        public void DecideWinner()
        {
            //Do we have a team?
            if (Team1Champions.Count == 0 || Team2Champions.Count == 0)
            {
                throw new Exception("Can not decide winner if teams are empty.");
            }

            //Get average of both teams
            double team1Average = Team1Champions.Average(a => a.ReleaseYear);
            double team2Average = Team2Champions.Average(a => a.ReleaseYear);

            if (team1Average > team2Average)
            {
                Winner = 1;
            }
            else if (team2Average > team1Average)
            {
                Winner = 2;
            }
            else
            {
                int assasinCount1 = Team1Champions.Count(a => a.Class.ToUpper() == "ASSASSIN");
                int assasinCount2 = Team2Champions.Count(a => a.Class.ToUpper() == "ASSASSIN");

                if (assasinCount1 < assasinCount2)
                {
                    Winner = 2;
                }
                else
                {
                    Winner = 1;
                }
            }
        }
    }
}
