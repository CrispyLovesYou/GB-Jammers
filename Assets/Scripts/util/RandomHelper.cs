using System;
using System.Text;

public static class RandomHelper
{
    public static Random random = new Random();

    public static string RandomString(int _size)
    {
        string input = "abcdefghijklmnopqrstuvwxyz0123456789";

        StringBuilder builder = new StringBuilder();
        char ch;

        for (int i = 0; i < _size; i++)
        {
            ch = input[random.Next(0, input.Length)];
            builder.Append(ch);
        }

        return builder.ToString();
    }
}