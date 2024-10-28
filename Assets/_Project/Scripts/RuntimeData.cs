using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    [Serializable]
    public class RuntimeData
    {
        public Pattern Pattern;
        public Level Level;
        public State State;
        public int CurrentLevel;
        public GameObject LevelView;
        public GameObject MouseMask;

        public Vector3 LeftLightPoint;
        public Vector3 RightLightPoint;
        public Vector3 CurrentBeamPosition;
        public LineRenderer CurrentBeamLineRenderer;
        public HashSet<object> RequireBeamUpdates = new();
    }
}