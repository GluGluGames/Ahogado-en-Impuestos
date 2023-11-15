using System;
using System.Collections;
using System.Collections.Generic;
using GGG.Classes.Tutorial;
using GGG.Components.Buildings;
using GGG.Components.UI;
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
        Action<string, Sprite, string> OnUiChange)
    {
        InitializeTutorial();
        
        yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, false, false);

        foreach (IEnumerator step in _tutorialSteps)
        {
            yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, true, true);
            yield return step;
        }

        yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, true, true, true);
        
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
        TutorialCompleted = true;

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
        
        while (magnitude < 25f)
        {
            magnitude += Quaternion.Angle(lastCameraRotation, _cameraTransform.transform.rotation);
            lastCameraRotation = _cameraTransform.transform.rotation;
            yield return null;
        }
    }

    private IEnumerator CameraZoomStep()
    {
        Vector3 lastCameraPosition = _mainCamera.localPosition;
        float magnitude = 0f;
        
        while (magnitude < 20f)
        {
            magnitude += (lastCameraPosition - _mainCamera.localPosition).magnitude;
            lastCameraPosition = _mainCamera.localPosition;
            yield return null;
        }
    }
}
