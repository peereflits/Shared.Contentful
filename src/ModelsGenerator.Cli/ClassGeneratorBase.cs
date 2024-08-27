using System.Text.RegularExpressions;

namespace Peereflits.Shared.Contentful.ModelsGenerator.Cli;

internal abstract class ClassGeneratorBase
{
    protected static string FormatClassName(string name) => FirstLetterToUpperCase(RemoveProhibitedCharacters(name));

    protected static string FirstLetterToUpperCase(string word)
    {
        if (string.IsNullOrEmpty(word))
        {
            return word;
        }

        var a = word.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }

    private static string RemoveProhibitedCharacters(string word) =>
        Regex.Replace(word, "[^A-Za-z0-9_]", string.Empty);
}
