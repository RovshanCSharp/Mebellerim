using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace Mebeller.Data.Services;

public sealed class TokenUrlEncoderService
{
    public static string EncodeToken(string token) => WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
}