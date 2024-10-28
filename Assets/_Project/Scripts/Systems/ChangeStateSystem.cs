using DCFApixels.DragonECS;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project
{
    public class ChangeStateSystem : IEcsRun, IEcsInit
    {
        class Aspect : EcsAspectAuto
        {
            [Inc] public EcsPool<ChangeState> ChangeStates;
            [Exc] EcsPool<Delay> Delays;
        }

        [EcsInject] EcsDefaultWorld _world;
        [EcsInject] RuntimeData _runtimeData;
        [EcsInject] SceneData _sceneData;
        [EcsInject] StaticData _staticData;

        public void Run()
        {
            if (Application.isEditor && Input.GetKeyDown(KeyCode.W))
            {
                _world.GetPool<ChangeState>().Add(_world.NewEntity()).Value = State.Win;
            }

            foreach (var e in _world.Where(out Aspect a))
            {
                var changeState = a.ChangeStates.Get(e);
                switch (changeState.Value)
                {
                    case State.Game:
                        break;
                    case State.Win:
                        if (_runtimeData.CurrentLevel == _staticData.Levels.Length - 1)
                        {
                            _sceneData.UI.FinalWinScreen.gameObject.SetActive(true);
                        }
                        else
                        {
                            Profile.CurrentLevel = _runtimeData.CurrentLevel + 1;
                            NextLevel(0);
                        }

                        //_sceneData.UI.WinScreen.Show(true);
                        break;
                    case State.Lose:
                        NextLevel(_staticData.LoseDelay);
                        //_sceneData.UI.LoseScreen.Show(true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                _runtimeData.State = changeState.Value;
                a.ChangeStates.Del(e);
            }
        }

        private async void NextLevel(float delay)
        {
            await Awaitable.WaitForSecondsAsync(delay);

            _sceneData.UI.UIDissolve.DOKill();
            DOTween.To(() => _sceneData.UI.UIDissolve.effectFactor, x => _sceneData.UI.UIDissolve.effectFactor = x, 0, _sceneData.UI.DissolveTime).SetTarget(_sceneData.UI.UIDissolve).SetEase(_sceneData.UI.DissolveEase);
            
            await Awaitable.WaitForSecondsAsync(_sceneData.UI.DissolveTime);
            SceneManager.LoadScene("Loading");
        }

        public void Init()
        {
            _sceneData.UI.FinalWinScreen.ResetProgress.onClick.AddListener(OnResetProgress);
            _sceneData.UI.FinalWinScreen.RestartLastLevel.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            NextLevel(0);
        }

        private void OnResetProgress()
        {
            PlayerPrefs.DeleteAll();
            NextLevel(0);
        }
    }
}