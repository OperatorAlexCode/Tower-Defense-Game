using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.X3DAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;
using System.Reflection.Metadata;
using Tower_defense_game.Managers;

namespace Tower_defense_game.Other
{
    static public class ParticleSystem
    {
        static internal float DrawLayer;
        static SpriteBatch SpriteBatch;
        static internal Texture2D Axe;

        static public void InitializeFields(SpriteBatch spriteBatch, float drawLayer, Texture2D axeTex)
        {
            SpriteBatch = spriteBatch;
            DrawLayer = drawLayer;
            Axe = axeTex;
        }

        static public Emitter CreateEmitter(Vector2 pos)
        {
            return new Emitter(SpriteBatch, pos);
        }
    }

    public class Emitter
    {
        // Float
        float LaserLifeTime = 0.1f;
        float Rotation;
        /// <summary>
        /// Margin for how big the margin for the cone where the particles eject from, originating from the rotation of emitter
        /// </summary>
        float ParticleConeMargin;
        float AxeScale = 1.2f;
        float AxeRotationSpeed = (float)(Math.PI / 3);

        // Texture2D
        Texture2D PixelTex;
        Texture2D AxeTex;

        // Vector2
        Vector2 Pos;
        Vector2 PORAxe;
        Vector2 PORLaser;

        // Other
        static List<Particle> Particles = new();
        SpriteBatch SpriteBatch;
        bool IsActive = true;
        int LaserHeight = 4;

        public Emitter(SpriteBatch spriteBatch, Vector2 pos)
        {
            SpriteBatch = spriteBatch;
            Pos = pos;
            SetConstants();
        }

        public void Update(float deltaTime)
        {
            if (Particles.Count > 0)
            {
                if (Particles.Any(p => p.LifeTime.IsDone()))
                    Particles.RemoveAll(p => p.LifeTime.IsDone());

                for (int x = 0; x < Particles.Count; x++)
                    Particles[x].Update(deltaTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Particles.Count > 0)
                for (int x = 0; x < Particles.Count; x++)
                    Particles[x].Draw(spriteBatch);
        }

        void SetConstants()
        {
            PixelTex = new(SpriteBatch.GraphicsDevice, 2, 2);
            PixelTex.SetData(new[] { Color.White, Color.White, Color.White, Color.White });
            AxeTex = ParticleSystem.Axe;
            PORAxe = new(4, AxeTex.Height-2);
            PORLaser = new(0, LaserHeight / 2);
        }

        public void Laser(Vector2 end)
        {
            float length = Vector2.Distance(Pos, end);
            float y = end.Y - Pos.Y;
            float angle;

            if (end.Y < Pos.Y)
            {
                y *= -1;
                if (end.X < Pos.X)
                    angle = MathF.PI + MathF.Asin(y / length);
                else
                    angle = MathF.PI * 2 - MathF.Asin(y / length);
            }
            else
            {
                if (end.X < Pos.X)
                    angle = MathF.PI - MathF.Asin(y / length);
                else
                    angle = MathF.Asin(y / length);
            }

            Rectangle destRec = new((int)Pos.X, (int)(Pos.Y - LaserHeight / 2), (int)length, LaserHeight);

            Particle newLaser = new(PixelTex, destRec, LaserLifeTime, Pos, Color.Red, angle, PORLaser);
            Particles.Add(newLaser);
        }

        public void Laser(Vector2 end, float lifeTime)
        {
            float length = Vector2.Distance(Pos, end);
            float y = end.Y - Pos.Y;
            float angle;

            if (end.Y < Pos.Y)
            {
                y *= -1;
                if (end.X < Pos.X)
                    angle = MathF.PI + MathF.Asin(y / length);
                else
                    angle = MathF.PI * 2 - MathF.Asin(y / length);
            }
            else
            {
                if (end.X < Pos.X)
                    angle = MathF.PI - MathF.Asin(y / length);
                else
                    angle = MathF.Asin(y / length);
            }

            Rectangle destRec = new((int)Pos.X, (int)(Pos.Y - LaserHeight / 2), (int)length, LaserHeight);

            Particle newLaser = new(PixelTex, destRec, lifeTime, Pos, ColorManager.Laser, angle, PORLaser);
            Particles.Add(newLaser);
        }

        public void Axe(Vector2 vel)
        {
            int axeHeight = (int)(AxeTex.Height * AxeScale);
            int axeWidth = (int)(AxeTex.Width * AxeScale);

            Rectangle destRec = new((int)(Pos.X), (int)(Pos.Y), axeWidth, axeHeight);

            Particle newAxe = new(AxeTex, destRec, float.PositiveInfinity, Pos, Color.White, 0.0f, PORAxe, AxeRotationSpeed);
            newAxe.SetVel(vel);
            Particles.Add(newAxe);
        }

        public Particle CreateAxe(Vector2 vel, float range = float.PositiveInfinity)
        {
            int axeHeight = (int)(AxeTex.Height * AxeScale);
            int axeWidth = (int)(AxeTex.Width * AxeScale);

            Rectangle destRec = new((int)(Pos.X/*- PORAxe.X/AxeTex.Width*axeWidth*/), (int)(Pos.Y/* - PORAxe.Y / AxeTex.Height * axeHeight*/), axeWidth, axeHeight);

            Particle newAxe = new(AxeTex, destRec, range, Pos, Color.White, 0.0f, PORAxe, AxeRotationSpeed);
            newAxe.SetVel(vel);
            return newAxe;
        }

        public void TurnOnOff(bool? setting = null)
        {
            if (setting.HasValue)
                IsActive = setting.Value;
            else
                IsActive = !IsActive;
        }

        public Vector2 GetPos()
        {
            return Pos;
        }

        public void SetPos(Vector2 newPos)
        {
            Pos = newPos;
        }
    }

