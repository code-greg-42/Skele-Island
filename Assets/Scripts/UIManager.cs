using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI buffText;
    public TextMeshProUGUI forcePullChargeDisplay;
    public TextMeshProUGUI damageBuffChargeDisplay;
    public TextMeshProUGUI waveNumberDisplay;

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

    public void UpdateWaveNumber(int waveNumber, int finalWaveNumber)
    {
        waveNumberDisplay.text = "Wave: " + waveNumber + "/" + finalWaveNumber;
    }

    private IEnumerator DisplayMessageCoroutine(string message)
    {
        buffText.text = message;
        yield return new WaitForSeconds(buffTextDuration);
        buffText.text = "";
    }
}
