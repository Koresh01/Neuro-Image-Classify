using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Пример использования системы графика: добавление, удаление и очистка через UI.
/// </summary>
public class GraphExample : MonoBehaviour
{
    [Header("Graph placeholder:")]
    public Image graphImage;

    [Header("UI buttons:")]
    public InputField inputX;
    public InputField inputY;
    public Button buttonAdd;
    public Button buttonRemove;
    public Button buttonClear;

    private GraphPlot plot;

    void Start()
    {
        plot = new GraphPlot(graphImage, 512, 512);

        buttonAdd.onClick.AddListener(() =>
        {
            if (float.TryParse(inputX.text, out float x) && float.TryParse(inputY.text, out float y))
            {
                plot.AddPoint(x, y);
            }
        });

        buttonRemove.onClick.AddListener(() => plot.RemoveLast());
        buttonClear.onClick.AddListener(() => plot.Clear());
    }
}
