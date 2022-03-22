using System.Text;

namespace GCore.Dynamics.Extensions;

public static class StringExtensions
{
    public static string ToLiteral(this char c)
    {
        switch (c)
        {
            case '\'': return @"\'";
            case '\"': return "\\\"";
            case '\\': return @"\\";
            case '\0': return @"\0";
            case '\a': return @"\a";
            case '\b': return @"\b";
            case '\f': return @"\f";
            case '\n': return @"\n";
            case '\r': return @"\r";
            case '\t': return @"\t";
            case '\v': return @"\v";
            default:
                // ASCII printable character
                if (c >= 0x20 && c <= 0x7e)
                {
                    return c.ToString();
                    // As UTF16 escaped character
                }
                else
                {
                    return @"\u" +((int)c).ToString("x4");
                }
        }
    }

    public static string ToLiteral(this string input)
    {
        StringBuilder literal = new StringBuilder(input.Length);
        foreach (var c in input)
            literal.Append(c.ToLiteral());
        return literal.ToString();
    }
}