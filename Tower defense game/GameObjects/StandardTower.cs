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
using Tower_defense_game.Managers;
using Tower_defense_game.Other;

namespace Tower_defense_game.GameObjects
{
    public class StandardTower : Tower
    {
        public StandardTower() : base()
        {

        }

        public StandardTower(Texture2D tex, Rectangle destRec, Rectangle? sourceRec, float drawLayer, bool isDeactivated = false) : base(tex, destRec, sourceRec, drawLayer, isDeactivated)
        {
            Damage = 1;
            Range = 100;
            SetConstants();
        }

        public StandardTower(Texture2D tex, Rectangle destRec, Rectangle? sourceRec, float drawLayer, int damage, int range, float attackDelay) : base(tex, destRec, sourceRec, drawLayer)
        {
            Damage = damage;
            Range = range;
            AttackDelayTime = attackDelay;
            SetConstants();
        }

        public void SetConstants()
        {
            CurrentColor = ColorManager.StandardTower;
            AttackDelay = new();
            Emitter = ParticleSystem.CreateEmitter(GetTruePos());
        }
    }
}
