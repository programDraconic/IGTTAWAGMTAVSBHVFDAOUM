using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string tutorialSceneName = "Tutorial";
    [SerializeField] private GameObject quitButton;

    private void Start()
    {
        Debug.Log("MainMenu started.");

#if UNITY_WEBGL
        if (quitButton != null) quitButton.SetActive(false);
#endif
    }

    public void OnPlayClicked()
    {
        Debug.Log("Play clicked. Loading: " + tutorialSceneName);
        SceneManager.LoadScene(tutorialSceneName);
    }

    public void OnQuitClicked()
    {
        Debug.Log("Quit clicked.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}