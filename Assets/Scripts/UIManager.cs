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
    public Image bossHealthBar;
    public Image castBar;
    public GameObject playerHealthText;
    public GameObject castBarBackground;
    public GameObject playerHealthBarBackground;
    public GameObject bossHealthBarBackground;
    public GameObject gameOverMenu;
    public GameObject winMenu;
    public GameObject startMenu;

    readonly int buffTextDuration = 3; // time that the UI component for buff messages will display what buff was picked up

    int gameTimer;

    private Coroutine buffMessageCoroutine;

    private void Awake()
    {
        Instance = this;
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

    public void UpdateBossHealthBar(float fillAmount)
    {
        bossHealthBar.fillAmount = fillAmount;
    }

    public void UpdateCastBar(float fillAmount)
    {
        castBar.fillAmount = fillAmount;
    }

    public void UpdateTotalTimeText()
    {
        // get minutes and seconds of timer
        int minutes = gameTimer / 60;
        int seconds = gameTimer % 60;

        // format string in mm:ss format
        totalTimeText.text = string.Format("Total Time: {0:00}:{1:00}", minutes, seconds);
    }

    public bool IsCastBarActive()
    {
        // used for preventing repetitive activation of castbar
        return castBarBackground.activeSelf;
    }

    public bool IsCastBarFull()
    {
        // using threshold to avoid floating point precision issues
        return castBar.fillAmount >= 0.999f;
    }

    public void ActivateCastBar()
    {
        castBarBackground.SetActive(true);
    }

    public void DeactivateCastBar()
    {
        // reset fill amount and deactivate
        castBarBackground.SetActive(false);
        castBar.fillAmount = 0;
    }

    public void ActivateBossHealthBar()
    {
        bossHealthBarBackground.SetActive(true);
    }

    public void ActivateGameOverMenu()
    {
        gameOverMenu.SetActive(true);
    }

    public void ActivateWinMenu()
    {
        winMenu.SetActive(true);
    }

    public void ActivateInGameUI()
    {
        // hide the start menu
        startMenu.SetActive(false);

        // display all in game UI components
        forcePullChargeDisplay.gameObject.SetActive(true); // left
        damageBuffChargeDisplay.gameObject.SetActive(true); // left
        waveNumberDisplay.gameObject.SetActive(true); // top left
        gameTimerText.gameObject.SetActive(true); // bottom right
        playerHealthText.SetActive(true); // bottom left
        playerHealthBarBackground.SetActive(true); // bottom left
        StartCoroutine(StartGameTimer());
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
