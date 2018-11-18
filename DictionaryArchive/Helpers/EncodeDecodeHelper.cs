using System;
using System.Collections;
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

        public static byte[] ConvertBitsToByte(List<bool> byteInBits, int amountBits)
        {
            BitArray a = new BitArray(byteInBits.ToArray());

            if (amountBits < 8)
            {
                byte[] bytes = new byte[1];
                a.CopyTo(bytes, 0);
                Array.Reverse(bytes);
                return bytes;
            }
            else
            {
                byte[] bytes = new byte[2];
                a.CopyTo(bytes, 0);
                Array.Reverse(bytes);
                return bytes;
            }

        }

        //Ковертируем ID слова в последователььность байт
        public static List<byte> UsortToBytes(ushort n)
        {
            byte[] keyBytes = BitConverter.GetBytes(n);
            Array.Reverse(keyBytes);
            return keyBytes.ToList();
        }
        public static IEnumerable<int> ConvertBytesToNumbers(byte[] stremOfByte, int amountBits)
        {
            List<int> result = new List<int>();

            if (amountBits < 8)
            {
                foreach (var b in stremOfByte)
                {
                    byte[] byteArr = new byte[] { b, 0 };
                    int number = BitConverter.ToInt16(byteArr, 0);

                    result.Add(number);
                }
            }
            else
            {

                for (var index = 0; index < stremOfByte.Length; index += 2)
                {
                    byte[] byteArr = new byte[] { stremOfByte[index], stremOfByte[index + 1] }.Reverse().ToArray();
                    int number = BitConverter.ToInt16(byteArr, 0);

                    result.Add(number);
                }
            }


            return result.Reverse<int>();
        }


    }
}
