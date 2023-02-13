using UnityEngine;
using UnityEngine.UI;

public delegate void OnStart();

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI nameStr;
    [SerializeField] private Button homeButton;
    [SerializeField] private GameObject welcomeScreen;
    [SerializeField] private GameObject loadingScreen;

    public void ShowHome(bool show)
    {
        homeButton.gameObject.SetActive(show);
    }

    public void SetLoading(bool loading)
    {
        loadingScreen.SetActive(loading);
    }

    public void SetPlayerName(string name)
    {
        nameStr.text = nameStr.text.Replace("NAME", name);
    }

    public event OnStart OnStart;

    public void OnStartClicked()
    {

        Debug.Log("Start clicked");
        OnStart?.Invoke();
    }

    public void ToggleMainMenu(bool show)
    {
        welcomeScreen.SetActive(show);
    }
}
