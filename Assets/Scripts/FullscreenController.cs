using UnityEngine;

public class FullscreenController : MonoBehaviour
{
    [Tooltip("If true, force fullscreen mode when the button is pressed.")]
    public bool enableFullscreen = true;

    public void EnterFullscreen()
    {
        if (!enableFullscreen)
            return;

#if UNITY_EDITOR
        Debug.Log("FullscreenController: EnterFullscreen called.");
#endif
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.fullScreen = true;
    }

    public void ExitFullscreen()
    {
#if UNITY_EDITOR
        Debug.Log("FullscreenController: ExitFullscreen called.");
#endif
        Screen.fullScreen = false;
    }
}
