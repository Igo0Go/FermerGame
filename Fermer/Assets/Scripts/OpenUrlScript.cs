using UnityEngine;

public class OpenUrlScript : MonoBehaviour
{
    [SerializeField] private string url;

    public void OpenUrl()
    {
        Application.OpenURL("https://nightales.itch.io/");
    }
}
