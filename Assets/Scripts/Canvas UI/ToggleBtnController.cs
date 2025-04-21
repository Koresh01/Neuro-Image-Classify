using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[AddComponentMenu("Custom/ToggleBtnController (Контроллер кнопки с режимом переключения)")]
public class ToggleBtnController : MonoBehaviour
{
    [SerializeField] Button btn;
    [SerializeField] Color pressedColor;
    Color originalColor;
    [SerializeField] UnityEvent toggleOn;
    [SerializeField] UnityEvent toggleOff;
    bool pressed = false;

    void Awake()
    {
        originalColor = btn.image.color;
    }

    void OnEnable()
    {
        btn.onClick.AddListener(Toggle);
    }

    void OnDisable()
    {
        btn.onClick.RemoveAllListeners();
    }

    void Toggle()
    {
        if (!pressed)
        {
            pressed = true;
            toggleOn?.Invoke();

            btn.image.color = pressedColor;
        }

        else if (pressed)
        {
            pressed = false;
            toggleOff?.Invoke();

            btn.image.color = originalColor;
        }
    }
}