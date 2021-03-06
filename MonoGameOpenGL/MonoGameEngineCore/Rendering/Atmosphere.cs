﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameEngineCore.GameObject.Components;
using MonoGameEngineCore.Procedural;
using MonoGameEngineCore.Helper;

namespace MonoGameEngineCore.Rendering
{
    public class SpaceScatteringHelper
    {

        float m_Kr = 0.0025f;		// Rayleigh scattering constant
        private float m_Kr4PI;
        float m_Km = 0.0010f;	// Mie scattering constant
        private float m_Km4PI;
        float m_ESun = 20.0f;		// Sun brightness constant
        float m_g = -0.990f;		// The Mie phase asymmetry factor
        private float m_fExposure = 2.0f;
        private float m_fScale;

        private Vector3 m_fWavelength;
        private Vector3 m_fWavelength4;
        private float m_fRayleighScaleDepth = 0.25f;
        private float m_fMieScaleDepth = 0.1f;
        private Effect atmosphereEffect;

        public SpaceScatteringHelper(Effect effect)
        {
           
            this.atmosphereEffect = effect;
            m_Kr4PI = m_Kr * 4.0f * MathHelper.Pi;
            m_Km4PI = m_Km * 4.0f * MathHelper.Pi;
           
            m_fWavelength.X = 0.650f;		// 650 nm for red
            m_fWavelength.Y = 0.570f;		// 570 nm for green
            m_fWavelength.Z = 0.475f;		// 475 nm for blue
            m_fWavelength4.X = (float)Math.Pow(m_fWavelength.X, 4.0f);
            m_fWavelength4.Y = (float)Math.Pow(m_fWavelength.Y, 4.0f);
            m_fWavelength4.Z = (float)Math.Pow(m_fWavelength.Z, 4.0f);





            atmosphereEffect.Parameters["v3InvWavelength"].SetValue(new Vector3(1 / m_fWavelength4.X,
               1 / m_fWavelength4.Y, 1 / m_fWavelength4.Z));

          

            atmosphereEffect.Parameters["fKrESun"].SetValue(m_Kr * m_ESun);
            atmosphereEffect.Parameters["fKmESun"].SetValue(m_Km * m_ESun);
            atmosphereEffect.Parameters["fKr4PI"].SetValue(m_Kr4PI);
            atmosphereEffect.Parameters["fKm4PI"].SetValue(m_Km4PI);



         
            atmosphereEffect.Parameters["fScaleDepth"].SetValue(m_fRayleighScaleDepth);
           

        }

        public void Update(float cameraHeight, Vector3 lightDir, Vector3 cameraPos, float m_fInnerRadius, float m_fOuterRadius)
        {

            m_fScale = 1 / (m_fOuterRadius - m_fInnerRadius);

            atmosphereEffect.Parameters["fInnerRadius"].SetValue(m_fInnerRadius);
            atmosphereEffect.Parameters["fInnerRadius2"].SetValue(m_fInnerRadius * m_fInnerRadius);
            atmosphereEffect.Parameters["fOuterRadius"].SetValue(m_fOuterRadius);
            atmosphereEffect.Parameters["fOuterRadius2"].SetValue(m_fOuterRadius * m_fOuterRadius);
            atmosphereEffect.Parameters["fScale"].SetValue(1.0f / (m_fOuterRadius - m_fInnerRadius));
            atmosphereEffect.Parameters["fScaleOverScaleDepth"].SetValue(1.0f /
                                                                                        (m_fOuterRadius -
                                                                                         m_fInnerRadius) /
                                                                                        m_fRayleighScaleDepth);

            atmosphereEffect.Parameters["fCameraHeight"].SetValue(cameraHeight);
            atmosphereEffect.Parameters["fCameraHeight2"].SetValue(cameraHeight * cameraHeight);
            atmosphereEffect.Parameters["v3LightPos"].SetValue(lightDir);
            atmosphereEffect.Parameters["v3CameraPos"].SetValue(cameraPos);
        }
    }
   
    /// <summary>
    /// For rendering things in the same atmosphere.
    /// </summary>
    public class GroundScatteringHelper
    {
       
