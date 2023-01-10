using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tower_defense_game.BaseClasses;
using Tower_defense_game.Enums;

namespace Tower_defense_game.GameObjects
{
    public class Minion : Enemy
    {
        public Minion() : base()
        {
        }

        public Minion(Texture2D tex, Rectangle destRec, Rectangle? sourceRec, float drawLayer, SimplePath path) : base(tex, destRec, sourceRec, drawLayer, path)
        {
            KillReward = 10;
            Health = 1;
            Speed = 1.0f;
        }

        public Minion(Texture2D tex, Rectangle destRec, Rectangle? sourceRec,float drawLayer, SimplePath path,int killReward, int health, float speed) : base(tex, destRec, sourceRec, drawLayer, path)
        {
            KillReward = killReward;
            Health = health;
            Speed = speed;
        }
    }
}
