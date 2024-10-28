using DG.Tweening;
using UnityEngine;

namespace Project
{
    [CreateAssetMenu]
    internal class StaticData : ScriptableObject
    {
        public Level[] Levels;
        public float PlacementErrorRadius = 0.1f;
        public ContactFilter2D ContactFilter;
        public float DeathDuration = 0.3f;
        public Ease DeathEase;
        public float BeforeWinDelay = 3f;
        public float AfterTeleportPunch = 0.3f;
        public float EndPositionWalkTime = 2f;

        public string PatternSortingLayerName;
        public int PatternSortingOrder;
        public Vector3 PatternScale = new Vector3(1, 0.5f, 1);
        public float MinPatternDistance = 0.3f;
        [Header("Wheat")]
        public GameObject Wheat;
        public Vector2 WheatDensity;
        public float WheatRandomMove = 0.1f;

        public HealthBar HealthBar;
        public float RockDeathDuration = 0.5f;
        [Header("Mouse Mask")] public GameObject MouseMask;

        [Header("Items")]

        public Material PatternMaterial;
        public GameObject ChainPrefab;
        public Tooltip Tooltip;
        public Vector3 PunchScale = new Vector3(0.5f, 0.5f, 0.5f);
        public float PunchDuration = 0.2f;
        public float PunchFrequency = 5;
        public Ease PunchEase;
        public Color SavedTextColor;
        public GameObject PunchPaticle;
        public string DeathMessage = "Ouch! I am dead!";
        public Color DeathMessageColor;
        public float LoseDelay = 2f;

        [Header("Beam Light")]
        public Material LightMaterial;
        public float SizeOffset = 0.5f;
        public SpriteRenderer BeamEnd;
        public float BeamEndHeight = 3;
        public float BeamEndAdditionalWidth;
        public float BeamFollowTime = 0.5f;
        public Ease BeamFollowEase;
        public string BeamSortingLayerName;
        public int BeamSortingOrder;

        [Header("Route follow")]
        public float MoveSpeed = 5;
        public float FollowRouteWaitTime = 1f;

        [Header("Ufo")]
        public float UfoFlyTime;
        public Ease UfoEase;
        public Ease UfoSuccingScaleEase;

        [Header("Death")]
        public float DeathCameraTime = 0.3f;
        public Ease DeathCameraEase;
        public Ease DeathCameraSizeEase;
        public float DeathCameraSize;
    }
}