using System;
using System.Collections.Generic;
using UnityEngine;
using SFB; // Standalone File Browser (Windows)
using System.IO;
using System.Linq;
using System.Collections;
using Zenject;
using UnityEngine.Events;

/// <summary>
/// ���� "��������� - ���-�� �����������".
/// </summary>
[Serializable]
public class CategoryInfo
{
    [Tooltip("��������� �����������")]
    public string name;

    [Tooltip("���������� �����������")]
    public int count;
}

/// <summary>
/// ���������, ��������� �� ����������� ������� ����������� ��� �������������.
/// </summary>
[AddComponentMenu("Custom/Dataset Validator (�������� ��������)")]
public class DatasetValidator : MonoBehaviour
{
    [Tooltip("���������� ����������� �������.")]
    [Inject] PopUpPanelsController popUpPanelsController;
    /* ---------------------- ����� ���������� ----------------------*/
    [Header("����� ����������:")]
    [Tooltip("�������� ��������� (����� ��� train � test).")]
    public List<string> categoryNames;

    [Header("�������� ��������:")]
    [Range(0, 1)] public float trainProgress;
    [Range(0, 1)] public float testProgress;

    /* ---------------------- ��������� ������� ----------------------*/
    [Header("��������� �������:")]
    [Tooltip("���-�� ����������� � ��������� �������.")]
    public int trainTotal;

    [Tooltip("���������� ����������� � ������ ��������� (��������� �������).")]
    public List<CategoryInfo> trainCategories;

    /* ---------------------- �������� ������� ----------------------*/
    [Header("�������� �������:")]
    [Tooltip("���-�� ����������� � �������� �������.")]
    public int testTotal;

    [Tooltip("���������� ����������� � ������ ��������� (�������� �������).")]
    public List<CategoryInfo> testCategories;

    /* ---------------------- ���� ----------------------*/
    [Header("����:")]
    [Tooltip("����� ���������� �����������.")]
    public Vector2Int imageSize;

    [Tooltip("������� ��������.")]
    [TextArea]
    public string verdict;

    [Tooltip("����� �� ������������ �������.")]
    public bool isValid = false;

    [Tooltip("���������� � �������������.")]
    public UnityAction OnReady;


    [ContextMenu("��������� �������")]
    public void ValidateDataset()
    {
        var paths = StandaloneFileBrowser.OpenFolderPanel("����� ����� � ���������", "", false);
        if (paths.Length == 0 || string.IsNullOrEmpty(paths[0])) return;

        string datasetPath = paths[0];

        string trainPath = Directory.GetDirectories(datasetPath).FirstOrDefault(p => Path.GetFileName(p).ToLower() == "train");
        string testPath = Directory.GetDirectories(datasetPath).FirstOrDefault(p => Path.GetFileName(p).ToLower() == "test");

        if (trainPath == null || testPath == null)
        {
            verdict = "� ����� ������ ���� �������� 'train' � 'test'.";
            isValid = false;
            return;
        }

        // ��������� ������ �����������
        StartCoroutine(ValidateAsync(trainPath, testPath));
    }

    /// <summary>
    /// ����������� ��������� ��������� � �������� ������ ����������
    /// </summary>
    private IEnumerator ValidateAsync(string trainPath, string testPath)
    {
        // ���������� ������������ ������ ��������:
        popUpPanelsController.ShowPanel("�������� ��������");

        trainProgress = 0;
        testProgress = 0;

        Vector2Int size = Vector2Int.zero;  // ����������� ������� ����������� [����].
        
        // ������������ � ������������ �����������.
        bool sizeMismatch1 = false; // -> � ��������� �������
        bool sizeMismatch2 = false; // -> � �������� �������

        // ���������� ���������� ����� �������.
        bool trainDone = false;
        bool testDone = false;

        StartCoroutine(AnalyzeCategoryFolderAsync(trainPath, v => trainProgress = v, (list, total, s, mismatch) =>
        {
            trainCategories = list;
            trainTotal = total;
            size = s;
            sizeMismatch1 = mismatch;
            trainDone = true;
        }));

        StartCoroutine(AnalyzeCategoryFolderAsync(testPath, v => testProgress = v, (list, total, _, mismatch) =>
        {
            testCategories = list;
            testTotal = total;
            sizeMismatch2 = mismatch;
            testDone = true;
        }));

        while (!trainDone || !testDone)
            yield return null;

        // ������ �������� �����������:
        if (sizeMismatch1 || sizeMismatch2)
        {
            verdict = "���������� ������������ ���� �������. ��� ����������� ������ ���� ����������� ����������!";
            isValid = false;
            OnReady?.Invoke();
            yield break;
        }
        imageSize = size;

        // ���������� �� ��������� ��������� � ��������� � �������� ��������:
        categoryNames = trainCategories.Select(c => c.name).ToList();
        if (!Enumerable.SequenceEqual(categoryNames, testCategories.Select(c => c.name)))
        {
            verdict = "���������� ������������ ���� �������. ��������� � ������ train � test �� ���������!";
            isValid = false;
            OnReady?.Invoke();
            yield break;
        }

        // ���������� �� ���������� �������� � ������ ���������.
        bool balanced = trainCategories.Select(c => c.count).Distinct().Count() == 1;

        // �������:
        isValid = true;
        verdict = balanced
            ? "������� ��������� � �������������."
            : "������� �������� � �������������, ������ ��������� �������� ������������ ���������� �����������.";

        // ����� ������������ ������ ��������:
        //popUpPanelsController.ClosePanel("�������� ��������");
        OnReady?.Invoke();
    }

    private IEnumerator AnalyzeCategoryFolderAsync(string path, Action<float> onProgress, Action<List<CategoryInfo>, int, Vector2Int, bool> onComplete)
    {
        var result = new List<CategoryInfo>();
        var categories = Directory.GetDirectories(path);
        int totalCount = 0;
        Vector2Int imageSize = Vector2Int.zero;
        bool sizeMismatch = false;

        int totalImages = categories.Sum(cat => Directory.GetFiles(cat)
            .Count(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg")));

        int processedImages = 0;

        foreach (var category in categories)
        {
            string catName = Path.GetFileName(category);
            string[] images = Directory.GetFiles(category)
                .Where(f => f.EndsWith(".png") || f.EndsWith(".jpg") || f.EndsWith(".jpeg"))
                .ToArray();

            result.Add(new CategoryInfo { name = catName, count = images.Length });
            totalCount += images.Length;

            foreach (var imgPath in images)
            {
                byte[] data = File.ReadAllBytes(imgPath);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(data);

                if (imageSize == Vector2Int.zero)
                    imageSize = new Vector2Int(tex.width, tex.height);
                else if (imageSize.x != tex.width || imageSize.y != tex.height)
                    sizeMismatch = true;

                UnityEngine.Object.Destroy(tex);

                processedImages++;
                onProgress?.Invoke((float)processedImages / totalImages);

                // ��������� 65 ����������� � ���������� ����, �������� ������� � ���������� �����. ���� ����� �� 65, � 1, �� ������ �� �����, � ��������� ����� �� 1 ����������� �� 1 ����. ������� ����� ������.
                if (processedImages % 65 == 0)
                    yield return null;
            }
        }
        onProgress?.Invoke(1.0f);
        onComplete?.Invoke(result, totalCount, imageSize, sizeMismatch);
    }
}
