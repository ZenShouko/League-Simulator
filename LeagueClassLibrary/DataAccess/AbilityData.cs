using LeagueClassLibrary.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LeagueClassLibrary.DataAccess
{
    public static class AbilityData
    {
        private static List<Ability> Abilities;

        public static void LoadCSV(string padNaarCsv)
        {
            //Initializeer
            Abilities = new List<Ability>();

            //Lees
            using (StreamReader reader = new StreamReader(padNaarCsv))
            {
                //Discard headers
                _ = reader.ReadLine();

                //Add all abilities to the list
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    //Error prevention
                    if (string.IsNullOrEmpty(line)) { continue; } 
                    
                    string[] parts = line.Split(';');

                    Ability a = new Ability(Convert.ToInt16(parts[0]), parts[1], parts[2]);
                    Abilities.Add(a);
                }
            }
        }

        public static List<Ability> GetAbilitiesByChampionName(string championName)
        {
            if (Abilities.Count == 0)
            {
                return null;
            }

            List<Ability> abilities = Abilities.FindAll(a => a.ChampionName == championName).ToList();
            return abilities;
        }
    }
}
