// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    // Ссылка на объект игрока
    private PlayerHealth playerHealth;
    
    // Переменная для хранения данных при переходе между сценами
    public static GameData dataToLoad = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // DontDestroyOnLoad(gameObject); // Раскомментируйте, если у вас будет много сцен и вы хотите, чтобы GameManager был один на всю игру
    }

    [System.Obsolete]
    private void Start()
    {
        // Находим игрока на сцене
        playerHealth = FindObjectOfType<PlayerHealth>();
        
        // Если у нас есть данные для загрузки, применяем их
        if (dataToLoad != null)
        {
            LoadData(dataToLoad);
            dataToLoad = null; // Очищаем, чтобы не загружать снова при перезапуске сцены
        }
    }

    /// <summary>
    /// Собирает все данные и сохраняет игру.
    /// </summary>
    public void SaveGame()
    {
        GameData data = new GameData();
        
        // Собираем данные
        data.playerHealth = playerHealth.GetCurrentHealth(); // Нужно будет добавить этот метод в PlayerHealth
        data.playerPosition = playerHealth.transform.position;
        data.sceneName = SceneManager.GetActiveScene().name;
        
        // Передаем в систему сохранения
        SaveSystem.SaveGame(data);
    }

    /// <summary>
    /// Загружает данные из файла и готовит их к применению.
    /// </summary>
    public void LoadGame()
    {
        dataToLoad = SaveSystem.LoadGame();
        
        // Загружаем нужную сцену. После загрузки Start() применит данные.
        SceneManager.LoadScene(dataToLoad.sceneName);
    }
    
    /// <summary>
    /// Применяет загруженные данные к объектам на сцене.
    /// </summary>
    private void LoadData(GameData data)
    {
        if (playerHealth != null)
        {
            playerHealth.transform.position = data.playerPosition;
            playerHealth.SetCurrentHealth(data.playerHealth); // Нужно будет добавить этот метод
        }
    }
}