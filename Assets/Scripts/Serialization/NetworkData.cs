using System.Collections.Generic;
using System;


/// <summary>
/// Представляет сериализуемую структуру для хранения нейросети в файле.
/// Используется для сохранения и загрузки значений всех слоёв, весов и смещений.
/// </summary>
    [Serializable]
public class NetworkData
{
    public List<Matrix> t;
    public List<Matrix> h;
    public List<Matrix> W;
    public List<Matrix> B;
}