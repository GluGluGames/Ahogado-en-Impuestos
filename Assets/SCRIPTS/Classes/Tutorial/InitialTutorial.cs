using GGG.Classes.Tutorial;
using GGG.Components.UI;
using GGG.Components.HexagonalGrid;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InitialTutorial", menuName = "Game/Tutorials/InitialTutorial")]
public class InitialTutorial : TutorialBase
{
    private GameObject _cameraTransform;
    private Transform _mainCamera;
    private List<IEnumerator> _tutorialSteps;
    private LateralUI _lateralUI;
    private HexTile[] _tiles;
    
    public override IEnumerator StartTutorial(Action OnTutorialStart, Action<bool, bool> OnTutorialEnd, 
        Action<string, Sprite, string> OnUiChange, Action<TutorialObjective> OnObjectivesChange)
    {
        InitializeTutorial();
        int i = 0;
        
        yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, false, false);

        foreach (IEnumerator step in _tutorialSteps)
        {
            yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, true, true);
            ObjectivesPanelOpen(OnObjectivesChange, i);
            yield return step;
            ObjectivesPanelOpen(OnObjectivesChange, -1);
            i++;
        }

        yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, true, true, true);
        ObjectivesPanelOpen(OnObjectivesChange, i);
        
        FinishTutorial();
    }

    protected override void InitializeTutorial()
    {
        _cameraTransform = GameObject.Find("CameraPivot");
        _mainCamera = Camera.main.transform;
        _tiles = FindObjectsOfType<HexTile>();
        _lateralUI = FindObjectOfType<LateralUI>();
        
        _lateralUI.ToggleOpenButton();
        
        foreach (HexTile tile in _tiles)
            tile.selectable = false;

        _tutorialSteps = new() {
            CameraMovementStep(),
            CameraRotationStep(),
            CameraZoomStep() 
        };
    }

    protected override void FinishTutorial()
    {
        base.FinishTutorial();

        foreach (HexTile tile in _tiles)
        {
            if (tile.tileType == TileType.Standard) tile.selectable = true;
            else tile.selectable = false;
        }
    }

    private IEnumerator CameraMovementStep()
    {
        Vector3 lastCameraPosition = _cameraTransform.transform.position;
        float magnitude = 0f;
        
        while (magnitude < 10f)
        {
            magnitude += (lastCameraPosition - _cameraTransform.transform.position).magnitude;
            lastCameraPosition = _cameraTransform.transform.position;
            yield return null;
        }
    }

    private IEnumerator CameraRotationStep()
    {
        Quaternion lastCameraRotation = _cameraTransform.transform.rotation;
        float magnitude = 0f;
        
        while (magnitude < 50f)
        {
            magnitude += Quaternion.Angle(lastCameraRotation, _cameraTransform.transform.rotation);
            lastCameraRotation = _cameraTransform.transform.rotation;
            yield return null;
        }
    }

    private IEnumerator CameraZoomStep()
    {
        #if !UNITY_ANDROID
        Vector3 lastCameraZoom = _cameraTransform.transform.localScale;
        float magnitude = 0f;
        
        while (magnitude < 5f)
        {
            magnitude += (lastCameraZoom - _cameraTransform.transform.localScale).magnitude;
            lastCameraZoom = _cameraTransform.transform.localScale;
            yield return null;
        }
        #else
        Vector3 lastCameraPosition = _cameraTransform.transform.localScale;
        float magnitude = 0f;
        
        while (magnitude < 5f)
        {
            magnitude += (lastCameraPosition - _cameraTransform.transform.localScale).magnitude;
            lastCameraPosition = _cameraTransform.transform.localScale;
            yield return null;
        }
        #endif
    }
}
