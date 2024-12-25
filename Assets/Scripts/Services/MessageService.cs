using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using TMPro;
using UnityEngine.UI;

public interface IMessageService
{
    void ShowMessage(string messageKey, params object[] args);
    void SetLanguage(string languageCode);
}

public class MessageService : IMessageService
{
    private Dictionary<string, Dictionary<string, string>> _localizations = new();
    private string _currentLanguage = "en";

    private TMP_Text _message;
    
    [Inject]
    public MessageService([Inject(Id = "Message")] TMP_Text message)
    {
        _message = message;
        LoadLocalizations();
    }

    private void LoadLocalizations()
    {
        _localizations["en"] = new Dictionary<string, string>
        {
            {"cube_added", "Cube added to tower!"},
            {"cube_removed", "Cube removed from tower!"},
            {"tower_full", "Tower is too high!"},
            {"cube_destroyed", "Cube destroyed!"},
            {"cube_dropped", "Cube dropped!"},
        };
    }

    public void ShowMessage(string messageKey, params object[] args)
    {
        if (_localizations[_currentLanguage].TryGetValue(messageKey, out string message))
        {
            string formattedMessage = string.Format(message, args);
            _message.text = formattedMessage;
        }
    }

    public void SetLanguage(string languageCode)
    {
        if (_localizations.ContainsKey(languageCode))
            _currentLanguage = languageCode;
    }
}
