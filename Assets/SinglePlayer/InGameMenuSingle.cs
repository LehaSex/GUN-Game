using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Контроллирует поведение внутриигрового меню
/// </summary>
public class InGameMenuSingle : MonoBehaviour
{
    public UnityEvent OnRequestQuitMatch;

    public Button ResumeButton;
    public Button ExitButton;
    private bool isOpen;

    private void Start()
    {
        // Слушать события кнопок
        ResumeButton.onClick.AddListener(Close);
        ExitButton.onClick.AddListener(QuitMatch);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }
    }


    private void OnDestroy()
    {
        // Убрать прослушивание кнопок
        ResumeButton.onClick.RemoveListener(Close);
        ExitButton.onClick.RemoveListener(QuitMatch);
    }

    /// <summary>
    /// Открыть меню
    /// </summary>
    public void Open()
    {
        gameObject.GetComponent<Canvas>().enabled = true;
        GameObject.Find("Player").GetComponent<PlayerInputController>().enabled = false;
        GameObject.Find("2nd Player").GetComponent<ndPlayerInputController>().enabled = false;
        isOpen = true;
    }

    /// <summary>
    /// Закрыть меню
    /// </summary>
    public void Close()
    {
        gameObject.GetComponent<Canvas>().enabled = false;
        GameObject.Find("Player").GetComponent<PlayerInputController>().enabled = true;
        GameObject.Find("2nd Player").GetComponent<ndPlayerInputController>().enabled = true;
        isOpen = false;
    }

    /// <summary>
    /// Покинуть текущий матч и закрыть меню
    /// </summary>
    /// <returns></returns>
    public void QuitMatch()
    {
        Close();
        GoToMenu();
    }

    public void GoToMenu()
    {
       SceneManager.LoadScene("Menu");
    }
}
