// SaveSystem.cs
using UnityEngine;
using System.IO;

public static class SaveSystem
{
    // Путь к файлу сохранения. Application.persistentDataPath - это специальная папка Unity
    // для хранения данных на любой платформе (Windows, Android, и т.д.)
    private static readonly string SAVE_PATH = Path.Combine(Application.persistentDataPath, "save.json");

    /// <summary>
    /// Сохраняет игровые данные в JSON файл.
    /// </summary>
    public static void SaveGame(GameData data)
    {
        // Конвертируем объект GameData в строку формата JSON
        string json = JsonUtility.ToJson(data, true); // "true" для красивого форматирования
        
        // Записываем строку в файл
        File.WriteAllText(SAVE_PATH, json);
        
        // Устанавливаем флаг для кнопки "Продолжить"
        PlayerPrefs.SetInt("SaveFileExists", 1);
        PlayerPrefs.Save();

        Debug.Log("Игра сохранена в: " + SAVE_PATH);
    }

    /// <summary>
    /// Загружает игровые данные из JSON файла.
    /// </summary>
    public static GameData LoadGame()
    {
        if (File.Exists(SAVE_PATH))
        {
            // Читаем весь текст из файла
            string json = File.ReadAllText(SAVE_PATH);
            
            // Конвертируем JSON строку обратно в объект GameData
            GameData data = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Сохранение загружено.");
            return data;
        }
        else
        {
            Debug.LogWarning("Файл сохранения не найден. Возвращаем данные по умолчанию.");
            return new GameData(); // Если файла нет, возвращаем "пустые" данные
        }
    }

    /// <summary>
    /// Проверяет, существует ли файл сохранения.
    /// </summary>
    public static bool DoesSaveFileExist()
    {
        return File.Exists(SAVE_PATH);
    }

    /// <summary>
    /// Удаляет файл сохранения.
    /// </summary>
    public static void DeleteSaveFile()
    {
        if (File.Exists(SAVE_PATH))
        {
            File.Delete(SAVE_PATH);
            PlayerPrefs.DeleteKey("SaveFileExists");
            Debug.Log("Файл сохранения удален.");
        }
    }
}