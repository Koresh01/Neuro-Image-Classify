using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Представляет сериализуемую структуру для хранения нейросети в файле.
/// Используется для сохранения и загрузки значений всех слоёв, весов и смещений.
/// </summary>
[Serializable]
public class NetworkData
{
    public List<string> categoryNames;
    public Vector2Int imageSize;

    public List<SerializableMatrix> t;
    public List<SerializableMatrix> h;
    public List<SerializableMatrix> W;
    public List<SerializableMatrix> B;

    /// <summary>
    /// Пустой конструктор, необходимый для десериализации.
    /// </summary>
    public NetworkData() { }

    /// <summary>
    /// Создает сериализуемую структуру из экземпляра нейросети.
    /// </summary>
    /// <param name="network">Экземпляр нейросети для сериализации.</param>
    public NetworkData(Network network)
    {
        t = ConvertToSerializable(network.t);
        h = ConvertToSerializable(network.h);
        W = ConvertToSerializable(network.W);
        B = ConvertToSerializable(network.B);
    }

    /// <summary>
    /// Преобразует список матриц в сериализуемую форму.
    /// </summary>
    /// <param name="matrices">Список обычных матриц.</param>
    /// <returns>Список сериализованных матриц.</returns>
    private List<SerializableMatrix> ConvertToSerializable(List<Matrix> matrices)
    {
        var result = new List<SerializableMatrix>();
        foreach (var matrix in matrices)
            result.Add(new SerializableMatrix(matrix));
        return result;
    }

    /// <summary>
    /// Применяет сохранённые данные к указанной нейросети.
    /// Перезаписывает внутренние состояния нейросети.
    /// </summary>
    /// <param name="network">Экземпляр нейросети, в которую будут загружены данные.</param>
    public void ApplyTo(Network network)
    {
        network.t = ConvertToMatrix(t);
        network.h = ConvertToMatrix(h);
        network.W = ConvertToMatrix(W);
        network.B = ConvertToMatrix(B);
        network.isReady = true;
    }

    /// <summary>
    /// Преобразует список сериализованных матриц обратно в обычные матрицы.
    /// </summary>
    /// <param name="serialized">Список сериализованных матриц.</param>
    /// <returns>Список восстановленных матриц.</returns>
    private List<Matrix> ConvertToMatrix(List<SerializableMatrix> serialized)
    {
        var result = new List<Matrix>();
        foreach (SerializableMatrix sm in serialized)
            result.Add(sm.ToMatrix());
        return result;
    }
}
