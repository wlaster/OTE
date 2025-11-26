
using UnityEngine;

[System.Serializable] 
/// <summary>
/// DTO для сериализации данных игры: здоровье, позиция игрока и имя сцены.
/// </summary>
public class GameData
{
    
    /// <summary>Текущее здоровье игрока.</summary>
    public float playerHealth;
    /// <summary>Позиция игрока в сцене.</summary>
    public Vector3 playerPosition;

    /// <summary>Имя сцены, в которой было сохранение.</summary>
    public string sceneName;

    
    /// <summary>
    /// Конструктор по умолчанию с базовыми значениями.
    /// </summary>
    public GameData()
    {
        this.playerHealth = 100f; 
        this.playerPosition = Vector3.zero; 
        this.sceneName = "Level1_Prototype"; 
    }
}