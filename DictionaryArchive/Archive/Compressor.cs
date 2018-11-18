using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using log4net;

namespace DictionaryArchive.Archive
{
    public class Compressor
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ArchiveDictionary _archiveDictionary;

        public Compressor(ArchiveDictionary archiveDictionary)
        {
            _archiveDictionary = archiveDictionary;
        }

        public List<byte> Compress(string sourceString)
        {
            var encodeBytes = new List<byte>();
            try
            {
                var encodeArrayBits = EncodeSourceTextToStreamOfBits(sourceString).ToArray();
                
                //делим весь поток битов на отдельные 8-битовые последовательности и конвертируем в число (чтобы сохнарить в файл)
                byte byteLength = 7;
                int byteIndex = 0;
                bool[] bits = new bool[8];
                var lenght = encodeArrayBits.Count();
                
                for (var index = 0; index <= lenght - 1; index++, byteIndex++)
                {
                    bits[byteIndex] = encodeArrayBits[index];
                
                    if (byteIndex == byteLength)
                    {
                        byteIndex = -1;// если 0 то добаляет лишний ноль в начле нового байта
                
                        var byteId = ConverBoolArrayToByte(bits);
                        encodeBytes.Add(byteId);
                        bits = new bool[8];
                    }
                
                    if (index >= lenght - 1)
                    {
                        byteIndex = -1;
                
                        var byteId = ConverBoolArrayToByte(bits);
                        encodeBytes.Add(byteId);
                        bits = new bool[8];
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return encodeBytes;
        }

        // ***** Алгоритм преобразования исходного текста в поток байтов *****
        private IList<bool> EncodeSourceTextToStreamOfBits(string sourceString)
        {
            string currentParseWord = string.Empty;
            Stack<byte> encodeResult = new Stack<byte>();

            Stack<bool> streamOfBits = new Stack<bool>();
            IList<byte> encodeId = new byte[] { };

            var bitsCount = _archiveDictionary.GetAmountOfBitsForEncode();

            string wordId;

            for (int index = 0; index <= sourceString.Length; index++)
            {
                try
                {
                    char gliphy = sourceString[index];

                    //Если глиф пробел или знак то ишем его в словаре и переходим к след глифу
                    if (char.IsWhiteSpace(gliphy) || char.IsPunctuation(gliphy))
                    {
                        wordId = _archiveDictionary.GetWordId(gliphy);

                        var bits = NubmerToBitList(wordId, bitsCount);
                        PushToStack(bits, ref streamOfBits);
                        continue;
                    }

                    //пока не достигли конца текста - ищем слова
                    if (index + 1 < sourceString.Length)
                    {
                        //Если след буква значит слово не закончилось - добавляем к недослову и преходи к след глифу
                        if (char.IsLetter(sourceString[index + 1]) || char.IsNumber(sourceString[index + 1]))
                        {
                            currentParseWord += gliphy;
                            continue;
                        }
                        //иначе все же будет конец слова то добавляем последний глиф и ищем недослово
                        else
                        {
                            currentParseWord += gliphy;
                            wordId = _archiveDictionary.GetWordId(currentParseWord);

                            var bits = NubmerToBitList(wordId, bitsCount);
                            PushToStack(bits, ref streamOfBits);

                            currentParseWord = string.Empty;
                            continue;

                            //Если служебный знак то идем к след глифу
                            if (sourceString[index + 1] == '\'') continue;
                        }
                    }
                    // если конец текста то добаляем последний глиф к недослову. Ищем недослово и конец парсинга
                    else
                    {
                        currentParseWord += gliphy;
                        wordId = _archiveDictionary.GetWordId(currentParseWord);

                        var bits = NubmerToBitList(wordId, bitsCount);
                        PushToStack(bits, ref streamOfBits);

                        currentParseWord = string.Empty;
                        continue;
                    }


                }
                catch (Exception ex)
                {
                    currentParseWord = string.Empty;
                    _logger.Error(ex);
                    continue;
                }

            }


            return streamOfBits.Reverse().ToList();
        }

        //оперделяем сколько бит нужно для конкретного словаря и используем это число для того, чтобы отрезать лишнюю часть битовой последовательности
        //var bitsCount = Math.Ceiling(Math.Log(number, 2)) + 1;
        private IList<bool> NubmerToBitList(string n, double bitsCount)
        {
            ushort number = Convert.ToUInt16(n);
            List<bool> result = new List<bool>();
            byte[] keyBytes = BitConverter.GetBytes(number);
            BitArray BA = new BitArray(keyBytes);

            for (var index = 0; index < bitsCount; index++)
                result.Add(BA[index]);

            return result;
        }


        //Конвертирует массив битов в число
        private byte ConverBoolArrayToByte(bool[] arr)
        {
            byte val = 0;
            foreach (bool b in arr)
            {
                val <<= 1;
                if (b) val |= 1;
            }

            return val;
        }

        private void PushToStack<T>(IList<T> encodeId, ref Stack<T> encodeResult)
        {
            foreach (var b in encodeId)
                encodeResult.Push(b);
        }

    }
}
