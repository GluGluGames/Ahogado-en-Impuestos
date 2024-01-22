using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GGG.Components.Dialogue;
using GGG.Components.HexagonalGrid;
using GGG.Components.UI;
using UnityEngine;

namespace GGG.Components.Tutorial.Tutorials
{
    public class InitialTutorial : Tutorial
    {
        private List<HexTile> _tiles;
        private LateralUI _lateralUI;

        private HistoryManager _historyManager;
        private GameObject _cameraTransform;

        protected override void InitializeTutorial()
        {
            base.InitializeTutorial();
            
            _cameraTransform = GameObject.Find("CameraPivot");
            
            _tiles = FindObjectsOfType<HexTile>().ToList();
            _tiles.ForEach(x => x.selectable = false);

            _lateralUI = FindObjectOfType<LateralUI>();
            _lateralUI.ToggleOpenButton();
            
            _steps = new List<IEnumerator>
            {
                MovementStep(),
                RotationStep(),
                ZoomStep()
            };
        }

        protected override void TutorialCondition()
        {
            BuildTutorial _buildTutorial = FindObjectOfType<BuildTutorial>(true);
            if (Completed() && !_buildTutorial.Completed()) Restore();
            
            _historyManager = HistoryManager.Instance;
            _historyManager.OnHistoryEnd += StartTutorialNoEnum;
        }

        private IEnumerator MovementStep()
        {
            Vector3 lastPosition = _cameraTransform.transform.position;
            float magnitude = 0;

            while (magnitude < 10f)
            {
                Vector3 position = _cameraTransform.transform.position;
                magnitude += (lastPosition - position).magnitude;
                lastPosition = position;
                yield return null;
            }
        }

        private IEnumerator RotationStep()
        {
            Quaternion lastRotation = _cameraTransform.transform.rotation;
            float magnitude = 0;

            while (magnitude < 50f)
            {
                Quaternion rotation = _cameraTransform.transform.rotation;
                magnitude += Quaternion.Angle(lastRotation, rotation);
                lastRotation = rotation;
                yield return null;
            }
        }

        private IEnumerator ZoomStep()
        {
#if !UNITY_ANDROID
            Vector3 lastZoom = _cameraTransform.transform.localScale;
            float magnitude = 0;

            while (magnitude < 5f)
            {
                Vector3 localScale = _cameraTransform.transform.localScale;
                magnitude += (lastZoom - localScale).magnitude;
                lastZoom = localScale;
                yield return null;
            }
#else
            Vector3 lastPosition = _cameraTransform.transform.localScale;
            float magnitude = 0;
            
            while (magnitude < 5f){
                magnitude += (lastPosition - _camera.transform.localScale).magnitude;
                lastPosition = _cameraTransform.transform.localScale;
                yield return null;
            }
#endif
        }

        protected override void FinishTutorial()
        {
            _historyManager.OnHistoryEnd -= StartTutorialNoEnum;
            _tiles.ForEach(x => x.selectable = x.tileType == TileType.Standard);
            base.FinishTutorial();
        }
    }
}
