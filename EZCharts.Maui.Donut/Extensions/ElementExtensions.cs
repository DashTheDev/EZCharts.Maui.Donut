namespace System;

// ORIGINAL: https://github.com/Redth/FFImageLoading.Compat/blob/main/source/FFImageLoading.Maui/MauiExtensions.cs
internal static class ElementExtensions
{
    public static IMauiContext? FindMauiContext(this Element element, bool fallbackToAppMauiContext = true)
    {
        if (element is IElement fe && fe.Handler?.MauiContext != null)
        {
            return fe.Handler.MauiContext;
        }

        foreach (var parent in element.GetParentsPath())
        {
            if (parent is IElement parentView && parentView.Handler?.MauiContext != null)
            {
                return parentView.Handler.MauiContext;
            }
        }

        return fallbackToAppMauiContext ? Application.Current?.FindMauiContext() : default;
    }

    private static IEnumerable<Element> GetParentsPath(this Element self)
    {
        Element current = self;

        while (!IsApplicationOrNull(current.RealParent))
        {
            current = current.RealParent;
            yield return current;
        }
    }

    private static bool IsApplicationOrNull(object? element) =>
        element == null || element is IApplication;
}