namespace Visitz.Views.SegmentedButtons;

#nullable enable

public class SegmentedOptions(string id, string text, ImageSource imageSource)
{
    public string Id { get; } = id;

    public string Text { get; } = text;

    public ImageSource ImageSource { get; } = imageSource;

    public override bool Equals(object? obj)
    {
        return obj is SegmentedOptions opts
            && Id == opts.Id
            && Text == opts.Text
            && EqualityComparer<ImageSource>.Default.Equals(ImageSource, opts.ImageSource);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Text, ImageSource);
    }

    public static bool operator ==(SegmentedOptions? a, SegmentedOptions? b)
    {
        if (a is null && b is null)
            return true;
        else if (a is null ^ b is null)
            return false;
        else
            return a?.Equals(b) ?? false;
    }

    public static bool operator !=(SegmentedOptions? a, SegmentedOptions? b)
    {
        return !(a == b);
    }
}
