using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Ringo.Common.Models;

namespace Ringo.Common.Services
{
    public class CanonicalService
    {
        public static RdostrId GetArtistId(string input)
        {
            string token = input.Trim().ToLower();
            string urn = $"urn:rdostr:v1:artist/{token}";

            var md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(urn));
            string checksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

            Console.WriteLine(checksum);

            RdostrId rdostrId = new RdostrId()
            {
                Version = "1",
                Urn = urn,
                Id = checksum,
                UrnId = $"urn:rdostr:v1:{checksum}"
            };
            return rdostrId;
        }
    }
}
