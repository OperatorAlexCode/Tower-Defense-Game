using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Spline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tower_defense_game.Enums;
using Tower_defense_game.Managers;
using Tower_defense_game.Other;

namespace Tower_defense_game.BaseClasses
{
    public class Enemy
    {
        // int
        protected int Health = 1;
        public int KillReward = 1;
        public int PlayerHealthLoss = 1;
        int HealthDecreaseReward = 1;

        // Float
        protected float DrawLayer;
        protected float Speed;
        protected float Pos;
        protected float Radius;

        // Rectangle
        public Rectangle DestRec;
        protected Rectangle? SourceRec;

        // Color
        protected Color DrawColor;
        protected Color[] ColorSchema;

        // Action
        protected Action<int> AddPlayerCurrency;
        protected Action<int> OnReachEnd;

        // Other
        protected Texture2D Tex;
        protected EnemyType Type = EnemyType.Normal;
        protected bool IsDead;
        protected SimplePath Path;

        public Enemy()
        {

        }

        public Enemy(Texture2D tex, Rectangle destRec, Rectangle? sourceRec,float drawLayer,SimplePath path)
        {
            Tex = tex;
            DestRec = destRec;
            SourceRec = sourceRec;
            DrawLayer = drawLayer;
            Path = path;
            ColorSchema = ColorManager.EnemyHealthSchema;
        }

        public void Update(float? deltaTime)
        {
            if (!IsDead)
            {
                EnemyLogic(deltaTime);
                UpdateDestRec();

                if (Pos >= Path.endT)
                {
                    OnReachEnd.Invoke(PlayerHealthLoss);
                    IsDead = true;
                }   
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!IsDead)
            {
                if (Health >= ColorSchema.Length && DrawColor != ColorSchema[ColorSchema.Length - 1])
                    DrawColor = ColorSchema[ColorSchema.Length - 1];

                else if (DrawColor != ColorSchema[Health - 1])
                    DrawColor = ColorSchema[Health - 1];

                if (SourceRec.HasValue)
                    spriteBatch.Draw(Tex, DestRec, SourceRec.Value, DrawColor, 0.0f, Vector2.Zero, SpriteEffects.None, DrawLayer);
                else
                    spriteBatch.Draw(Tex, DestRec, null, DrawColor, 0.0f, Vector2.Zero, SpriteEffects.None, DrawLayer);
            }  
        }

        public virtual void EnemyLogic(float? deltaTime)
        {
            Move();
        }

        public void SetFunctions(Action<int> onDeath, Action<int> onReachEnd)
        {
            AddPlayerCurrency = onDeath;
            OnReachEnd = onReachEnd;
        }

        public virtual void Hurt(int damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                IsDead = true;
                AddPlayerCurrency.Invoke(KillReward);
            } 
            else
                AddPlayerCurrency.Invoke(HealthDecreaseReward);
        }

        public EnemyType GetEnemyType()
        {
            return Type;
        }

        public bool IsEnemyDead()
        {
            return IsDead;
        }

        public void SetColor(Color newColor)
        {
            DrawColor = newColor;
        }
        
        public float GetPos()
        {
            return Pos;
        }

        public Vector2 GetVectorPos()
        {
            return Path.GetPos(Pos);
        }

        public float GetRadius()
        {
            return Radius;
        }

        protected void UpdateDestRec()
        {
            DestRec.X = (int)Path.GetPos(Pos).X - DestRec.Width / 2;
            DestRec.Y = (int)Path.GetPos(Pos).Y - DestRec.Height / 2;
        }

        public virtual void Move()
        {
            if (Pos + Speed >= Path.endT)
                Pos = Path.endT;
            else
                Pos += Speed;
        }
    }
}
