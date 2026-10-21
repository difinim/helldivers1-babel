using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using Babel.Core.Models;

namespace Babel.Core.Utilities;

/// <summary>
/// Utility for exporting and importing Quick Chat messages to and from JSON format.
/// Strictly handles message text only; shortcuts/keybindings and active states are neither exported nor imported.
/// </summary>
public static class QuickChatJsonHelper
{
    private static readonly JsonSerializerOptions ExportOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private static readonly JsonDocumentOptions DocumentOptions = new()
    {
        CommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private static readonly string[] CandidateMessageProperties =
    [
        "MessageText",
        "messageText",
        "Message",
        "message",
        "Text",
        "text",
        "Content",
        "content"
    ];

    /// <summary>
    /// Exports a collection of message strings to an indented JSON array of strings.
    /// Only the message texts are exported.
    /// </summary>
    public static string ExportMessages(IEnumerable<string> messages)
    {
        ArgumentNullException.ThrowIfNull(messages);
        var list = messages.ToList();
        return JsonSerializer.Serialize(list, ExportOptions);
    }

    /// <summary>
    /// Exports the message texts from a collection of QuickChatMacro items to an indented JSON array of strings.
    /// Keybindings and IsEnabled states are explicitly excluded.
    /// </summary>
    public static string ExportMessages(IEnumerable<QuickChatMacro> macros)
    {
        ArgumentNullException.ThrowIfNull(macros);
        return ExportMessages(macros.Select(m => m.MessageText));
    }

    /// <summary>
    /// Parses quick chat messages from a JSON string.
    /// Extracts only message texts, strictly ignoring any shortcuts, keybindings, or active states.
    /// Supports:
    /// - Array of strings: ["msg1", "msg2"]
    /// - Array of objects: [{"MessageText": "msg1"}, {"message": "msg2"}]
    /// - Object with messages array: {"messages": ["msg1", "msg2"]}
    /// </summary>
    public static List<string> ParseMessages(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("JSON content cannot be empty or whitespace.", nameof(json));
        }

        using var doc = JsonDocument.Parse(json, DocumentOptions);
        var root = doc.RootElement;
        var result = new List<string>();

        if (root.ValueKind == JsonValueKind.Array)
        {
            ExtractFromArray(root, result);
        }
        else if (root.ValueKind == JsonValueKind.Object)
        {
            bool foundArray = false;
            foreach (var prop in root.EnumerateObject())
            {
                if (prop.Value.ValueKind == JsonValueKind.Array)
                {
                    ExtractFromArray(prop.Value, result);
                    foundArray = true;
                    break;
                }
            }

            if (!foundArray)
            {
                if (TryExtractMessageString(root, out var msg))
                {
                    result.Add(msg);
                }
            }
        }
        else
        {
            throw new JsonException($"Expected JSON array or object, but got {root.ValueKind}.");
        }

        return result;
    }

    private static void ExtractFromArray(JsonElement arrayElement, List<string> result)
    {
        foreach (var element in arrayElement.EnumerateArray())
        {
            if (element.ValueKind == JsonValueKind.String)
            {
                result.Add(element.GetString() ?? string.Empty);
            }
            else if (element.ValueKind == JsonValueKind.Object)
            {
                if (TryExtractMessageString(element, out var msg))
                {
                    result.Add(msg);
                }
            }
        }
    }

    private static bool TryExtractMessageString(JsonElement objElement, out string message)
    {
        message = string.Empty;

        foreach (var name in CandidateMessageProperties)
        {
            if (objElement.TryGetProperty(name, out var prop) && prop.ValueKind == JsonValueKind.String)
            {
                message = prop.GetString() ?? string.Empty;
                return true;
            }
        }

        return false;
    }
}
