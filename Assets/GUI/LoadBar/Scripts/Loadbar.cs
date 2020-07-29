using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Loadbar: MonoBehaviour {
    [SerializeField] RectTransform barBackground;
    [SerializeField] RectTransform bar;
    [SerializeField] TextMeshProUGUI titleLabel;
    [SerializeField] TextMeshProUGUI valueLabel;

    public string titleString;
    public float valueMax;
    private float _valueCurrent;
    public float ValueCurrent
    {
        get { return _valueCurrent; }
        set
        {
            _valueCurrent = value; 
            //UpdateBar(ValueCurrent, valueMax);
            valueLabel.text = FormatNumber(_valueCurrent) + " / " + FormatNumber(valueMax);
            valueCurrentOld = _valueCurrent;
        }
    }
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
        //if (system.math.abs(valuecurrentold - valuecurrent) > mathf.epsilon)
        //{
        //    updatebar(valuecurrent, valuemax);
        //}

        //makes bar pulse when under 50%
        if (ValueCurrent / valueMax < 0.5f)
        {
            var color = barImage.color;
            color.a = Mathf.PingPong(Time.time, 0.5f) + ValueCurrent / valueMax;
            color.a = Mathf.Clamp(color.a, ValueCurrent / valueMax, pulseMax);
            barImage.color = color;
        } 
        else
        {
            var color = barImage.color;
            color.a = pulseMax;
            barImage.color = color;
        }

        //disable bar color when too small
        if (ValueCurrent < 1 && barImage.enabled)
        {
            barImage.enabled = false;
        }

        if (ValueCurrent > 0 && !barImage.enabled)
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
        this.ValueCurrent = valueCurrent;
        this.valueMax = valueMax;

        bar.sizeDelta = new Vector2(barWidthMin + (valueCurrent / valueMax * scale), bar.sizeDelta.y);
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
