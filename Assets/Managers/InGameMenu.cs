using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Контроллирует поведение внутриигрового меню
/// </summary>
public class InGameMenu : MonoBehaviour
{
    public UnityEvent OnRequestQuitMatch;

    public Button ResumeButton;
    public Button ExitButton;

    private bool isOpen;

    private void Start()
    {
        //Инициализируйте событие OnRequestQuitMatch, если требуется.
        if (OnRequestQuitMatch == null)
        {
            OnRequestQuitMatch = new UnityEvent();
        }

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
        GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<PlayerInputController>().enabled = false;
        isOpen = true;
    }

    /// <summary>
    /// Закрыть меню
    /// </summary>
    public void Close()
    {
        gameObject.GetComponent<Canvas>().enabled = false;
        GameObject.FindGameObjectWithTag("LocalPlayer").GetComponent<PlayerInputController>().enabled = true;
        isOpen = false;
    }

    /// <summary>
    /// Покинуть текущий матч и закрыть меню
    /// </summary>
    /// <returns></returns>
    public void QuitMatch()
    {
        OnRequestQuitMatch.Invoke();
        Close();
    }
}