    public class Particle
    {
        // Rectangle
        Rectangle DestRec;
        Rectangle? SourceRec;

        // Float
        float DrawLayer;
        float Rotation;
        float RotationChange;
        float MaxDistance;

        // Vector2
        Vector2 Pos;
        Vector2 Vel;
        Vector2 PointOfRotation;
        Vector2 StartPos;

        // Other
        Texture2D Tex;
        Color DrawColor = Color.White;
        public Timer LifeTime;
        bool DistanceBased;

        public Particle(Texture2D tex, Rectangle destRec, float lifeTime, Vector2 startPos)
        {
            Tex = tex;
            DestRec = destRec;
            LifeTime = new();
            LifeTime.StartTimer(lifeTime);
            StartPos = startPos;
            Pos = StartPos;
            SetConstants();
        }

        public Particle(Texture2D tex, Rectangle destRec, Rectangle sourceRec, float lifeTime, Vector2 startPos)
        {
            Tex = tex;
            DestRec = destRec;
            SourceRec = sourceRec;
            LifeTime = new();
            LifeTime.StartTimer(lifeTime);
            StartPos = startPos;
            Pos = StartPos;
            SetConstants();
        }

        public Particle(Texture2D tex, Rectangle destRec, float lifeTime, Vector2 startPos, Color color, float rotation)
        {
            Tex = tex;
            DestRec = destRec;
            LifeTime = new();
            LifeTime.StartTimer(lifeTime);
            StartPos = startPos;
            Pos = StartPos;
            DrawColor = color;
            Rotation = rotation;
            SetConstants();
        }

        public Particle(Texture2D tex, Rectangle destRec, float lifeTime, Vector2 startPos, Color color, float rotation, Vector2 pointOfRotation)
        {
            Tex = tex;
            DestRec = destRec;
            LifeTime = new();
            LifeTime.StartTimer(lifeTime);
            StartPos = startPos;
            Pos = StartPos;
            DrawColor = color;
            Rotation = rotation;
            PointOfRotation = pointOfRotation;
            SetConstants();
        }

        public Particle(Texture2D tex, Rectangle destRec, float lifeTime, Vector2 startPos, Color color, float rotation, Vector2 pointOfRotation, float rotationChange)
        {
            Tex = tex;
            DestRec = destRec;
            LifeTime = new();
            LifeTime.StartTimer(lifeTime);
            StartPos = startPos;
            Pos = StartPos;
            DrawColor = color;
            Rotation = rotation;
            PointOfRotation = pointOfRotation;
            RotationChange = rotationChange;
            SetConstants();
        }

        public void Update(float deltaTime)
        {
            LifeTime.Update(deltaTime);

            if (!LifeTime.IsDone())
            {
                Pos += Vel;
                DestRec.X += (int)Vel.X;
                DestRec.Y += (int)Vel.Y;
                Rotation += RotationChange;

                if (DistanceBased && Vector2.Distance(StartPos, Pos) >= MaxDistance)
                    LifeTime.StartTimer(0);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!LifeTime.IsDone())
            {
                if (SourceRec.HasValue)
                    spriteBatch.Draw(Tex, DestRec, SourceRec.Value, DrawColor, Rotation, PointOfRotation, SpriteEffects.None, DrawLayer);

                else
                    spriteBatch.Draw(Tex, DestRec, null, DrawColor, Rotation, PointOfRotation, SpriteEffects.None, DrawLayer);
            }
        }

        public void SetColor(Color newColor)
        {
            DrawColor = newColor;
        }

        public void SetVel(Vector2 newVel)
        {
            Vel = newVel;
        }

        public void SetPos(Vector2 newPos)
        {
            Pos = newPos;
        }

        public void UseDistance(float maxDistance)
        {
            DistanceBased = true;
            LifeTime.StartTimer(float.PositiveInfinity);
            MaxDistance = maxDistance;
        }

        public void SetConstants()
        {
            DrawLayer = ParticleSystem.DrawLayer;

        }
    }
}
