using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Bar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public TMP_Text amount;

    public Color blinkColor = Color.white;
    public float blinkTime = 0.5f;
    private float remainingBlinkTime;

    private void Start()
    {
        remainingBlinkTime = blinkTime;
    }

    public void SetMax(float value)
    {
        slider.maxValue = value;
    }

    public void SetValue(float value)
    {
        slider.value = value;
        fill.color = gradient.Evaluate(slider.normalizedValue);
        if (amount)
            amount.text = ((int) Math.Round(value)).ToString();
    }

    public void Blink()
    {
        remainingBlinkTime -= Time.deltaTime;
        if (remainingBlinkTime < 0)
            remainingBlinkTime += blinkTime;

        if (remainingBlinkTime > blinkTime/2)
            fill.color = blinkColor;
        else
            fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void ResetBlink()
    {
        remainingBlinkTime = blinkTime;
    }
}
