using Zenject;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Custom/NetworkConfigPanel (Конфиг слоёв сети)")]
public class NetworkConfigPanel : MonoBehaviour
{
    [Inject] DatasetValidator datasetValidator;

    [Header("Vertical Scroll View:")]
    [SerializeField] Transform content;

    [Tooltip("Эл-т vertical Scroll View")]
    [SerializeField] List<LayerConfig> elements;

    [Header("Сохранение/отмена:")]
    [SerializeField] Button save;
    [SerializeField] Button abort;

    void OnEnable()
    {
        // смотрит прошла ли валидация и если да, то заполняет данные о 1 и последнем слоях
        // иначе посылает нах.

        // Возможность добавления/удаления слоёв

        // При нажатии на кнопку сохранить ссчитываем все промежуточные слои List<LayerConfig> elements;
        // Далее передаём эти данные в Network в метод Init(). Там формируется список слоёв t[], а на его основе уже и h[] и W[]...
    }

    void OnDisable()
    {
        
    }
}
