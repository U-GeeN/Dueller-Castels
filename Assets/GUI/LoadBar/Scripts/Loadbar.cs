using UnityEngine;
using UnityEngine.UI;

public class Loadbar: MonoBehaviour {
    [SerializeField] RectTransform barBackground;
    [SerializeField] RectTransform bar;
    [SerializeField] Text titleLabel;
    [SerializeField] Text valueLabel;

    public string titleString;
    public float valueMax;
    public float valueCurrent;
    public float barWidthMin = 10;
    float scale;
    float valueCurrentOld = -1;

    Image barImage;
    float pulseMax;
    float pulseMin;

	// Use this for initialization
	void Start () {
        titleLabel.text = titleString;
        OnRectTransformDimensionsChange();
        barImage = bar.GetComponent<Image>();
        pulseMax = barImage.color.a;

	}

    // Update is called once per frame
    void Update()
    {
        if (valueCurrentOld != valueCurrent)
        {
            UpdateBar(valueCurrent, valueMax);
        }

        if (valueCurrent / valueMax < 0.5f)
        {
            var color = barImage.color;
            color.a = Mathf.PingPong(Time.time, 0.5f) + valueCurrent / valueMax;
            color.a = Mathf.Clamp(color.a, valueCurrent / valueMax, pulseMax);
            barImage.color = color;
        } 
        else
        {
            var color = barImage.color;
            color.a = pulseMax;
            barImage.color = color;
        }

        if (valueCurrent < 1 && barImage.enabled)
        {
            barImage.enabled = false;
        }

        if (valueCurrent > 0 && !barImage.enabled)
        {
            barImage.enabled = true;
        }
    }

    private string FormatNumber(float Number) {
        var NumString = Number + "";
        if (Number / 1000 > 100) {
            NumString = Mathf.Round(Number / 1000) + "K";
        }
        if (Number / 1000000 > 100)
        {
            NumString = Mathf.Round(Number / 1000000) + "M";
        }
        return NumString;
    }

    public void UpdateBar (float valueCurrent, float valueMax) 
    {
        this.valueCurrent = valueCurrent;
        this.valueMax = valueMax;

            bar.sizeDelta = new Vector2(barWidthMin + valueCurrent / valueMax * scale, bar.sizeDelta.y);
            valueCurrent = Mathf.Round(valueCurrent);

        valueLabel.text = FormatNumber(valueCurrent) + " / " + FormatNumber(valueMax);
        valueCurrentOld = valueCurrent;
    }

    public void UpdateBar (float valueCurrent) 
    {
        UpdateBar(valueCurrent, valueMax);
    }

    private void OnRectTransformDimensionsChange()
    {
        scale = barBackground.rect.width - barWidthMin;
    }


}
