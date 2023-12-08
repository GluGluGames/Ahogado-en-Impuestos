using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera _camera;

    // Start is called before the first frame update
    private void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.LookAt(transform.position + _camera.transform.rotation * Vector3.back, _camera.transform.rotation * Vector3.up);
    }
}