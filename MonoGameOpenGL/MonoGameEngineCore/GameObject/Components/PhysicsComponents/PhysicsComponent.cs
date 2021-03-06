﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using Microsoft.Xna.Framework;
using MonoGameEngineCore.Helper;
using XNAMatrix = Microsoft.Xna.Framework.Matrix;
using Matrix = BEPUutilities.Matrix;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.NarrowPhaseSystems.Pairs;

namespace MonoGameEngineCore.GameObject.Components
{

    public enum PhysicsMeshType
    {
        sphere,
        box
    }

    public class PhysicsComponent : IComponent, IUpdateable, IDisposable
    {
        public GameObject ParentObject { get; set; }
        public Entity PhysicsEntity { get; private set; }
        public bool Simulated { get; set; }
        public PhysicsMeshType PhysicsMeshType { get; private set; }
        private bool movable;

        public PhysicsComponent(Entity physicsEntity, bool movable, bool simulated)
        {
            Enabled = true;
            Simulated = simulated;
            this.PhysicsEntity = physicsEntity;
            if (!movable)
                PhysicsEntity.BecomeKinematic();

        }

        public PhysicsComponent(bool movable, bool simulated, PhysicsMeshType type)
        {
            this.PhysicsMeshType = type;
            Enabled = true;
            Simulated = simulated;
            this.movable = movable;

        }

        public void PostInitialise()
        {

        }

        public void Initialise()
        {
            if (PhysicsEntity == null)
            {
                if (PhysicsMeshType == PhysicsMeshType.box)
                    GenerateBoxCollider();
                if (PhysicsMeshType == PhysicsMeshType.sphere)
                    GenerateSphereCollider();

                if (!movable)
                    PhysicsEntity.BecomeKinematic();
            }

            PhysicsEntity.Tag = ParentObject;
            PhysicsEntity.CollisionInformation.Tag = ParentObject;

            if (!SystemCore.PhysicsOnBackgroundThread)
            {
                PhysicsEntity.WorldTransform =
                    MonoMathHelper.GenerateBepuMatrixFromMono(ParentObject.Transform.AbsoluteTransform);
                SystemCore.PhysicsSimulation.Add(PhysicsEntity);
            }
            else
            {
                PhysicsEntity.BufferedStates.States.WorldTransform =
                    MonoMathHelper.GenerateBepuMatrixFromMono(ParentObject.Transform.AbsoluteTransform);
                SystemCore.PhysicsSimulation.SpaceObjectBuffer.Add(PhysicsEntity);
            }


        }

      

        private void GenerateSphereCollider()
        {
            RenderGeometryComponent geometry = ParentObject.GetComponent<RenderGeometryComponent>();
            List<Vector3> verts = geometry.GetVertices();
            BoundingSphere sphere = BoundingSphere.CreateFromPoints(verts);

            PhysicsEntity = new Sphere(MonoMathHelper.Translate(ParentObject.Transform.AbsoluteTransform.Translation),
                sphere.Radius, 1);
            PhysicsEntity.Tag = ParentObject;
        }

        private void GenerateBoxCollider()
        {
            RenderGeometryComponent geometry = ParentObject.GetComponent<RenderGeometryComponent>();
            List<Vector3> verts = geometry.GetVertices();
            BoundingBox testBox = BoundingBox.CreateFromPoints(verts);
            float width = testBox.Max.X - testBox.Min.X;
            float height = testBox.Max.Y - testBox.Min.Y;
            float depth = testBox.Max.Z - testBox.Min.Z;

            PhysicsEntity = new Box(new BEPUutilities.Vector3(ParentObject.Transform.AbsoluteTransform.Translation.X,
                ParentObject.Transform.AbsoluteTransform.Translation.X,
                ParentObject.Transform.AbsoluteTransform.Translation.X), width, height, depth, 1);

            
        }

        public bool Enabled { get; set; }

        public event EventHandler<EventArgs> EnabledChanged;

        public void Update(GameTime gameTime)
        {
            if (!SystemCore.PhysicsOnBackgroundThread)
            {
                if (Simulated)
                {

                   
                    ParentObject.Transform.AbsoluteTransform = MonoMathHelper.GenerateMonoMatrixFromBepu(PhysicsEntity.WorldTransform);
                    
                }
                else
                {
                    PhysicsEntity.WorldTransform =
                        MonoMathHelper.GenerateBepuMatrixFromMono(ParentObject.Transform.AbsoluteTransform);
                }
            }
            else
            {
                if (Simulated)
                    ParentObject.Transform.AbsoluteTransform =
                        MonoMathHelper.GenerateMonoMatrixFromBepu(PhysicsEntity.BufferedStates.States.WorldTransform);
                else
                {
                    PhysicsEntity.BufferedStates.States.WorldTransform =
                        MonoMathHelper.GenerateBepuMatrixFromMono(ParentObject.Transform.AbsoluteTransform);
                }
            }
        }

        public void DoCollisionResponse(float speed)
        {
            
        }

        public int UpdateOrder { get; set; }

        public event EventHandler<EventArgs> UpdateOrderChanged;

        public bool InCollision()
        {
            if (PhysicsEntity.CollisionInformation.Pairs.Count > 0)
                if (PhysicsEntity.CollisionInformation.Pairs[0].Contacts.Count > 0)
                    return true;
            return false;
        }

        public bool CollidedWithEntity(int entityID)
        {
            for (int i = 0; i < PhysicsEntity.CollisionInformation.Pairs.Count; i++)
            {
                if ((int)PhysicsEntity.CollisionInformation.Pairs[i].EntityA.Tag == entityID)
                    return true;

                if ((int)PhysicsEntity.CollisionInformation.Pairs[i].EntityB.Tag == entityID)
                    return true;
            }
            return false;
        }

        internal void SetPosition(Vector3 position)
        {
            //not yet initialized
            if(PhysicsEntity == null)
                return;
            
            PhysicsEntity.WorldTransform =
                MonoMathHelper.GenerateBepuMatrixFromMono(ParentObject.Transform.AbsoluteTransform);
        }

        public void Dispose()
        {
            SystemCore.PhysicsSimulation.Remove(PhysicsEntity);
        }
    }


}
