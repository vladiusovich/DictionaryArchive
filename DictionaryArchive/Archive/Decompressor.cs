using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using log4net;
using DictionaryArchive.Helpers;

namespace DictionaryArchive.Archive
{
    public class Decompressor
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ArchiveDictionary _archiveDictionary;

        public Decompressor(ArchiveDictionary archiveDictionary)
        {
            _archiveDictionary = archiveDictionary;
        }


        public string Decode(byte[] encodeBytes)
        {
            string decodeString = "";

            try
            {
                //** Массив байт в поток бит
                BitArray encodeBits = new BitArray(encodeBytes);

                var streamSize = encodeBits.Length - 1;
                List<bool> revertSteamOfBits = new List<bool>();

                const byte byteLength = 7;
                byte bitIndex = 0;
                List<bool> byteInBits = new List<bool>();

                //переворачиваем поток битов в обратную сторону. Как в исходном
                foreach (var bit in encodeBits)
                {
                    byteInBits.Add((bool)bit);

                    if (bitIndex == byteLength)
                    {
                        var reversBits = byteInBits.Reverse<bool>();
                        revertSteamOfBits.AddRange(reversBits);
                        byteInBits.Clear();
                        bitIndex = 0;
                    }
                    else
                        bitIndex++;
                }


                //** Из потока бит узнать какой длинны был один ИД слова в двоично системе
                var bitsCount = _archiveDictionary.GetAmountOfBitsForEncode();

                List<bool> sourceStreamOfBits = new List<bool>();
                bitIndex = 0;
                byteInBits.Clear();

                List<byte> stremOfByte = new List<byte>();

                //** Парсить каждый бит до этого числа
                foreach (var bit in revertSteamOfBits)
                {
                    byteInBits.Add((bool)bit);

                    if (bitIndex == bitsCount - 1)
                    {
                        var byteOfData = EncodeDecodeHelper.ConvertBitsToByte(byteInBits, (int)bitsCount);
                        stremOfByte.AddRange(byteOfData);
                        bitIndex = 0;
                        byteInBits.Clear();
                    }
                    else
                        bitIndex++;
                }
                //** Конвертировать в десятичный формат
                var decodeList = EncodeDecodeHelper.ConvertBytesToNumbers(stremOfByte.ToArray(), (int)bitsCount).Reverse();

                //** Искать слово по этому ИД
                //** Добавлять в рашифрованную строку

                foreach (var id in decodeList)
                {
                    var word = _archiveDictionary.GetWordById((ushort)id);

                    decodeString += word;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }


            return decodeString;
        }

    }
}
