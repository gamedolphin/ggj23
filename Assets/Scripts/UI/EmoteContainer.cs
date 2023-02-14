using UnityEngine;

public class EmoteContainer : MonoBehaviour
{
    private bool show = false;

    private void Start()
    {
        Toggle(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            show = !show;
            Toggle(show);
        }
    }

    private void Toggle(bool open)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(open);
        }
    }
}
