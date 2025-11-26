using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Управляет паузой игры: показывать/скрывать меню паузы, настройки и сохранение прогресса.
/// </summary>
public class PauseMenuManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject pauseMenuPanel;
    
    [Header("Scene Names")]
    [Tooltip("Имя сцены главного меню для загрузки.")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Tooltip("Ссылка на объект с игровым интерфейсом (здоровье и т.д.), чтобы скрывать его при паузе.")]
    [SerializeField] private GameObject gameInterface; 

    [Tooltip("Панель с настройками, которая будет открываться поверх паузы.")]
    [SerializeField] private GameObject settingsMenuPanel; 

    public static bool IsGamePaused { get; private set; }

    /// <summary>
    /// Инициализация состояния паузы на старте (убирает панели и восстанавливает время).
    /// </summary>
    private void Start()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);
        if (gameInterface != null) gameInterface.SetActive(true);

        IsGamePaused = false;
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Отслеживает нажатие `Escape` для переключения паузы.
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    
    
    
    /// <summary>
    /// Включает режим паузы: показывает панель паузы, скрывает интерфейс и останавливает время.
    /// </summary>
    public void PauseGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);

        if (gameInterface != null) gameInterface.SetActive(false);

        Time.timeScale = 0f;
        IsGamePaused = true;
    }

    
    
    
    /// <summary>
    /// Выключает паузу: скрывает панель паузы, восстанавливает интерфейс и время.
    /// </summary>
    public void ResumeGame()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);

        if (gameInterface != null) gameInterface.SetActive(true);

        Time.timeScale = 1f;
        IsGamePaused = false;
    }

    
    
    
    /// <summary>
    /// Запрашивает у `GameManager` сохранение текущего состояния игры.
    /// </summary>
    public void SaveGame()
    {
        GameManager.Instance?.SaveGame(); 
    }

    
    
    
    /// <summary>
    /// Открывает панель настроек из меню паузы.
    /// </summary>
    public void OpenSettings()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);

        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(true);
    }

    /// <summary>
    /// Закрывает настройки и возвращает меню паузы.
    /// </summary>
    public void CloseSettings()
    {
        if (settingsMenuPanel != null) settingsMenuPanel.SetActive(false);

        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
    }
    
    /// <summary>
    /// Возвращает в главное меню: отменяет паузу и загружает сцену главного меню.
    /// </summary>
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        IsGamePaused = false;

        Debug.Log("Возвращение в главное меню...");
        SceneManager.LoadScene(mainMenuSceneName);
    }

    
    
    
    /// <summary>
    /// Выходит из приложения (работает в билде).
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Выход из игры...");
        Application.Quit();
    }
}