using UnityEngine;

public class WallTriggerController : MonoBehaviour
{
    [SerializeField] GameObject _walls;
    public void ToggleWallVisibility(bool isActive)
    {
        _walls.SetActive(isActive);
    }
}
