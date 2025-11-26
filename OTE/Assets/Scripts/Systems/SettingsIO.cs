using UnityEngine;
using System.IO;

/// <summary>
/// Утилита для сохранения/загрузки настроек игры в JSON-файл.
/// </summary>
public static class SettingsIO
{
    
    private static string _filePath = Path.Combine(Application.persistentDataPath, "settings.json");

    
    /// <summary>Путь к файлу настроек.</summary>
    public static string FilePath => _filePath;

    
    [System.Diagnostics.Conditional("UNITY_INCLUDE_TESTS")]
    /// <summary>Переопределяет путь к файлу (для тестов).</summary>
    public static void OverrideFilePath(string newPath)
    {
        _filePath = newPath;
    }

    /// <summary>Сохраняет `SettingsData` в JSON-файл.</summary>
    public static void SaveSettings(SettingsData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_filePath, json); 
        Debug.Log($"Настройки сохранены в: {_filePath}");
    }

    /// <summary>Загружает настройки из файла или возвращает дефолтные при ошибке/отсутствии файла.</summary>
    public static SettingsData LoadSettings()
    {
        if (!File.Exists(_filePath)) 
        {
            return new SettingsData(); 
        }

        try
        {
            string json = File.ReadAllText(_filePath);
            return JsonUtility.FromJson<SettingsData>(json);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка загрузки настроек: {e.Message}. Сброс на дефолтные.");
            return new SettingsData();
        }
    }
}