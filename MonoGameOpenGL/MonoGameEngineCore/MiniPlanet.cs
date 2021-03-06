using System.Collections.Generic;
using System.Linq;
using LibNoise;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameEngineCore;
using MonoGameEngineCore.GameObject;
using MonoGameEngineCore.GameObject.Components;
using MonoGameEngineCore.Procedural;
using MonoGameEngineCore.Rendering;
using MonoGameEngineCore.Helper;

namespace GridForgeResurrected.Screens
{
    public class MiniPlanet
    {
        private readonly Vector3 startPosition;
        public readonly float planetRadius;
        private readonly IModule noiseModule;
        private int heightMapVertCount;
        private float heightMapScale;
        private float originalHeightMapScale;
        private readonly Color terrainColor;
        private readonly Color oceanColour;
        private readonly bool useAtmosphere;
        private readonly float innerAtmosphereRatio;
        private readonly float outerAtmosphereRatio;
        private readonly float atmosphericScale;
        private readonly float groundScale;
        private GroundScatteringHelper groundScatteringHelper;
        private Atmosphere atmosphere;
        private List<GameObject> planetPieces;
        private Effect planetEffect;
        public Vector3 CurrentCenterPosition { get; private set; }
        public Orbit CurrentOrbit { get; set; }
        public Spin CurrentSpin { get; set; }
        private List<MiniPlanet> childPlanets;
        private float verticalScale = 1f;

        public class Spin
        {
            public Vector3 Axis { get; set; }
            public float Speed { get; set; }
        }
        public class Orbit
        {
            public MiniPlanet BodyToOrbit { get; set; }
            public Vector3 Axis { get; set; }
            public Vector3 OrbitPoint { get; set; }
            public float Speed { get; set; }
        }

        public MiniPlanet(Vector3 startPosition, float planetRadius, IModule noiseModule, int heightMapVertCount, float heightMapScale, Color landColor, Color oceanColour, bool useAtmosphere = false,
            float innerAtmosphereRatio = 0.97f, float outerAtmosphereRatio = 1.1f, float atmosphericScale = 30, float groundScale = 20, float verticalScale = 1f)
        {

            this.verticalScale = verticalScale;
            this.startPosition = startPosition;
            this.planetRadius = planetRadius;

            this.noiseModule = noiseModule;
            this.heightMapVertCount = heightMapVertCount;
            this.heightMapScale = heightMapScale;
            this.originalHeightMapScale = heightMapScale;
            this.terrainColor = landColor;
            this.oceanColour = oceanColour;
            this.useAtmosphere = useAtmosphere;
            this.innerAtmosphereRatio = innerAtmosphereRatio;
            this.outerAtmosphereRatio = outerAtmosphereRatio;
            this.atmosphericScale = atmosphericScale;
            this.groundScale = groundScale;

            if (!useAtmosphere)
            {
                planetEffect = EffectLoader.LoadSM5Effect("flatshaded").Clone();
                GenerateGeometry();
            }
            else
            {
                GenerateAtmosphereAndGeometry(innerAtmosphereRatio, outerAtmosphereRatio, atmosphericScale, groundScale);
            }


            childPlanets = new List<MiniPlanet>();
        }

        private void GenerateAtmosphereAndGeometry(float innerAtmosphereRatio, float outerAtmosphereRatio, float atmosphericScale, float groundScale)
        {

            planetEffect = EffectLoader.LoadSM5Effect("AtmosphericScatteringGround").Clone();
            groundScatteringHelper = new GroundScatteringHelper(planetEffect, planetRadius * outerAtmosphereRatio,
                planetRadius * innerAtmosphereRatio, groundScale);

            atmosphere = new Atmosphere(planetRadius * outerAtmosphereRatio, planetRadius * innerAtmosphereRatio, atmosphericScale);
            SystemCore.GameObjectManager.AddAndInitialiseGameObject(atmosphere);

            GenerateGeometry();
        }

