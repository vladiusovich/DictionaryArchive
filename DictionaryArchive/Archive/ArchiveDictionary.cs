using DictionaryArchive.Infrastructure;
using Json2KeyValue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace DictionaryArchive.Archive
{
    public class ArchiveDictionary: IArchiveDictonary
    {
        //private Regex wordsPattern = new Regex("\\w+|\\W+");
        private Regex decodePattern = new Regex("\\d+");
        private Regex wordsPattern = new Regex("\\w+");

        private string _sourceString = "";
        private string _encodeString = "";
        private string _decodeString = "";

        private Dictionary<int, string> _wordsDictionary = new Dictionary<int, string>();
        private List<string> _allWords = new List<string>();

        private int keyId = 0;

        public string EncodeString
        {
            get { return _encodeString; }
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

        public Dictionary<int, string> Dictonary
        {
            get { return _wordsDictionary; }
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
                var refa = RefactoringDictionary();
                _encodeString = EncodeProcess();
            }

            return _encodeString.Length != 0;
        }

        public bool Decode(string encodeString)
        {
            _decodeString = DecodeProcess(encodeString);
            return _decodeString.Length > 0;
        }

        //Добавить в словарь слова если их нет
        public void AddWordToDictionary(string word)
        {
            int id;

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
        public bool AddToDictionary(string sourceString)
        {
            if (sourceString != string.Empty)
            {
                _allWords = SplitText(_sourceString);

                foreach (var word in _allWords)
                {
                    AddWordToDictionary(word);
                }
            }

            return _wordsDictionary.Count > 0;
        }

        private string DecodeProcess(string encodeString)
        {
            var progressString = encodeString;

            foreach (var keyValue in _wordsDictionary)
            {
                var wordPattern = @"\b" + $"{keyValue.Key}" + @"\b";
                progressString = Regex.Replace(progressString, wordPattern, $"{keyValue.Value} ");
            }

            return progressString;
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

            foreach (var keyValue in wordsDictionary)
            {
                //progressDictionaryString += $"{keyValue.Key}: {keyValue.Value} {Environment.NewLine}";
            }

            return wordsDictionary;
        }

        private string EncodeProcess()
        {
            var progressString = _sourceString;
            foreach (var keyValue in _wordsDictionary)
            {
                var trimWord = keyValue.Value.Trim();

                var wordPattern = @"\b" + $"{keyValue.Value}" + @"\b";
                var punctuationPattern = $"[{trimWord}]";

                progressString = Regex.Replace(progressString, wordPattern, $"{keyValue.Key}");


                //if (trimWord.Length == 1)
                //{
                //    if (char.IsPunctuation(trimWord[0]))
                //        progressString = Regex.Replace(progressString, punctuationPattern, $"{keyValue.Key} ");
                //} else
                //{
                //    progressString = Regex.Replace(progressString, wordPattern, $"{keyValue.Key} ");
                //}

            }

            return progressString;
        }

        private List<KeyValuePair<int, SortedModel>> RefactoringDictionary()
        {
            Dictionary<int, SortedModel> sortedDictionary = new Dictionary<int, SortedModel>();
            int id = 0;

            /*
                У коэффициэнтов на частоту вхождений и на длинну слова должны быть разные значения. 
            */
            const decimal lengthWordFactor = 1;
            const decimal frequencyWordInTextFactor = 1;

            foreach (var keyValue in _wordsDictionary)
            {
                int frequencyWordInText = Regex.Matches(_sourceString, keyValue.Value).Count;

                /*
                    sortFactor прямо пропорционален частоте вхождения слова и обратно пропорционален его длинне
                */

                decimal sortFactor = (decimal)(frequencyWordInText * frequencyWordInTextFactor)/ lengthWordFactor * keyValue.Value.Length;

                sortedDictionary.Add(id++, new SortedModel { SortFactor = sortFactor, Word = keyValue.Value });
            }

            var list = sortedDictionary.ToList().OrderBy(i => i.Value.SortFactor).ToList();

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

            return JsonConvert.SerializeObject(bufferDic, Formatting.None);
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
