using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tower_defense_game.Managers
{
    static public class ColorManager
    {
        static public Color MainMenu { get; private set; } = Color.CornflowerBlue;
        static public Color Text { get; private set; } = Color.Black;
        static public Color Laser { get; private set; } = Color.Red;
        static public Color StandardTower { get; private set; } = Color.Gray;
        static public Color AOETower { get; private set; } = Color.Orange;
        static public Color BackGround { get; private set; } = Color.ForestGreen;
        static public Color Path { get; private set; } = Color.SandyBrown;
        static public Color PathEnds { get; private set; } = Color.Purple;
        static public Color CanBuy { get; private set; } = new(Color.White, 1.0f);
        static public Color CantBuy { get; private set; } = new(Color.White, 0.4f);
        static public Color[] EnemyHealthSchema { get; private set; } = new Color[] { Color.Red, Color.Blue, Color.Green, Color.Yellow, Color.DeepPink }; 
    }
}
