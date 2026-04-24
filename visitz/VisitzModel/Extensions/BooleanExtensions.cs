namespace VisitzModel.Extensions;

#nullable enable

public static class BooleanExtensions
{
    public static string AsTruthyChar(this bool value)
    {
        return value ? "Y" : "N";
    }

    public static string AsTruthyWord(this bool value)
    {
        return value ? "Yes" : "No";
    }
}