        public void GenerateGeometry()
        {

            planetPieces = new List<GameObject>();


            //top
            GameObject oTop;
            VertexPositionColorTextureNormal[] vTop;
            var hTop = CreatePlanetGameObjects(out oTop, out vTop);

            for (int i = 0; i < vTop.Length; i++)
            {
                vTop[i].Position = vTop[i].Position - new Vector3(planetRadius, 0, planetRadius);
                vTop[i].Position = vTop[i].Position + Vector3.Up * planetRadius;
                vTop[i].Position = Vector3.Normalize(vTop[i].Position) * planetRadius;

                ModifyHeight(vTop, i);
            }

            InitialiseAndAddPlanetPiece(vTop, hTop, oTop);


            //front
            GameObject oFront;
            VertexPositionColorTextureNormal[] vFront;
            var hFront = CreatePlanetGameObjects(out oFront, out vFront);


            for (int i = 0; i < vFront.Length; i++)
            {
                vFront[i].Position = vFront[i].Position - new Vector3(planetRadius, 0, planetRadius);
                vFront[i].Position = Vector3.Transform(vFront[i].Position, Matrix.CreateRotationX(MathHelper.PiOver2));
                vFront[i].Position = vFront[i].Position - Vector3.Forward * planetRadius;
                vFront[i].Position = Vector3.Normalize(vFront[i].Position) * planetRadius;

                ModifyHeight(vFront, i);
            }

            InitialiseAndAddPlanetPiece(vFront, hFront, oFront);


            //back
            GameObject oBack;
            VertexPositionColorTextureNormal[] vBack;
            var hBack = CreatePlanetGameObjects(out oBack, out vBack);


            for (int i = 0; i < vBack.Length; i++)
            {
                vBack[i].Position = vBack[i].Position - new Vector3(planetRadius, 0, planetRadius);
                vBack[i].Position = Vector3.Transform(vBack[i].Position, Matrix.CreateRotationX(MathHelper.PiOver2));
                vBack[i].Position = vBack[i].Position - Vector3.Backward * planetRadius;
                vBack[i].Position = Vector3.Normalize(vBack[i].Position) * planetRadius;

                ModifyHeight(vBack, i);
            }

            InitialiseAndAddPlanetPiece(vBack, hBack, oBack, true);


            //left
            GameObject oLeft;
            VertexPositionColorTextureNormal[] vLeft;
            var hLeft = CreatePlanetGameObjects(out oLeft, out vLeft);


            for (int i = 0; i < vLeft.Length; i++)
            {
                vLeft[i].Position = vLeft[i].Position - new Vector3(planetRadius, 0, planetRadius);
                vLeft[i].Position = Vector3.Transform(vLeft[i].Position, Matrix.CreateRotationZ(MathHelper.PiOver2));
                vLeft[i].Position = vLeft[i].Position - Vector3.Left * planetRadius;
                vLeft[i].Position = Vector3.Normalize(vLeft[i].Position) * planetRadius;

                ModifyHeight(vLeft, i);
            }

            InitialiseAndAddPlanetPiece(vLeft, hLeft, oLeft, true);

            //right 
            GameObject oRight;
            VertexPositionColorTextureNormal[] vRight;
            var hRight = CreatePlanetGameObjects(out oRight, out vRight);


            for (int i = 0; i < vRight.Length; i++)
            {
                vRight[i].Position = vRight[i].Position - new Vector3(planetRadius, 0, planetRadius);
                vRight[i].Position = Vector3.Transform(vRight[i].Position, Matrix.CreateRotationZ(MathHelper.PiOver2));
                vRight[i].Position = vRight[i].Position - Vector3.Right * planetRadius;
                vRight[i].Position = Vector3.Normalize(vRight[i].Position) * planetRadius;

                ModifyHeight(vRight, i);
            }

            InitialiseAndAddPlanetPiece(vRight, hRight, oRight);


            //bottom
            GameObject oBottom;
            VertexPositionColorTextureNormal[] vBottom;
            var hBottom = CreatePlanetGameObjects(out oBottom, out vBottom);

            for (int i = 0; i < vBottom.Length; i++)
            {
                vBottom[i].Position = vBottom[i].Position - new Vector3(planetRadius, 0, planetRadius);
                vBottom[i].Position = vBottom[i].Position + Vector3.Down * planetRadius;
                vBottom[i].Position = Vector3.Normalize(vBottom[i].Position) * planetRadius;

                ModifyHeight(vBottom, i);
            }

            InitialiseAndAddPlanetPiece(vBottom, hBottom, oBottom, true);


            SetPosition(startPosition);

        }

        public void SetOrbit(Vector3 point, Vector3 axis, float speed)
        {
            CurrentOrbit = new Orbit() { Axis = axis, OrbitPoint = point, Speed = speed };
        }

        public void SetOrbit(MiniPlanet bodyToOrbit, Vector3 axis, float speed)
        {
            CurrentOrbit = new Orbit() { Axis = axis, BodyToOrbit = bodyToOrbit, Speed = speed };
            bodyToOrbit.AddChildBody(this);
        }

        public void AddChildBody(MiniPlanet miniPlanet)
        {
            childPlanets.Add(miniPlanet);
        }

        public void SetRotation(Vector3 axis, float speed)
        {
            CurrentSpin = new Spin() { Axis = axis, Speed = speed };
        }

        private void ModifyHeight(VertexPositionColorTextureNormal[] v, int i)
        {
            double heightValue = noiseModule.GetValue(v[i].Position.X, v[i].Position.Y, v[i].Position.Z);
            v[i].Position = (Vector3.Normalize(v[i].Position) * (planetRadius + ((float)heightValue) * verticalScale));

            float finalHeight = v[i].Position.Length();
            if (finalHeight < planetRadius)
                v[i].Color = oceanColour;
            else
            {
                v[i].Color = terrainColor;
            }
        }

