using System;
using System.Text;

namespace Hospital.Client.Services;

public static class JwtTokenHelper
{
    public static bool IsTokenExpiredOrNearExpiry(string? token, TimeSpan threshold)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return true;
        }

        var parts = token.Split('.');
        if (parts.Length != 3)
        {
            return true; // Invalid format, treat as expired
        }

        try
        {
            var payload = parts[1];
            
            // Normalize Base64 URL to standard Base64
            payload = payload.Replace('-', '+').Replace('_', '/');
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            var jsonBytes = Convert.FromBase64String(payload);
            var json = Encoding.UTF8.GetString(jsonBytes);

            // Simple search to locate the "exp" claim value
            var expIndex = json.IndexOf("\"exp\":");
            if (expIndex == -1)
            {
                return true;
            }

            var start = expIndex + 6;
            var end = json.IndexOfAny(new[] { ',', '}' }, start);
            if (end == -1)
            {
                return true;
            }

            var expString = json.Substring(start, end - start).Trim();
            if (long.TryParse(expString, out var expUnix))
            {
                var expTime = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
                return expTime <= DateTime.UtcNow.Add(threshold);
            }
        }
        catch (Exception)
        {
            return true;
        }

        return true;
    }
}
