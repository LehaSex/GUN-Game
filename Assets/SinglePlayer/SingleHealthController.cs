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

    private void Start()
    {
        playerMovementController = GetComponentInChildren<PlayerMove>();
        playerWeaponController = GetComponentInChildren<PlayerWeapon>();
        playerInputController = GetComponent<PlayerInputController>();
    }

    public IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("SinglePlayer");
        yield break;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Floor")
        {
            WinnerNotify.text = "Следующий раунд начнётся чeрез 2 секунды...";
            StartCoroutine(RestartGame());
        }
    }

}
