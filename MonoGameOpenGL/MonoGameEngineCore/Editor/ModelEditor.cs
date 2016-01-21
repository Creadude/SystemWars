﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameEngineCore.GameObject.Components;
using MonoGameEngineCore.Helper;
using MonoGameEngineCore.Procedural;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MonoGameEngineCore.Rendering.Camera;

namespace MonoGameEngineCore.Editor
{
    public class SimpleModelEditor : IGameSubSystem
    {
        public bool RenderGrid { get; set; }
        public Vector3 CurrentSnapPoint { get; set; }
        public EditMode CurrentMode { get; set; }  
        public int CurrentXIndex { get; set; }
        public int CurrentZIndex { get; set; }
        public int CurrentYIndex { get; set; }

        public enum EditMode
        {
            Vertex,
            Voxel
        }
        private ProceduralShapeBuilder shapeBuilder;
        private string shapeFolderPath = "//Editor//Shapes//";
        private float modellingAreaSize = 8;
        private GameObject.GameObject cameraGameObject;
       
        public SimpleModelEditor()
        {
            RenderGrid = true;
        }

        public void AddTriangle(Vector3 a, Vector3 b, Vector3 c, Color col)
        {
            shapeBuilder.AddTriangleWithColor(col, a, b, c);
        }

        public void AddFace(Color col, params Vector3[] points)
        {
            shapeBuilder.AddFaceWithColor(col, points);
        }

        public void Clear()
        {
            shapeBuilder = new ProceduralShapeBuilder();
        }

        public void SaveCurrentShape(string name)
        {
            ProceduralShape s = shapeBuilder.BakeShape();

            BinaryFormatter bf = new BinaryFormatter();
            using (FileStream fs = new FileStream(name + ".shape", FileMode.Create))
            {
                bf.Serialize(fs, s);
            }

        }

        public ProceduralShape LoadShape(string name)
        {
            BinaryFormatter bf = new BinaryFormatter();
            ProceduralShape s = null;
            using (FileStream fs = new FileStream(name + ".shape", FileMode.Open))
            {
                s = bf.Deserialize(fs) as ProceduralShape;
            }

            return s;
        }

        public void Initalise()
        {
            shapeBuilder = new ProceduralShapeBuilder();

            CurrentMode = EditMode.Voxel;
            cameraGameObject = new GameObject.GameObject("camera");
            cameraGameObject.AddComponent(new ComponentCamera());
            cameraGameObject.Transform.SetPosition(new Vector3(0, 10, 0));
            cameraGameObject.Transform.SetLookAndUp(new Vector3(0, -1, 0), new Vector3(0, 0, 1));
            cameraGameObject.AddComponent(new MouseController());
            SystemCore.GameObjectManager.AddAndInitialiseGameObject(cameraGameObject);
            SystemCore.SetActiveCamera(cameraGameObject.GetComponent<ComponentCamera>());

        }

        public void Update(GameTime gameTime)
        {
            if (SystemCore.Input.KeyPress(Keys.X))
            {
                CurrentXIndex++;
                if (CurrentXIndex > modellingAreaSize/2)
                    CurrentXIndex = -(int)modellingAreaSize/2;
            }

            if (SystemCore.Input.KeyPress(Keys.Y))
            {
                CurrentYIndex++;
                if (CurrentYIndex > modellingAreaSize / 2)
                    CurrentYIndex = -(int)modellingAreaSize / 2;
            }

            if (SystemCore.Input.KeyPress(Keys.Z))
            {
                CurrentZIndex++;
                if (CurrentZIndex > modellingAreaSize / 2)
                    CurrentZIndex = -(int)modellingAreaSize / 2;
            }

        }

        public void Render(GameTime gameTime)
        {
            if (RenderGrid)
            {
                for (float i = -modellingAreaSize / 2; i <= modellingAreaSize / 2; i++)
                {
                    DebugShapeRenderer.AddXYGrid(new Vector3(-modellingAreaSize / 2, -modellingAreaSize / 2, i),
                        modellingAreaSize,
                        modellingAreaSize, 1, (i == CurrentZIndex) ? Color.Green : Color.DarkGray);

                    DebugShapeRenderer.AddXZGrid(new Vector3(-modellingAreaSize / 2, i, -modellingAreaSize / 2),
                        modellingAreaSize,
                        modellingAreaSize, 1, (i == CurrentYIndex) ? Color.Blue : Color.DarkGray);

                    DebugShapeRenderer.AddYZGrid(new Vector3(i, -modellingAreaSize/2, -modellingAreaSize/2),
                        modellingAreaSize, modellingAreaSize, 1, (i == CurrentXIndex) ? Color.OrangeRed : Color.DarkGray);

                }
            }
        }
    }
}