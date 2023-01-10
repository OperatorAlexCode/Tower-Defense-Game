using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Aseprite.Documents;
using MonoGame.Aseprite.Graphics;
using Tower_defense_game.BaseClasses;
using Tower_defense_game.Enums;
using Spline;
using System;
using System.Runtime.InteropServices;
using System.Linq;
using Tower_defense_game.Other;
using SharpDX.Direct2D1;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;
using System.Net;
using Tower_defense_game.Managers;
using System.Collections.Generic;
using Tower_defense_game.GameObjects;
using Newtonsoft.Json;
using System.IO;
using Microsoft.Xna.Framework.Media;

namespace Tower_defense_game
{
    public class Game1 : Game
    {
        // Int
        int ScreenHeight = 800;
        int ScreenWidth = 800;
        int StartHealth = 5;
        int CurrentHealth;
        int PlayerCurrency;
        int CurrentWave = -1;
        int GameStartCurrencyAmount = 250;
        int RoundEndRewardMin = 49;

        // AsepriteDocument
        AsepriteDocument PathTextures;
        AsepriteDocument EnemyTextures;

        // Vector2
        Vector2 PathDimensions = new(40, 40);
        Vector2[] PathNodes = new Vector2[] {
                new(100, 100),
                new(440, 100),
                new(600, 210),
                new(660, 155),
                new(600, 100),
                new(440, 250),
                new(400, 250),
                new(300, 310),
                new(200, 250),
                new(100, 400),
                new(200, 500),
                new(660, 500),
                new(700, 540),
                new(700, 660),
                new(660, 700),
                new(100, 700),
            };

        // Float
        float DeltaTime;
        float PathTimeDrawInterval = 0.05f;
        #region Layers
        float BackgroundLayer = 0.0f;
        float EnemyLayer = 0.5f;
        float Towerlayer = 0.6f;
        float ParticleLayer = 0.9f;
        float PathLayer = 0.1f;
        float TextLayer = 1.0f;
        #endregion
        float VolumeIncrement = 0.05f;
        float StartVolume = 0.4f;

        // Keys
        Keys[] TowerSelectKeys = { Keys.S, Keys.A };
        Keys[] MusicKeys = { Keys.M, Keys.Up, Keys.Down };

        // Bool
        bool IsKeyPressed;
        bool MouseClick;
        bool IsMusicKeysPressed;

        // Other
        private GraphicsDeviceManager Graphics;
        private SpriteBatch SpriteBatch;
        RenderTarget2D PathTarget;
        GameState CurrentState = GameState.InMenu;
        SimplePath Path;
        Texture2D TowerTex;
        Tower? SelectedTower;
        public Game1()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            InitializeTextures();

            SpriteFont defaultFont = Content.Load<SpriteFont>("DefaultFont");
            SpriteFont titleFont = Content.Load<SpriteFont>("TitleFont");

            Components.Add(new UIManager(this, ScreenHeight, ScreenWidth, StartRestartGame, StartCurrentWave, SetState, defaultFont, titleFont, TowerTex, TextLayer));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            PathMaker.SetGraphicsDevice(GraphicsDevice);

            Path = PathMaker.CreatePath(PathNodes, new int[] { 50, 0, 0, 0, 0, 10, 0, 0, 0, 0, 40, 0, 30, 0, 50 });

            SetScreenDimensions();
            DrawOnRenderTarget();

            #region Initializations for Managers
            EnemyManager.InstantializeFields(EnemyTextures.Texture, EnemyLayer, AddCurrency, LoseHP, EndWave, WinGame);
            EnemyManager.SetSourceRecs(CreateEnemySourceRecs());
            EnemyManager.SetPath(Path);
            TowerManager.InstantializeFields(TowerTex, Towerlayer);
            ParticleSystem.InitializeFields(SpriteBatch, ParticleLayer, Content.Load<AsepriteDocument>("Axe").Texture);
            #endregion

            MediaPlayer.Volume = StartVolume;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(Content.Load<Song>("Testv2"));

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();
            Vector2 mousePos = new(mouse.X, mouse.Y);

