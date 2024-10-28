using DCFApixels.DragonECS;
using UnityEngine;

namespace Project
{
    public static class LineRendererExt
    {
        [System.ThreadStatic]
        private static Vector3[] _pointsBuffer = new Vector3[64];
        public static void SetLine(this LineRenderer self, Vector3 startPosition, Vector3 endPosition, float startSize, float endSize)
        {
            int count = self.positionCount;
            if (_pointsBuffer.Length < count)
            {
                _pointsBuffer = new Vector3[count];
            }
            self.GetPositions(_pointsBuffer);

            if (Mathf.Approximately(self.startWidth, startSize) || Mathf.Approximately(self.endWidth, endSize))
            {
                var curve = self.widthCurve;

                float num = (endSize - startSize) / 1;
                Keyframe[] keys = new Keyframe[2]
                {
                    new Keyframe(0, startSize, 0f, num),
                    new Keyframe(1, endSize, num, 0f)
                };

                curve.keys = keys;
            }

            switch (count)
            {
                case 0:
                    break;
                case 1:
                    _pointsBuffer[0] = startPosition;
                    break;
                case 2:
                    _pointsBuffer[0] = startPosition;
                    _pointsBuffer[1] = endPosition;
                    break;
                default:
                    {
                        count--;
                        _pointsBuffer[0] = startPosition;
                        _pointsBuffer[count] = endPosition;
                        for (int i = 1; i < count; i++)
                        {
                            float t = (float)i / count;
                            _pointsBuffer[i] = Vector3.Lerp(startPosition, endPosition, t);
                        }
                        count++;
                    }
                    break;
            }

            self.SetPositions(_pointsBuffer);
        }
    }
    public class FollowLightSystem : IEcsRun, IEcsInit
    {
        [EcsInject] RuntimeData _runtimeData;
        [EcsInject] SceneData _sceneData;
        [EcsInject] StaticData _staticData;

        SpriteRenderer _beamEnd;
        Vector3[] _vertices;
        Vector3 _prevPosition;
        Vector3 _prevUfoPosition;

        public void Run()
        {
            if (_prevPosition != _runtimeData.CurrentBeamPosition ||
                _prevUfoPosition != _sceneData.Ufo.transform.position ||
                _runtimeData.RequireBeamUpdates.Count > 0)
            {
                SetTo(_runtimeData.CurrentBeamPosition);
                _prevPosition = _runtimeData.CurrentBeamPosition;
                _prevUfoPosition = _sceneData.Ufo.transform.position;
            }
        }

        public void SetTo(Vector3 center)
        {
            _runtimeData.CurrentBeamLineRenderer.SetLine(_sceneData.UFOBeamStart.position, center, _sceneData.LightWidth, _staticData.SizeOffset * 2f + _staticData.BeamEndAdditionalWidth);

            _beamEnd.transform.position = center;
            _beamEnd.size = new Vector2(_staticData.SizeOffset * 2f + _staticData.BeamEndAdditionalWidth, _staticData.BeamEndHeight);
        }

        public void Init()
        {
            var topLeft = _sceneData.UFOBeamStart.position - Vector3.right * _sceneData.LightWidth * .5f;
            var topRight = _sceneData.UFOBeamStart.position + Vector3.right * _sceneData.LightWidth * .5f;

            _runtimeData.LeftLightPoint = topLeft - Vector3.up;
            _runtimeData.RightLightPoint = topRight - Vector3.up;

            _runtimeData.CurrentBeamPosition = _sceneData.StartBeamPosition.transform.position;
            _runtimeData.CurrentBeamLineRenderer = _sceneData.StartBeamPosition;

            _beamEnd = Object.Instantiate(_staticData.BeamEnd, _sceneData.StartBeamPosition.transform);
            _beamEnd.sortingOrder = _staticData.BeamSortingOrder;
            _beamEnd.sortingLayerName = _staticData.BeamSortingLayerName;

        }
    }
}