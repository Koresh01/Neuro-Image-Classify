using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

/*
 
INPUT_DIM

   v

 LAYER 1

   v

   H1 (Relu)
   
   v

 LAYER 2

   v

Softmax()

 */

/// <summary>
/// Нейросеть обучаемая.
/// </summary>
public class Network : MonoBehaviour
{
    [Inject] DatasetValidator datasetValidator;

    [Header("Слои:")]
    public List<Matrix> s;

    [Header("Активированные слои:")]
    public List<Matrix> h;

    [Header("Матрицы весов:")]
    public List<Matrix> W;

    [Header("Нелинейности:")]
    public List<Matrix> B;

    void OnEnable()
    {
        datasetValidator.OnReady += Init;
    }

    private void OnDisable()
    {
        datasetValidator.OnReady -= Init;
    }

    #region ИНИЦИАЛИЗАЦИЯ
    /// <summary>
    /// Датасет валиден, значит указываем конфигурацию нейросети.
    /// </summary>
    void Init()
    {
        InitIntermediateLayers();
        InitActivatedLayers();
        InitWeights();
        InitBiases();
    }

    /// <summary>
    /// Инициализирует промежуточные слои.
    /// </summary>
    void InitIntermediateLayers()
    {
        // s.Clear();

        int inputDim = datasetValidator.imageSize[0] * datasetValidator.imageSize[1];
        int outputDim = datasetValidator.categoryNames.Count;

        // Входной слой
        s[0] = Numpy.Zeros(1, inputDim);

        // Промежуточные слои добавляются через инспектор —> пропускаем

        // Выходной слой
        s[s.Count - 1] = Numpy.Zeros(1, outputDim);
    }

    /// <summary>
    /// Инициализирует слои активационные Relu или Sigmoid.
    /// </summary>
    void InitActivatedLayers()
    {
        h = new List<Matrix>();

        for (int i = 0; i < s.Count; i++)
        {
            h.Add(Numpy.Zeros(1, s[i].GetLength(1))); // Вход и выход не активируем
        }
    }

    /// <summary>
    /// Инициализирует веса.
    /// </summary>
    void InitWeights()
    {
        W.Clear();

        for (int i = 0; i < s.Count-1; i++)
        {
            // Матрицы весов между слоями:
            int curColumns = s[i].GetLength(1);
            int nextColumns = s[i+1].GetLength(1);

            Matrix w = Numpy.Random.Randn(curColumns, nextColumns);
            W.Add(w);
        }
    }

    /// <summary>
    /// Инициализирует нелинейности.
    /// </summary>
    void InitBiases()
    {
        B.Clear();

        for (int i = 0; i < s.Count - 1; i++)
        {
            int to = s[i + 1].GetLength(1);
            B.Add(Numpy.Random.Randn(1, to));
        }
    }
    #endregion

}
