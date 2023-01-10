using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Spline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Tower_defense_game.BaseClasses;
using Tower_defense_game.Enums;
using Tower_defense_game.Managers;
using Tower_defense_game.Other;

namespace Tower_defense_game.GameObjects
{
    public class AOETower : Tower
    {
        // Float
        float ProjectileHitBoxRad = 17.0f;
        float ProjectileSpeed = 4.3f;

        // Other
        int ProjectileAmount = 8;
        List<Projectile> Projectiles;

        public AOETower() : base()
        {
        }

        public AOETower(Texture2D tex, Rectangle destRec, Rectangle? sourceRec, float drawLayer, bool isDeactivated = false) : base(tex, destRec, sourceRec, drawLayer, isDeactivated)
        {
            Damage = 1;
            Range = 100;
            AttackDelayTime = 2.5f;
            SetConstants();
        }

        public AOETower(Texture2D tex, Rectangle destRec, Rectangle? sourceRec, float drawLayer, int damage, int range, float attackDelay) : base(tex, destRec, sourceRec, drawLayer)
        {
            Damage = damage;
            Range = range;
            AttackDelayTime = attackDelay;
            SetConstants();
        }

        public override void Update(float? deltaTime)
        {
            base.Update(deltaTime);

            if (!Deactivated)
                if (Projectiles.Count > 0)
                {
                    if (Projectiles.Any(p => !p.IsActive))
                        Projectiles.RemoveAll(p => !p.IsActive);

                    for (int x = 0; x < Projectiles.Count; x++)
                        Projectiles[x].Update(deltaTime.Value);
                }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            for (int x = 0; x < Projectiles.Count; x++)
                Projectiles[x].Draw(spriteBatch);
        }

        public void SetConstants()
        {
            CurrentColor = ColorManager.AOETower;
            AttackDelay = new();
            Emitter = ParticleSystem.CreateEmitter(GetTruePos());
            CurrentTargets = new Enemy[1];
            Projectiles = new();
        }

        public override void TowerLogic()
        {
            if (EnemyManager.GetEnemies().Any(e => IsInRange(e) && ValidTargets.Contains(e.GetEnemyType())))
                EnemiesInRange = EnemyManager.GetEnemies().FindAll(e => IsInRange(e) && ValidTargets.Contains(e.GetEnemyType())).OrderBy(e => Vector2.Distance(e.GetVectorPos(), GetTruePos())).ToList();

            else
                EnemiesInRange.Clear();

            if (AttackDelay.IsDone() && EnemiesInRange.Count > 0)
            {
                for (int x = 0; x < ProjectileAmount; x++)
                {
                    float throwAngle = (float)(Math.PI * 2 / ProjectileAmount * x);
                    Vector2 vel = new(MathF.Cos(throwAngle), MathF.Sin(throwAngle));

                    Projectile projectile = new(GetTruePos(), vel * ProjectileSpeed, Range, ProjectileHitBoxRad);
                    projectile.ApplyParticle(Emitter.CreateAxe(vel * ProjectileSpeed, Range));
                    Projectiles.Add(projectile);
                }

                AttackDelay.StartTimer(AttackDelayTime);
            }
        }

        class Projectile
        {
            // Vector2
            Vector2 Pos;
            Vector2 Vel;
            Vector2 StartPos;

            // Float
            float Range;
            float HitBoxRadius;

            // Other
            public bool IsActive = true;
            Particle ParticleEffect;
            int Damage = 1;

            public Projectile(Vector2 start, Vector2 vel, float range, float hitBoxRadius)
            {
                StartPos = start;
                Vel = vel;
                Range = range;
                Pos = StartPos;
                HitBoxRadius = hitBoxRadius;
            }

            public void Update(float deltaTime)
            {
                if (IsActive)
                {
                    Pos += Vel;

                    ParticleEffect.Update(deltaTime);

                    if (EnemyManager.GetEnemies().Any(e => Vector2.Distance(e.GetVectorPos(), Pos) <= e.GetRadius() + HitBoxRadius))
                    {
                        Enemy enemy = EnemyManager.GetEnemies().FirstOrDefault(e => Vector2.Distance(e.GetVectorPos(), Pos) <= e.GetRadius() + HitBoxRadius);
                        enemy.Hurt(Damage);
                        IsActive = false;
                    }

                    if (Vector2.Distance(StartPos, Pos) >= Range)
                        IsActive = false;
                }
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                if (IsActive)
                    ParticleEffect.Draw(spriteBatch);
                    
            }

            public void ApplyParticle(Particle particle)
            {
                ParticleEffect = particle;
            }
        }
    }
}
