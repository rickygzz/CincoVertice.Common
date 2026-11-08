using System.Text;

namespace CincoVertice.Common.Utils.Helpers;

public static class StringLineEndingHelper
{
    public static string ToCRLF(string text)
    {
        if (text == null || text.Length == 0)
        {
            return string.Empty;
        }

        int initialCapacity = (int)(text.Length * 1.4);
        StringBuilder sb = new(initialCapacity);

        int i = 0;
        while (i < text.Length)
        {
            if (text[i] == '\r')
            {
                if (CharAfterPos(text, i) == '\n')
                {
                    i++;
                }

                sb.Append("\r\n");
            }
            else if (text[i] == '\n')
            {
                if (CharAfterPos(text, i) == '\r')
                {
                    i++;
                }

                sb.Append("\r\n");
            }
            else
            {
                sb.Append(text[i]);
            }

            i++;
        }

        return sb.ToString();
    }

    public static string ToLF(string text)
    {
        if (text == null || text.Length == 0)
        {
            return string.Empty;
        }

        int initialCapacity = (int)(text.Length * 1.0);
        StringBuilder sb = new(initialCapacity);

        int i = 0;
        while (i < text.Length)
        {
            if (text[i] == '\r')
            {
                if (CharAfterPos(text, i) == '\n')
                {
                    i++;
                }

                sb.Append('\n');
            }
            else if (text[i] == '\n')
            {
                if (CharAfterPos(text, i) == '\r')
                {
                    i++;
                }

                sb.Append('\n');
            }
            else
            {
                sb.Append(text[i]);
            }

            i++;
        }

        return sb.ToString();
    }

    public static char CharBeforePos(string text, int pos)
    {
        if (pos <= 0)
        {
            return '\0';
        }

        return text[pos - 1];
    }

    public static char CharAfterPos(string text, int pos)
    {
        if (pos + 1 >= text.Length)
        {
            return '\0';
        }

        return text[pos + 1];
    }
}
