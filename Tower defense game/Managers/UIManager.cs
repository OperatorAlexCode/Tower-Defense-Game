using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.UI.Forms;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tower_defense_game.Enums;
using Tower_defense_game.GameObjects;
using SpriteBatch = Microsoft.Xna.Framework.Graphics.SpriteBatch;

namespace Tower_defense_game.Managers
{
    public class UIManager : ControlManager
    {
        // int
        int ScreenHeight;
        int ScreenWidth;
        int PlayerCurrency;
        int PlayerHealth;
        int CurrentWave;
        #region Indexes
        int StartGameBtnIndex;
        int StartWaveBtnIndex;
        int CurrencyDisplayIndex;
        int GameTitleIndex;
        int GameOverIndex;
        int YesBtnIndex;
        int NoBtnIndex;
        int GameWinIndex;
        int HealthDisplayIndex;
        int WinBtnIndex;
        int WaveDisplayIndex;
        #endregion

        // String
        string GameTitle = " Toys Tower \n   Defense  ";
        string GameOverText = " Game Over\nTry Again?";
        string CurrencySymbol = "G:";
        string HealthIcon = "Health:";
        string GameWinText = "You Won!";
        string WaveText = " :Wave";

        // Action
        Action StartGame;
        Action StartWave;
        Action<GameState> ChangeGaneState;

        // SpriteFont
        SpriteFont DefaultFont;
        SpriteFont TitleFont;

        // Rectangle
        Rectangle DisplayStandardRec = new(0, 40, 40, 40);
        Rectangle DisplayAOERec = new(0, 80, 40, 40);

        // Other
        Texture2D TowerTex;
        float TextLayer;

        public UIManager(Game game, int screenHeight, int screenWidth, Action startGame, Action startWave, Action<GameState> changeGameState, SpriteFont defaultFont, SpriteFont titleFont, Texture2D towerTex, float textLayer) : base(game)
        {
            ScreenHeight = screenHeight;
            ScreenWidth = screenWidth;
            StartGame = startGame;
            StartWave = startWave;
            DefaultFont = defaultFont;
            TitleFont = titleFont;
            TowerTex = towerTex;
            TextLayer = textLayer;
            ChangeGaneState = changeGameState;
        }

