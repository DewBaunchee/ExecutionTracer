namespace App.Utils
{
    public static class StringUtils
    {
        public static string GetSymbols(int count, char c)
        {
            string nestingString = "";
            while (count-- > 0)
                nestingString += c;
            return nestingString;
        }
    }
}