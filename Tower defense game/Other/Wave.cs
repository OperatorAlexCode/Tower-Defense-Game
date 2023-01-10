using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tower_defense_game.BaseClasses;

namespace Tower_defense_game.Other
{
    public class Wave
    {
        protected List<Snippet> EnemiesToSpawn { get; set; }
        
        public void AddWaveData(List<Snippet> snippets)
        {
            EnemiesToSpawn = snippets;
        }

        public Snippet? GetNextEnemy()
        {
            if (EnemiesToSpawn.Count > 0)
            {
                Snippet output = EnemiesToSpawn[0];
                EnemiesToSpawn.RemoveAt(0);
                return output;
            }
            else
                return null;
        }

        public bool EnemiesLeft()
        {
            return EnemiesToSpawn.Count > 0;
        }

        public Wave Copy()
        {
            return this;
        }
    }

    public class Snippet
    {
        public int Type { get; set; }
        public int? Health { get; set; }
        public float? Speed { get; set; }
        public float SpawnDelay { get; set; }
    }
}

    