        public override void InitializeComponent()
        {
            #region Vector Diménsions
            Vector2 startGameBtnDim = new(120, 48);
            Vector2 startWaveBtnDim = new(120, 48);
            Vector2 startWaveBtnMargin = new(10, 10);
            Vector2 currencyDisplayDim = MeasureString(DefaultFont, CreateDisplayText(0));
            Vector2 healthDisplayDim = MeasureString(DefaultFont, CreateDisplayText(1));
            Vector2 waveDisplayDim = MeasureString(DefaultFont, CreateDisplayText(2));
            Vector2 gameTitleDim = MeasureString(TitleFont, GameTitle);
            Vector2 gameOverDim = MeasureString(TitleFont, GameOverText);
            Vector2 yesNoBtnDim = new(60, 48);
            #endregion

            #region Main menu
            Label gameTitle = new()
            {
                Text = GameTitle,
                TextColor = ColorManager.Text,
                Location = new((ScreenWidth - gameTitleDim.X) / 2, ScreenHeight / 3 - gameTitleDim.Y / 2),
                FontName = "TitleFont"
            };

            Button startGameBtn = new()
            {
                Text = "Start Game",
                TextColor = ColorManager.Text,
                Size = startGameBtnDim,
                Location = new((ScreenWidth - startGameBtnDim.X) / 2, (ScreenHeight - startGameBtnDim.Y) / 2),
                BackgroundColor = Color.DeepPink,
                BtnLeftTexture = "ButtonLeft",
                BtnMiddleTexture = "ButtonMiddle",
                BtnRightTexture = "ButtonRight",
                TextAlign = ContentAlignment.MiddleCenter,

            };
            #endregion

            #region HUD
            Button startWaveBtn = new()
            {
                Text = "Start Wave",
                TextColor = ColorManager.Text,
                Size = startWaveBtnDim,
                Location = new(ScreenWidth - startWaveBtnDim.X - startWaveBtnMargin.X, startWaveBtnMargin.Y),
                BackgroundColor = Color.Red,
                BtnLeftTexture = "BtnLG",
                BtnMiddleTexture = "BtnMG",
                BtnRightTexture = "BtnRG",
                TextAlign = ContentAlignment.MiddleCenter,
                Enabled = false,
                IsVisible = false,
            };

            Label currencyDisplay = new()
            {
                Text = CreateDisplayText(0),
                TextColor = ColorManager.Text,
                Size = currencyDisplayDim,
                Location = new(0, 0),
                Enabled = false,
                IsVisible = false,
            };

            Label healthDisplay = new()
            {
                Text = CreateDisplayText(1),
                TextColor = ColorManager.Text,
                Size = healthDisplayDim,
                Location = new(0, ScreenHeight-healthDisplayDim.Y),
                Enabled = false,
                IsVisible = false,
            };

            Label waveDisplay = new()
            {
                Text = CreateDisplayText(2),
                TextColor = ColorManager.Text,
                Size = healthDisplayDim,
                Location = new(ScreenWidth-waveDisplayDim.X, ScreenHeight - waveDisplayDim.Y),
                Enabled = false,
                IsVisible = false,
            };
            #endregion

            #region Game Over Screen
            Label gameOver = new()
            {
                Text = GameOverText,
                TextColor = ColorManager.Text,
                Location = new((ScreenWidth - gameTitleDim.X) / 2, ScreenHeight / 3 - gameTitleDim.Y / 2),
                FontName = "TitleFont",
                Enabled = false,
                IsVisible = false,
            };

            Button yesBtn = new()
            {
                Text = "Yes",
                TextColor = ColorManager.Text,
                Size = yesNoBtnDim,
                Location = new(ScreenWidth/3- yesNoBtnDim.X, ScreenHeight*3/5- yesNoBtnDim.Y/2),
                BtnLeftTexture = "BtnLG",
                BtnMiddleTexture = "BtnMG",
                BtnRightTexture = "BtnRG",
                TextAlign = ContentAlignment.MiddleCenter,
                Enabled = false,
                IsVisible = false,
            };

            Button noBtn = new()
            {
                Text = "No",
                TextColor = ColorManager.Text,
                Size = yesNoBtnDim,
                Location = new(ScreenWidth*2 / 3 - yesNoBtnDim.X/2, ScreenHeight * 3 / 5 - yesNoBtnDim.Y / 2),
                BtnLeftTexture = "BtnLR",
                BtnMiddleTexture = "BtnMR",
                BtnRightTexture = "BtnRR",
                TextAlign = ContentAlignment.MiddleCenter,
                Enabled = false,
                IsVisible = false,
            };
            #endregion

            #region Game Win Screen
            Label gameWin = new()
            {
                Text = GameWinText,
                TextColor = ColorManager.Text,
                Location = new((ScreenWidth - gameTitleDim.X) / 2, ScreenHeight / 3 - gameTitleDim.Y / 2),
                FontName = "TitleFont",
                Enabled = false,
                IsVisible = false,
            };

            Button winBtn = new()
            {
                Text = ":)",
                TextColor = ColorManager.Text,
                Size = yesNoBtnDim,
                Location = new((ScreenWidth - yesNoBtnDim.X) / 2, ScreenHeight * 3 / 5 - yesNoBtnDim.Y / 2),
                BtnLeftTexture = "BtnLG",
                BtnMiddleTexture = "BtnMG",
                BtnRightTexture = "BtnRG",
                TextAlign = ContentAlignment.MiddleCenter,
                Enabled = false,
                IsVisible = false,
            };
            #endregion

            startGameBtn.Clicked += StartGameBtn_Clicked;
            startWaveBtn.Clicked += StartWaveBtn_Clicked;
            yesBtn.Clicked += YesBtn_Clicked;
            noBtn.Clicked += NoBtn_Clicked;
            winBtn.Clicked += WinBtn_Clicked;

            #region Add controls
            AddToControls(startGameBtn, out StartGameBtnIndex);
            AddToControls(gameTitle, out GameTitleIndex);
            AddToControls(startWaveBtn, out StartWaveBtnIndex);
            AddToControls(currencyDisplay, out CurrencyDisplayIndex);
            AddToControls(waveDisplay, out WaveDisplayIndex);
            AddToControls(healthDisplay, out HealthDisplayIndex);
            AddToControls(gameOver, out GameOverIndex);
            AddToControls(yesBtn, out YesBtnIndex);
            AddToControls(noBtn, out NoBtnIndex);
            AddToControls(gameWin, out GameWinIndex);
            AddToControls(winBtn, out WinBtnIndex);
            #endregion
        }

