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
        private Regex wordsPattern = new Regex("\\w+");

        private string _sourceString = "";
        private string _encodeString = "";

        private Dictionary<int, string> _wordsDictionary = new Dictionary<int, string>();
        private List<string> _allWords = new List<string>();

        private int keyId = 0;

        public string EncodeString
        {
            get { return _encodeString; }
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

        public bool Encode()
        {
            if (_sourceString != string.Empty)
            {
                _allWords = SplitText(_sourceString);
                _wordsDictionary = CreateDictionary(_allWords);
            }

            if (_wordsDictionary.Any())
                _encodeString = EncodeProcess();

            return _encodeString.Length != 0;
        }

        public bool Decode()
        {
            throw new NotImplementedException();
        }

        public string DictonaryToJSON()
        {
            Dictionary<string, string> bufferDic= new Dictionary<string, string>();

            foreach (var keyValue in _wordsDictionary)
            {
                bufferDic.Add(keyValue.Key.ToString(), keyValue.Value);
            }
            
            return JsonConvert.SerializeObject(bufferDic, Formatting.Indented);
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
                var pattern = @"\b" + $"{keyValue.Value}" + @"\b";
                progressString = Regex.Replace(progressString, pattern, $"{keyValue.Key}");
            }

            return progressString;
        }


    }
}
