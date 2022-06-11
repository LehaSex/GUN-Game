using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SingleHealthController : MonoBehaviour
{
    [SerializeField]
    private Text WinnerNotify;

    private PlayerMove playerMovementController;
    private PlayerWeapon playerWeaponController;
    private PlayerInputController playerInputController;
    [SerializeField]
    private GameObject scoreManager;

    private void Start()
    {
        playerMovementController = GetComponentInChildren<PlayerMove>();
        playerWeaponController = GetComponentInChildren<PlayerWeapon>();
        playerInputController = GetComponent<PlayerInputController>();
    }

    public IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(2);
        if (playerWeaponController.name == "Player")
        {
            scoreManager.GetComponent<ScoreManager>().AddScoreRight();
        }

        if (playerWeaponController.name == "2nd Player")
        {
            scoreManager.GetComponent<ScoreManager>().AddScoreLeft();
        }

        SceneManager.LoadScene("SinglePlayer");
        yield break;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Floor")
        {
            WinnerNotify.text = "Матч будет перезапущен через 2 секунды...";
            StartCoroutine(RestartGame());
        }
    }

}
