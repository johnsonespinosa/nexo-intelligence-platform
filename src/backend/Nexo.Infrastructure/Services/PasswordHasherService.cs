using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Nexo.Application.Common.Interfaces;

namespace Nexo.Infrastructure.Services;

public class PasswordHasherService : IPasswordHasher
{
    public string HashPassword(string password)
    {
        // Generar salt
        var salt = RandomNumberGenerator.GetBytes(128 / 8);

        // Hashear usando PBKDF2
        var hash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8
        );

        // Combinar salt y hash
        var combined = new byte[salt.Length + hash.Length];
        Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
        Buffer.BlockCopy(hash, 0, combined, salt.Length, hash.Length);

        return Convert.ToBase64String(combined);
    }

    public bool VerifyPassword(string password, string hash)
    {
        var combined = Convert.FromBase64String(hash);
        var salt = new byte[128 / 8];
        var storedHash = new byte[256 / 8];

        Buffer.BlockCopy(combined, 0, salt, 0, salt.Length);
        Buffer.BlockCopy(combined, salt.Length, storedHash, 0, storedHash.Length);

        var computedHash = KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8
        );

        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
    }
}