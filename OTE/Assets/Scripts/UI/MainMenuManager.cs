using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    [Header("Scene Names")]
    [Tooltip("Имя сцены, которую нужно загрузить для новой игры.")]
    [SerializeField] private string newGameSceneName = "Level1_Prototype"; // Укажите имя вашей первой игровой сцены

    private void Start()
    {
        // Используем более надежную проверку
        if (continueButton != null)
        {
            continueButton.interactable = SaveSystem.DoesSaveFileExist();
        }
    }

    private void CheckForSaveFile()
    {
        // PlayerPrefs - это простейшая система сохранения в Unity.
        // Мы проверяем, есть ли у нас ключ "SaveFileExists".
        // HasKey возвращает 1 (true) если ключ есть, и 0 (false) если нет.
        bool saveFileExists = PlayerPrefs.GetInt("SaveFileExists", 0) == 1;

        if (continueButton != null)
        {
            // interactable делает кнопку активной или неактивной (серой).
            continueButton.interactable = saveFileExists;
        }
    }

    // --- МЕТОДЫ ДЛЯ КНОПОК ---

    public void NewGame()
    {
        // Удаляем старый файл сохранения при старте новой игры
        SaveSystem.DeleteSaveFile();
        SceneManager.LoadScene(newGameSceneName);
    }
    public void ContinueGame()
    {
        // Создаем временный объект, который загрузит игру
        GameObject tempManager = new GameObject("TempGameManager");
        tempManager.AddComponent<GameManager>().LoadGame();
        // Этот объект самоуничтожится при загрузке новой сцены
    }

    public void OpenSettings()
    {
        // Загружаем сцену с настройками
        SceneManager.LoadScene("SettingsMenu"); // Убедитесь, что у вас есть сцена с таким именем
    }

    public void QuitGame()
    {
        Debug.Log("Выход из игры...");
        Application.Quit();
    }
}