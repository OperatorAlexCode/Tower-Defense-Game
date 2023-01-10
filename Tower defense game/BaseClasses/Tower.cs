using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Tower_defense_game.Enums;
using Tower_defense_game.Managers;
using Tower_defense_game.Other;
using Timer = Tower_defense_game.Other.Timer;

namespace Tower_defense_game.BaseClasses
{
    public class Tower
    {
        // Int
        protected int Range = 1;
        protected int Damage = 50;

        // Float
        protected float DrawLayer;
        protected float AttackDelayTime;

        // Enemy
        protected Enemy[] CurrentTargets;
        protected List<Enemy> EnemiesInRange = new();

        // Rectangle
        public Rectangle DestRec;
        protected Rectangle? SourceRec;

        // Other
        protected Texture2D Tex;
        protected EnemyType[] ValidTargets = { EnemyType.Normal };
        protected Timer AttackDelay;
        protected Color CurrentColor;
        protected bool Deactivated;
        protected Emitter Emitter;

        public Tower()
        {
        }

        public Tower(Texture2D tex, Rectangle destRec, Rectangle? sourceRec, float drawLayer, bool isDummy = false)
        {
            Tex = tex;
            DestRec = destRec;
            SourceRec = sourceRec;
            DrawLayer = drawLayer;
            CurrentTargets = new Enemy[1];
        }

        public virtual void Update(float? deltaTime)
        {
            if (!Deactivated)
            {
                if (Emitter.GetPos() != GetTruePos())
                    Emitter.SetPos(GetTruePos());

                if (deltaTime.HasValue)
                {
                    UpdateTimers(deltaTime.Value);
                    Emitter.Update(deltaTime.Value);
                }

                TowerLogic();
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Tex, DestRec, SourceRec ?? null, CurrentColor, 0.0f, Vector2.Zero, SpriteEffects.None, DrawLayer);
            Emitter.Draw(spriteBatch);
        }

        public Texture2D GetTexture()
        {
            return Tex;
        }

        protected void UpdateTimers(float deltaTime)
        {
            AttackDelay.Update(deltaTime);
        }

        /// <summary>
        /// Does logic pertaining how the tower works
        /// </summary>
        public virtual void TowerLogic()
        {
            if (EnemyManager.GetEnemies().Any(e => IsInRange(e) && ValidTargets.Contains(e.GetEnemyType())))
                EnemiesInRange = EnemyManager.GetEnemies().FindAll(e => IsInRange(e) && ValidTargets.Contains(e.GetEnemyType())).OrderBy(e => e.GetPos()).Reverse().ToList();

            if (CurrentTargets[0] == null && EnemiesInRange.Count > 0)
                CurrentTargets[0] = EnemiesInRange[0];

            if (AttackDelay.IsDone() && CurrentTargets[0] != null && !CurrentTargets[0].IsEnemyDead())
            {
                CurrentTargets[0].Hurt(Damage);
                Emitter.Laser(CurrentTargets[0].GetVectorPos());
                AttackDelay.StartTimer(AttackDelayTime);
            }

            if (CurrentTargets[0] != null)
                if (CurrentTargets[0].IsEnemyDead())
                    CurrentTargets[0] = null;
        }

        public bool IsInRange(Enemy enemy)
        {
            float dist = Vector2.Distance(GetTruePos(), enemy.GetVectorPos());

            return dist <= Range;
        }

        /// <summary>
        /// Returns the true position of Tower
        /// </summary>
        /// <returns>Vector2 corresponding to center of destrec</returns>
        public Vector2 GetTruePos()
        {
            Vector2 temp1 = DestRec.Center.ToVector2();
            Vector2 temp2 = new(DestRec.X + DestRec.Width / 2, DestRec.Y + DestRec.Height / 2);

            return temp2;
        }

        public void ActivateDeactivate()
        {
            Deactivated = !Deactivated;
        }

        public void ActivateDeactivate(bool manualSet)
        {
            Deactivated = manualSet;
        }

        public void SetPos(Vector2 newPos)
        {
            DestRec.X = (int)newPos.X - DestRec.Width / 2;
            DestRec.Y = (int)newPos.Y - DestRec.Height / 2;
        }

        public void SetColor(Color newColor)
        {
            CurrentColor = newColor;
        }

        public Color GetColor()
        {
            return CurrentColor;
        }
    }
}