        public void Draw(SpriteBatch spriteBatch, GameState currentState)
        {
            if (currentState == GameState.InGame)
            {
                UpdateDisplayText(CurrencyDisplayIndex, CreateDisplayText(0));
                UpdateDisplayText(HealthDisplayIndex, CreateDisplayText(1));
                UpdateDisplayText(WaveDisplayIndex, CreateDisplayText(2));
                DrawDisplayTowers(spriteBatch);
            }
            else
            {
                if (Controls[StartWaveBtnIndex].Enabled || Controls[StartWaveBtnIndex].IsVisible)
                    EnableDisable(StartWaveBtnIndex, false);
            }
        }

        void StartGameBtn_Clicked(object sender, EventArgs e)
        {
            TurnOnOffMenu(false);
            TurnOnOffHud(true);
            StartGame.Invoke();
        }

        void YesBtn_Clicked(object sender, EventArgs e)
        {
            TurnOnOffLoseScreen(false);
            TurnOnOffHud(true);
            StartGame.Invoke();
        }

        void NoBtn_Clicked(object sender, EventArgs e)
        {
            TurnOnOffLoseScreen(false);
            TurnOnOffMenu(true);
            ChangeGaneState.Invoke(GameState.InMenu);
        }

        void StartWaveBtn_Clicked(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            EnableDisable(btn);
            StartWave.Invoke();
        }

        void WinBtn_Clicked(object sender, EventArgs e)
        {
            TurnOnOffWinScreen(false);
            TurnOnOffMenu(true);
            ChangeGaneState.Invoke(GameState.InMenu);
        }

        public void UpdateVariables(int playerCurrency, int playerHealth, int currentWave)
        {
            PlayerCurrency = playerCurrency;
            PlayerHealth = playerHealth;
            CurrentWave = currentWave;
        }

        void EnableDisable(Control control)
        {
            control.Enabled = !control.Enabled;
            control.IsVisible = !control.IsVisible;
        }

        void EnableDisable(Control control, bool value)
        {
            control.Enabled = value;
            control.IsVisible = value;
        }

        void EnableDisable(int index)
        {
            Controls[index].Enabled = !Controls[index].Enabled;
            Controls[index].IsVisible = !Controls[index].IsVisible;
        }

        void EnableDisable(int index,bool value)
        {
            Controls[index].Enabled = value;
            Controls[index].IsVisible = value;
        }

        public void WaveEnd()
        {
            EnableDisable(StartWaveBtnIndex, true);
        }

        public void WinGame()
        {
            
            TurnOnOffHud(false);
            TurnOnOffWinScreen(true);
        }

        public void LoseGame()
        {
            TurnOnOffHud(false);
            TurnOnOffLoseScreen(true);
        }

        void AddToControls(Control control, out int indexVar)
        {
            Controls.Add(control);
            indexVar = Controls.Count - 1;
        }

        static Vector2 MeasureString(SpriteFont font, string stringToMeasure)
        {
            return font.MeasureString(stringToMeasure);
        }

