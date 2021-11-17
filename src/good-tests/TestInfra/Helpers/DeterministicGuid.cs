using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace good_tests.TestInfra.Helpers
{
    [DebuggerStepThrough]
    public static class DeterministicGuid
    {
        public static Guid CreateFrom(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Value cannot be null or empty.", paramName: nameof(input));
            //use MD5 hash to get a 16-byte hash of the string: 

            var provider = new MD5CryptoServiceProvider();

            var inputBytes = Encoding.Default.GetBytes(input);

            var hashBytes = provider.ComputeHash(inputBytes);

            //generate a guid from the hash: 

            var hashGuid = new Guid(hashBytes);

            return hashGuid;
        }
    }
}