
using UnityEngine;
using System.IO;

/// <summary>
/// Утилитный статический класс для сохранения/загрузки данных игры в JSON-файл.
/// </summary>
public static class SaveSystem
{
    
    
    private static string _filePath = Path.Combine(Application.persistentDataPath, "save.json");
    
    [System.Diagnostics.Conditional("UNITY_INCLUDE_TESTS")]
    /// <summary>
    /// Переопределяет путь сохранения (используется в тестах).
    /// </summary>
    public static void OverrideFilePath(string newPath)
    {
        _filePath = newPath;
    }

    
    
    
    /// <summary>
    /// Сохраняет переданные `GameData` в JSON-файл и отмечает наличие сохранения.
    /// </summary>
    public static void SaveGame(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_filePath, json); 
        PlayerPrefs.SetInt("SaveFileExists", 1);
        PlayerPrefs.Save();
        Debug.Log("Игра сохранена в: " + _filePath);
    }

    
    
    
    /// <summary>
    /// Загружает `GameData` из файла; если файла нет — возвращает новый экземпляр по умолчанию.
    /// </summary>
    public static GameData LoadGame()
    {
        if (File.Exists(_filePath))
        {
            string json = File.ReadAllText(_filePath);
            GameData data = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Сохранение загружено.");
            return data;
        }
        else
        {
            Debug.LogWarning("Файл сохранения не найден. Возвращаем данные по умолчанию.");
            return new GameData(); 
        }
    }

    
    
    
    /// <summary>
    /// Проверяет, существует ли файл сохранения.
    /// </summary>
    public static bool DoesSaveFileExist()
    {
        return File.Exists(_filePath);
    }


    private static readonly string SavePath = Path.Combine(Application.persistentDataPath, "save.json");
    
    
    
    /// <summary>
    /// Удаляет файл сохранения и очищает флаг в PlayerPrefs.
    /// </summary>
    public static void DeleteSaveFile()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }
        PlayerPrefs.SetInt("SaveFileExists", 0);
        PlayerPrefs.Save();
    }
}