//using static System.Runtime.InteropServices.JavaScript.JSType;

using Microsoft.Data.SqlClient;

static class MyExtensions
{
    public static string Replicate(this string str, int num)
    {
        string result = "";
        for (int i = 0; i < num; i++)
        {
            result += str;
        }
        return result;
    }

    public static bool IsValidExpirationDate(this string date)
    {
        string[] parts = date.Split('/');
        if (parts.Length != 2 || parts[0].Length != 2 || parts[1].Length != 2)
            return false;

        if (!parts[0].All(char.IsDigit) || !parts[1].All(char.IsDigit))
            return false;
        return true;
    }

    public static async Task<int> TryExecuteNonQueryAsync(this SqlCommand command)
    {
        int rows = 0;
        try
        {
            rows = await command.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return rows;
    }

    public static async Task<SqlDataReader> TryExecuteReaderAsync(this SqlCommand command)
    {
        try
        {
            return await command.ExecuteReaderAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error executing reader: {e.Message}");
            return null;
        }
    }
}