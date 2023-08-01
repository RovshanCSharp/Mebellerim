using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace Mebeller.Data.Utilities;

public static class ExtensionsMethods
{
    private const string NullIpAddress = "::1";

    public static string GetDescription(this Enum en)
    {
        var type = en.GetType();
        var memInfo = type.GetMember(en.ToString());
        return memInfo.Length > 0
            ? memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false).Length > 0
                ? ((DescriptionAttribute)memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false)[0])
                .Description
                : en.ToString()
            : en.ToString();
    }

    public static bool NotNullOrEmpty<T>(this IEnumerable<T> source) => source != null && source.Any();

    public static bool IsValidEmailAddress(this string email)
    {
        var emailAddressAttribute = new EmailAddressAttribute();
        var result = emailAddressAttribute.IsValid(email);
        return result;
    }

    public static string ToSolarWithTime(this DateTime dateTime) =>
        dateTime.ToString("HH:mm - yyyy/MM/dd", CultureInfo.InvariantCulture);

    public static bool IsLocal(this HttpRequest req)
    {
        var connection = req.HttpContext.Connection;
        return !connection.RemoteIpAddress.IsSet() ||
               //We have a remote address set up
               connection.RemoteIpAddress != null && (connection.LocalIpAddress.IsSet()
                   //Is local is same as remote, then we are local
                   ? connection.RemoteIpAddress.Equals(connection.LocalIpAddress)
                   //else we are remote if the remote IP address is not a loopback address
                   : IPAddress.IsLoopback(connection.RemoteIpAddress));
    }

    private static bool IsSet(this IPAddress address) => address != null && address.ToString() != NullIpAddress;
}