using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

/// <summary>
/// Результат предсказания нейросети после одного шага обучения.
/// </summary>
[Serializable]
public class PredictionResult
{
    /// <summary>
    /// Индекс правильной категории (истинная метка).
    /// </summary>
    public int TrueLabelIndex;

    /// <summary>
    /// Индекс предсказанной категории.
    /// </summary>
    public int PredictedCategoryIndex;

    /// <summary>
    /// Значение функции потерь (Cross-Entropy Loss).
    /// </summary>
    public float Error;
}



/// <summary>
/// Нейросеть обучаемая.
/// </summary>
public class Network : MonoBehaviour
{
    [Inject] DatasetValidator datasetValidator;

    [Tooltip("Готовность нейросети к использованию.")]
    public bool isReady = false;

    [Header("Слои:")]
    public List<Matrix> t;

    [Header("Активированные слои:")]
    public List<Matrix> h;

    [Header("Матрицы весов:")]
    public List<Matrix> W;

    [Header("Нелинейности:")]
    public List<Matrix> B;

    [Header("Шаг обучения:")]
    public float learningRate = 0.0001f;

    [Header("Предположение нейросети:")]
    [SerializeField] PredictionResult predict;

    /// <summary>
    /// Вектор предположения сети. Нормализованный.
    /// </summary>
    Matrix z;
    /// <summary>
    /// Истиный индекс категории изображения.
    /// </summary>
    int y;

    /// <summary>
    /// Устанавливает новую конфигурацию слоёв нейросети.
    /// </summary>
    public void Init(List<Matrix> t_midterm)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        InitIntermediateLayers(t_midterm);
        InitActivatedLayers();
        InitWeights();
        InitBiases();

        stopwatch.Stop();
        UnityEngine.Debug.Log($"Создание матриц Numpy: {stopwatch.Elapsed.TotalSeconds} секунд");

        isReady = true;
    }

    /// <summary>
    /// Инициализирует промежуточные слои.
    /// </summary>
    void InitIntermediateLayers(List<Matrix> t_midterm)
    {
        int inputDim = datasetValidator.imageSize[0] * datasetValidator.imageSize[1];
        int outputDim = datasetValidator.categoryNames.Count;
        
        
        int totalLayers = t_midterm.Count + 2; // вход + промежуточные + выход
        t = new List<Matrix>(new Matrix[totalLayers]);

        // Входной слой
        t[0] = Numpy.Zeros(1, inputDim);

        // Промежуточные слои добавляются через инспектор —> пропускаем
        for (int i = 0; i < t_midterm.Count; i++)
        {
            t[i + 1] = t_midterm[i];
        }
        
        // Выходной слой
        t[^1] = Numpy.Zeros(1, outputDim);
    }

    /// <summary>
    /// Инициализирует слои активационные Relu или Sigmoid.
    /// </summary>
    void InitActivatedLayers()
    {
        h = new List<Matrix>();

        for (int i = 0; i < t.Count; i++)
        {
            h.Add(Numpy.Zeros(1, t[i].Columns)); // Вход и выход не активируем
        }
    }

    /// <summary>
    /// Инициализирует веса.
    /// </summary>
    void InitWeights()
    {
        W.Clear();

        for (int i = 0; i < t.Count-1; i++)
        {
            // Матрицы весов между слоями:
            int curColumns = t[i].Columns;
            int nextColumns = t[i+1].Columns;


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

        for (int i = 0; i < t.Count - 1; i++)
        {
            int to = t[i + 1].Columns;
            B.Add(Numpy.Random.Randn(1, to));
        }
    }


    /// <summary>
    /// Применяет шаг градиентного спуска к весам указанного слоя.
    /// </summary>
    /// <param name="layer">Индекс слоя, к которому применяется обновление.</param>
    void ApplyGradientStep(Matrix dE_dW, Matrix dE_dB, int layer)
    {
        W[layer] -= (learningRate * dE_dW);
        B[layer] -= (learningRate * dE_dB);
    }

    public void ForwardPropogation()
    {   
        // Проход:
        for (int i = 0; i < t.Count-1; i++)
        {
            t[i+1] = (h[i] ^ W[i]) + B[i];    // Осторожно, h[0] должен быть таким же как t[0], это входной слой.
            h[i+1] = MatrixUtils.Sigmoid(t[i + 1]);
            // UnityEngine.Debug.Log($"t[{i+1}] \t max: {Numpy.Max(t[i+1])} \t min: {Numpy.Min(t[i+1])} \t h[{i + 1}] \t max: {Numpy.Max(h[i + 1])} \t min: {Numpy.Min(h[i + 1])}");
        }
        Matrix lastLayer = t[^1];  // последний слой, к которому не применялась функция активации.
        z = MatrixUtils.SoftMax(lastLayer);
    }

    void BackPropogation()
    {
        // One hot encoding
        Matrix y_full = MatrixUtils.ToFull(y, t[^1].Columns);

        // Последний слой:
        Matrix dE_dt = z - y_full;
        Matrix dE_dW = h[t.Count - 2].T() ^ dE_dt;
        Matrix dE_dB = dE_dt;
        ApplyGradientStep(dE_dW, dE_dB, t.Count-2);
        Matrix dE_dh = dE_dt ^ W[t.Count - 2].T();
        dE_dt = dE_dh * MatrixUtils.SigmoidDeriv(t[^2]);

        //
        for (int i = t.Count-3; i >= 1; i--)
        {
            dE_dW = h[i].T() ^ dE_dt;
            dE_dB = dE_dt;
            ApplyGradientStep(dE_dW, dE_dB, i);
            dE_dh = dE_dt ^ W[i].T();
            dE_dt = dE_dh * MatrixUtils.SigmoidDeriv(t[i]);
        }

        // Первый слой немного отличается:
        dE_dW = h[0].T() ^ dE_dt;
        dE_dB = dE_dt;
        ApplyGradientStep(dE_dW, dE_dB, 0);
    }


    /// <summary>
    /// Выполняет один шаг обучения на одном примере и возвращает результат.
    /// </summary>
    /// <param name="y">Индекс истинной категории (ground truth).</param>
    /// <returns>Результат предсказания нейросети.</returns>
    public PredictionResult Fit(int y)
    {
        // Устанавливаем правильный индекс категории для текущего изображения
        this.y = y;

        // Прямой и обратный проходы
        ForwardPropogation();

        // Значение функции потерь (Cross-Entropy)
        float Error = MatrixUtils.CrossEntropy(z, y);

        BackPropogation();

        // Индекс категории, предсказанный нейросетью
        int predicted_Y = Numpy.argmax(z);

        

        // Возвращаем результат обучения
        predict = new PredictionResult
        {
            TrueLabelIndex = y,
            PredictedCategoryIndex = predicted_Y,
            Error = Error
        };

        return predict;
    }



}
