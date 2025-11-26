
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Менеджер игры (singleton): обеспечивает сохранение/загрузку и передачу данных между сценами.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    
    private PlayerHealth playerHealth;
    
    
    public static GameData dataToLoad = null;

    /// <summary>
    /// Реализация singleton: сохраняет экземпляр и уничтожает дубликаты.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [System.Obsolete]
    /// <summary>
    /// На старте ищет игрока и применяет данные, если они были загружены перед сменой сцены.
    /// </summary>
    private void Start()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();

        if (dataToLoad != null)
        {
            LoadData(dataToLoad);
            dataToLoad = null; 
        }
    }

    
    
    
    /// <summary>
    /// Собирает текущие данные игрока и сохраняет их через `SaveSystem`.
    /// </summary>
    public void SaveGame()
    {
        GameData data = new GameData();

        data.playerHealth = playerHealth.GetCurrentHealth(); 
        data.playerPosition = playerHealth.transform.position;
        data.sceneName = SceneManager.GetActiveScene().name;

        SaveSystem.SaveGame(data);
    }

    
    
    
    /// <summary>
    /// Загружает данные сохранения и переключается на сцену из сохранения.
    /// </summary>
    public void LoadGame()
    {
        dataToLoad = SaveSystem.LoadGame();
        SceneManager.LoadScene(dataToLoad.sceneName);
    }
    
    
    
    
    /// <summary>
    /// Применяет поля `GameData` к текущим объектам в сцене (позиция и здоровье игрока).
    /// </summary>
    private void LoadData(GameData data)
    {
        if (playerHealth != null)
        {
            playerHealth.transform.position = data.playerPosition;
            playerHealth.SetCurrentHealth(data.playerHealth); 
        }
    }
}