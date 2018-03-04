using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryArchive.Helpers
{
    public static class EncodeDecodeHelper
    {
        public static byte[] EncodeTo64(this string toEncode)
        {
            return Encoding.ASCII.GetBytes(toEncode);
        }

        public static string DecodeFrom64(this byte[] encodedData)
        {
            return Encoding.ASCII.GetString(encodedData);
        }
    }
}
