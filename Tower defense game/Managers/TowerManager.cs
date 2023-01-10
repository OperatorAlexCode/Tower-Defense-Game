using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tower_defense_game.BaseClasses;
using Tower_defense_game.GameObjects;

namespace Tower_defense_game.Managers
{
    static public class TowerManager
    {
        // Float
        static float DrawLayer;
        static public readonly float TowerSellValue = 0.5f;
        static float StandardTowerAtkDelay = 1.20f;
        static float AOETowerAtkDelay = 2.3f;

        // Int
        static int StandardRange = 140;
        static int StandardDamage = 1;
        static int AOERange = 90;
        static int AOEDamage = 1;

        // Other
        static List<Tower> Towers = new();
        static public readonly Dictionary<Type, int> TowerCostTable = new() {
            { new StandardTower().GetType(), 100 },
            { new AOETower().GetType(), 200 }
        };
        static Texture2D TowerTexs;
        static Rectangle[] SourceRecs;
        static Vector2[] TowerDimensions = new Vector2[] { new(32, 32), new(40, 40) };

        static public void Update(float deltaTime)
        {
            if (Towers.Count > 0)
                for (int x = 0; x < Towers.Count; x++)
                    Towers[x].Update(deltaTime);
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            if (Towers.Count > 0)
                for (int x = 0; x < Towers.Count; x++)
                    Towers[x].Draw(spriteBatch);
        }

        static public void InstantializeFields(Texture2D tex, float drawLayer)
        {
            TowerTexs = tex;
            Towers = new();
            DrawLayer = drawLayer;

            SourceRecs = new Rectangle[] { new(0, 0, TowerTexs.Width, TowerTexs.Height), new(0, 0, TowerTexs.Width, TowerTexs.Height) };
        }

        static public bool IsEmpty()
        {
            return Towers.Count == 0;
        }

        static public void SpawnTower(Tower towerToSpawn)
        {
            Towers.Add(towerToSpawn);
        }

        static public void SpawnTower(int type, Vector2 spawnPos)
        {
            Tower newTower = null;
            Rectangle destRec = new();
            destRec = new((int)(spawnPos.X - TowerDimensions[type].X / 2), (int)(spawnPos.Y - TowerDimensions[type].Y / 2), (int)TowerDimensions[type].X, (int)TowerDimensions[type].Y);
            Rectangle sourceRec = SourceRecs[type];

            switch (type)
            {
                case 0:
                    newTower = new StandardTower(TowerTexs, destRec, sourceRec, DrawLayer, StandardDamage, StandardRange, StandardTowerAtkDelay);
                    break;
                case 1:
                    newTower = new AOETower(TowerTexs, destRec, sourceRec, DrawLayer, AOEDamage, AOERange, AOETowerAtkDelay);
                    break;
            }

            if (newTower != null)
                Towers.Add(newTower);
        }

        static public Tower? CreateTower(int type, Vector2 spawnPos)
        {
            Tower? newTower = null;
            Rectangle destRec = new();
            destRec = new((int)(spawnPos.X - TowerDimensions[type].X / 2), (int)(spawnPos.Y - TowerDimensions[type].Y / 2), (int)TowerDimensions[type].X, (int)TowerDimensions[type].Y);
            Rectangle sourceRec = SourceRecs[type];

            switch (type)
            {
                case 0:
                    newTower = new StandardTower(TowerTexs, destRec, sourceRec, DrawLayer, StandardDamage, StandardRange, StandardTowerAtkDelay);
                    break;
                case 1:
                    newTower = new AOETower(TowerTexs, destRec, sourceRec, DrawLayer, AOEDamage, AOERange, AOETowerAtkDelay);
                    break;
            }

            return newTower;
        }

        static public Tower? CreateTower(int type, Rectangle destRec)
        {
            Tower? newTower = null;
            Rectangle sourceRec = SourceRecs[type];

            switch (type)
            {
                case 0:
                    newTower = new StandardTower(TowerTexs, destRec, sourceRec, DrawLayer, StandardDamage, StandardRange, StandardTowerAtkDelay);
                    break;
                case 1:
                    newTower = new AOETower(TowerTexs, destRec, sourceRec, DrawLayer, AOEDamage, AOERange, AOETowerAtkDelay);
                    break;
            }

            return newTower;
        }

        static public List<Tower> GetTowers()
        {
            return Towers;
        }

        static public void SetSourceRecs(Rectangle[] recs)
        {
            SourceRecs = recs;
        }

        static public void DestroyTower(Tower towerToDestroy)
        {
            Towers.Remove(towerToDestroy);
        }

        static public void Clear()
        {
            Towers.Clear();
        }
    }
}
