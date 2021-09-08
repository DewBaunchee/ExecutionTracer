using System;

namespace App.Utils
{
    public static class InputUtils
    {
        public static string EnterString(string name)
        {
            Console.WriteLine("Enter " + name + ": ");
            return Console.ReadLine();
        }
    }
}