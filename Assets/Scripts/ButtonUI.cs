using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour
{
    public Button button;

    public virtual void OnEnable()
    {
        button.onClick.AddListener(ButtonFunction);
    }

    public virtual void OnDisable()
    {
        button.onClick.RemoveListener(ButtonFunction);
    }

    public virtual void ButtonFunction()
    {
        
    }
}
