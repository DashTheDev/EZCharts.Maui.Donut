using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using EZCharts.Maui.Donut.Models;

namespace EZCharts.Maui.Donut.Controls;

public partial class DonutChartView
{
    /// <summary>Bindable property for <see cref="EntriesSource"/>.</summary>
    public static readonly BindableProperty EntriesSourceProperty = BindableProperty.Create(
        nameof(EntriesSource),
        typeof(IEnumerable),
        typeof(DonutChartView),
        propertyChanged: OnEntriesSourcePropertyChanged,
        defaultValueCreator: bindable =>
        {
            ObservableCollection<object> entries = [];
            entries.CollectionChanged += bindable.ToDonutChartView().OnEntriesSourceCollectionChanged;
            return entries;
        });

    /// <summary>
    /// Gets or sets the source of entry data to be used for rendering the chart.<br/><br/>
    /// This is a bindable property which defaults to an empty <see cref="ObservableCollection{T}"/> with type <see cref="object"/>.
    /// </summary>
    public IEnumerable EntriesSource
    {
        get => (IEnumerable)GetValue(EntriesSourceProperty);
        set => SetValue(EntriesSourceProperty, value);
    }

    /// <summary>Bindable property for <see cref="EntryValuePath"/>.</summary>
    public static readonly BindableProperty EntryValuePathProperty = BindableProperty.Create(
        nameof(EntryValuePath),
        typeof(string),
        typeof(DonutChartView),
        defaultValue: Defaults.EntryValuePath,
        propertyChanged: OnEntryValuePathPropertyChanged);

    /// <summary>
    /// Gets or sets the path of the value property to be accessed on each data entry.<br/><br/>
    /// This is a bindable property which defaults to <c>"Value"</c>.
    /// </summary>
    public string EntryValuePath
    {
        get => (string)GetValue(EntryValuePathProperty);
        set => SetValue(EntryValuePathProperty, value);
    }

    /// <summary>Bindable property for <see cref="EntryLabelPath"/>.</summary>
    public static readonly BindableProperty EntryLabelPathProperty = BindableProperty.Create(
        nameof(EntryLabelPath),
        typeof(string),
        typeof(DonutChartView),
        defaultValue: Defaults.EntryLabelPath,
        propertyChanged: OnEntryLabelPathPropertyChanged);

    /// <summary>
    /// Gets or sets the path of the label property to be accessed on each data entry.<br/><br/>
    /// This is a bindable property which defaults to <c>"Label"</c>.
    /// </summary>
    public string EntryLabelPath
    {
        get => (string)GetValue(EntryLabelPathProperty);
        set => SetValue(EntryLabelPathProperty, value);
    }

    /// <summary>Bindable property for <see cref="EntryIconTemplate"/>.</summary>
    public static readonly BindableProperty EntryIconTemplateProperty = BindableProperty.Create(
        nameof(EntryIconTemplate),
        typeof(DataTemplate),
        typeof(DonutChartView),
        propertyChanged: OnEntryPropertyChanged);

    /// <summary>
    /// Gets or sets the template to be used for rendering the image for each entry.<br/>
    /// Currently, it is expected that the templated view is a <see cref="FileImageSource"/>.<br/><br/>
    /// This is a bindable property which defaults to <b><see langword="null"/></b>.
    /// </summary>
    public DataTemplate? EntryIconTemplate
    {
        get => (DataTemplate?)GetValue(EntryIconTemplateProperty);
        set => SetValue(EntryIconTemplateProperty, value);
    }

    /// <summary>Bindable property for <see cref="EntryImageScale"/>.</summary>
    public static readonly BindableProperty EntryImageScaleProperty = BindableProperty.Create(
        nameof(EntryImageScale),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: Defaults.EntryImageScale,
        propertyChanged: OnEntryPropertyChanged);

    /// <summary>
    /// Gets or sets the scale to render entry images at.<br/><br/>
    /// This is a bindable property which defaults to <c>0.1f</c>.
    /// </summary>
    public float EntryImageScale
    {
        get => (float)GetValue(EntryImageScaleProperty);
        set => SetValue(EntryImageScaleProperty, value);
    }

