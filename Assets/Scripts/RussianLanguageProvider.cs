using System.Collections.Generic;

public class RussianLanguageProvider : ILanguageProvider
{
    private Dictionary<string, string> _GameText = new Dictionary<string, string>
    {
        { "CubeDropped", "Куб установлен" },
        { "CubeDestroyed", "Куб уничтожен" },
        { "CubeThrown", "Куб выброшен" },
        { "HeightLimit", "Башня слишком высокая" },
        { "DropHoleMiss", "Куб не попал в дыру" },
    };

    public string this[string key] => _GameText[key];
}