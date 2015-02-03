﻿#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using MonoGameEngineCore;
using MonoGameEngineCore.GUI;
using MonoGameEngineCore.Rendering;
using Vector.Screens;

#endregion

namespace Vector
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ScreenResolutionName resToUse = ScreenResolutionName.WXGA;
            SystemCore.ActiveColorScheme = ColorScheme.ColorSchemes["space"];

            if (System.Environment.MachineName == "NICKMCCREA-PC")
                resToUse = ScreenResolutionName.WUXGA;

            using (var game = new MonoEngineGame(typeof(MainMenuScreen), resToUse, DepthFormat.Depth24))
                game.Run();
        }


        /*
         * Vector
- Geometry wars meets tower defence, with local coop.
- Goal is to escape planets by protecting and fueling your ship.
- If you lose your ship, game over.
- Dying means you're out of the action for a bit, and you may lose your ship.
- With physics?
- Top down view. Player controls little vehicle, which has fun, skiddy, physics.
- Enemies stream in, when they die they drop energy. 
- particles, explosions, dead enemies bouncing around with physics etc.
- Player can use energy to fund fuel mines, which gather fuel, or defensive structures, which help protect your rocket + fuel installations.
         * */
    }
#endif
}