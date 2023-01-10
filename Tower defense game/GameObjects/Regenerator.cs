using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation;
using Spline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tower_defense_game.BaseClasses;
using Tower_defense_game.Other;
using Timer = Tower_defense_game.Other.Timer;

namespace Tower_defense_game.GameObjects
{
    public class Regenerator : Enemy
    {
        float RegenDelay = 2.0f;
        int MaxHealth;
        Timer RegenTimer;

        public Regenerator(Texture2D tex, Rectangle destRec, Rectangle? sourceRec, float drawLayer, SimplePath path) : base(tex, destRec, sourceRec, drawLayer, path)
        {
            KillReward = 10;
            Health = 1;
            Speed = 1.0f;
            SetConstants();
        }

        public Regenerator(Texture2D tex, Rectangle destRec, Rectangle? sourceRec, float drawLayer, SimplePath path, int killReward, int health, float speed) : base(tex, destRec, sourceRec, drawLayer, path)
        {
            KillReward = killReward;
            Health = health;
            Speed = speed;
            SetConstants();
        }

        void SetConstants()
        {
            MaxHealth = Health;
            RegenTimer = new();
        }

        public override void EnemyLogic(float? deltaTime)
        {
            base.EnemyLogic(deltaTime);

            if (deltaTime.HasValue)
                RegenTimer.Update(deltaTime.Value);

            if (RegenTimer.IsDone() && Health < MaxHealth)
            {
                Health++;
                RegenTimer.StartTimer(RegenDelay);
            }
        }

        public override void Hurt(int damage)
        {
            base.Hurt(damage);
            RegenTimer.StartTimer(RegenDelay);
        }
    }
}
