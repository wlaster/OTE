
using NUnit.Framework;
using UnityEngine;
using System.IO;

/// <summary>
/// Класс DataSystems_PlayModeTests: Короткое описание.
/// </summary>
public class DataSystems_PlayModeTests
{
    
    private readonly string TEST_SAVE_PATH = Path.Combine(Application.persistentDataPath, "test_save.json");
    private readonly string TEST_SETTINGS_PATH = Path.Combine(Application.persistentDataPath, "test_settings.json");

    
    [TearDown]
    public void Teardown()
    {
        
        if (File.Exists(TEST_SAVE_PATH))
        {
            File.Delete(TEST_SAVE_PATH);
        }
        if (File.Exists(TEST_SETTINGS_PATH))
        {
            File.Delete(TEST_SETTINGS_PATH);
        }
        
        PlayerPrefs.DeleteKey("SaveFileExists");
    }

    
    [Test]
    public void B16_GameData_Constructor_SetsDefaultValues()
    {
        
        var data = new GameData();

        
        Assert.AreEqual(100f, data.playerHealth);
        Assert.AreEqual(Vector3.zero, data.playerPosition);
        Assert.AreEqual("Level1_Prototype", data.sceneName);
    }

    
    [Test]
    public void B17_SaveSystem_SavesAndLoadsData()
    {
        
        GameData dataToSave = new GameData();
        dataToSave.playerHealth = 85;
        dataToSave.playerPosition = new Vector3(10, 20, 30);
        
        
        SaveSystem.SaveGame(dataToSave);
        GameData loadedData = SaveSystem.LoadGame();
        
        
        Assert.IsTrue(SaveSystem.DoesSaveFileExist());
        Assert.AreEqual(85, loadedData.playerHealth);
        Assert.AreEqual(new Vector3(10, 20, 30), loadedData.playerPosition);
        
        
        SaveSystem.DeleteSaveFile();
    }

    
    
    [Test]
    public void B18_GameManager_Simulation_CallsSaveSystem()
    {
        
        SaveSystem.OverrideFilePath(TEST_SAVE_PATH);
        var dataFromGame = new GameData { playerHealth = 75 };

        
        SaveSystem.SaveGame(dataFromGame);

        
        Assert.IsTrue(File.Exists(TEST_SAVE_PATH), "SaveSystem.SaveGame должен был создать файл по тестовому пути.");
    }


    
    [Test]
    public void B19_SettingsData_Constructor_SetsDefaultValues()
    {
        
        var settings = new SettingsData();

        
        Assert.IsTrue(settings.isFullscreen, "Поле isFullscreen должно быть true по умолчанию.");
    }

    
    [Test]
    public void B20_SettingsIO_LoadSettings_ReturnsDefaultWhenFileMissing()
    {
        
        
        string path = Path.Combine(Application.persistentDataPath, "settings.json");
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        
        
        SettingsData settings = SettingsIO.LoadSettings();
        
        
        Assert.IsNotNull(settings);
        Assert.IsTrue(settings.isFullscreen, "Значение isFullscreen должно быть true по умолчанию.");
    }
}