﻿using Officeman_1._1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OfficeMan_1._1
{
    public partial class Form1 : Form
    {
        public bool prev = false;
        private int buildingsBackMoveCounter = 0;
        double globalGameTime = 0;
        private const int MaxFormWidth = 500, MaxFormHeight = 500;
        Sources source = new Sources();
        Mechanics mech = new Mechanics();
        Timer timerGame = new Timer();
        Rectangle CharacterPlace = new Rectangle(130, 120, 45, 56);
        Rectangle PegionPlace = new Rectangle(450, 450, 13, 7);
        Rectangle CleanerPlace = new Rectangle(-70, 70, 100, 120); // 25, 36 prev size
        Rectangle BuildingPlace = new Rectangle(0,0, 400, 462);
        Rectangle CloudsBack = new Rectangle(0, 0, 4200, 4200);
        Rectangle CloudsFont = new Rectangle(0, 0, 9000, 9000);
        Rectangle BuildingsBackForm = new Rectangle(-95, 110, 590, 600);
        int stand_pic = 0;
        int jump_anim_pic = -1;
        int cleaner_anim = 0;
        int pegion_pic = 0;
        int fall_pic = 0;
        int buildingY1 = 0;
        int buildingY2 = 462;
        int sky_fontX = 0;
        int sky_fontY = 0;
        int sky_trans_fontX = 0;
        int sky_trans_fontY = 0;
        int points100_anim = 0;
        private int cloudsFontTransparencyIndex = -1;
        Rectangle[] PegionFlock_Place = new Rectangle[5];
        
        public Form1()
        {
            Rectangle screen = Screen.PrimaryScreen.Bounds;
            Point maximizedLocation = new Point((screen.Width - MaxFormWidth) / 2, (screen.Height - MaxFormHeight) / 2);
            this.MaximumSize = new Size(MaxFormWidth, MaxFormHeight);
            this.MaximizedBounds = new Rectangle(maximizedLocation, this.MaximumSize);
            this.StartPosition = FormStartPosition.CenterScreen;

            //System.Media.SoundPlayer Audio;
            //Audio = new System.Media.SoundPlayer("..\\..\\sounds\\main.wav");
            //Audio.Load(); Audio.PlayLooping();

            InitializeComponent();
            timerGame.Tick += delegate
            {
                if (globalGameTime >= 10)
                {

                }
                if (globalGameTime >= 35)
                    if (!prev)
                    {
                        cloudsFontTransparencyIndex++;
                        prev = true;
                    }
                if (globalGameTime >= 60)
                {
                    mech[Mechanics.character.falling] = false;
                    mech[Mechanics.game.end] = true;
                }
                else
                    globalGameTime += 0.150;
                if (mech[Mechanics.character.falling])
                {
                    PointsLabel.Visible = true;
                    FormElement.DrawPoints(PointsLabel);
                }
                if (!mech[Mechanics.game.bird])
                {
                    PegionPlace.X = 450;
                    PegionPlace.Y = 450;   
                    Random pegion_probability = new Random();
                    int s = pegion_probability.Next(2);
                    if (s == 1)
                        mech[Mechanics.game.bird] = true;
                }
                if (!mech[Mechanics.game.birds])
                {
                    PegionFlock_Place[0] = new Rectangle(600, 280, 13, 7);
                    PegionFlock_Place[1] = new Rectangle(470, 250, 13, 7);
                    PegionFlock_Place[2] = new Rectangle(485, 315, 13, 7);
                    PegionFlock_Place[3] = new Rectangle(530, 265, 13, 7);
                    PegionFlock_Place[4] = new Rectangle(560, 330, 13, 7);
                    Random pegion_probability = new Random();
                    int s = pegion_probability.Next(2);
                    if (s == 1)
                        mech[Mechanics.game.birds] = true;
                }
                Invalidate();
            };
            timerGame.Interval = 150;
            timerGame.Start();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (!mech[Mechanics.character.falling])
            {
                e.Graphics.DrawImage(source.Clouds_When_Stand(ref CloudsBack), CloudsBack.X, CloudsBack.Y, CloudsBack.Width, CloudsBack.Height);
                e.Graphics.DrawImage(source.Buildings_Back(), BuildingsBackForm.X, BuildingsBackForm.Y, BuildingsBackForm.Width, BuildingsBackForm.Height);
                e.Graphics.DrawImage(source.DrawBuilding(), BuildingPlace.X, BuildingPlace.Y, BuildingPlace.Width, BuildingPlace.Height);
                if (mech[Mechanics.character.stand])
                    e.Graphics.DrawImage(source.DrawMan_Stand(ref stand_pic), CharacterPlace.X, CharacterPlace.Y, CharacterPlace.Width, CharacterPlace.Height);///////////// !!!!!!!!!
                if (mech[Mechanics.character.jumping])
                    e.Graphics.DrawImage(source.JumpPic(ref jump_anim_pic, ref CharacterPlace, mech), CharacterPlace.X, CharacterPlace.Y);
                e.Graphics.DrawImage(source.Transparent_Clouds_When_Stand(ref CloudsFont), CloudsFont.X, CloudsFont.Y, 10000, 10000);

            }
            if (mech[Mechanics.character.falling])
            {
                buildingsBackMoveCounter++;
                e.Graphics.DrawImage(source.Clouds_When_Fall(ref sky_fontX, ref sky_fontY), sky_fontX, sky_fontY, CloudsBack.Width, CloudsBack.Height);
                e.Graphics.DrawImage(source.Buildings_Back(), BuildingsBackForm.X, BuildingsBackForm.Y, BuildingsBackForm.Width, BuildingsBackForm.Height);                
                if (buildingsBackMoveCounter == 5)
                {
                    source.Buildings_Back_Move(ref BuildingsBackForm);
                    buildingsBackMoveCounter = 0;
                }
                e.Graphics.DrawImage(source.DrawBuilding_Fall(ref buildingY1), 0, buildingY1, BuildingPlace.Width, BuildingPlace.Height);
                e.Graphics.DrawImage(source.DrawBuilding_Fall(ref buildingY2), 0, buildingY2, BuildingPlace.Width, BuildingPlace.Height);
                e.Graphics.DrawImage(source.DrawMan_Fall(ref fall_pic), CharacterPlace.X, CharacterPlace.Y);
                if (buildingY1 <= -462)
                {
                    buildingY1 = -11;
                    buildingY2 = 451;
                }
                e.Graphics.DrawImage(source.Transparent_Clouds_When_Fall(ref CloudsFont), CloudsFont.X, CloudsFont.Y, 10000, 10000);
                CheckIntersection(e, ref points100_anim);
                Draw100pointsAnimation(e, ref points100_anim);
            }
            DrawAllBirds(e);
            if (mech[Mechanics.game.pause])
                DrawPauseMenu(e);
            else
                HidePauseMenu();
            if (mech[Mechanics.game.end])
            {
                OKLabel.Visible = true;
                e.Graphics.DrawImage(source.BuildingEnter(), BuildingPlace.X, BuildingPlace.Y, BuildingPlace.Width, BuildingPlace.Height);
                e.Graphics.DrawImage(source.DrawMan_Stand(ref stand_pic), CharacterPlace.X, CharacterPlace.Y);
                DrawTotalScore(e);
            }
            e.Dispose();
            //e.Graphics.RotateTransform(180); //  -- draws cleaner
            //e.Graphics.TranslateTransform(0, -MaxFormHeight);
            //e.Graphics.DrawImage(source.DrawCleaner(ref cleaner_anim), CleanerPlace);
            //e.Graphics.TranslateTransform(MaxFormWidth, MaxFormHeight);
            //e.Graphics.RotateTransform(180); // --X
        }

        private void CheckIntersection(PaintEventArgs e, ref int points100_anim)
        {
            if (Rectangle.Intersect(CharacterPlace, PegionPlace) != Rectangle.Empty)
            {
                points100_anim = 1;
                FormElement.Add100Points(PointsLabel);
                return;
            }
            for (int i = 0; i < 5; i++)
                if (Rectangle.Intersect(CharacterPlace, PegionFlock_Place[i]) != Rectangle.Empty)
                {
                    points100_anim = 1;
                    FormElement.Add100Points(PointsLabel);
                    return;
                }
        }

        private void Draw100pointsAnimation(PaintEventArgs e, ref int points100_anim)
        {
            switch(points100_anim)
            {
                case 0:
                        break;
                case 1:
                    {
                        e.Graphics.DrawImage(source.Get100Points(), CharacterPlace.X, CharacterPlace.Y - 30);
                        points100_anim = 2;
                        break;
                    }
                case 2:
                    {
                        points100_anim = 3;
                        break;
                    }
                case 3:
                    {
                        e.Graphics.DrawImage(source.Get100PointsHalfTransparent(), CharacterPlace.X, CharacterPlace.Y - 40);
                        points100_anim = 4;
                        break;
                    }
                case 4:
                    {
                        points100_anim = 5;
                        break;
                    }
                case 5:
                    {
                        e.Graphics.DrawImage(source.Get100PointsTransparent(), CharacterPlace.X, CharacterPlace.Y - 50);
                        points100_anim = 0;
                        break;
                    }
                default:
                    break;
            }
        }

        private void DrawAllBirds(PaintEventArgs e)
        {
            if (mech[Mechanics.game.bird])
            {
                if (PegionPlace.X >= 0)
                {
                    e.Graphics.DrawImage(source.PegionPic(ref pegion_pic), PegionPlace.X, PegionPlace.Y);
                    mech.PegionFly(ref PegionPlace);
                }
                else
                    mech[Mechanics.game.bird] = false;
            }
            if (mech[Mechanics.game.birds])
            {
                int gone = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (PegionFlock_Place[i].X >= 0)
                    {
                        e.Graphics.DrawImage(source.PegionPic(ref pegion_pic), PegionFlock_Place[i].X, PegionFlock_Place[i].Y);
                        mech.PegionFly(ref PegionFlock_Place[i]);
                    }
                    else
                        gone++;
                }
                if (gone == 5)
                    mech[Mechanics.game.birds] = false;
            }
        }

        private void DrawPauseMenu(PaintEventArgs e)
        {
            e.Graphics.DrawImage(source.GetMenuFont(), 140, 100);
            FormElement.ShowPauseMenuItems(PauseMenu_ContinueLabel, PauseMenu_FAQLabel, PauseMenu_LeaderboardLabel, PauseMenu_ExitLabel);
            timerGame.Stop();
        }

        private void DrawTotalScore(PaintEventArgs e)
        {
            FormElement.ShowTotalScore(TotalScoreLabel);
            FormElement.DrawTotalScore(TotalScoreLabel, PointsLabel);
            FormElement.ShowButtonOK(OKLabel);
            timerGame.Stop();
            Timer totalScoreAnimation = new Timer();
            totalScoreAnimation.Tick += delegate
            {
                //e.Graphics.DrawImage(source.Clouds_When_Stand(ref CloudsBack), CloudsBack.X, CloudsBack.Y , CloudsBack.Width, CloudsBack.Height);
                FormElement.TotalScore_ChangeImage(TotalScoreLabel, source);
                e.Dispose();
            };
            totalScoreAnimation.Interval = 150;
            totalScoreAnimation.Start();
        }

        private void HidePauseMenu()
        {
            FormElement.HidePauseMenuItems(PauseMenu_ContinueLabel, PauseMenu_FAQLabel, PauseMenu_LeaderboardLabel, PauseMenu_ExitLabel);           
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                mech[Mechanics.game.end] = true;
            }
            if (e.KeyCode == Keys.Escape)
            {
                if (!mech[Mechanics.game.pause])
                {
                    mech[Mechanics.game.pause] = true;
                    return;
                }
                mech[Mechanics.game.pause] = false;
                timerGame.Start();
            }
            if (e.KeyCode == Keys.Space)
            {
                mech[Mechanics.character.stand] = false;
                mech[Mechanics.character.jumping] = true;

            }
            if (e.KeyCode == Keys.A)
            {
                if (!mech[Mechanics.character.stand] & mech[Mechanics.character.falling])
                    mech.TurnLeft(ref CharacterPlace);
            }
            if (e.KeyCode == Keys.D)
            {
                if (!mech[Mechanics.character.stand] & mech[Mechanics.character.falling])
                    mech.TurnRight(ref CharacterPlace);
            }
            if (e.KeyCode == Keys.W)
            {
                if (!mech[Mechanics.character.stand] & mech[Mechanics.character.falling])                
                    mech.TurnUp(ref CharacterPlace);
            }
            if (e.KeyCode == Keys.S)
            {
                if (!mech[Mechanics.character.stand] & mech[Mechanics.character.falling])                
                    mech.TurnDown(ref CharacterPlace);
            }
            
        }

        private void PauseMenu_ContinueLabel_MouseClick(object sender, MouseEventArgs e)
        {
            KeyEventArgs k = new KeyEventArgs(Keys.Escape);
            Form1_KeyDown(sender, k);
        }

        private void PauseMenu_ContinueLabel_MouseHover(object sender, EventArgs e)
        {
            PauseMenu_ContinueLabel.Image = source.PauseMenu_Focus(0);
        }

        private void PauseMenu_ContinueLabel_MouseLeave(object sender, EventArgs e)
        {
            PauseMenu_ContinueLabel.Image = source.PauseMenu_Regular(0);
        }

        private void PauseMenu_ExitLabel_MouseHover(object sender, EventArgs e)
        {
            PauseMenu_ExitLabel.Image = source.PauseMenu_Focus(3);
        }

        private void PauseMenu_ExitLabel_MouseLeave(object sender, EventArgs e)
        {
            PauseMenu_ExitLabel.Image = source.PauseMenu_Regular(3);
        }

        private void PauseMenu_FAQLabel_MouseHover(object sender, EventArgs e)
        {
            PauseMenu_FAQLabel.Image = source.PauseMenu_Focus(1);
        }

        private void PauseMenu_FAQLabel_MouseLeave(object sender, EventArgs e)
        {
            PauseMenu_FAQLabel.Image = source.PauseMenu_Regular(1);
        }

        private void PauseMenu_LeaderboardLabel_MouseHover(object sender, EventArgs e)
        {
            PauseMenu_LeaderboardLabel.Image = source.PauseMenu_Focus(2);
        }

        private void PauseMenu_LeaderboardLabel_MouseLeave(object sender, EventArgs e)
        {
            PauseMenu_LeaderboardLabel.Image = source.PauseMenu_Regular(2);
        }

        private void ButtonOK_MouseDown(object sender, MouseEventArgs e)
        {
            OKLabel.Image = source.PressedButtonOK();
        }

        private void ButtonOK_MouseClick(object sender, MouseEventArgs e)
        {
            Application.Exit();
        }
    }
}