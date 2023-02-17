using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Options;
using ScrumPoker.WebApp.Models;

namespace ScrumPoker.WebApp.Services;

public class SessionTokenService
{
    public const int TOKEN_BYTE_LENGTH = 32;
    public const int TOKEN_UNPROTECTED_LENGTH = 64;
    public const int TOKEN_PROTECTED_LENGTH = 352;
    private readonly RandomNumberGenerator _randomNumberGenerator = RandomNumberGenerator.Create();
    private readonly string _adminToken;

    public SessionTokenService(IOptions<ScrumPokerAppSettings> settings, ILogger<SessionTokenService> logger)
    {
        if (TryUnrotectToken(settings.Value.AdminToken, out string? token))
            _adminToken = token;
        else
        {
            logger.LogCritical("Invalid Admin token");
            _adminToken = GenerateSessionToken();
        }
    }

    public void GetRandomBytes(byte[] data) => _randomNumberGenerator.GetBytes(data);

    public void GetRandomBytes(byte[] data, int offset, int count) => _randomNumberGenerator.GetBytes(data, offset, count);

    public void GetRandomBytes(Span<byte> data) => _randomNumberGenerator.GetBytes(data);

    public void GetRandomNonZeroBytes(byte[] data) => _randomNumberGenerator.GetNonZeroBytes(data);

    public void GetRandomNonZeroBytes(Span<byte> data) => _randomNumberGenerator.GetNonZeroBytes(data);

    /// <summary>
    /// Creates a new token string.
    /// </summary>
    /// <param name="isProtected">True to create a protected token string; otherwise, an unprotected token string is created.</param>
    /// <returns>A random token string.</returns>
    public string GenerateSessionToken(bool isProtected = false)
    {
        byte[] data = new byte[TOKEN_BYTE_LENGTH];
        _randomNumberGenerator.GetBytes(data);
        return isProtected ? Convert.ToBase64String(ProtectData(data)) : Convert.ToHexString(data).ToLower();
    }

    /// <summary>
    /// Protects data for use on the local machine.
    /// </summary>
    /// <param name="data">The data to be protected.</param>
    /// <returns>The protected data.</returns>
    public byte[] ProtectData(byte[] data) => ProtectedData.Protect(data, null, DataProtectionScope.LocalMachine);

    /// <summary>
    /// Unprotects data that was protected for use on the local machine.
    /// </summary>
    /// <param name="data">The data to be unprotected.</param>
    /// <returns>The unprotected data.</returns>
    public byte[] UnprotectData(byte[] data) => ProtectedData.Unprotect(data, null, DataProtectionScope.LocalMachine);

    /// <summary>
    /// Converts an unprotected token string to a protected token string.
    /// </summary>
    /// <param name="token">The unprotected token string.</param>
    /// <returns>A protected token string.</returns>
    public string ToProtectedToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return token;
        if (token.Length == TOKEN_UNPROTECTED_LENGTH)
            try
            {
                byte[] data = Convert.FromHexString(token);
                if (data is not null && data.Length == TOKEN_BYTE_LENGTH)
                    return Convert.ToBase64String(ProtectData(data), Base64FormattingOptions.None);
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Invalid token", nameof(token), exc);
            }
        throw new ArgumentException("Invalid token", nameof(token));
    }

    /// <summary>
    /// Converts a protected token string to an unprotected token string.
    /// </summary>
    /// <param name="token">The protected token string.</param>
    /// <returns>A unprotected token string.</returns>
    public string FromProtectedToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return token;
        if (token.Length == TOKEN_PROTECTED_LENGTH)
            try
            {
                byte[] data = Convert.FromBase64String(token);
                if (data is not null && data.Length == TOKEN_BYTE_LENGTH)
                    return Convert.ToHexString(UnprotectData(data)).ToLower();
            }
            catch (Exception exc)
            {
                throw new ArgumentException("Invalid token", nameof(token), exc);
            }
        throw new ArgumentException("Invalid token", nameof(token));
    }

    /// <summary>
    /// Attempts to convert an unprotected token string to a protected token string.
    /// </summary>
    /// <param name="token">The unprotected token string.</param>
    /// <param name="protectedToken">The protected token string.</param>
    /// <returns>True if the token was a valid unprotected token string; otherwise, false.</returns>
    public bool TryProtectToken([NotNullWhen(true)] string? token, [NotNullWhen(true)] out string? protectedToken)
    {
        if (token is not null && token.Length == TOKEN_UNPROTECTED_LENGTH)
            try
            {
                byte[] data = Convert.FromHexString(token);
                if (data is not null && data.Length == TOKEN_BYTE_LENGTH)
                {
                    protectedToken = Convert.ToBase64String(ProtectData(data));
                    return true;   
                }
            }
            catch { /* okay to ignore */ }
        protectedToken = null;
        return false;
    }

    /// <summary>
    /// Attempts to convert a protected token string to an unprotected token string.
    /// </summary>
    /// <param name="token">The protected token string.</param>
    /// <param name="decryptedToken">The unroteted token string.</param>
    /// <returns>True if the token was a valid protected token string; otherwise, false.</returns>
    public bool TryUnrotectToken([NotNullWhen(true)] string? token, [NotNullWhen(true)] out string? decryptedToken)
    {
        if (token is not null && token.Length == TOKEN_PROTECTED_LENGTH)
            try
            {
                byte[] data = UnprotectData(Convert.FromBase64String(token));
                if (data is not null && data.Length == TOKEN_BYTE_LENGTH)
                {
                    decryptedToken = Convert.ToHexString(data).ToLower();
                    return true;   
                }
            }
            catch { /* okay to ignore */ }
        decryptedToken = null;
        return false;
    }

    /// <summary>
    /// Tests whether a protected token string matches an 
    /// </summary>
    /// <param name="token">The protected token string.</param>
    /// <param name="unprotectedToken">The unprotected token string.</param>
    /// <returns>True if the tokens match; otherwise, false.</returns>
    public bool ValidateTokenString(string token, string unprotectedToken)
    {
        if (token is null || token.Length == 0)
            return unprotectedToken is null || unprotectedToken.Length == 0;
        if (unprotectedToken is null || unprotectedToken.Length != TOKEN_UNPROTECTED_LENGTH || token.Length != TOKEN_PROTECTED_LENGTH)
            return false;
        
        byte[] dataX, dataY;
        try
        {
            dataX = UnprotectData(Convert.FromBase64String(token));
            dataY = Convert.FromHexString(unprotectedToken);
        }
        catch { return false; }
        if (dataX is null || dataY is null || dataX.Length != TOKEN_BYTE_LENGTH || dataX.Length != TOKEN_BYTE_LENGTH)
            return false;

        for (int i = 0; i < TOKEN_BYTE_LENGTH; i++)
        {
            if (dataX[i] != dataY[i])
                return false;
        }
        return true;
    }

    /// <summary>
    /// Tests whether the value matches the protected admin token string.
    /// </summary>
    /// <param name="value">Protected token string.</param>
    /// <returns>True if the parameter matches the admin token string; otherwise, false.</returns>
    public bool ValidateAdminTokenString(string value) => ValidateTokenString(value, _adminToken);
}