            float m_Kr = 0.0025f;		// Rayleigh scattering constant
            private float m_Kr4PI;
            float m_Km = 0.0010f;	// Mie scattering constant
            private float m_Km4PI;
            float m_ESun = 20.0f;		// Sun brightness constant
            float m_g = -0.990f;		// The Mie phase asymmetry factor
            private float m_fExposure = 2.0f;

            private float m_fInnerRadius;
            private float m_fOuterRadius;
            private float m_fScale;

            private Vector3 m_fWavelength;
            private Vector3 m_fWavelength4;
            private float m_fRayleighScaleDepth = 0.25f;
            private float m_fMieScaleDepth = 0.1f;
            private Effect atmosphereEffect;

            public GroundScatteringHelper(Effect effect, float radius, float planetRadius, float scale=1)
            {
                m_fInnerRadius = planetRadius;
                m_fOuterRadius = radius;
                this.atmosphereEffect = effect;
                m_Kr4PI = m_Kr * 4.0f * MathHelper.Pi;
                m_Km4PI = m_Km * 4.0f * MathHelper.Pi;
                m_fScale = 1 / (m_fOuterRadius - m_fInnerRadius);

                m_fWavelength.X = 0.650f;		// 650 nm for red
                m_fWavelength.Y = 0.570f;		// 570 nm for green
                m_fWavelength.Z = 0.475f;		// 475 nm for blue
                m_fWavelength4.X = (float)Math.Pow(m_fWavelength.X, 4.0f);
                m_fWavelength4.Y = (float)Math.Pow(m_fWavelength.Y, 4.0f);
                m_fWavelength4.Z = (float)Math.Pow(m_fWavelength.Z, 4.0f);


            

           
                atmosphereEffect.Parameters["v3InvWavelength"].SetValue(new Vector3(1 / m_fWavelength4.X,
                   1 / m_fWavelength4.Y, 1 / m_fWavelength4.Z));

                atmosphereEffect.Parameters["fInnerRadius"].SetValue(m_fInnerRadius);
                atmosphereEffect.Parameters["fInnerRadius2"].SetValue(m_fInnerRadius * m_fInnerRadius);
                atmosphereEffect.Parameters["fOuterRadius"].SetValue(m_fOuterRadius);
                atmosphereEffect.Parameters["fOuterRadius2"].SetValue(m_fOuterRadius * m_fOuterRadius);

                atmosphereEffect.Parameters["fKrESun"].SetValue(m_Kr * m_ESun);
                atmosphereEffect.Parameters["fKmESun"].SetValue(m_Km * m_ESun);
                atmosphereEffect.Parameters["fKr4PI"].SetValue(m_Kr4PI);
                atmosphereEffect.Parameters["fKm4PI"].SetValue(m_Km4PI);

           

                atmosphereEffect.Parameters["fScale"].SetValue(scale / (m_fOuterRadius - m_fInnerRadius));
                atmosphereEffect.Parameters["fScaleDepth"].SetValue(m_fRayleighScaleDepth);
                atmosphereEffect.Parameters["fScaleOverScaleDepth"].SetValue(1.0f /
                                                                                             (m_fOuterRadius -
                                                                                              m_fInnerRadius) /
                                                                                             m_fRayleighScaleDepth);

            }

            public void Update(float cameraHeight, Vector3 lightDir, Vector3 cameraPos)
            {
                  
                atmosphereEffect.Parameters["fCameraHeight"].SetValue(cameraHeight);
                atmosphereEffect.Parameters["fCameraHeight2"].SetValue(cameraHeight * cameraHeight);
                atmosphereEffect.Parameters["v3LightPos"].SetValue(lightDir);
                atmosphereEffect.Parameters["v3CameraPos"].SetValue(cameraPos);
            }
        }
    

    /// <summary>
    /// For rendering the atmosphere itself.
    /// </summary>
    public class Atmosphere : GameObject.GameObject
    {
        float m_Kr = 0.0025f;		// Rayleigh scattering constant
        private float m_Kr4PI;
        float m_Km = 0.0010f;	// Mie scattering constant
        private float m_Km4PI;
        float m_ESun = 20.0f;		// Sun brightness constant
        float m_g = -0.990f;		// The Mie phase asymmetry factor
        private float m_fExposure = 2.0f;

