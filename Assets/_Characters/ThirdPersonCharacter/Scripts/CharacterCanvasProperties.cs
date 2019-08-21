
using UnityEngine;
using UnityEngine.UI;

public class CharacterCanvasProperties: MonoBehaviour
{
    InteractableControl selectableController;
    public Loadbar healthBar;
    public Loadbar staminaBar;
    public Text characterName;
    public Text damage;
    public float damageValue;
    public RawImage icon;
    public Text buttonText;
    public delegate void ButtonAction();
    public ButtonAction m_ButtonAction;
    IDisplayable displayable;

    private void Start()
    {
        selectableController = GetComponentInParent<InteractableControl>();
    }

	public void SetDisplayable(IDisplayable newDisplayable) {
        print("new displayable " + displayable.Name());
        this.displayable = newDisplayable;
        InvokeRepeating("UpdateDisplayable", 0.0f, 0.1f);
    }

    public void SetProperties(IDisplayable displayable)
    {
        SetProperties(displayable.Hitpoints(), 
                      displayable.HitpointsMax(), 
                      displayable.Stamina(), 
                      displayable.StaminaMax(),
                      displayable.Damage(),
                      displayable.Name(), 
                      displayable.Icon());
       
    }

    public void SetProperties(float hitPoints, 
                              float hitPointsMax, 
                              float stamina, 
                              float staminaMax, 
                              float damage, 
                              string name, 
                              Texture icon = null) {
        
        healthBar.UpdateBar(hitPoints, hitPointsMax);
        staminaBar.UpdateBar(stamina, staminaMax);

        this.damage.text = SetIntValue(damage);
        characterName.text = SetStringValue(name);
        SetIcon(icon);

    }

    public void SetProperties()
    {
        SetProperties(0, 0, 0, 0, 0, "", null);
        CancelInvoke("UpdateDisplayable");
    }

 
    public string SetIntValue(float value = -1)
    {
        if (value < 0)
            return "";
        else
            return value + "";
    }

    public void SetIcon(Texture icon = null)
    {
        this.icon.texture = icon;
    }

    public string SetStringValue(string value)
    {
         return value;
    }

    public void SetButtonAction(ButtonAction action, string buttonName)
    {
        buttonText.text = buttonName;
        m_ButtonAction = action;
    }

    public void OnButtonAction () 
    {
        print("button Action");
        m_ButtonAction();
    }

    /// <summary>
    /// Invoked UI Updates of the displayable.
    /// </summary>
    private void UpdateDisplayable() {
        SetProperties(displayable);
    }
}

// 
public interface IDisplayable
{
    Texture Icon();

    float Hitpoints();

    float HitpointsMax();

    string Name();

    float Damage();

    float Stamina();

    float StaminaMax();

    void UpdateStats();
}