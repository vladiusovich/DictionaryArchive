using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DictionaryArchive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<int, string> wordsDictionary = new Dictionary<int, string>();
        private string sourceString;
        private string resultString;

        private int keyId = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void openFileHandler(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                var inputString = File.ReadAllText(openFileDialog.FileName);

                SourceString.Text = inputString;
                sourceString = inputString;
            }

            Regex wordsPattern = new Regex("\\w+");

            var allWords = wordsPattern.Matches(sourceString).Cast<Match>().Select(match => match.Value).ToList();

            foreach (var word in allWords)
            {
                var contain = wordsDictionary.ContainsValue(word);

                if (!contain)
                    wordsDictionary.Add(keyId++, word);
            }

            string dictionaryString = "";
            foreach (var keyValue in wordsDictionary)
            {
                dictionaryString += $"{keyValue.Key}: {keyValue.Value} {Environment.NewLine}";
            }

            WordDictionary.Text = dictionaryString;
            DictionarySize.Text += wordsDictionary.Count;

            string progressString = sourceString;
            foreach (var keyValue in wordsDictionary)
            {
                progressString = Regex.Replace(progressString, $"{keyValue.Value}", $"{keyValue.Key}");
            }

            resultString = progressString;
            ResultForm.Text = resultString;
        }

        private void SaveResultHandler(object sender, RoutedEventArgs e)
        {

        }

        private void SaveDictionaryHandler(object sender, RoutedEventArgs e)
        {

        }
    }
}
