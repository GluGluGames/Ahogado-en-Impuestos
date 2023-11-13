using System;
using System.Collections;
using System.Collections.Generic;
using GGG.Classes.Tutorial;
using UnityEngine;

[CreateAssetMenu(fileName = "InitialTutorial", menuName = "Game/Tutorials/InitialTutorial")]
public class InitialTutorial : TutorialBase
{
    private GameObject _cameraTransform;
    private Transform _mainCamera;
    
    public override IEnumerator StartTutorial(Action OnTutorialStart, Action<bool> OnTutorialEnd, 
        Action<string, Sprite, string> OnUiChange)
    {
        _cameraTransform = GameObject.Find("CameraPivot");
        _mainCamera = Camera.main.transform; 

        List<IEnumerator> tutorialSteps = new()
        {
            CameraMovementStep(),
            CameraRotationStep(),
            CameraZoomStep()
        };
        
        yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, false);
        yield return null;

        foreach (IEnumerator step in tutorialSteps)
        {
            yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, true);
            yield return step;
        }

        yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, true, true);
        TutorialCompleted = true;
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
