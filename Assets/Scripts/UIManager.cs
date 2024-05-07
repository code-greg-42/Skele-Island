using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ENCAPSULATION
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    // ENCAPSULATION
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI buffText;
    [SerializeField] private TextMeshProUGUI forcePullChargeDisplay;
    [SerializeField] private TextMeshProUGUI damageBuffChargeDisplay;
    [SerializeField] private TextMeshProUGUI waveNumberDisplay;
    [SerializeField] private TextMeshProUGUI gameTimerText;
    [SerializeField] private TextMeshProUGUI totalTimeText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private Image playerHealthBar;
    [SerializeField] private Image bossHealthBar;
    [SerializeField] private Image castBar;
    [SerializeField] private GameObject playerHealthText;
    [SerializeField] private GameObject castBarBackground;
    [SerializeField] private GameObject playerHealthBarBackground;
    [SerializeField] private GameObject bossHealthBarBackground;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject winMenu;
    [SerializeField] private GameObject startMenu;

    private readonly int buffTextDuration = 3; // time that the UI component for buff messages will display what buff was picked up

    private int gameTimer;

    private Coroutine buffMessageCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateBestTimeText();
    }

    // ENCAPSULATION
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

    public void UpdateCastBar(float fillAmount, bool castTimeMinMet)
    {
        // activate castbar if not already active
        if (!castBarBackground.activeSelf)
        {
            castBarBackground.SetActive(true);
        }

        // check if castbar is full; if not, update the fill amount
        if (castBar.fillAmount < 1.0f)
        {
            castBar.fillAmount = fillAmount;
        }

        // set color to green if button has been held long enough for a shot to occur, otherwise set to red
        castBar.color = castTimeMinMet ? Color.green : Color.red;
    }

    public void UpdateTotalTimeText()
    {
        // get minutes and seconds of timer
        int minutes = gameTimer / 60;
        int seconds = gameTimer % 60;

        // format string in mm:ss format
        totalTimeText.text = string.Format("Total Time: {0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateBestTimeText()
    {
        int bestTime = DataManager.Instance.GetBestWinTime();

        if (bestTime != 0)
        {
            int minutes = bestTime / 60;
            int seconds = bestTime % 60;
            // format string in mm:ss format
            bestTimeText.text = string.Format("Best Time: {0:00}:{1:00}", minutes, seconds);
        }
    }

    public void UpdateBestTime()
    {
        DataManager.Instance.SetBestWinTime(gameTimer);
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
