using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private Text MatchScore;
    private int LeftPlayer,RightPlayer;

    /// <summary>
    /// Устанавливает счёт матча.
    /// </summary>
    void Start()
    {
        FBPP.Start(new FBPPConfig
		{
				SaveFileName = "PlayerPrefs.json",
				AutoSaveData = true,
				ScrambleSaveData = false
		});
        GetScore();
        SetScoreText();
    }

    public void GetScore()
    {
        LeftPlayer = FBPP.GetInt("PlayerScore");
        RightPlayer = FBPP.GetInt("ndPlayerScore");
    }

    public void SetScoreText()
    {
        MatchScore.text = LeftPlayer+":"+RightPlayer;
    }

    public void AddScoreLeft() => FBPP.SetInt("PlayerScore", FBPP.GetInt("PlayerScore")+1);
    public void AddScoreRight() => FBPP.SetInt("ndPlayerScore", FBPP.GetInt("ndPlayerScore")+1);

}
