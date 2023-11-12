using System;
using System.Collections;
using GGG.Classes.Tutorial;
using UnityEngine;

[CreateAssetMenu(fileName = "InitialTutorial", menuName = "Game/Tutorials/InitialTutorial")]
public class InitialTutorial : TutorialBase
{
    public override IEnumerator StartTutorial(Action OnTutorialStart, Action OnTutorialEnd, 
        Action<string, Sprite, string> OnUiChange)
    {
        yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange);
        yield return CameraMovementStep();
        Debug.Log("Step 1 complete");
        
        // OnTutorialEnd?.Invoke();
    }

    private IEnumerator CameraMovementStep()
    {
        GameObject cameraTransform = GameObject.Find("CameraPivot");
        Vector3 lastCameraPosition = cameraTransform.transform.position;
        float magnitude = 0f;
        
        while (magnitude < 10f)
        {
            magnitude += (lastCameraPosition - cameraTransform.transform.position).magnitude;
            lastCameraPosition = cameraTransform.transform.position;
            yield return null;
        }
    }
}
