/*
    THIS FILE IS NOT AUTO-GENERATED.

    But it should be.

    TODO: Implement a Source Generator to generate this file from Dimensions.xaml.
 */

using Visitz.Extensions;

namespace Visitz.Resources.Styles;

internal static class VisitzDimensions
{
    public static double TryGetDimension(string name, double? fallback = null)
    {
        return Application.Current.Resources.TryGetDimension(name, fallback)
            ?? throw new InvalidOperationException($"Dimension '{name}' not found in resources");
    }

    public static readonly double TopAppBarHeight = TryGetDimension(nameof(TopAppBarHeight));

    public static readonly double DraftIndicatorWidth = TryGetDimension(nameof(DraftIndicatorWidth));

    public static readonly double DefaultPadding = TryGetDimension(nameof(DefaultPadding));

    public static readonly double DefaultPaddingDouble = TryGetDimension(nameof(DefaultPaddingDouble));

    public static readonly double DefaultSpacingHalf = TryGetDimension(nameof(DefaultSpacingHalf));

    public static readonly double DefaultSpacing = TryGetDimension(nameof(DefaultSpacing));

    public static readonly double DefaultMargin = TryGetDimension(nameof(DefaultMargin));

    public static readonly double DefaultMarginDouble = TryGetDimension(nameof(DefaultMarginDouble));
}
