using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    public Button button;
    public ActionMenueControl actionMenueController;
    [SerializeField] private Image background;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI title;
    public int actionNumber = 0;

    public Color Color
    {
        get {
            return background.color;
        }
        set {
            background.color = value;
        }
    }
    public Sprite Sprite
    {
        get {
            return icon.sprite;
        }
        set {
            icon.sprite = value;
        }
    }
    public string Label
    {
        get {
            return title.text;
        }
        set {
            string value1 = value;
            title.text = value1;
        }
    }

	public void Start()
	{
        actionMenueController = GetComponentInParent<ActionMenueControl>();
	}
    public void Configure(ActionMenueControl actionMenueControl, Sprite sprite, int actionNumber, Color color, string label = "")
    {
        actionMenueController = actionMenueControl;
        this.Sprite = sprite;
        this.actionNumber = actionNumber;
        this.Color = color;
        Label = label;
    }

    public void Configure(Sprite sprite, int actionNumber)
    {
        this.Sprite = sprite;
        this.actionNumber = actionNumber;
    }

    // Button Function
    public void ReturnActionNumber()
    {
        print("action number " + actionNumber);
        actionMenueController.SetActionNumber(actionNumber);
    }
}
