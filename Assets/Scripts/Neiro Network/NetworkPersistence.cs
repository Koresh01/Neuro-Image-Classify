using UnityEngine;
using Zenject;
using System.IO;
using SFB; // Не забудь подключить StandaloneFileBrowser

/// <summary>
/// Импорт/экспорт нейросети в файл из файла.
/// </summary>
public class NetworkPersistence : MonoBehaviour
{
    [Inject] private Network network;

    [AddComponentMenu("Экспорт в .json")]
    public void Save()
    {
        string filePath = GetSaveFilePath();
        if (string.IsNullOrEmpty(filePath)) return;

        NetworkData data = new NetworkData
        {
            t = network.t,
            h = network.h,
            W = network.W,
            B = network.B
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"Сеть сохранена в файл: {filePath}");
    }

    [AddComponentMenu("Импорт из .json")]
    public void Load()
    {
        string filePath = GetLoadFilePath();
        if (string.IsNullOrEmpty(filePath)) return;

        if (!File.Exists(filePath))
        {
            Debug.LogError($"Файл не найден: {filePath}");
            return;
        }

        string json = File.ReadAllText(filePath);
        NetworkData data = JsonUtility.FromJson<NetworkData>(json);

        network.t = data.t;
        network.h = data.h;
        network.W = data.W;
        network.B = data.B;

        network.isReady = true;
        Debug.Log($"Сеть загружена из файла: {filePath}");
    }

    /// <summary>
    /// Показывает окно выбора файла для ЗАГРУЗКИ .json
    /// </summary>
    private string GetLoadFilePath()
    {
        var extensions = new[] {
            new ExtensionFilter("JSON файл", "json")
        };

        var paths = StandaloneFileBrowser.OpenFilePanel("Выберите .json файл нейросети", "", extensions, false);
        return (paths.Length > 0 && !string.IsNullOrEmpty(paths[0])) ? paths[0] : null;
    }

    /// <summary>
    /// Показывает окно выбора места для СОХРАНЕНИЯ .json
    /// </summary>
    private string GetSaveFilePath()
    {
        var extension = new ExtensionFilter("JSON файл", "json");

        string defaultName = "network.json";
        string path = StandaloneFileBrowser.SaveFilePanel("Сохранить сеть в .json", "", defaultName, extension.Extensions[0]);

        return !string.IsNullOrEmpty(path) ? path : null;
    }
}
