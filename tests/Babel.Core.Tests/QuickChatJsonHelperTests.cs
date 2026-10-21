using System;
using System.Collections.Generic;
using System.Text.Json;
using Babel.Core.Models;
using Babel.Core.Utilities;
using Xunit;

namespace Babel.Core.Tests;

public class QuickChatJsonHelperTests
{
    [Fact]
    public void ExportMessages_WithStrings_ProducesIndentedJsonArrayWithoutShortcutsOrStates()
    {
        var messages = new List<string>
        {
            "Affirmative / Roger that!",
            "Negative / Cancel that!"
        };

        string json = QuickChatJsonHelper.ExportMessages(messages);

        Assert.NotNull(json);
        Assert.Contains("Affirmative / Roger that!", json);
        Assert.Contains("Negative / Cancel that!", json);
        Assert.DoesNotContain("Keybinding", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("IsEnabled", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("SlotId", json, StringComparison.OrdinalIgnoreCase);

        // Verify valid JSON array
        using var doc = JsonDocument.Parse(json);
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
        Assert.Equal(2, doc.RootElement.GetArrayLength());
        Assert.Equal("Affirmative / Roger that!", doc.RootElement[0].GetString());
        Assert.Equal("Negative / Cancel that!", doc.RootElement[1].GetString());
    }

    [Fact]
    public void ExportMessages_WithQuickChatMacroObjects_ExportsOnlyMessagesNotShortcutsOrStates()
    {
        var macros = new List<QuickChatMacro>
        {
            new(1, "NumPad1", "Help me!", true),
            new(2, "NumPad2", "On my way!", false)
        };

        string json = QuickChatJsonHelper.ExportMessages(macros);

        Assert.NotNull(json);
        Assert.Contains("Help me!", json);
        Assert.Contains("On my way!", json);
        Assert.DoesNotContain("NumPad1", json);
        Assert.DoesNotContain("NumPad2", json);
        Assert.DoesNotContain("Keybinding", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("IsEnabled", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("SlotId", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("true", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("false", json, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ParseMessages_FromStringArray_ReturnsMessageList()
    {
        string json = """
            [
              "Affirmative / Roger that!",
              "Negative / Cancel that!",
              "Need ammo / Resupply here!"
            ]
            """;

        var result = QuickChatJsonHelper.ParseMessages(json);

        Assert.Equal(3, result.Count);
        Assert.Equal("Affirmative / Roger that!", result[0]);
        Assert.Equal("Negative / Cancel that!", result[1]);
        Assert.Equal("Need ammo / Resupply here!", result[2]);
    }

    [Fact]
    public void ParseMessages_FromObjectArray_ExtractsOnlyMessagesAndIgnoresShortcutsAndStates()
    {
        string json = """
            [
              {
                "SlotId": 1,
                "Keybinding": "F1",
                "MessageText": "Drop the stratagem!",
                "IsEnabled": false
              },
              {
                "SlotId": 2,
                "Keybinding": "Ctrl+Shift+X",
                "message": "Follow me!",
                "IsEnabled": true
              },
              {
                "text": "For democracy!"
              }
            ]
            """;

        var result = QuickChatJsonHelper.ParseMessages(json);

        Assert.Equal(3, result.Count);
        Assert.Equal("Drop the stratagem!", result[0]);
        Assert.Equal("Follow me!", result[1]);
        Assert.Equal("For democracy!", result[2]);
    }

    [Fact]
    public void ParseMessages_FromWrappedObject_ExtractsMessagesArray()
    {
        string json = """
            {
              "title": "My Custom Macros",
              "messages": [
                "Target acquired",
                "Retreating"
              ]
            }
            """;

        var result = QuickChatJsonHelper.ParseMessages(json);

        Assert.Equal(2, result.Count);
        Assert.Equal("Target acquired", result[0]);
        Assert.Equal("Retreating", result[1]);
    }

    [Fact]
    public void ParseMessages_WithTrailingCommasAndComments_ParsesSuccessfully()
    {
        string json = """
            [
              // First tactical callout
              "Affirmative",
              /* Second callout */
              "Negative",
            ]
            """;

        var result = QuickChatJsonHelper.ParseMessages(json);

        Assert.Equal(2, result.Count);
        Assert.Equal("Affirmative", result[0]);
        Assert.Equal("Negative", result[1]);
    }

    [Fact]
    public void ParseMessages_WithUnicodeAndSpecialCharacters_PreservesIntegrity()
    {
        string json = """
            [
              "Ajuda aqui! (Minas)",
              "¡Cuidado con los bichos!",
              "За демократию! \ud83d\udca5"
            ]
            """;

        var result = QuickChatJsonHelper.ParseMessages(json);

        Assert.Equal(3, result.Count);
        Assert.Equal("Ajuda aqui! (Minas)", result[0]);
        Assert.Equal("¡Cuidado con los bichos!", result[1]);
        Assert.Contains("За демократию!", result[2]);
    }

    [Fact]
    public void ParseMessages_NullOrWhitespace_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => QuickChatJsonHelper.ParseMessages(""));
        Assert.Throws<ArgumentException>(() => QuickChatJsonHelper.ParseMessages("   "));
        Assert.Throws<ArgumentException>(() => QuickChatJsonHelper.ParseMessages(null!));
    }

    [Fact]
    public void ParseMessages_InvalidJson_ThrowsJsonException()
    {
        Assert.ThrowsAny<JsonException>(() => QuickChatJsonHelper.ParseMessages("Not a json at all"));
        Assert.ThrowsAny<JsonException>(() => QuickChatJsonHelper.ParseMessages("12345"));
    }

    [Fact]
    public void ImportMessages_ToExistingMacros_UpdatesOnlyMessageText_PreservesKeybindingAndIsEnabled()
    {
        // Simulate existing configured macros with custom shortcuts and states
        var existingMacros = new List<QuickChatMacro>
        {
            new(1, "Ctrl+1", "Original Msg 1", isEnabled: true),
            new(2, "Ctrl+2", "Original Msg 2", isEnabled: false),
            new(3, "Ctrl+3", "Original Msg 3", isEnabled: true)
        };

        // JSON has new messages, but also tries to specify different shortcuts/states
        string importedJson = """
            [
              { "SlotId": 1, "Keybinding": "F1", "MessageText": "New Tactical Callout 1", "IsEnabled": false },
              { "SlotId": 2, "Keybinding": "F2", "MessageText": "New Tactical Callout 2", "IsEnabled": true }
            ]
            """;

        var parsed = QuickChatJsonHelper.ParseMessages(importedJson);
        int count = Math.Min(existingMacros.Count, parsed.Count);
        for (int i = 0; i < count; i++)
        {
            existingMacros[i].MessageText = parsed[i];
        }

        // Slot 1: message updated, keybinding & state preserved
        Assert.Equal("New Tactical Callout 1", existingMacros[0].MessageText);
        Assert.Equal("Ctrl+1", existingMacros[0].Keybinding);
        Assert.True(existingMacros[0].IsEnabled);

        // Slot 2: message updated, keybinding & state preserved
        Assert.Equal("New Tactical Callout 2", existingMacros[1].MessageText);
        Assert.Equal("Ctrl+2", existingMacros[1].Keybinding);
        Assert.False(existingMacros[1].IsEnabled);

        // Slot 3: untouched
        Assert.Equal("Original Msg 3", existingMacros[2].MessageText);
        Assert.Equal("Ctrl+3", existingMacros[2].Keybinding);
        Assert.True(existingMacros[2].IsEnabled);
    }
}
