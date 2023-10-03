using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraSelectionRaycaster : MonoBehaviour
{

    public Camera camera;
    private Ray _ray;
    private HexTile _target;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        _ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(_ray, out hit))
        {
            Transform objectHit = hit.transform;

            if (objectHit.TryGetComponent<HexTile>(out _target)) 
            {
                _target.OnHighlightTile();

                if(Input.GetMouseButtonDown(0))
                {
                    _target.OnSelectTile();
                }
            }
        }

    }
}

