using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace rccs_new.MyClass
{
    internal class KeySource
    {
        public static string GenerateFormattedKey(int totalLength = 24, int groupSize = 4)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new StringBuilder();

            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] buffer = new byte[totalLength];
                rng.GetBytes(buffer);

                for (int i = 0; i < totalLength; i++)
                {
                    result.Append(chars[buffer[i] % chars.Length]);

                    
                    if ((i + 1) % groupSize == 0 && i != totalLength - 1)
                        result.Append("-");
                }
            }

            return result.ToString();
        }
    }
}
