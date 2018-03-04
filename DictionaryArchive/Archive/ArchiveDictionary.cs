using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DictionaryArchive.Archive
{
    public class ArchiveDictionary
    {
        private static Regex wordsPattern = new Regex("\\w+");
        private static Regex punctuationPattern = new Regex("\\W+");

        private static Dictionary<int, string> wordsDictionary = new Dictionary<int, string>();
        private static string dictionaryString;
        private static string resultString;

        private static int keyId = 0;


        public static List<string> GetAllWords(string input)
        {
            var allWords = wordsPattern.Matches(input).Cast<Match>().Select(match => match.Value).Distinct().ToList();
            var allPunctuations = punctuationPattern.Matches(input).Cast<Match>().Select(match => match.Value).Distinct().ToList();

            allWords.AddRange(allPunctuations);

            foreach (var word in allWords)
            {
                var contain = wordsDictionary.ContainsValue(word);

                if (!contain)
                    wordsDictionary.Add(keyId++, word);
            }

            return allWords;
        }

    }
}
