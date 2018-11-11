using DictionaryArchive.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Collections;
using DictionaryArchive.Models;

namespace DictionaryArchive.Archive
{
    public class ArchiveDictionary
    {
        //private Regex wordsPattern = new Regex("\\w+|\\W+");
        private Regex decodePattern = new Regex("\\d+");
        //private Regex wordsPattern = new Regex("([a-zA-Zа-яА-Я'-]+)");
        private Regex wordsPattern = new Regex("[a-zA-Zа-яА-Я1-9]+");

        private List<byte> encodeBytes = new List<byte>();

        private Dictionary<string, string> _wordsDictionary = new Dictionary<string, string>();
        private List<string> _allWords = new List<string>();

        private int globalKeyId = 0;

        public List<byte> EncodeBytes
        {
            get { return encodeBytes; }
            set { encodeBytes = value; }
        }

        public Dictionary<string, string> Dictonary
        {
            get { return _wordsDictionary; }
        }


        public CompressResultContainer Compress(string sourceString)
        {
            var result = new CompressResultContainer();
            try
            {
                CreateDictionary(sourceString);

                if (_wordsDictionary.Any())
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

                        if (index >= lenght)
                        {
                            byteIndex = -1;

                            var byteId = ConverBoolArrayToByte(bits);
                            encodeBytes.Add(byteId);
                            bits = new bool[8];
                        }
                    }
                }

                result.Dictionary = DictonaryToJSON();
                result.EncodeBytes = encodeBytes;
            }
            catch (Exception ex) {

            }


