using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNet.Identity;

namespace RavenDB.AspNet.Identity
{
    public static class Util
    {
        public static string ToHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);

            foreach (var t in bytes)
            {
                sb.Append(t.ToString("x2"));
            }

            return sb.ToString();
        }

        public static byte[] FromHex(string hex)
        {
            if (hex == null)
            {
                throw new ArgumentNullException(nameof(hex));
            }

            if (hex.Length%2 != 0)
            {
                throw new ArgumentException("Hex string must be an even number of characters to convert to bytes.");
            }

            var bytes = new byte[hex.Length / 2];

            for (int i = 0, b = 0; i < hex.Length; i += 2, b++)
            {
                bytes[b] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }

        public static IList<T> ToIList<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.ToList();
        }

        public static string GetLoginId(UserLoginInfo login)
        {
            using (var sha = new SHA1CryptoServiceProvider())
            {
                var clearBytes = Encoding.UTF8.GetBytes($"{login.LoginProvider}|{login.ProviderKey}");
                var hashBytes = sha.ComputeHash(clearBytes);

                return $"IdentityUserLogins/{Util.ToHex(hashBytes)}";
            }
        }

        public static string GetIdentityUserByUserNameId(string userName)
        {
            return $"IdentityUserByUserNames/{userName}";
        }
    }
}