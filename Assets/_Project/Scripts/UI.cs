using Coffee.UIEffects;
using DG.Tweening;
using UnityEngine;

namespace Project
{
    internal class UI : MonoBehaviour
    {
        public UIDissolve UIDissolve;
        public WinScreen WinScreen;
        public WinScreen LoseScreen;

        public Ease DissolveEase;
        public float DissolveTime = 0.5f;
        public FinalWinScreen FinalWinScreen;
    }
}