            if (keyboard.GetPressedKeyCount() == 0)
                IsKeyPressed = false;

            if (keyboard.GetPressedKeys().All(k => !MusicKeys.Contains(k)))
                IsMusicKeysPressed = false;

            if (mouse.LeftButton == ButtonState.Released)
                MouseClick = false;

            #region Music Controls
            if (keyboard.GetPressedKeys().Any(k => MusicKeys.Contains(k)) && !IsMusicKeysPressed)
            {
                int index = Array.IndexOf(MusicKeys, keyboard.GetPressedKeys().First(k => MusicKeys.Contains(k)));

                switch (index)
                {
                    case 0:
                        StartStopMusic();
                        break;
                    case 1:
                        MediaPlayer.Volume += VolumeIncrement;

                        if (MediaPlayer.Volume > 1.0f)
                            MediaPlayer.Volume = 1.0f;
                        break;
                    case 2:
                        MediaPlayer.Volume -= VolumeIncrement;

                        if (MediaPlayer.Volume < 0.0f)
                            MediaPlayer.Volume = 0.0f;
                        break;
                }

                IsMusicKeysPressed = true;
            }
            #endregion

            switch (CurrentState)
            {
                case GameState.InMenu:
                    break;
                case GameState.InGame:
                    EnemyManager.Update(DeltaTime);
                    TowerManager.Update(DeltaTime);
                    var tempCache = Components[0] as UIManager;
                    UpdateUImanager(tempCache);

                    if (CurrentHealth <= 0)
                        LoseGame();

                    if ((keyboard.GetPressedKeys().Any(k => TowerSelectKeys.Contains(k)) && keyboard.GetPressedKeyCount() == 1 && !IsKeyPressed)
                        || (mouse.RightButton == ButtonState.Pressed && !MouseClick))
                    {
                        if (SelectedTower != null)
                        {
                            SelectedTower = null;
                            MouseClick = true;
                        }

                        else if (!(mouse.RightButton == ButtonState.Pressed && !MouseClick))
                        {
                            Type? towertype = null;
                            int towerIndex = -1;
                            switch (Array.IndexOf(TowerSelectKeys, keyboard.GetPressedKeys()[0]))
                            {
                                case 0:
                                    towertype = new StandardTower().GetType();
                                    towerIndex = 0;
                                    break;
                                case 1:
                                    towertype = new AOETower().GetType();
                                    towerIndex = 1;
                                    break;
                            }

                            if (towertype != null)
                                if (PlayerCurrency - TowerManager.TowerCostTable[towertype] >= 0)
                                {
                                    SelectedTower = TowerManager.CreateTower(towerIndex, mousePos);
                                    SelectedTower.ActivateDeactivate(true);
                                }

                            IsKeyPressed = true;
                        }
                    }

                    else if (mouse.LeftButton == ButtonState.Pressed && !MouseClick && SelectedTower != null)
                    {
                        if (!TowerManager.GetTowers().Any(t => t.DestRec.Intersects(SelectedTower.DestRec)) && CanPlaceTower(SelectedTower))
                        {
                            SelectedTower.ActivateDeactivate();
                            TowerManager.SpawnTower(SelectedTower);
                            PlayerCurrency -= TowerManager.TowerCostTable[SelectedTower.GetType()];
                            SelectedTower = null;
                            MouseClick = true;
                        }
                    }

                    if (mouse.RightButton == ButtonState.Pressed && !MouseClick && TowerManager.GetTowers().Any(t => t.DestRec.Contains(mousePos)))
                    {
                        Tower towerToSell = TowerManager.GetTowers().Find(t => t.DestRec.Contains(mousePos));
                        TowerManager.DestroyTower(towerToSell);
                        PlayerCurrency += (int)(TowerManager.TowerCostTable[towerToSell.GetType()] * TowerManager.TowerSellValue);
                    }

                    SelectedTower?.SetPos(mousePos);

                    break;
                case GameState.GameWin:
                    break;
                case GameState.GameLoss:
                    EnemyManager.Update(DeltaTime);
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (CurrentState == GameState.InMenu)
                GraphicsDevice.Clear(ColorManager.MainMenu);
            else
                GraphicsDevice.Clear(ColorManager.BackGround);

            SpriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointWrap);

