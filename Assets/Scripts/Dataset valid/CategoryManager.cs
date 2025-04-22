using System;
using System.Collections.Generic;
using Zenject;

/// <summary>
/// Управляет категориями изображений: предоставляет доступ по имени или индексу.
/// </summary>
public class CategoryManager
{
    [Inject] DatasetValidator datasetValidator;

    /// <summary>
    /// Получить индекс категории по её названию.
    /// </summary>
    /// <param name="categoryName">Название категории.</param>
    /// <returns>Индекс категории.</returns>
    public int GetIndex(string categoryName)
    {
        if (categoryName == null)
            throw new ArgumentNullException(nameof(categoryName));

        int index = datasetValidator.categoryNames.IndexOf(categoryName);
        if (index == -1)
            throw new ArgumentException($"Категория с именем \"{categoryName}\" не найдена.", nameof(categoryName));
        return index;
    }

    /// <summary>
    /// Получить название категории по её индексу.
    /// </summary>
    /// <param name="index">Индекс категории.</param>
    /// <returns>Название категории.</returns>
    public string GetName(int index)
    {
        if (index < 0 || index >= datasetValidator.categoryNames.Count)
            throw new ArgumentOutOfRangeException(nameof(index), $"Индекс {index} вне диапазона категорий.");
        return datasetValidator.categoryNames[index];
    }

    /// <summary>
    /// Получить все категории.
    /// </summary>
    public IReadOnlyList<string> GetAllCategories()
    {
        return datasetValidator.categoryNames.AsReadOnly();
    }
}
