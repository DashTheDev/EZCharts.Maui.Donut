using System.Globalization;
using EZCharts.Maui.Donut.Samples.Models;

namespace EZCharts.Maui.Donut.Samples.Converters;

public class ResultCategoryIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ResultCategory category)
        {
            return FasIcons.ScrollGlyph;
        }

        return category switch
        {
            ResultCategory.English => FasIcons.CommentGlpyh,
            ResultCategory.Mathematics => FasIcons.CalculatorGlpyh,
            ResultCategory.Science => FasIcons.FlaskGlyph,
            ResultCategory.Geography => FasIcons.BookAtlasGlyph,
            ResultCategory.Technology => FasIcons.MicrochipGlyph,
            ResultCategory.Sports => FasIcons.BaseballGlyph,
            ResultCategory.Music => FasIcons.MusicGlyph,
            ResultCategory.Drama => FasIcons.MaskGlpyh,
            ResultCategory.Languages => FasIcons.GlobeGlpyh,
            _ => FasIcons.ScrollGlyph
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}