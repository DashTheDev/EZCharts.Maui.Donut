using System.Globalization;
using EZCharts.Maui.Donut.Samples.Models;

namespace EZCharts.Maui.Donut.Samples.Converters;

public class ResultCategoryImageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ResultCategory category)
        {
            return FasIcons.ScrollFile;
        }

        return category switch
        {
            ResultCategory.English => FasIcons.CommentFile,
            ResultCategory.Mathematics => FasIcons.CalculatorFile,
            ResultCategory.Science => FasIcons.FlaskFile,
            ResultCategory.Geography => FasIcons.BookAtlasFile,
            ResultCategory.Technology => FasIcons.MicrochipFile,
            ResultCategory.Sports => FasIcons.BaseballFile,
            ResultCategory.Music => FasIcons.MusicFile,
            ResultCategory.Drama => FasIcons.MaskFile,
            ResultCategory.Languages => FasIcons.GlobeFile,
            _ => FasIcons.ScrollFile
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}