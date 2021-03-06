﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MonoGameDirectX11;
using MonoGameDirectX11.Screens;
using MonoGameEngineCore;
using MonoGameEngineCore.GUI;
using MonoGameEngineCore.GUI.Controls;
using MonoGameEngineCore.ScreenManagement;
using OldGameTest.Screens;

namespace OldGameTest
{
    class MainMenuScreen : Screen
    {
        public MainMenuScreen()
            : base()
        {

            string screenOne = "Pong";
            string screenTwo = "Chip 8 Emulator";
            string screenThree = "Simulation Challenge";


            SystemCore.GetSubsystem<GUIManager>().CreateDefaultMenuScreen("Main Menu", SystemCore.ActiveColorScheme, screenOne, screenTwo, screenThree);
            SystemCore.CursorVisible = true;

            Button b = SystemCore.GetSubsystem<GUIManager>().GetControl(screenOne) as Button;
            b.OnClick += (sender, args) =>
            {
                SystemCore.ScreenManager.AddAndSetActive(new PongScreen());
            };

            Button a = SystemCore.GetSubsystem<GUIManager>().GetControl(screenTwo) as Button;
            a.OnClick += (sender, args) =>
            {
                SystemCore.ScreenManager.AddAndSetActive(new Chip8Screen());
            };

            Button c = SystemCore.GetSubsystem<GUIManager>().GetControl(screenThree) as Button;
            c.OnClick += (sender, args) =>
            {
                SystemCore.ScreenManager.AddAndSetActive(new SimChalleneScreen());
            };




        }

        public override void OnRemove()
        {
            SystemCore.GUIManager.ClearAllControls();

            base.OnRemove();
        }

        public override void Update(GameTime gameTime)
        {
            input.AddKeyPressBinding("Escape", Microsoft.Xna.Framework.Input.Keys.Escape).InputEventActivated += (x, y) =>
            {
                SystemCore.Game.Exit();
            };

            base.Update(gameTime);
        }

        public override void Render(GameTime gameTime)
        {
            base.Render(gameTime);
        }
    }
}