            switch (CurrentState)
            {
                case GameState.InMenu:
                    break;
                case GameState.InGame:
                    SpriteBatch.Draw(PathTarget, Vector2.Zero, Color.White);

                    EnemyManager.Draw(SpriteBatch);
                    TowerManager.Draw(SpriteBatch);

                    if (SelectedTower != null)
                        SelectedTower.Draw(SpriteBatch);
                    break;
                case GameState.GameWin:
                    SpriteBatch.Draw(PathTarget, Vector2.Zero, Color.White);
                    break;
                case GameState.GameLoss:
                    SpriteBatch.Draw(PathTarget, Vector2.Zero, Color.White);
                    break;
            }

            var tempCache = Components[0] as UIManager;
            tempCache.Draw(SpriteBatch, CurrentState);

            SpriteBatch.End();

            base.Draw(gameTime);
        }

        void InitializeTextures()
        {
            TowerTex = Content.Load<AsepriteDocument>("Tower").Texture;
            EnemyTextures = Content.Load<AsepriteDocument>("Enemy");
            PathTextures = Content.Load<AsepriteDocument>("Path");
        }

        void SetScreenDimensions()
        {
            Graphics.PreferredBackBufferHeight = ScreenHeight;
            Graphics.PreferredBackBufferWidth = ScreenWidth;
            Graphics.ApplyChanges();

            PathTarget = new(GraphicsDevice, ScreenWidth, ScreenHeight);
        }

        void SetScreenDimensions(int height, int width, bool overwrite)
        {
            Graphics.PreferredBackBufferHeight = height;
            Graphics.PreferredBackBufferWidth = width;
            Graphics.ApplyChanges();

            PathTarget = new(GraphicsDevice, width, height);

            if (overwrite)
            {
                ScreenHeight = height;
                ScreenWidth = width;
            }
        }

        void DrawOnRenderTarget()
        {
            GraphicsDevice.SetRenderTarget(PathTarget);
            GraphicsDevice.Clear(Color.Transparent);

            SpriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointWrap);

            //Path.Draw(SpriteBatch);
            //Path.DrawPoints(SpriteBatch);

            Func<float, Rectangle> createPathDestRec = pos => new((int)/*MathF.Ceiling*/(Path.GetPos(pos).X - PathDimensions.X / 2), (int)/*MathF.Ceiling*/(Path.GetPos(pos).Y - PathDimensions.Y / 2), (int)PathDimensions.X, (int)PathDimensions.Y);
            Func<int, Rectangle> createPathSourceRec = index => new(PathTextures.Frames[index].X, PathTextures.Frames[index].Y, PathTextures.Frames[index].Width, PathTextures.Frames[index].Height);

            int pathEndsIndex = 0;
            int pathNormalIndex = 1;
            float pathEndsLayerAddition = 0.01f;


            for (float x = 0f; x < Path.endT; x += PathTimeDrawInterval)
                DrawTexOnScreen(PathTextures.Texture, createPathDestRec(x), createPathSourceRec(pathNormalIndex), ColorManager.Path, PathLayer);

            for (int x = 0; x < 2; x++)
                switch (x)
                {
                    case 0:
                        DrawTexOnScreen(PathTextures.Texture, createPathDestRec(0), createPathSourceRec(pathEndsIndex), ColorManager.PathEnds, PathLayer + pathEndsLayerAddition);
                        break;
                    case 1:
                        DrawTexOnScreen(PathTextures.Texture, createPathDestRec(Path.endT), createPathSourceRec(pathEndsIndex), ColorManager.PathEnds, PathLayer + pathEndsLayerAddition);
                        break;
                }

            SpriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
        }

        bool CanPlaceTower(Tower tower)
        {
            Texture2D towerTex = tower.GetTexture();

            Color[] towerPixels = new Color[TowerTex.Width * TowerTex.Height];
            Color[] renderPixels = new Color[tower.DestRec.Width * tower.DestRec.Height];

            towerTex.GetData(towerPixels);

            try
            {
                PathTarget.GetData(0, tower.DestRec, renderPixels, 0, renderPixels.Length);

                for (int x = 0; x < towerPixels.Length; x++)
                    if (towerPixels[x].A > 0.0f && renderPixels[x].A > 0.0f)
                        return false;
            }
            catch
            {

            }

            return true;
        }

        Rectangle[] CreateEnemySourceRecs()
        {
            List<Rectangle> sourceRecs = new();

            foreach (AsepriteFrame frame in EnemyTextures.Frames)
                sourceRecs.Add(new(frame.X, frame.Y, frame.Width, frame.Height));

            return sourceRecs.ToArray();
        }

        void DrawTexOnScreen(Texture2D tex, Rectangle destRec, float layer)
        {
            SpriteBatch.Draw(tex, destRec, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, layer);
        }

        void DrawTexOnScreen(Texture2D tex, Rectangle destRec, Color color, float layer)
        {
            SpriteBatch.Draw(tex, destRec, null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
        }

        void DrawTexOnScreen(Texture2D tex, Rectangle destRec, Rectangle sourceRec, float layer)
        {
            SpriteBatch.Draw(tex, destRec, sourceRec, Color.White, 0f, Vector2.Zero, SpriteEffects.None, layer);
        }

        void DrawTexOnScreen(Texture2D tex, Rectangle destRec, Rectangle sourceRec, Color color, float layer)
        {
            SpriteBatch.Draw(tex, destRec, sourceRec, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
        }

        public void LoseHP(int dmg = 1)
        {
            if (CurrentHealth - dmg <= 0)
                CurrentState = GameState.GameLoss;

            CurrentHealth -= dmg;
        }

        public void AddCurrency(int currency)
        {
            PlayerCurrency += currency;
        }

        List<Wave> LoadWavesFromFile()
        {
            List<Wave> fileData = new();

            List<List<Snippet>> wavesData = JsonConvert.DeserializeObject<List<List<Snippet>>>(File.ReadAllText("Waves.json"));

            foreach (List<Snippet> wavedata in wavesData)
            {
                Wave newWave = new();
                newWave.AddWaveData(wavedata);
                fileData.Add(newWave);
            }

            return fileData;
        }

        public void StartRestartGame()
        {
            EnemyManager.SetWaves(LoadWavesFromFile());
            EnemyManager.Clear();
            TowerManager.Clear();
            CurrentState = GameState.InGame;
            PlayerCurrency = GameStartCurrencyAmount;
            CurrentHealth = StartHealth;
            CurrentWave = -1;
        }

        public void StartCurrentWave()
        {
            GameState? newState;
            EnemyManager.StartWave(CurrentWave + 1, out newState);
            CurrentWave++;
            if (newState != null)
                if (newState.Value == GameState.GameWin)
                    WinGame();
        }

        public void UpdateUImanager(UIManager controls)
        {
            controls.UpdateVariables(PlayerCurrency, CurrentHealth, CurrentWave + 1);
        }

        public void EndWave()
        {
            var controlsManager = Components[0] as UIManager;
            controlsManager.WaveEnd();

            AddCurrency(RoundEndRewardMin+CurrentWave+1);
        }

        public void WinGame()
        {
            var controlsManager = Components[0] as UIManager;
            controlsManager.WinGame();
            CurrentState = GameState.GameWin;
        }

        public void LoseGame()
        {
            var controlsManager = Components[0] as UIManager;
            controlsManager.LoseGame();
            CurrentState = GameState.GameLoss;
        }

        public void SetState(GameState newState)
        {
            CurrentState = newState;
        }

        public void StartStopMusic()
        {
            switch (MediaPlayer.State)
            {
                case MediaState.Playing:
                    MediaPlayer.Pause();
                    break;
                case MediaState.Paused:
                    MediaPlayer.Resume();
                    break;
            }
        }
    }
}