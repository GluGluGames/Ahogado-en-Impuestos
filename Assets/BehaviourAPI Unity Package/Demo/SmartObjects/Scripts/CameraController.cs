using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float _speed;
    Camera m_Camera;

    private void Awake()
    {
        m_Camera = Camera.main;
    }

    private void Update()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        m_Camera.transform.position += movement * _speed * Time.deltaTime;
    }
}
