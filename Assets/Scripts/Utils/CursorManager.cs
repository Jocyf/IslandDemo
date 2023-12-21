using UnityEngine;

public class CursorManager : MonoBehaviour
{
	public bool _showCursor = true;

    #region Singleton
    public static CursorManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public void ShowCursor(bool _show)
	{
        _showCursor = _show;
		UpdateCursor();
    }

    private void Start()
    {
		UpdateCursor ();
    }

	private void UpdateCursor()
	{
		Cursor.visible = _showCursor;
		Cursor.lockState = _showCursor ? CursorLockMode.None : CursorLockMode.Locked;
	}
}