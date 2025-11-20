using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject pauseMenuPanel;
    
    [Header("Scene Names")]
    [Tooltip("Имя сцены главного меню для загрузки.")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public static bool IsGamePaused { get; private set; }

    private void Start()
    {
        // Убедимся, что при старте уровня меню выключено, а игра не на паузе
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        IsGamePaused = false;
        Time.timeScale = 1f; // Убедимся, что время идет нормально
    }

    private void Update()
    {
        // Отслеживаем нажатие клавиши Escape
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
    /// Ставит игру на паузу.
    /// </summary>
    public void PauseGame()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }

        // Останавливаем время в игре
        Time.timeScale = 0f;
        IsGamePaused = true;
        Debug.Log("Игра на паузе.");
    }

    /// <summary>
    /// Снимает игру с паузы.
    /// </summary>
    public void ResumeGame()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // Возобновляем течение времени
        Time.timeScale = 1f;
        IsGamePaused = false;
        Debug.Log("Игра снята с паузы.");
    }

    /// <summary>
    /// Сохраняет игру (пока что заглушка).
    /// </summary>
    public void SaveGame()
    {
        // Находим GameManager на сцене и вызываем его метод сохранения
        GameManager.Instance?.SaveGame(); 
        // Знак '?' - это защита на случай, если GameManager не найден
    }

    /// <summary>
    /// Открывает меню настроек.
    /// </summary>
    public void OpenSettings()
    {
        // Важно: мы не можем просто загрузить сцену, так как это выгрузит наш игровой уровень.
        // Мы должны загрузить сцену настроек "поверх" текущей.
        Debug.Log("Открытие настроек... (пока не реализовано, требует SceneManager.LoadSceneAsync)");
        // SceneManager.LoadScene("SettingsMenu", LoadSceneMode.Additive);
    }
    
    public void LoadMainMenu()
    {
        // ВАЖНО: Перед выходом из игровой сцены нужно обязательно вернуть Time.timeScale к 1.
        // Иначе, если вы выйдете на паузе (timeScale = 0), вся игра "замрет" навсегда,
        // включая анимации в главном меню.
        Time.timeScale = 1f;
        IsGamePaused = false;

        Debug.Log("Возвращение в главное меню...");
        SceneManager.LoadScene(mainMenuSceneName);
    }

    /// <summary>
    /// Полностью закрывает приложение.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Выход из игры...");
        Application.Quit();
    }
}