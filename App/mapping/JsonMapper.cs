using System.Text;
using System.Text.Json;
using App.Utils;

namespace App.Mapping
{
    public class JsonMapper : IMapper
    {
        public string ToString(object obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public string ToPrettyString(object obj, int spaceCount)
        {
            return FormatJson(ToString(obj), spaceCount);
        }

        private string FormatJson(string json, int spaceCount)
        {
            StringBuilder sb = new StringBuilder();
            int level = 0;

            char stringSign = '\0';
            bool isInString = false;
            bool isShielding = false;
            foreach (var symbol in json)
            {
                if (isShielding)
                {
                    isShielding = false;
                    sb.Append(symbol);
                    continue;
                }

                if (IsStringSeparator(symbol))
                {
                    if (isInString)
                    {
                        if (symbol == stringSign)
                        {
                            isInString = false;
                            stringSign = '\0';
                        }
                    }
                    else
                    {
                        isInString = true;
                        stringSign = symbol;
                    }
                }

                isShielding = isInString && IsShielding(symbol);
                if (isInString)
                {
                    sb.Append(symbol);
                    continue;
                }

                // after passing a string literal

                if (IsIncreaseNesting(symbol))
                    level++;
                else if (IsDecreaseNesting(symbol))
                    level--;

                if (NeedNewLineBefore(symbol))
                    sb.Append("\n").Append(StringUtils.GetSymbols(level * spaceCount, ' '));

                sb.Append(symbol);

                if (NeedNewLineAfter(symbol))
                    sb.Append("\n").Append(StringUtils.GetSymbols(level * spaceCount, ' '));

                if (NeedSpaceAfter(symbol))
                    sb.Append(" ");
            }

            return sb.ToString();
        }

        private bool IsShielding(char symbol)
        {
            return "\\".Contains(symbol);
        }

        private bool IsStringSeparator(char symbol)
        {
            return "\"'`".Contains(symbol);
        }

        private bool NeedSpaceAfter(char c)
        {
            return ":".Contains(c);
        }

        private bool IsIncreaseNesting(char c)
        {
            return "{[".Contains(c);
        }

        private bool IsDecreaseNesting(char c)
        {
            return "}]".Contains(c);
        }

        private bool NeedNewLineAfter(char c)
        {
            return "{[,".Contains(c);
        }

        private bool NeedNewLineBefore(char c)
        {
            return "}]".Contains(c);
        }
    }
}