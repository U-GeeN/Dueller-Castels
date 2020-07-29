
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCanvasProperties : MonoBehaviour
{
    public Loadbar healthBar;
    public Loadbar staminaBar;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI damage;
    public float damageValue = 0;
    public RawImage icon;
    public TextMeshProUGUI buttonText;
    public delegate void ButtonAction();
    public ButtonAction m_ButtonAction;
    IDisplayable Displayable;

    // TODO: Action zum updaten bauen
    

    private void Start()
    {
        damage.text = damageValue.ToString();
        gameObject.SetActive(false);
    }

    public void SetDisplayable(IDisplayable newDisplayable)
    {
        if (newDisplayable != null)
        {
            gameObject.SetActive(true);
            Displayable = newDisplayable;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Invoked UI Updates of the displayable.
    /// </summary>
    private void SetProperties()
    {
        SetProperties(Displayable);
    }

    private void FixedUpdate()
    {
        if (Displayable != null)
        {
            SetProperties(Displayable);
        }
    }

    public void SetProperties(IDisplayable displayable)
    {
        SetProperties(displayable.Hitpoints(),
                      displayable.Stamina(),
                      displayable.Damage(),
                      displayable.Name(), 
                      displayable.Icon());

    }

    private void SetProperties(Stat hitPoints, Stat stamina, Stat damage, string name, Texture icon = null)
    {
        healthBar.UpdateBar(hitPoints.current, hitPoints.max);
        staminaBar.UpdateBar(stamina.current, stamina.max);

        this.damage.text = SetValue(damage.current);
        characterName.text = SetValue(name);
        SetIcon(icon);
    }

    private void SetProperties(float hitPoints,
                              float hitPointsMax, 
                              float stamina, 
                              float staminaMax, 
                              float damage, 
                              string name, 
                              Texture icon = null) {
        
        healthBar.UpdateBar(hitPoints, hitPointsMax);
        staminaBar.UpdateBar(stamina, staminaMax);

        this.damage.text = SetValue(damage);
        characterName.text = SetValue(name);
        SetIcon(icon);

    }

    public void ResetProperties()
    {
        SetProperties(0, 0, 0, 0, 0, "", null);
        //CancelInvoke("UpdateDisplayable");

    }


    public string SetValue(float value = -1)
    {
        if (value < 0)
            return "";
        return value + "";
    }

    public string SetValue(string value)
    {
        return value;
    }

    public void SetIcon(Texture icon = null)
    {
        this.icon.texture = icon;
    }

    public void SetButtonAction(ButtonAction action, string buttonName)
    {
        buttonText.text = buttonName;
        m_ButtonAction = action;
    }

    public void OnButtonAction()
    {
        print("button Action");
        m_ButtonAction();
    }

}

// 
public interface IDisplayable
{
    Texture Icon();

    Stat Hitpoints();

    Stat Stamina();

    Stat Damage();

    string Name();

    void UpdateStats();
}
