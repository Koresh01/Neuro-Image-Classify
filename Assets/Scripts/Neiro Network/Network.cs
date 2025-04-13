using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Нейросеть обучаемая.
/// </summary>
public class Network : MonoBehaviour
{
    [Header("Слои:")]
    public List<Matrix> s;

    [Header("Активированные слои:")]
    public List<Matrix> h;

    [Header("Матрицы весов:")]
    public List<Matrix> m;

    [Header("Нелинейности:")]
    public List<Matrix> b;

    void Start()
    {
        Matrix x = new Matrix(new double[,] {
            { 1, 2 }
        });
        Matrix W = new Matrix(new double[,] {
            { 5, 6 },
            { 7, 8 }
        });
        Matrix b = new Matrix(new double[,] {
            { 1, 1 }
        });

        m.Add(x); m.Add(W); m.Add(b);

        Matrix result = x * W.T() + b;
        Debug.Log(result);
    }
}
