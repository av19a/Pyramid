using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IMessageService
{
    void ShowMessage(string messageKey, params object[] args);
    void SetLanguage(string languageCode);
}

public class MessageService : IMessageService
{
    private Dictionary<string, Dictionary<string, string>> _localizations = new();
    private string _currentLanguage = "en";
    
    public MessageService()
    {
        LoadLocalizations();
    }

    private void LoadLocalizations()
    {
        _localizations["en"] = new Dictionary<string, string>
        {
            {"cube_added", "Cube added to tower!"},
            {"cube_removed", "Cube removed from tower!"},
            {"tower_full", "Tower is too high!"},
            {"cube_destroyed", "Cube destroyed!"}
        };
    }

    public void ShowMessage(string messageKey, params object[] args)
    {
        if (_localizations[_currentLanguage].TryGetValue(messageKey, out string message))
        {
            string formattedMessage = string.Format(message, args);
            Debug.Log(formattedMessage);
        }
    }

    public void SetLanguage(string languageCode)
    {
        if (_localizations.ContainsKey(languageCode))
            _currentLanguage = languageCode;
    }
}
