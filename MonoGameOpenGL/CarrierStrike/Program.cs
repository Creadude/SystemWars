﻿using Microsoft.Xna.Framework.Graphics;
using MonoGameEngineCore;
using MonoGameEngineCore.GUI;
using MonoGameEngineCore.Rendering;

namespace CarrierStrike
{
    class Program
    {
        static void Main(string[] args)
        {
            SystemCore.ActiveColorScheme = ColorScheme.ColorSchemes["flatui"];
            ScreenResolutionName resToUse = ScreenResolutionName.WXGA;

            if (System.Environment.MachineName == "NICKMCCREA-PC")
                resToUse = ScreenResolutionName.WUXGA;

            using (var game = new MonoEngineGame(typeof(MainMenuScreen), resToUse, DepthFormat.Depth24, true, false))
                game.Run();
        }
    }
}
