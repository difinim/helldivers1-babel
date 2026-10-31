using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Babel.Core.Models;

public class TranslationLogEntry : INotifyPropertyChanged
{
    private DateTime _timestamp = DateTime.Now;
    private string _date = string.Empty;
    private string _time = DateTime.Now.ToString("HH:mm:ss");
    private string _author = string.Empty;
    private string _sourceLang = string.Empty;
    private string _originalMessage = string.Empty;
    private string _translatedMessage = string.Empty;
    private string _latency = string.Empty;
    private string _colorHex = "#FFD700";

    public DateTime Timestamp
    {
        get => _timestamp;
        set
        {
            if (_timestamp != value)
            {
                _timestamp = value;
                OnPropertyChanged();
            }
        }
    }

    public string Date
    {
        get => _date;
        set
        {
            if (_date != value)
            {
                _date = value;
                OnPropertyChanged();
            }
        }
    }

    public string Time
    {
        get => _time;
        set
        {
            if (_time != value)
            {
                _time = value;
                OnPropertyChanged();
            }
        }
    }

    public string Author
    {
        get => _author;
        set
        {
            if (_author != value)
            {
                _author = value;
                OnPropertyChanged();
            }
        }
    }

    public string SourceLang
    {
        get => _sourceLang;
        set
        {
            if (_sourceLang != value)
            {
                _sourceLang = value;
                OnPropertyChanged();
            }
        }
    }

    public string OriginalMessage
    {
        get => _originalMessage;
        set
        {
            if (_originalMessage != value)
            {
                _originalMessage = value;
                OnPropertyChanged();
            }
        }
    }

    public string TranslatedMessage
    {
        get => _translatedMessage;
        set
        {
            if (_translatedMessage != value)
            {
                _translatedMessage = value;
                OnPropertyChanged();
            }
        }
    }

    public string Latency
    {
        get => _latency;
        set
        {
            if (_latency != value)
            {
                _latency = value;
                OnPropertyChanged();
            }
        }
    }

    public string ColorHex
    {
        get => _colorHex;
        set
        {
            if (_colorHex != value)
            {
                _colorHex = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
