using Base62;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RingoBotNet.Helpers
{
    public static class CryptoHelper
    {
        private static readonly Base62Converter _base62Converter = new Base62Converter();

        public static string Sha256(string input)
        {
            using (var algorithm = SHA256.Create())
            {
                return GetStringFromHash(algorithm.ComputeHash(Encoding.UTF8.GetBytes(input)));                
            }
        }

        //TODO: Surely there is a better way than this?
        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }

        // https://stackoverflow.com/a/11743162/610731
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string Base62Encode(string plainText)
        {
            return _base62Converter.Encode(plainText);
        }

        public static string Base62Decode(string base62EncodedData)
        {
            return _base62Converter.Decode(base62EncodedData);
        }
    }
}
