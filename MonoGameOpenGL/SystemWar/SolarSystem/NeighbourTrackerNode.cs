﻿using Microsoft.Xna.Framework;

namespace MonoGameEngineCore.Procedural
{
    public class NeighbourTrackerNode
    {
        public enum Quadrant
        {
            none,
            se,
            nw,
            ne,
            sw
        }

        public enum CubeSide
        {
            none,
            top,
            bottom,
            front,
            back,
            left,
            right
        }

        public int depth;
        public Vector3 keyPoint;
        public Quadrant quadrant;
        public CubeSide side; 


        public NeighbourTrackerNode()
        {

        }

        public NeighbourTrackerNode(int depth, Vector3 keyPoint)
        {
            this.depth = depth;
            this.keyPoint = keyPoint;
        }

        public override string ToString()
        {
            return depth + " , " + keyPoint.ToString() + " , " + quadrant.ToString() + " , " + side.ToString();
        }
    }
}