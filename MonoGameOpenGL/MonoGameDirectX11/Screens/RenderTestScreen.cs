﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameEngineCore;
using MonoGameEngineCore.GameObject;
using MonoGameEngineCore.GameObject.Components;
using MonoGameEngineCore.Helper;
using MonoGameEngineCore.Procedural;
using MonoGameEngineCore.Rendering;
using MonoGameEngineCore.Rendering.Camera;
using System.Collections.Generic;

namespace MonoGameDirectX11
{
    public class RenderTestScreen : Screen
    {
        protected MouseFreeCamera mouseCamera;

        public RenderTestScreen()
            : base()
        {
            SystemCore.CursorVisible = false;
            fpsLabel.Visible = true;
            SystemCore.ActiveScene.SetUpDefaultAmbientAndDiffuseLights();

            var effect = EffectLoader.LoadSM5Effect("FlatShaded");


            mouseCamera = new MouseFreeCamera(new Vector3(0, 0, 0));
            SystemCore.SetActiveCamera(mouseCamera);

            var shape = new ProceduralSphere(20, 20);
            shape.SetColor(SystemCore.ActiveColorScheme.Color5);




            for (int i = 0; i < 100; i++)
            {
                var gameObject = new GameObject();
                gameObject.AddComponent(new RenderGeometryComponent(BufferBuilder.VertexBufferBuild(shape), BufferBuilder.IndexBufferBuild(shape), shape.PrimitiveCount));

                gameObject.AddComponent(new EffectRenderComponent(effect));
                //gameObject.AddComponent(new BasicEffectRenderComponent(effect));
                gameObject.Transform.SetPosition(RandomHelper.GetRandomVector3(Vector3.One * -100, Vector3.One * 100));
                gameObject.AddComponent(new RotatorComponent(Vector3.Up));
                SystemCore.GetSubsystem<GameObjectManager>().AddAndInitialiseGameObject(gameObject);

                gameObject.Transform.Scale = 5f;
                gameObject.Transform.Velocity = RandomHelper.GetRandomVector3(-Vector3.One, Vector3.One) * 0.01f;
            }


            AddInputBindings();

            //Model geoDesicModel = SystemCore.ContentManager.Load<Model>("Models/geodesic");
            //ProceduralShape geodesicShape = ModelMeshParser.GetShapeFromModelNoUVs(geoDesicModel);
            //geodesicShape.Scale(20f);
            //geodesicShape.InsideOut();
            //GameObject geoDesic = GameObjectFactory.CreateRenderableGameObjectFromShape(geodesicShape, EffectLoader.LoadSM5Effect("cockpitscreen"));
            //SystemCore.GameObjectManager.AddAndInitialiseGameObject(geoDesic);



            var crateObject = AddTestModel("Models/Crate", "Textures/Crate", "DiffuseSpecularTextured");
            crateObject.Transform.SetPosition(new Vector3(0, 0, 0));
            crateObject.Transform.Scale = 0.1f;

            var marioObject = AddTestModel("Models/mario-sculpture", "Textures/marioD", "DiffuseSpecularTextured");
            marioObject.Transform.SetPosition(new Vector3(20, 5, 0));
            marioObject.Transform.Scale = 0.1f;

        }

        private GameObject AddTestModel(string model, string texture, string effect)
        {
            GameObject gameObject = new GameObject();
            gameObject.AddComponent(new ModelComponent(SystemCore.ContentManager.Load<Model>(model)));
            gameObject.AddComponent(new EffectRenderComponent(EffectLoader.LoadSM5Effect(effect)));
            gameObject.AddComponent(new TextureComponent(SystemCore.ContentManager.Load<Texture2D>(texture)));
            gameObject.AddComponent(new RotatorComponent(Vector3.Up, 0.001f));
            SystemCore.GameObjectManager.AddAndInitialiseGameObject(gameObject);
            return gameObject;
        }

        private void AddInputBindings()
        {
            input = SystemCore.GetSubsystem<InputManager>();
            input.AddKeyDownBinding("CameraForward", Keys.Up);
            input.AddKeyDownBinding("CameraBackward", Keys.Down);
            input.AddKeyDownBinding("CameraLeft", Keys.Left);
            input.AddKeyDownBinding("CameraRight", Keys.Right);

            var binding = input.AddKeyPressBinding("WireframeToggle", Keys.Space);
            binding.InputEventActivated += (x, y) => { SystemCore.Wireframe = !SystemCore.Wireframe; };
        }

        public override void Update(GameTime gameTime)
        {
            if (!SystemCore.CursorVisible)
            {
                if (input.EvaluateInputBinding("CameraForward"))
                    mouseCamera.MoveForward();
                if (input.EvaluateInputBinding("CameraBackward"))
                    mouseCamera.MoveBackward();
                if (input.EvaluateInputBinding("CameraLeft"))
                    mouseCamera.MoveLeft();
                if (input.EvaluateInputBinding("CameraRight"))
                    mouseCamera.MoveRight();


                mouseCamera.Update(gameTime, input.MouseDelta.X, input.MouseDelta.Y);
                input.CenterMouse();
            }

            List<GameObject> activeGameObjects = SystemCore.GetSubsystem<GameObjectManager>().GetAllObjects();
            BoundingBox testVolume = new BoundingBox(new Vector3(-100, -100, -100), new Vector3(100, 100, 100));

            foreach (GameObject activeGameObject in activeGameObjects)
            {

                if (testVolume.Contains(activeGameObject.Transform.WorldMatrix.Translation) == ContainmentType.Disjoint)
                {
                    activeGameObject.Transform.SetPosition(-activeGameObject.Transform.WorldMatrix.Translation);
                }

            }

            base.Update(gameTime);
        }

        public override void Render(GameTime gameTime)
        {
            SystemCore.GraphicsDevice.Clear(SystemCore.ActiveColorScheme.Color2);
            DebugShapeRenderer.VisualiseAxes(5f);

            base.Render(gameTime);


        }

        protected void SetCameraMovement(bool active)
        {
            SystemCore.CursorVisible = !active;
        }
    }
}