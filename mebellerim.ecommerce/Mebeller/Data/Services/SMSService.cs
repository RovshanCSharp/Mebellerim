using System;
using Mebeller.Data.Context;

namespace Mebeller.Data.Services;

public interface ISmsSender
{
    public void SendMessage(ApplicationUser user, params string[] body);
}

public class ConsoleSmsSender : ISmsSender
{
    public void SendMessage(ApplicationUser user, params string[] body)
    {
        Console.WriteLine("--- SMS Starts ---");
        Console.WriteLine($"To: {user?.PhoneNumber}");
        foreach (var str in body) Console.WriteLine(str);

        Console.WriteLine("--- SMS Ends ---");
    }
}