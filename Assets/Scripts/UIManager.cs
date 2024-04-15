using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI buffText;
    public TextMeshProUGUI forcePullChargeDisplay;

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

    private IEnumerator DisplayMessageCoroutine(string message)
    {
        buffText.text = message;
        yield return new WaitForSeconds(buffTextDuration);
        buffText.text = "";
    }
}
