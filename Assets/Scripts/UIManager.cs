using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI buffText;
    public TextMeshProUGUI forcePullChargeDisplay;
    public TextMeshProUGUI damageBuffChargeDisplay;
    public TextMeshProUGUI waveNumberDisplay;
    public TextMeshProUGUI gameTimerText;
    public TextMeshProUGUI totalTimeText;
    public Image playerHealthBar;
    public GameObject gameOverMenu;
    public GameObject winMenu;

    readonly int buffTextDuration = 3;

    int gameTimer;

    private Coroutine buffMessageCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(StartGameTimer());
    }

    public void DisplayBuffMessage(string message)
    {
        if (buffMessageCoroutine != null)
        {
            StopCoroutine(buffMessageCoroutine);
        }
        buffMessageCoroutine = StartCoroutine(DisplayMessageCoroutine(message));
    }

    public void UpdateForcePullCharges(int forcePullCharges)
    {
        forcePullChargeDisplay.text = "Pull Charges: " + forcePullCharges;
    }

    public void UpdateDamageBuffCharges(int damageBuffCharges)
    {
        damageBuffChargeDisplay.text = "Buff Charges: " + damageBuffCharges;
    }

    public void UpdateWaveNumber(int waveNumber, int finalWaveNumber, bool bossWave = false)
    {
        if (!bossWave)
        {
            waveNumberDisplay.text = "Wave: " + waveNumber + "/" + finalWaveNumber;
        }
        else
        {
            waveNumberDisplay.text = "Wave: BOSS";
        }
    }

    public void UpdatePlayerHealthBar(float fillAmount)
    {
        playerHealthBar.fillAmount = fillAmount;
    }

    public void UpdateTotalTimeText()
    {
        // get minutes and seconds of timer
        int minutes = gameTimer / 60;
        int seconds = gameTimer % 60;

        totalTimeText.text = string.Format("Total Time: {0:00}:{1:00}", minutes, seconds);
    }

    public void ActivateGameOverMenu()
    {
        gameOverMenu.SetActive(true);
    }

    public void ActivateWinMenu()
    {
        winMenu.SetActive(true);
    }

    private IEnumerator DisplayMessageCoroutine(string message)
    {
        buffText.text = message;
        yield return new WaitForSeconds(buffTextDuration);
        buffText.text = "";
    }

    private IEnumerator StartGameTimer()
    {
        while (true)
        {
            // increment timer each second
            yield return new WaitForSeconds(1);
            gameTimer++;

            // calculate minutes and seconds
            int minutes = gameTimer / 60;
            int seconds = gameTimer % 60;

            // format string in mm:ss format
            gameTimerText.text = string.Format("Time Elapsed: {0:00}:{1:00}", minutes, seconds);
        }
    }
}
