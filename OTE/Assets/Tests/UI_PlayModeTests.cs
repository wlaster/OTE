
using NUnit.Framework;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;



/// <summary>
/// Класс UI_PlayModeTests: Короткое описание.
/// </summary>
public class UI_PlayModeTests
{
    [Test]
    public void B21_MainMenuManager_NewGame_DeletesSaveFile()
    {
        
        
        
        string savePath = Path.Combine(Application.persistentDataPath, "save.json");
        File.WriteAllText(savePath, "test data");
        Assert.IsTrue(File.Exists(savePath), "Подготовка не удалась: тестовый файл сохранения не был создан.");

        var mainMenuGO = new GameObject();
        var mainMenu = mainMenuGO.AddComponent<MainMenuManager>();

        
        
        
        SaveSystem.DeleteSaveFile(); 

        
        Assert.IsFalse(File.Exists(savePath), "Файл сохранения должен был быть удален.");

        
        Object.Destroy(mainMenuGO);
    }

    
    [UnityTest]
    public IEnumerator B22_PauseMenuManager_PausesTheGame()
    {
        
        var pauseManagerGO = new GameObject();
        var pauseManager = pauseManagerGO.AddComponent<PauseMenuManager>();
        
        
        yield return null;

        
        pauseManager.PauseGame();
        
        
        yield return null;

        
        Assert.AreEqual(0f, Time.timeScale, "Time.timeScale должен быть равен 0.");
        Assert.IsTrue(PauseMenuManager.IsGamePaused, "Статический флаг IsGamePaused должен быть true.");

        
        Time.timeScale = 1f; 
        Object.Destroy(pauseManagerGO);
    }

    
    [UnityTest] 
    public IEnumerator B23_SettingsMenu_SetFullscreen_SavesSettings()
    {
        
        var settingsMenuGO = new GameObject();
        var settingsMenu = settingsMenuGO.AddComponent<SettingsMenu>();
        var toggleGO = new GameObject("Toggle");
        toggleGO.AddComponent<Toggle>();
        SetPrivateField(settingsMenu, "fullscreenToggle", toggleGO.GetComponent<Toggle>());
        
        string testSettingsPath = Path.Combine(Application.persistentDataPath, "test_settings.json");
        SettingsIO.OverrideFilePath(testSettingsPath);

        yield return null; 

        
        settingsMenu.SetFullscreen(false);

        
        var loadedSettings = SettingsIO.LoadSettings();
        Assert.IsFalse(loadedSettings.isFullscreen);

        
        Object.Destroy(settingsMenuGO);
        Object.Destroy(toggleGO);
        if (File.Exists(testSettingsPath)) File.Delete(testSettingsPath);
    }

    
    [UnityTest]
    public IEnumerator B24_HealthBar_UpdatesFillAmount()
    {
        // Arrange
        var healthBarGO = new GameObject("HealthBar");
        var healthBar = healthBarGO.AddComponent<HealthBar>();
        
        // --- ИСПРАВЛЕНИЕ: Создаем Image и Text как ОТДЕЛЬНЫЕ дочерние объекты ---
        var imageGO = new GameObject("Fill");
        imageGO.transform.SetParent(healthBarGO.transform);
        var imageComponent = imageGO.AddComponent<Image>();
        
        var textGO = new GameObject("Text");
        textGO.transform.SetParent(healthBarGO.transform);
        var textComponent = textGO.AddComponent<TextMeshProUGUI>();
        // -------------------------------------------------------------------
        
        SetPrivateField(healthBar, "healthBarFill", imageComponent);
        SetPrivateField(healthBar, "hpText", textComponent);
        
        // Act
        healthBar.UpdateHealth(0, 100);
        yield return null;

        // Assert
        Assert.AreEqual(0, imageComponent.fillAmount);
        Assert.AreEqual("0 / 100", textComponent.text);
        
        // Cleanup
        Object.Destroy(healthBarGO);
    }
    
    
    private void SetPrivateField<T>(object obj, string fieldName, T value)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field == null) Assert.Fail($"Приватное поле '{fieldName}' не найдено в классе '{obj.GetType().Name}'.");
        field.SetValue(obj, value);
    }
}