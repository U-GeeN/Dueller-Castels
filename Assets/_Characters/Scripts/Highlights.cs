
using UnityEngine;

public class Highlights : MonoBehaviour
{
    Interactable interactable;

    [System.Serializable] 
    struct Highlighters {
        public GameObject highlighter;
        public bool highlighted;
        public bool selected;
    }

    [SerializeField] Highlighters[] highlights;

    void Awake()
	{
        interactable = GetComponentInParent<Interactable>();
        interactable.OnHighlighted += OnHighlighted;
        interactable.OnEnabled += OnEnabled;
	}

	void Start()
	{
        OnHighlighted(false);
        OnEnabled(false);
	}

    void OnDestroy()
    {
        interactable.OnHighlighted -= OnHighlighted;
        interactable.OnEnabled -= OnEnabled;
    }

    void OnHighlighted(bool isActive) 
    {
        foreach (var item in highlights)
        {
            if (item.highlighted)
            {
                item.highlighter.SetActive(isActive);
            }
        }
    }

    void OnEnabled(bool isActive)
    {
        foreach (var item in highlights)
        {
            if (item.selected)
            {
                item.highlighter.SetActive(isActive);
            }
        }
    }
}