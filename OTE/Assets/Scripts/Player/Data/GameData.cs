// GameData.cs
using UnityEngine;

[System.Serializable] // Этот атрибут обязателен, чтобы Unity мог сериализовать класс в JSON
public class GameData
{
    // --- ДАННЫЕ ИГРОКА ---
    public float playerHealth;
    public Vector3 playerPosition;

    // --- ДАННЫЕ О МИРЕ ---
    public string sceneName;

    // Конструктор для создания "пустых" данных по умолчанию
    public GameData()
    {
        this.playerHealth = 100f; // Начальное здоровье
        this.playerPosition = Vector3.zero; // Начальная позиция
        this.sceneName = "Level1_Prototype"; // Начальная сцена
    }
}