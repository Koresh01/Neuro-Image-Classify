using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Пользователь сам настраивает сколько промежуточных слоёв нейросети он хочет. Это делается через UI панель "Настройка архитекрутры сети".
/// Текущий скрипт обрабатывает кнопки "сохранить" / "отменить" инициализацию размеров слоёв сети и их кол-ва.
/// </summary>
class NetworkConfigSaver : MonoBehaviour
{
    [Header("Сохранение/отмена:")]
    [SerializeField] Button save;
    [SerializeField] Button abort;


    void OnEnable()
    {
        /*
        При сохранении мы должны взять все дочерние
        
        <LayerElement>
        
        из контейнера
        
        Transform context 

        И посмотреть какое значение сформировано в каждом образце слоя нейросети.

        На основе InputDim, OutDim, и List<int> middleDims построить numpy массив t[] и передать его как то в Network.
        */
    }

    void OnDisable()
    {
        
    }

    

}