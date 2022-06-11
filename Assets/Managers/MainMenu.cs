/*
Поведение главного меню
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject MenuPanel;
    public GameObject MatchmakingPanel;
    public GameObject CreditsPanel;
    public Button CreditsButton;
    public Button BackButton;
    public Button FindMatchButton;
    public Button SinglePlayerButton;
    public Button CancelMatchmakingButton;
    public InputField NameField;
    public Dropdown PlayersDropdown;
    public Text MatchmakingText;
    private IEnumerator coroutine;
    
    private GameManager gameManager;

    private void Start()
    {
        FBPP.Start(new FBPPConfig
		{
				SaveFileName = "PlayerPrefs.json",
				AutoSaveData = true,
				ScrambleSaveData = false
		});
        FBPP.SetInt("PlayerScore", 0);
        FBPP.SetInt("ndPlayerScore", 0);
        
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        coroutine = TextUpdater();
        if (PlayerPrefs.HasKey("Name"))
        {
            NameField.text = PlayerPrefs.GetString("Name");
        }


        // Слушать события кнопок
        CreditsButton.onClick.AddListener(GoToCredits);
        BackButton.onClick.AddListener(BackFromCredits);
        FindMatchButton.onClick.AddListener(FindMatch);
        SinglePlayerButton.onClick.AddListener(SinglePlayer);
        CancelMatchmakingButton.onClick.AddListener(CancelMatchmaking);
    }


    private void OnDestroy()
    {
        CreditsButton.onClick.RemoveListener(GoToCredits);
        BackButton.onClick.RemoveListener(BackFromCredits);
        FindMatchButton.onClick.RemoveListener(FindMatch);
        SinglePlayerButton.onClick.RemoveListener(SinglePlayer);
        CancelMatchmakingButton.onClick.RemoveListener(CancelMatchmaking);
    }
    
    /// <summary>
    /// Включает кнопку поиска игры
    /// </summary>
    public void EnableFindMatchButton()
    {
        FindMatchButton.interactable = true;
    }

    /// <summary>
    /// Отключает кнопку поиска игры
    /// </summary>
    public void DisableFindMatchButton()
    {
        FindMatchButton.interactable = false;
    }

    /// <summary>
    /// Скрывает главное меню
    /// </summary>
    public void DeactivateMenu()
    {
        StopCoroutine(coroutine);
        MenuPanel.SetActive(true);
        MatchmakingPanel.SetActive(false);
        CreditsPanel.SetActive(false);
        gameObject.SetActive(false);
    }

    IEnumerator TextUpdater () 
    {
        while(true)
        {
            for(int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(1f);
                MatchmakingText.text += ".";
            }

        yield return new WaitForSeconds(1f);
        MatchmakingText.text = MatchmakingText.text.Remove(MatchmakingText.text.Length - 3);
        }
    }

    public void GoToSingle()
    {
       SceneManager.LoadScene("SinglePlayer");
    }

    /// <summary>
    /// Начинает одиночную игру
    /// </summary>
    public void SinglePlayer()
    {
        GoToSingle();
    }

    /// <summary>
    /// Начинает поиск матча
    /// </summary>
    public async void FindMatch()
    {
        MenuPanel.SetActive(false);
        MatchmakingPanel.SetActive(true);
        CreditsPanel.SetActive(false);
        StartCoroutine(coroutine);
        PlayerPrefs.SetString("Name", NameField.text);
        gameManager.SetDisplayName(NameField.text);
        await gameManager.NakamaConnection.FindMatch(int.Parse(PlayersDropdown.options[PlayersDropdown.value].text));
    }

    /// <summary>
    /// Отменяет поиск матча
    /// </summary>
    public async void CancelMatchmaking()
    {
        MenuPanel.SetActive(true);
        MatchmakingPanel.SetActive(false);
        CreditsPanel.SetActive(false);
        StopCoroutine(coroutine);
        await gameManager.NakamaConnection.CancelMatchmaking();
    }

    /// <summary>
    /// Открывает вкладку об игре
    /// </summary>
    public void GoToCredits()
    {
        MenuPanel.SetActive(false);
        MatchmakingPanel.SetActive(false);
        CreditsPanel.SetActive(true);
    }

    /// <summary>
    /// Обратно в главное меню
    /// </summary>
    public void BackFromCredits()
    {
        MenuPanel.SetActive(true);
        MatchmakingPanel.SetActive(false);
        CreditsPanel.SetActive(false);
    }
}
