﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameEngineCore.GameObject;
using MonoGameEngineCore.GameObject.Components;
using MonoGameEngineCore.Procedural;
using MonoGameEngineCore.Rendering;
using MonoGameEngineCore.ScreenManagement;
using Particle3DSample;
using MonoGameEngineCore;

namespace MonoGameDirectX11.Screens
{
    public class AtmosphereTest : MouseCamScreen
    {


        private Texture2D atmosphereTexture;
        private EffectRenderComponent atmosphereRenderComponent;

        public AtmosphereTest()
            : base()
        {
        
            ProceduralSphere planet = new ProceduralSphere(20,20);
            ProceduralSphere atmosphere = new ProceduralSphere(50,50);
            atmosphere.Indices = atmosphere.Indices.Reverse().ToArray();
            atmosphereTexture = SystemCore.ContentManager.Load<Texture2D>("Textures/AtmosphereGradient3");

            planet.Scale(100f);
            atmosphere.Scale(120f);
            atmosphere.SetColor(Color.CornflowerBlue);

            var planetObject = GameObjectFactory.CreateRenderableGameObjectFromShape(planet,
                EffectLoader.LoadEffect("flatshaded"));


            var atmosphereObject = GameObjectFactory.CreateRenderableGameObjectFromShape(atmosphere,
                EffectLoader.LoadEffect("atmosphere"));

            
            SystemCore.GameObjectManager.AddAndInitialiseGameObject(atmosphereObject);
            SystemCore.GameObjectManager.AddAndInitialiseGameObject(planetObject);

            atmosphereRenderComponent = atmosphereObject.GetComponent<EffectRenderComponent>();


            atmosphereRenderComponent.effect.Parameters["gTex"].SetValue(atmosphereTexture);

          

        }

        public override void Update(GameTime gameTime)
        {
         
            base.Update(gameTime);
        }

        public override void Render(GameTime gameTime)
        {
            atmosphereRenderComponent.effect.Parameters["ViewInverse"].SetValue(Matrix.Invert(SystemCore.ActiveCamera.View));

            base.Render(gameTime);
        }
    }
}