using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    [CreateAssetMenu]
    public class Pattern : ScriptableObject
    {
        [Serializable]
        public class ItemPos
        {
            public ItemView Item;
            public Vector3 RelativePosition;

            public bool Random;
            public Vector3 Min;
            public Vector3 Max;
        }

        public List<ItemPos> Items;
    }
}