    /// <summary>Bindable property for <see cref="EntryColors"/>.</summary>
    public static readonly BindableProperty EntryColorsProperty = BindableProperty.Create(
        nameof(EntryColors),
        typeof(Color[]),
        typeof(DonutChartView),
        defaultValue: Defaults.ChartColors,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the colors to be used when displaying data.<br/><br/>
    /// This is a bindable property which defaults to <c>Blue,Green,Yellow,Purple,Orange</c>.
    /// </summary>
    public Color[] EntryColors
    {
        get => (Color[])GetValue(EntryColorsProperty);
        set => SetValue(EntryColorsProperty, value);
    }

    /// <summary>Bindable property for <see cref="EntryClickedCommand"/>.</summary>
    public static readonly BindableProperty EntryClickedCommandProperty = BindableProperty.Create(
        nameof(EntryClickedCommand),
        typeof(ICommand),
        typeof(DonutChartView));

    /// <summary>
    /// Gets or sets the command to be invoked when an entry is clicked.<br/>
    /// The command will receive an <see langword="object"/> parameter which represents the clicked entry.<br/><br/>
    /// This is a bindable property which defaults to <b><see langword="null"/></b>.
    /// </summary>
    public ICommand? EntryClickedCommand
    {
        get => (ICommand?)GetValue(EntryClickedCommandProperty);
        set => SetValue(EntryClickedCommandProperty, value);
    }

    /// <summary>Bindable property for <see cref="EntrySpacing"/>.</summary>
    public static readonly BindableProperty EntrySpacingProperty = BindableProperty.Create(
        nameof(EntrySpacing),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: Defaults.EntrySpacing,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the spacing between each data entry.<br/>
    /// <b>NOTE:</b> Currently, anything above <c>20f</c> may produce unexpected results.<br/><br/>
    /// This is a bindable property which defaults to <c>10f</c>.
    /// </summary>
    public float EntrySpacing
    {
        get => (float)GetValue(EntrySpacingProperty);
        set => SetValue(EntrySpacingProperty, value);
    }

    /// <summary>Bindable property for <see cref="Padding"/>.</summary>
    public static readonly BindableProperty PaddingProperty = BindableProperty.Create(
        nameof(Padding),
        typeof(Thickness),
        typeof(DonutChartView),
        defaultValue: Defaults.Padding,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the padding of the entire view.<br/><br/>
    /// This is a bindable property which defaults to <c>10d</c>.
    /// </summary>
    public Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    /// <summary>Bindable property for <see cref="ChartRotationDegrees"/>.</summary>
    public static readonly BindableProperty ChartRotationDegreesProperty = BindableProperty.Create(
        nameof(ChartRotationDegrees),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: Defaults.ChartRotationDegrees,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the rotation offset of the chart.<br/><br/>
    /// This is a bindable property which defaults to <c>90f</c>.
    /// </summary>
    public float ChartRotationDegrees
    {
        get => (float)GetValue(ChartRotationDegreesProperty);
        set => SetValue(ChartRotationDegreesProperty, value);
    }

    /// <summary>Bindable property for <see cref="ChartOuterRadius"/>.</summary>
    public static readonly BindableProperty ChartOuterRadiusProperty = BindableProperty.Create(
        nameof(ChartOuterRadius),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: Defaults.ChartOuterRadius,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets how big the outer circle of the chart will be.<br/>
    /// If the provided value is too big for the display, it will be adjusted to fit.<br/><br/>
    /// This is a bindable property which defaults to <c>250f</c>.
    /// </summary>
    public float ChartOuterRadius
    {
        get => (float)GetValue(ChartOuterRadiusProperty);
        set => SetValue(ChartOuterRadiusProperty, value);
    }

    /// <summary>Bindable property for <see cref="ChartInnerRadius"/>.</summary>
    public static readonly BindableProperty ChartInnerRadiusProperty = BindableProperty.Create(
        nameof(ChartInnerRadius),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: Defaults.ChartInnerRadius,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets how big the inner circle of the chart will be.<br/>
    /// If the provided value is too big for the display, it will be adjusted to fit.<br/><br/>
    /// This is a bindable property which defaults to <c>125f</c>.
    /// </summary>
    public float ChartInnerRadius
    {
        get => (float)GetValue(ChartInnerRadiusProperty);
        set => SetValue(ChartInnerRadiusProperty, value);
    }

    /// <summary>Bindable property for <see cref="LabelStyle"/>.</summary>
    public static readonly BindableProperty LabelStyleProperty = BindableProperty.Create(
        nameof(LabelStyle),
        typeof(LabelStyle),
        typeof(DonutChartView),
        defaultValue: Defaults.MainLabelStyle,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the style to be used for chart labels.<br/><br/>
    /// This is a bindable property which defaults to <b><see cref="LabelStyle.Key"/></b>.
    /// </summary>
    public LabelStyle LabelStyle
    {
        get => (LabelStyle)GetValue(LabelStyleProperty);
        set => SetValue(LabelStyleProperty, value);
    }

    /// <summary>Bindable property for <see cref="LabelFontColor"/>.</summary>
    public static readonly BindableProperty LabelFontFamilyProperty = BindableProperty.Create(
        nameof(LabelFontFamily),
        typeof(string),
        typeof(DonutChartView),
        defaultValue: Defaults.LabelFontFamily,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the system font family used for the chart labels.<br/><br/>
    /// This is a bindable property which defaults to <c>"Arial"</c>.
    /// </summary>
    public string LabelFontFamily
    {
        get => (string)GetValue(LabelFontFamilyProperty);
        set => SetValue(LabelFontFamilyProperty, value);
    }

    /// <summary>Bindable property for <see cref="LabelFontColor"/>.</summary>
    public static readonly BindableProperty LabelFontColorProperty = BindableProperty.Create(
        nameof(LabelFontColor),
        typeof(Color),
        typeof(DonutChartView),
        defaultValue: Defaults.LabelFontColor,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the color of the font used for the chart labels.<br/>
    /// This value will be ignored if <c>LabelUseAutoFontColor</c> is set to <b><see langword="true"/></b>.<br/><br/>
    /// This is a bindable property which defaults to <c>White</c>.
    /// </summary>
    public Color LabelFontColor
    {
        get => (Color)GetValue(LabelFontColorProperty);
        set => SetValue(LabelFontColorProperty, value);
    }

    /// <summary>Bindable property for <see cref="LabelUseAutoFontColor"/>.</summary>
    public static readonly BindableProperty LabelUseAutoFontColorProperty = BindableProperty.Create(
        nameof(LabelUseAutoFontColor),
        typeof(bool),
        typeof(DonutChartView),
        defaultValue: Defaults.LabelUseAutoFontColor,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets if the label font colors will be assigned based on their corresponding entry color.<br/><br/>
    /// This is a bindable property which defaults to <b><see langword="false"/></b>.
    /// </summary>
    public bool LabelUseAutoFontColor
    {
        get => (bool)GetValue(LabelUseAutoFontColorProperty);
        set => SetValue(LabelUseAutoFontColorProperty, value);
    }

    /// <summary>Bindable property for <see cref="LabelFontSize"/>.</summary>
    public static readonly BindableProperty LabelFontSizeProperty = BindableProperty.Create(
        nameof(LabelFontSize),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: Defaults.LabelFontSize,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the size of the font used for the chart labels.<br/><br/>
    /// This is a bindable property which defaults to <c>20f</c>.
    /// </summary>
    public float LabelFontSize
    {
        get => (float)GetValue(LabelFontSizeProperty);
        set => SetValue(LabelFontSizeProperty, value);
    }

    /// <summary>Bindable property for <see cref="LabelKeySpacing"/>.</summary>
    public static readonly BindableProperty LabelKeySpacingProperty = BindableProperty.Create(
        nameof(LabelKeySpacing),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: Defaults.LabelKeySpacing,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the vertical spacing between each chart label.<br/>
    /// This value is only applied when <c>LabelStyle</c> is set to <b><see cref="LabelStyle.Key"/></b>.<br/><br/>
    /// This is a bindable property which defaults to <c>10f</c>.
    /// </summary>
    public float LabelKeySpacing
    {
        get => (float)GetValue(LabelKeySpacingProperty);
        set => SetValue(LabelKeySpacingProperty, value);
    }

    /// <summary>Bindable property for <see cref="LabelKeyColorOffset"/>.</summary>
    public static readonly BindableProperty LabelKeyColorOffsetProperty = BindableProperty.Create(
        nameof(LabelKeyColorOffset),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: Defaults.LabelKeyColorOffset,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the horizontal offset of the color circles rendered next to each label.<br/>
    /// This value is only applied when <c>LabelStyle</c> is set to <b><see cref="LabelStyle.Key"/></b>.<br/><br/>
    /// This is a bindable property which defaults to <c>20f</c>.
    /// </summary>
    public float LabelKeyColorOffset
    {
        get => (float)GetValue(LabelKeyColorOffsetProperty);
        set => SetValue(LabelKeyColorOffsetProperty, value);
    }

    /// <summary>Bindable property for <see cref="LabelOutsideRadius"/>.</summary>
    public static readonly BindableProperty LabelOutsideRadiusProperty = BindableProperty.Create(
        nameof(LabelOutsideRadius),
        typeof(float),
        typeof(DonutChartView),
        defaultValue: Defaults.LabelOutsideRadius,
        propertyChanged: OnVisualPropertyChanged);

    /// <summary>
    /// Gets or sets the radius from the <c>ChartOuterRadius</c> where the outside labels will be rendered.<br/>
    /// This value is only applied when <c>LabelStyle</c> is set to <b><see cref="LabelStyle.Outside"/></b>.<br/><br/>
    /// This is a bindable property which defaults to <c>50f</c>.
    /// </summary>
    public float LabelOutsideRadius
    {
        get => (float)GetValue(LabelOutsideRadiusProperty);
        set => SetValue(LabelOutsideRadiusProperty, value);
    }
}