        private void InitialiseAndAddPlanetPiece(VertexPositionColorTextureNormal[] v, Heightmap hMap,
            GameObject terrainObject, bool reverseIndices = false)
        {
            var verts =
                BufferBuilder.VertexBufferBuild(v);

            var inds = hMap.GenerateIndices();

            if (reverseIndices)
                inds = inds.Reverse().ToArray();

            var indices = BufferBuilder.IndexBufferBuild(inds);


            List<Vector3> verticesList = new List<Vector3>();

            foreach (VertexPositionColorTextureNormal v1 in v)
                verticesList.Add(v1.Position);


            terrainObject.AddComponent(new RenderGeometryComponent(verts, indices, indices.IndexCount / 3));
            terrainObject.AddComponent(new EffectRenderComponent(planetEffect));
            terrainObject.AddComponent(new StaticMeshColliderComponent("", verticesList, MonoMathHelper.ConvertShortToInt(inds), startPosition));
            SystemCore.GameObjectManager.AddAndInitialiseGameObject(terrainObject);
            planetPieces.Add(terrainObject);
        }

        private Heightmap CreatePlanetGameObjects(out GameObject terrainTopObject, out VertexPositionColorTextureNormal[] v)
        {
            Heightmap top = new Heightmap(heightMapVertCount, heightMapScale);
            terrainTopObject = new GameObject();
            v = top.GenerateVertexArray();

            for (int i = 0; i < v.Length; i++)
            {
                v[i].Color = terrainColor;
            }


            return top;
        }

        public void Update(GameTime gameTime, float heightAbovePlanetSurface, Vector3 cameraPos)
        {
            if (SystemCore.ActiveScene.LightsInScene.Count > 0)
            {
                DiffuseLight light = SystemCore.ActiveScene.LightsInScene[0] as DiffuseLight;

                if (groundScatteringHelper != null)
                {
                    groundScatteringHelper.Update(heightAbovePlanetSurface, light.LightDirection,
                        cameraPos - CurrentCenterPosition);

                    if (atmosphere != null)
                        atmosphere.Update(light.LightDirection, cameraPos, heightAbovePlanetSurface);
                }

                if (CurrentOrbit != null)
                {
                    Vector3 pointToOrbit = CurrentOrbit.OrbitPoint;
                    if (CurrentOrbit.BodyToOrbit != null)
                        pointToOrbit = CurrentOrbit.BodyToOrbit.CurrentCenterPosition;

                    CalculateOrbit(pointToOrbit, CurrentOrbit.Axis, CurrentOrbit.Speed);
                }
                if (CurrentSpin != null)
                {
                    CalculateRotation(CurrentSpin.Axis, CurrentSpin.Speed);
                }
            }

        }

        public void SetPosition(Vector3 position)
        {
            foreach (GameObject piece in planetPieces)
            {
                piece.Transform.SetPosition(position);
            }
            if (atmosphere != null)
                atmosphere.Transform.AbsoluteTransform.Translation = position;
            CurrentCenterPosition = position;
        }

        internal void CalculateRotation(Vector3 axis, float p)
        {
            foreach (GameObject gameObject in planetPieces)
            {
                gameObject.Transform.Rotate(axis, p);
            }
        }

        internal void CalculateOrbit(Vector3 point, Vector3 axis, float p)
        {
            Vector3 oldPosition = CurrentCenterPosition;
            Vector3 posAverage = Vector3.Zero;
            foreach (GameObject gameObject in planetPieces)
            {
                gameObject.Transform.RotateAround(axis, point, p);
                posAverage += gameObject.Transform.AbsoluteTransform.Translation;
            }
            posAverage /= 6;
            CurrentCenterPosition = posAverage;

            if (atmosphere != null)
                atmosphere.Transform.SetPosition(CurrentCenterPosition);

            foreach (MiniPlanet childPlanet in childPlanets)
            {
                childPlanet.Translate(CurrentCenterPosition - oldPosition);
            }
        }

        public void Translate(Vector3 translation)
        {
            Vector3 newPos = CurrentCenterPosition + translation;
            SetPosition(newPos);
        }

        public void DestroyGeometry()
        {
            if (atmosphere != null)
                SystemCore.GameObjectManager.RemoveObject(atmosphere);

            foreach (GameObject planetPiece in planetPieces)
            {
                SystemCore.GameObjectManager.RemoveObject(planetPiece);
            }
        }

        public void SetLOD(int lodLevel)
        {
            if (lodLevel > 5 || lodLevel < 1)
                return;

            DestroyGeometry();


            if (lodLevel == 1)
                heightMapVertCount = 101;
            if (lodLevel == 2)
                heightMapVertCount = 51;
            if (lodLevel == 3)
                heightMapVertCount = 35;
            if (lodLevel == 4)
                heightMapVertCount = 26;
            if (lodLevel == 5)
                heightMapVertCount = 21;



            heightMapScale = originalHeightMapScale * lodLevel;

            if (useAtmosphere)
                GenerateAtmosphereAndGeometry(innerAtmosphereRatio, outerAtmosphereRatio, atmosphericScale, groundScale);
            else
            {
                GenerateGeometry();
            }
        }


    }
}