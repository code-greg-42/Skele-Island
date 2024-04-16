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
    public Image playerHealthBar;

    readonly int buffTextDuration = 3;

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

    private IEnumerator DisplayMessageCoroutine(string message)
    {
        buffText.text = message;
        yield return new WaitForSeconds(buffTextDuration);
        buffText.text = "";
    }
}
