using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldBank.Shared.Helper
{
    public static class StringHelper
    {
        public static string Encrypt(string text,string encryptionKey)
        {
            return ECBEnvryption.Encrypt(text, encryptionKey);
        }

        public static string Decrypt(string hash, string encryptionKey)
        {
            return ECBEnvryption.Decrypt(hash, encryptionKey);
        }
    }
}
