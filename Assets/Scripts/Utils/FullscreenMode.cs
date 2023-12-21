using UnityEngine;

public class FullscreenMode : MonoBehaviour
{
    private bool isFullscreen = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) { SetFullscreen(!isFullscreen); }
    }

    private void SetFullscreen(bool _active)
    {
        isFullscreen = _active;
        Screen.fullScreen = _active;
        CursorManager.Instance.ShowCursor(false);
    }

}