        private float m_fInnerRadius;
        public float m_fOuterRadius;
        private float m_fScale;

        private Vector3 m_fWavelength;
        private Vector3 m_fWavelength4;
        private float m_fRayleighScaleDepth = 0.2f;
        private float m_fMieScaleDepth = 0.1f;
        private Effect atmosphereEffect;

        public Atmosphere(float radius, float planetRadius, float scale=1)
        {
            m_fInnerRadius = planetRadius;
            m_fOuterRadius = radius;

            m_Kr4PI = m_Kr * 4.0f * MathHelper.Pi;
            m_Km4PI = m_Km * 4.0f * MathHelper.Pi;
            m_fScale = scale / (m_fOuterRadius - m_fInnerRadius);

            m_fWavelength.X = 0.650f;		// 650 nm for red
            m_fWavelength.Y = 0.570f;		// 570 nm for green
            m_fWavelength.Z = 0.475f;		// 475 nm for blue
            m_fWavelength4.X = (float)Math.Pow(m_fWavelength.X, 4.0f);
            m_fWavelength4.Y = (float)Math.Pow(m_fWavelength.Y, 4.0f);
            m_fWavelength4.Z = (float)Math.Pow(m_fWavelength.Z, 4.0f);

            atmosphereEffect = EffectLoader.LoadSM5Effect("AtmosphericScatteringSky").Clone();
            EffectRenderComponent effectRenderComponent = new EffectRenderComponent(atmosphereEffect);
            AddComponent(effectRenderComponent);

            ProceduralSphere sphere = new ProceduralSphere(100,100);
            sphere.Scale(m_fOuterRadius);
            sphere.InsideOut();

            AddComponent(new RenderGeometryComponent(sphere));


            atmosphereEffect.Parameters["v3InvWavelength"].SetValue(new Vector3(1 / m_fWavelength4.X,
               1 / m_fWavelength4.Y, 1 / m_fWavelength4.Z));

            atmosphereEffect.Parameters["fInnerRadius"].SetValue(m_fInnerRadius);
            atmosphereEffect.Parameters["fInnerRadius2"].SetValue(m_fInnerRadius * m_fInnerRadius);
            atmosphereEffect.Parameters["fOuterRadius"].SetValue(m_fOuterRadius);
            atmosphereEffect.Parameters["fOuterRadius2"].SetValue(m_fOuterRadius * m_fOuterRadius);

            atmosphereEffect.Parameters["fKrESun"].SetValue(m_Kr * m_ESun);
            atmosphereEffect.Parameters["fKmESun"].SetValue(m_Km * m_ESun);
            atmosphereEffect.Parameters["fKr4PI"].SetValue(m_Kr4PI);
            atmosphereEffect.Parameters["fKm4PI"].SetValue(m_Km4PI);

            atmosphereEffect.Parameters["g"].SetValue(m_g);
            atmosphereEffect.Parameters["g2"].SetValue(m_g * m_g);

            atmosphereEffect.Parameters["fScale"].SetValue(m_fScale);
            atmosphereEffect.Parameters["fScaleDepth"].SetValue(m_fRayleighScaleDepth);
            atmosphereEffect.Parameters["fScaleOverScaleDepth"].SetValue(1 /
                                                                                         (m_fOuterRadius -
                                                                                          m_fInnerRadius) /
                                                                                         m_fRayleighScaleDepth);

        }

        public void Update(Vector3 lightDir, Vector3 cameraPos, float heightAboveSurface)
        {
            
            atmosphereEffect.Parameters["fCameraHeight"].SetValue(heightAboveSurface);
            atmosphereEffect.Parameters["fCameraHeight2"].SetValue(heightAboveSurface * heightAboveSurface);
            atmosphereEffect.Parameters["v3LightPos"].SetValue(lightDir);
            atmosphereEffect.Parameters["v3CameraPos"].SetValue(cameraPos - Transform.AbsoluteTransform.Translation);
        }
    }
}
