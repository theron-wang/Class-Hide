﻿using Microsoft.VisualStudio.Text;
using System.IO;

namespace ClassHide;
internal static class LegacyRazorEditorHelper
{
    /// <summary>
    /// Given a ITextBuffer, determine if the buffer is using the legacy Razor editor.
    /// </summary>
    internal static bool IsLegacyRazorEditor(this ITextBuffer textBuffer)
    {
        var fileName = textBuffer.GetFileName();
        return !string.IsNullOrEmpty(fileName) && (Path.GetExtension(fileName) == ".cshtml" || Path.GetExtension(fileName) == ".razor");
    }
}
