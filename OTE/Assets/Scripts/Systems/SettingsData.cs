using System;

[Serializable]
/// <summary>
/// Хранилище пользовательских настроек (сериализуемое).
/// </summary>
public class SettingsData
{
    /// <summary>Флаг полноэкранного режима.</summary>
    public bool isFullscreen;

    /// <summary>Конструктор по умолчанию: включён полноэкранный режим.</summary>
    public SettingsData()
    {
        isFullscreen = true;
    }
}