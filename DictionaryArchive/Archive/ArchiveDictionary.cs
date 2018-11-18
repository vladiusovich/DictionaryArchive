using DictionaryArchive.Models;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DictionaryArchive.Archive
{
    public class ArchiveDictionary
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<string, string> dictionary = new Dictionary<string, string>();
        private List<string> _allWords = new List<string>();

        private Regex decodePattern = new Regex("\\d+");
        private Regex wordsPattern = new Regex("[a-zA-Zа-яА-Я1-9]+");

        private int globalKeyId = 0;

        public Dictionary<string, string> Dictonary
        {
            get { return dictionary; }
        }

        public bool Initialize(string sourceString)
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

            return dictionary.Count > 0;
        }

        public bool SetDictionary(string dictionaryJsonString)
        {
            var deserializeDictionary = JsonConvert.DeserializeObject<IDictionary<string, string>>(dictionaryJsonString);

            if (deserializeDictionary == null) return false;

            foreach (var keyValue in deserializeDictionary)
            {
                dictionary.Add(keyValue.Key, keyValue.Value);
            }

            return true;
        }

        public string GetWordById(ushort id)
        {
            string strId = id.ToString();
            if (!dictionary.ContainsKey(strId)) return string.Empty;

            var word = dictionary[strId];
            return word ?? string.Empty;
        }

        //оперделяем сколько бит нужно для конкретного словаря и используем это число для того, чтобы отрезать лишнюю часть битовой последовательности
        public double GetAmountOfBitsForEncode()
        {
            return Math.Ceiling(Math.Log(dictionary.Count, 2)) + 1;
        }

        public string GetWordId(string word)
        {
            return dictionary.First(x => x.Value == word).Key;
        }

        public string GetWordId(char gliph)
        {
            return dictionary.First(x => x.Value == gliph.ToString()).Key;
        }

        public string DictonaryToJSON()
        {
            Dictionary<string, string> bufferDic = new Dictionary<string, string>();

            foreach (var keyValue in dictionary)
            {
                bufferDic.Add(keyValue.Key.ToString(), keyValue.Value);
            }

            return JsonConvert.SerializeObject(bufferDic);
        }

        //Добавить в словарь слова если их нет
        private void AddWordToDictionary(string word)
        {
            string id;

            try
            {
                if (word != string.Empty)
                {
                    if (!dictionary.ContainsValue(word))
                    {
                        if (dictionary.Any())
                        {
                            id = dictionary.Last().Key;
                        }
                        else
                        {
                            id = (++globalKeyId).ToString();
                        }

                        var parseId = Convert.ToUInt16(id);
                        parseId++;

                        dictionary.Add(parseId.ToString(), word);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
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


        //private List<KeyValuePair<ushort, SortedModel>> RefactoringDictionary(string sourceString)
        //{
        //    Dictionary<ushort, SortedModel> sortedDictionary = new Dictionary<ushort, SortedModel>();
        //    ushort id = 0;

        //    /*
        //        У коэффициэнтов на частоту вхождений и на длинну слова должны быть разные значения. 
        //    */
        //    const decimal lengthWordFactor = 1;
        //    const decimal frequencyWordInTextFactor = 1;

        //    foreach (var keyValue in dictionary)
        //    {
        //        /* Проблема с [] ()  */
        //        int frequencyWordInText = Regex.Matches(sourceString, $"{keyValue.Value}").Count;

        //        /*
        //            sortFactor прямо пропорционален частоте вхождения слова и обратно пропорционален его длинне
        //        */

        //        //decimal sortFactor = (decimal)(frequencyWordInText * frequencyWordInTextFactor)/ lengthWordFactor * keyValue.Value.Length;

        //        decimal sortFactor = frequencyWordInText;

        //        sortedDictionary.Add(id++, new SortedModel { SortFactor = sortFactor, Word = keyValue.Value });
        //    }

        //    var list = sortedDictionary.ToList().OrderByDescending(i => i.Value.SortFactor).ToList();

        //    dictionary.Clear();

        //    id = 0;
        //    foreach (var item in list)
        //    {
        //        dictionary.Add((id++).ToString(), item.Value.Word);
        //    }

        //    return list;
        //}
    }
}
