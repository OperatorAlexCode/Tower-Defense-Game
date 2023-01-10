using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1;
using Spline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tower_defense_game.BaseClasses;
using Tower_defense_game.Enums;
using Tower_defense_game.GameObjects;
using Tower_defense_game.Other;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;

namespace Tower_defense_game.Managers
{
    static public class EnemyManager
    {
        // Rectangle
        static Rectangle MinionSourceRec;
        static Rectangle RegeneratorsourceRec;

        // Action
        static Action<int> OnDeath;
        static Action<int> OnReachEnd;
        static Action EndWave;
        static Action WinGame;

        // Wave
        static List<Wave> Waves;
        static Wave CurrentWave;

        // Float
        static float DrawLayer;
        static float WaveStartSpawnDelay = 1.5f;

        // int
        static int KillReward = 10;
        static int CurrentWaveIndex;

        // Other
        static Texture2D EnemyTexs;
        static Vector2 EnemyDimensions = new(32, 32);
        static SimplePath Path;
        static Timer SpawnTimer;
        static List<Enemy> SpawnedEnemies = new();
        static Rectangle[] SourceRecs;
       

        static public void Update(float deltaTime)
        {
            UpdateTimers(deltaTime);

            if (SpawnedEnemies.Any(e => e.IsEnemyDead()))
                SpawnedEnemies.RemoveAll(e => e.IsEnemyDead());

            for (int x = 0; x < SpawnedEnemies.Count; x++)
                SpawnedEnemies[x].Update(deltaTime);

            if (SpawnTimer.IsDone())
            {
                Snippet? enemyToSpawn = CurrentWave.GetNextEnemy();
                if (enemyToSpawn != null)
                {
                    SpawnEnemy(enemyToSpawn.Type, enemyToSpawn.Health, enemyToSpawn.Speed);
                    SpawnTimer.StartTimer(enemyToSpawn.SpawnDelay);
                }
                else
                    SpawnTimer.StartTimer(float.PositiveInfinity);
            }

            if (CurrentWave != null)
                if (!CurrentWave.EnemiesLeft() && SpawnedEnemies.Count == 0)
                {
                    if (CurrentWaveIndex+1 >= Waves.Count)
                    {
                        WinGame.Invoke();
                        CurrentWave = null;
                    }

                    else
                    {
                        CurrentWave = null;
                        EndWave.Invoke();
                    }
                }       
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < SpawnedEnemies.Count; x++)
                SpawnedEnemies[x].Draw(spriteBatch);
        }

        static public void InstantializeFields(Texture2D tex, float drawLayer, Action<int> onDeath, Action<int> onReachEnd, Action endWave, Action winGame)
        {
            SpawnedEnemies = new();
            DrawLayer = drawLayer;
            EnemyTexs = tex;
            OnDeath = onDeath;
            OnReachEnd = onReachEnd;
            WinGame = winGame;
            EndWave = endWave;
            SetConstants();
        }

        static void SetConstants()
        {
            SpawnTimer = new();
            SpawnTimer.StartTimer(float.PositiveInfinity);
        }

        static void UpdateTimers(float deltaTime)
        {
            SpawnTimer.Update(deltaTime);
        }

        static public void SetPath(SimplePath path)
        {
            Path = path;
        }

        static public void SetSourceRecs(Rectangle[] recs)
        {
            SourceRecs = recs;
        }

        static public void SpawnEnemy(Enemy enemyToSpawn)
        {
            enemyToSpawn.SetFunctions(OnDeath, OnReachEnd);
            SpawnedEnemies.Add(enemyToSpawn);
        }

        static public void SpawnEnemy(int type)
        {
            Enemy? newEnemy = null;
            Rectangle destRec = new();
            destRec = new((int)(Path.GetPos(Path.beginT).X - EnemyDimensions.X / 2), (int)(Path.GetPos(Path.beginT).Y - EnemyDimensions.Y / 2), (int)EnemyDimensions.X, (int)EnemyDimensions.Y);
            Rectangle sourceRec = SourceRecs[type];

            switch (type)
            {
                // Minion
                case 0:
                    newEnemy = new Minion(EnemyTexs, destRec, sourceRec, DrawLayer, Path, KillReward, 1, 1);
                    break;
                // Regenerator
                case 1:
                    newEnemy = new Regenerator(EnemyTexs, destRec, sourceRec, DrawLayer, Path, KillReward, 1, 1);
                    break;
            }

            if (newEnemy != null)
            {
                newEnemy.SetFunctions(OnDeath, OnReachEnd);
                SpawnedEnemies.Add(newEnemy);
            }
        }

        static public void SpawnEnemy(int type, int? health, float? speed)
        {
            Enemy? newEnemy = null;
            Rectangle destRec = new();
            destRec = new((int)(Path.GetPos(Path.beginT).X - EnemyDimensions.X / 2), (int)(Path.GetPos(Path.beginT).Y - EnemyDimensions.Y / 2), (int)EnemyDimensions.X, (int)EnemyDimensions.Y);
            Rectangle sourceRec = SourceRecs[type];

            int healthCache = 1;
            float speedCache = 1;

            if (health.HasValue)
                healthCache = health.Value;

            if (speed.HasValue)
                speedCache = speed.Value;

            switch (type)
            {
                // Minion
                case 0:
                    newEnemy = new Minion(EnemyTexs, destRec, sourceRec, DrawLayer, Path, KillReward, healthCache, speedCache);
                    break;
                // Regenerator
                case 1:
                    newEnemy = new Regenerator(EnemyTexs, destRec, sourceRec, DrawLayer, Path, KillReward, healthCache, speedCache);
                    break;
            }

            if (newEnemy != null)
            {
                newEnemy.SetFunctions(OnDeath, OnReachEnd);
                SpawnedEnemies.Add(newEnemy);
            }
        }

        static public bool IsEmpty()
        {
            return SpawnedEnemies.Count == 0;
        }

        static public void Clear()
        {
            SpawnedEnemies.Clear();
        }

        static public List<Enemy> GetEnemies()
        {
            return SpawnedEnemies;
        }

        /// <summary>
        /// Starts a Specific Wave
        /// </summary>
        /// <param name="waveIndex">Index of the wave to spawn</param>
        static public void StartWave(int waveIndex, out GameState? newState)
        {
            newState = null;
            if (waveIndex >= Waves.Count)
                newState = GameState.GameWin;
            else
            {
                CurrentWave = Waves[waveIndex];
                SpawnTimer.StartTimer(WaveStartSpawnDelay);
                CurrentWaveIndex = waveIndex;
            }
        }

        static public void SetWaves(List<Wave> waves)
        {
            Waves = waves;
        }

        static public void SetCurrentWaveIndex(int currentWave)
        {
            CurrentWaveIndex = currentWave;
        }
    }
}
