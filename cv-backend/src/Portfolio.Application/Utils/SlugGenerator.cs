using System.Text.RegularExpressions;
public static class SlugGenerator
{
    public static string Slugify(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var slug = input.ToLowerInvariant();
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");
        slug = Regex.Replace(slug, @"\-{2,}", "-");
        slug = slug.Trim('-');

        return slug;
    }
}