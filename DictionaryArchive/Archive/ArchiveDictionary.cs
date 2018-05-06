using DictionaryArchive.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace DictionaryArchive.Archive
{
    public class ArchiveDictionary: IArchiveDictonary
    {
        //private Regex wordsPattern = new Regex("\\w+|\\W+");
        private Regex decodePattern = new Regex("\\d+");
        //private Regex wordsPattern = new Regex("([a-zA-Zа-яА-Я'-]+)");
        private Regex wordsPattern = new Regex("[a-zA-Zа-яА-Я1-9]+");

        private string _sourceString = "";
        private byte[] encodeBytes;
        private string _decodeString = "";

        private Dictionary<ushort, string> _wordsDictionary = new Dictionary<ushort, string>();
        private List<string> _allWords = new List<string>();

        private ushort keyId = 0;

        public byte[] EncodeBytes
        {
            get { return encodeBytes; }
            set { encodeBytes = value; }
        }

        public string DecodeString
        {
            get { return _decodeString; }
        }

        public string SourceString
        {
            get { return _sourceString; }
            set { _sourceString = value; }
        }

        public Dictionary<ushort, string> Dictonary
        {
            get { return _wordsDictionary; }
        }

        public byte[] EncodeString
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public ArchiveDictionary() { }

        public void OpenDictionary(string dictionaryJsonString)
        {
            var deserializeDictionary = JsonConvert.DeserializeObject<IDictionary<string, string>>(dictionaryJsonString);

            if (deserializeDictionary == null) return;

            foreach (var keyValue in deserializeDictionary)
            {
                AddWordToDictionary(keyValue.Value);
            }
        }

        public bool Encode()
        {
            if (_wordsDictionary.Any())
            {
                //var refa = RefactoringDictionary();
                encodeBytes = EncodeProcess();
            }

            return (encodeBytes != null);
        }

        public bool Decode(byte[] encodeBytes)
        {
            _decodeString = DecodeProcess(encodeBytes);
            return _decodeString.Length > 0;
        }

        //Добавить в словарь слова если их нет
        public void AddWordToDictionary(string word)
        {
            ushort id;

            if (word != string.Empty)
            {
                if (!_wordsDictionary.ContainsValue(word))
                {
                    if (_wordsDictionary.Any())
                    {
                        id = _wordsDictionary.Last().Key;
                    } else
                    {
                        id = keyId;
                    }
                    _wordsDictionary.Add(++id, word);
                }
            }
        }

        //Переписчать
        public bool CreateDictionary(string sourceString)
        {
            if (sourceString != string.Empty)
            {
                _allWords = SplitText(_sourceString);

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
                if (char.IsPunctuation(symbol) || symbol == ' ' || symbol == '\r' || symbol == '\n') {
                    AddWordToDictionary(symbol.ToString());
                }
            }
            return wordsPattern.Matches(input).Cast<Match>().Select(match => match.Value.Trim()).Distinct().ToList();
        }
        private List<string> SplitText(string input)
        {
            return wordsPattern.Matches(input).Cast<Match>().Select(match => match.Value.Trim()).Distinct().ToList();
        }

        private Dictionary<int, string> CreateDictionary(List<string> allWords)
        {
            Dictionary<int, string> wordsDictionary = new Dictionary<int, string>();

            foreach (var word in allWords)
            {
                if (word == string.Empty) continue;

                var contain = wordsDictionary.ContainsValue(word);

                if (!contain)
                    wordsDictionary.Add(keyId++, word);
            }

            return wordsDictionary;
        }

        private byte[] EncodeProcess()
        {
            byte[] encodeResult = ParseString();

            return encodeResult;
        }

        //implemented
        private string DecodeProcess(byte[] encodeBytes)
        {
            string decodeString = "";

            byte[] encodeSymbol = new byte[2];
            int encodeIndex = 0;
            foreach (var b in encodeBytes)
            {
                if (encodeIndex <= 1)
                {
                    encodeSymbol[encodeIndex++] = b;
                } else
                {
                    Array.Reverse(encodeSymbol);
                    ushort symbolId = BitConverter.ToUInt16(encodeSymbol, 0);
                    try
                    {
                        var keyValue = Dictonary.Single(w => w.Key == symbolId);
                        var word = keyValue.Value;
                        decodeString += word;
                        encodeIndex = 0;

                        encodeSymbol[encodeIndex++] = b;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
         
                }

            }

            return decodeString;
        }


        private byte[] ParseString()
        {
            string currentParseWord = string.Empty;
            Stack<byte> encodeResult = new Stack<byte>();
            byte[] encodeId = new byte[] { };

            ushort wordId;

            for (int index = 0; index <= _sourceString.Length; index++)
            {
                try
                {

                    char gliphy = _sourceString[index];

                    //Если глиф пробел или знак то ишем его в словаре и переходим к след глифу
                    if (char.IsWhiteSpace(gliphy) || char.IsPunctuation(gliphy))
                    {
                        wordId = GetWordId(gliphy);
                        encodeId = UsortToButes(wordId);
                        PushToStack(ref encodeId, ref encodeResult);
                        continue;
                    }

                    //пока не достигли конца текста - ищем слова
                    if (index + 1 < _sourceString.Length)
                    {
                        //Если след буква значит слово не закончилось - добавляем к нелослоау и преходи к след глифу
                        if (char.IsLetter(_sourceString[index + 1]) || char.IsNumber(_sourceString[index + 1]))
                        {
                            currentParseWord += gliphy;
                            continue;
                        }
                        //иначе все же будет конец слова то добавляем последний глиф и ищем недослово
                        else
                        {
                            currentParseWord += gliphy;
                            wordId = GetWordId(currentParseWord);
                            encodeId = UsortToButes(wordId);
                            PushToStack(ref encodeId, ref encodeResult);
                            currentParseWord = string.Empty;
                            continue;

                            //Если служебный знак то идем к след глифу
                            if (_sourceString[index + 1] == '\'')
                                continue;
                        }
                    }
                    // если конец текста то добаляем последний глиф к недослову. Ищем недослово и конец парсинга
                    else
                    {
                        currentParseWord += gliphy;
                        wordId = GetWordId(currentParseWord);
                        encodeId = UsortToButes(wordId);
                        PushToStack(ref encodeId, ref encodeResult);
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

            byte[] byteArrayEncodeResult = encodeResult.ToArray();

            Array.Reverse(byteArrayEncodeResult);
            return byteArrayEncodeResult;
        }

        private void PushToStack(ref byte[] encodeId, ref Stack<byte> encodeResult)
        {
            foreach (var b in encodeId)
            {
                encodeResult.Push(b);
            }
        }

        private byte[] UsortToButes(ushort n)
        {
            byte[] keyBytes = BitConverter.GetBytes(n);
            Array.Reverse(keyBytes);
            return keyBytes;
        }

        private ushort GetWordId(string word)
        {
            return _wordsDictionary.First(x => x.Value == word).Key;
        }

        private ushort GetWordId(char gliph)
        {
            return _wordsDictionary.First(x => x.Value == gliph.ToString()).Key;
        }

        private List<KeyValuePair<ushort, SortedModel>> RefactoringDictionary()
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
                int frequencyWordInText = Regex.Matches(_sourceString,$"{keyValue.Value}").Count;

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
                _wordsDictionary.Add(id++, item.Value.Word);
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
