using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject gameInterface; // Ссылка на HUD, чтобы скрыть его

    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private int totalEnemies;

    private void Awake()
    {
        // Простой Синглтон, чтобы к менеджеру был доступ отовсюду
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Считаем всех врагов на сцене в начале игры
        // Ищем все объекты, у которых есть компонент EnemyHealth
        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();
        totalEnemies = enemies.Length;
        
        Debug.Log($"На уровне найдено врагов: {totalEnemies}");

        // Убеждаемся, что панели выключены, а время идет
        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // --- ЛОГИКА ВРАГОВ ---

    public void OnEnemyDied()
    {
        totalEnemies--;
        Debug.Log($"Враг убит. Осталось: {totalEnemies}");

        if (totalEnemies <= 0)
        {
            Victory();
        }
    }

    // --- ЛОГИКА ПОБЕДЫ И ПОРАЖЕНИЯ ---

    private void Victory()
    {
        Debug.Log("Победа!");
        // Показываем курсор
        Cursor.visible = true;
        // Останавливаем время (по желанию, можно оставить замедленным)
        Time.timeScale = 0f; 
        
        if (gameInterface != null) gameInterface.SetActive(false);
        victoryPanel.SetActive(true);
    }

    public void GameOver()
    {
        Debug.Log("Поражение!");
        Cursor.visible = true;
        Time.timeScale = 0f;

        if (gameInterface != null) gameInterface.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    // --- МЕТОДЫ ДЛЯ КНОПОК ---

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}