using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class FinalGameDispetcher : MonoBehaviour
{
    [SerializeField] private PlayableDirector cinematic;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Invoke("StartTimeline", 2);
    }

    public void OnPanelShowing()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitGame() => Application.Quit();

    public void Restart() => SceneManager.LoadScene(0);

    public void StartTimeline() => cinematic.Play();
}