        void DrawTexOnScreen(SpriteBatch spriteBatch,Texture2D tex, Rectangle destRec, Color color, float layer)
        {
            spriteBatch.Draw(tex, destRec, null, color, 0f, Vector2.Zero, SpriteEffects.None, layer);
        }

        void DrawDisplayTowers(SpriteBatch spriteBatch)
        {
            Color displayStandardClr = new(ColorManager.StandardTower.R, ColorManager.StandardTower.G, ColorManager.StandardTower.B, ColorManager.CantBuy.A);
            Color displayAOEClr = new(ColorManager.AOETower.R, ColorManager.AOETower.G, ColorManager.AOETower.B, ColorManager.CantBuy.A);

            if (PlayerCurrency >= TowerManager.TowerCostTable[new StandardTower().GetType()])
                displayStandardClr.A = ColorManager.CanBuy.A;

            if (PlayerCurrency >= TowerManager.TowerCostTable[new AOETower().GetType()])
                displayAOEClr.A = ColorManager.CanBuy.A;

            DrawTexOnScreen(spriteBatch, TowerTex, DisplayStandardRec, displayStandardClr, TextLayer);
            DrawTexOnScreen(spriteBatch, TowerTex, DisplayAOERec, displayAOEClr, TextLayer);
        }

        void TurnOnOffMenu(bool? value = null)
        {
            if (value.HasValue)
            {
                EnableDisable(StartGameBtnIndex, value.Value);
                EnableDisable(GameTitleIndex, value.Value);
            }
            else
            {
                EnableDisable(StartGameBtnIndex);
                EnableDisable(GameTitleIndex);
            }
        }

        void TurnOnOffHud(bool? value = null)
        {
            if (value.HasValue)
            {
                EnableDisable(CurrencyDisplayIndex, value.Value);
                EnableDisable(HealthDisplayIndex, value.Value);
                EnableDisable(WaveDisplayIndex, value.Value);
                EnableDisable(StartWaveBtnIndex, value.Value);
            }
            else
            {
                EnableDisable(CurrencyDisplayIndex);
                EnableDisable(HealthDisplayIndex);
                EnableDisable(WaveDisplayIndex);
                EnableDisable(StartWaveBtnIndex);
            }
        }

        void TurnOnOffLoseScreen(bool? value = null)
        {
            if (value.HasValue)
            {
                EnableDisable(GameOverIndex, value.Value);
                EnableDisable(YesBtnIndex, value.Value);
                EnableDisable(NoBtnIndex, value.Value);
            }
            else
            {
                EnableDisable(GameWinIndex);
                EnableDisable(YesBtnIndex);
                EnableDisable(NoBtnIndex);
            }
        }

        void TurnOnOffWinScreen(bool? value = null)
        {
            if (value.HasValue)
            {
                EnableDisable(GameWinIndex, value.Value);
                EnableDisable(WinBtnIndex, value.Value);
            }
            else
            {
                EnableDisable(GameWinIndex);
                EnableDisable(WinBtnIndex);
            }
        }

        void UpdateDisplayText(int index, string newText)
        {
            Label display = Controls[index] as Label;
            display.Text = newText;

            display.Size = MeasureString(DefaultFont, newText);

            if (index == CurrencyDisplayIndex)
                display.Location = new(0, 0);
            else if (index == HealthDisplayIndex)
                display.Location = new(0, ScreenHeight - MeasureString(DefaultFont, newText).Y);
            else if (index == WaveDisplayIndex)
                display.Location = new(ScreenWidth- MeasureString(DefaultFont, newText).X, ScreenHeight - MeasureString(DefaultFont, newText).Y);
        }

        string? CreateDisplayText(int display)
        {
            switch (display)
            {
                case 0:
                    return CurrencySymbol + $" {PlayerCurrency}";
                case 1:
                    return HealthIcon + $" {PlayerHealth}";
                case 2:
                    return $"{CurrentWave}" + WaveText;
                default:
                    return null;
            }
        }
    }
}
