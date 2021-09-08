using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using App.Utils;

namespace App.Mapping
{
    public class XmlMapper : IMapper
    {
        public string ToString(object obj)
        {
            return MapMember(obj);
        }

        private string MapMember(object obj)
        {
            if (obj == null)
                return "";

            Type objType = obj.GetType();
            PropertyInfo[] properties = obj.GetType().GetProperties()
                .Where(property => property.CanRead && property.GetValue(obj) != null).ToArray();

            PropertyInfo[] primitivesOrString = properties.Where(property =>
                property.PropertyType.IsPrimitive || IsString(property.PropertyType)).ToArray();

            StringBuilder sb = new StringBuilder();
            sb.Append("<").Append(objType.Name);
            foreach (var prop in primitivesOrString)
            {
                sb.Append(" ").Append(prop.Name).Append("=\"").Append(prop.GetValue(obj)).Append("\"");
            }

            sb.Append(">");

            PropertyInfo[] mappable = properties.Where(property => !primitivesOrString.Contains(property)).ToArray();
            foreach (var prop in mappable)
            {
                var propValue = prop.GetValue(obj);
                if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                {
                    if (propValue != null)
                    {
                        foreach (var value in (IEnumerable) propValue)
                        {
                            sb.Append(MapMember(value));
                        }
                    }
                }
                else
                {
                    sb.Append(MapMember(propValue));
                }
            }

            return sb.Append("</").Append(objType.Name).Append(">").ToString();
        }

        private bool IsString(Type type)
        {
            return "String".Equals(type?.Name);
        }

        public string ToPrettyString(object obj, int spaceCount)
        {
            return FormatXml(ToString(obj), spaceCount);
        }

        private string FormatXml(string xml, int spaceCount)
        {
            StringBuilder sb = new StringBuilder();
            int level = 0;
            bool isInClosingTag = false;
            char prevSymbol = '\0';

            char stringSign = '\0';
            bool isShielding = false;
            bool isInString = false;
            foreach (var symbol in xml)
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

                if (IsOpeningTag(prevSymbol) && IsClosingTagFeature(symbol))
                {
                    isInClosingTag = true;
                    sb.Remove(sb.Length - spaceCount - 1, spaceCount);
                }

                sb.Append(symbol);

                if (IsClosingTag(symbol))
                {
                    level = level + (isInClosingTag ? -1 : 1);
                    isInClosingTag = false;
                    sb.Append("\n").Append(StringUtils.GetSymbols(level * spaceCount, ' '));
                }

                prevSymbol = symbol;
            }

            return sb.ToString();
        }

        private bool IsClosingTag(char c)
        {
            return ">".Contains(c);
        }

        private bool IsClosingTagFeature(char c)
        {
            return "/".Contains(c);
        }

        private bool IsOpeningTag(char c)
        {
            return "<".Contains(c);
        }

        private bool IsStringSeparator(char c)
        {
            return "\"'`".Contains(c);
        }

        private bool IsShielding(char c)
        {
            return "\\".Contains(c);
        }
    }
}