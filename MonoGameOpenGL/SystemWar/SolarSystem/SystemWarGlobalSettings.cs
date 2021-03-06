﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemWar
{
    public static class SystemWarGlobalSettings
    {
        public static bool FixedTimeStep = true;
        public static bool PhysicsOnBackgroundThread = true;
        public static bool BuildPatchesOnBackgroundThread = false;
        public static bool RepairSeams = false;
        public static bool VisualisePatches = false;
        public static bool RenderQuadtreeConnectivity = false;
        public static bool TerrainCollisionsEnabled = false;
        public static bool EnableQuadTreeInterconnections = true;
    }
}