            return result;
        }


        public string Decode(byte[] encodeBytes)
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
            var bitsCount = GetAmountOfBitsForEncode();

            List<bool> sourceStreamOfBits = new List<bool>();
            bitIndex = 0;
            byteInBits.Clear();

            //** Воостановить исходный поток битов
            foreach (var bit in revertSteamOfBits)
            {
                byteInBits.Add(bit);

                if (bitIndex == bitsCount - 1)
                {
                    //** Заполнять остаток нулями
                    List<bool> zeros = new List<bool>();
                    int mod = (int)(8 - bitsCount);
                    for (var a = 0; a < mod; a++)
                        zeros.Add(false);
                    //** Заполнять остаток нулями -- END

                    byteInBits.AddRange(zeros);
                    sourceStreamOfBits.AddRange(byteInBits);
                    bitIndex = 0;
                    byteInBits.Clear();
                }
                else
                    bitIndex++;
            }

            List<byte> stremOfByte = new List<byte>();
            bitIndex = 0;
            byteInBits.Clear();

            var amountBits = GetAmountOfBitsForEncode();

            //** Парсить каждый бит до этого числа
            foreach (var bit in sourceStreamOfBits)
            {
                byteInBits.Add((bool)bit);

                if (bitIndex == bitsCount - 1)
                {
                    var byteOfData = ConvertBitsToByte(byteInBits, (int)amountBits);
                    stremOfByte.AddRange(byteOfData);
                    bitIndex = 0;
                    byteInBits.Clear();
                }
                else
                    bitIndex++;
            }
            //** Конвертировать в десятичный формат
            var decodeList = ConvertBytesToNumbers(stremOfByte.ToArray(), (int)amountBits).Reverse();

            //** Искать слово по этому ИД
            //** Добавлять в рашифрованную строку

            string decodeString = "";
            foreach (var id in decodeList)
            {
                var word = GetWordById((ushort)id);

                decodeString += word;
            }

            return decodeString;
        }


        // ***** Алгоритм преобразования исходного текста в поток байтов *****
        private IList<bool> EncodeSourceTextToStreamOfBits(string sourceString)
        {
            string currentParseWord = string.Empty;
            Stack<byte> encodeResult = new Stack<byte>();

            Stack<bool> streamOfBits = new Stack<bool>();
            IList<byte> encodeId = new byte[] { };

            var bitsCount = GetAmountOfBitsForEncode();

            string wordId;

            for (int index = 0; index <= sourceString.Length; index++)
            {
                try
                {
                    char gliphy = sourceString[index];

                    //Если глиф пробел или знак то ишем его в словаре и переходим к след глифу
                    if (char.IsWhiteSpace(gliphy) || char.IsPunctuation(gliphy))
                    {
                        wordId = GetWordId(gliphy);

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
                            wordId = GetWordId(currentParseWord);

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
                        wordId = GetWordId(currentParseWord);

                        var bits = NubmerToBitList(wordId, bitsCount);
                        PushToStack(bits, ref streamOfBits);

                        currentParseWord = string.Empty;
                        continue;
                    }


                }
                catch (Exception ex)
                {
                    currentParseWord = string.Empty;
                    continue;
                }

            }


            return streamOfBits.Reverse().ToList();
        }

        public void SetDictionary(string dictionaryJsonString)
        {
            var deserializeDictionary = JsonConvert.DeserializeObject<IDictionary<string, string>>(dictionaryJsonString);

            if (deserializeDictionary == null) return;

            foreach (var keyValue in deserializeDictionary)
            {
                AddWordToDictionary(keyValue.Value);
            }
        }


        //Переписчать
        public bool CreateDictionary(string sourceString)
        {
            if (sourceString != string.Empty)
            {
                _allWords = SplitText(sourceString);

                foreach (var word in _allWords)
                {
                    AddWordToDictionary(word);
                }

                SavePunctuation(sourceString);
            }

            return _wordsDictionary.Count > 0;
        }

        private List<string> SavePunctuation(string input)
        {
            foreach (var symbol in input)
            {
                if (char.IsPunctuation(symbol) || symbol == ' ' || symbol == '\r' || symbol == '\n')
                {
                    AddWordToDictionary(symbol.ToString());
                }
            }
            return wordsPattern.Matches(input).Cast<Match>().Select(match => match.Value.Trim()).Distinct().ToList();
        }

        private List<string> SplitText(string input)
        {
            return wordsPattern.Matches(input).Cast<Match>().Select(match => match.Value.Trim()).Distinct().ToList();
        }

        private Dictionary<string, string> CreateDictionary(List<string> allWords)
        {
            Dictionary<string, string> wordsDictionary = new Dictionary<string, string>();

            foreach (var word in allWords)
            {
                if (word == string.Empty) continue;

                var contain = wordsDictionary.ContainsValue(word);

                if (!contain)
                    wordsDictionary.Add((globalKeyId++).ToString(), word);
            }

            return wordsDictionary;
        }

  
        //Добавить в словарь слова если их нет
        private void AddWordToDictionary(string word)
        {
            string id;

            try
            {
                if (word != string.Empty)
                {
                    if (!_wordsDictionary.ContainsValue(word))
                    {
                        if (_wordsDictionary.Any())
                        {
                            id = _wordsDictionary.Last().Key;
                        }
                        else
                        {
                            id = (++globalKeyId).ToString();
                        }

                        var parseId = Convert.ToUInt16(id);
                        parseId++;

                        _wordsDictionary.Add(parseId.ToString(), word);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private string GetWordById(ushort id)
        {
            string strId = id.ToString();
            if (!_wordsDictionary.ContainsKey(strId)) return string.Empty;

            var word = _wordsDictionary[strId];
            return word ?? string.Empty;
        }

        //**************** CONVERTES **************** 

        // Скорее всего здес ошибка 
        private IEnumerable<int> ConvertBytesToNumbers(byte[] stremOfByte, int amountBits)
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
            } else
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

        // Скорее всего здес ошибка 
        private byte[] ConvertBitsToByte(List<bool> byteInBits, int amountBits) {
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
        private List<byte> UsortToBytes(ushort n)
        {
            byte[] keyBytes = BitConverter.GetBytes(n);
            Array.Reverse(keyBytes);
            return keyBytes.ToList();
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



        //оперделяем сколько бит нужно для конкретного словаря и используем это число для того, чтобы отрезать лишнюю часть битовой последовательности
        private double GetAmountOfBitsForEncode()
        {
            return Math.Ceiling(Math.Log(_wordsDictionary.Count, 2)) + 1;
        }



        private void PushToStack<T>(IList<T> encodeId, ref Stack<T> encodeResult)
        {
            foreach (var b in encodeId)
                encodeResult.Push(b);
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

        private string GetWordId(string word)
        {
            return _wordsDictionary.First(x => x.Value == word).Key;
        }

        private string GetWordId(char gliph)
        {
            return _wordsDictionary.First(x => x.Value == gliph.ToString()).Key;
        }

        private List<KeyValuePair<ushort, SortedModel>> RefactoringDictionary(string sourceString)
        {
            Dictionary<ushort, SortedModel> sortedDictionary = new Dictionary<ushort, SortedModel>();
            ushort id = 0;

            /*
                У коэффициэнтов на частоту вхождений и на длинну слова должны быть разные значения. 
            */
            const decimal lengthWordFactor = 1;
            const decimal frequencyWordInTextFactor = 1;

            foreach (var keyValue in _wordsDictionary)
            {
                /* Проблема с [] ()  */
                int frequencyWordInText = Regex.Matches(sourceString, $"{keyValue.Value}").Count;

                /*
                    sortFactor прямо пропорционален частоте вхождения слова и обратно пропорционален его длинне
                */

                //decimal sortFactor = (decimal)(frequencyWordInText * frequencyWordInTextFactor)/ lengthWordFactor * keyValue.Value.Length;

                decimal sortFactor = frequencyWordInText;

                sortedDictionary.Add(id++, new SortedModel { SortFactor = sortFactor, Word = keyValue.Value });
            }

            var list = sortedDictionary.ToList().OrderByDescending(i => i.Value.SortFactor).ToList();

            _wordsDictionary.Clear();

            id = 0;
            foreach (var item in list)
            {
                _wordsDictionary.Add((id++).ToString(), item.Value.Word);
            }

            return list;
        }

        public string DictonaryToJSON()
        {
            Dictionary<string, string> bufferDic = new Dictionary<string, string>();

            foreach (var keyValue in _wordsDictionary)
            {
                bufferDic.Add(keyValue.Key.ToString(), keyValue.Value);
            }

            return JsonConvert.SerializeObject(bufferDic);
        }

        public string DictonaryToString()
        {
            string bufferString = "";

            foreach (var keyValue in _wordsDictionary)
            {
                bufferString += $"{keyValue.Key}:{keyValue.Value},";
            }

            return bufferString;
        }

    }
}
