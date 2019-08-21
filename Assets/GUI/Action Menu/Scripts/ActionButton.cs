using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    public Button button;
    public ActionMenueControl actionMenueController;
    [SerializeField] Image background;
    [SerializeField] Image icon;
    [SerializeField] Text title;
    public int actionNumber = 0;

    public Color color {
        get {
            return background.color;
        }
        set {
            background.color = value;
        }
    }
    public Sprite sprite {
        get {
            return icon.sprite;
        }
        set {
            icon.sprite = value;
        }
    }
    public string label {
        get {
            return title.text;
        }
        set {
            title.text = value;
        }
    }

	public void Start()
	{
        actionMenueController = GetComponentInParent<ActionMenueControl>();
	}
	public void Configure(ActionMenueControl actionMenueControl, Sprite sprite, int actionNumber, Color color, string label = "")
    {
        this.actionMenueController = actionMenueControl;
        this.sprite = sprite;
        this.actionNumber = actionNumber;
        this.color = color;
        this.label = label;
    }

    public void Configure (Sprite sprite, int actionNumber) {
        this.sprite = sprite;
        this.actionNumber = actionNumber;

    }
    // Button Function
    public void ReturnActionNumber () {
        print("return action number " + actionNumber);
        actionMenueController.SetActionNumber(actionNumber);

    }
}
