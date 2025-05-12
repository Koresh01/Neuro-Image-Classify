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
        
    }

    void OnDisable()
    {
        
    }

}