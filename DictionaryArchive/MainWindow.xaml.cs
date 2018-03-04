using DictionaryArchive.Archive;
using DictionaryArchive.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace DictionaryArchive
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<int, string> wordsDictionary = new Dictionary<int, string>();
        private string sourceString;
        private string dictionaryString;
        private string resultString;

        private int keyId = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void openFileHandler(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            string inputString = "";
            if (openFileDialog.ShowDialog() == true)
            {
                inputString = File.ReadAllText(openFileDialog.FileName);
                SourceString.Text = inputString;
                sourceString = inputString;
            }

            var allWords = ArchiveDictionary.GetAllWords(inputString);

            string progressDictionaryString = "";
            foreach (var keyValue in wordsDictionary)
            {
                progressDictionaryString += $"{keyValue.Key}: {keyValue.Value} {Environment.NewLine}";
            }

            WordDictionary.Text = progressDictionaryString;
            dictionaryString = progressDictionaryString;
            DictionarySize.Text += wordsDictionary.Count;

            WordCount.Text += allWords.Count;

            string progressString = sourceString;
            foreach (var keyValue in wordsDictionary)
            {
                var pattern = @"\b" + $"{keyValue.Value}" + @"\b";
                progressString = Regex.Replace(progressString, pattern, $"{keyValue.Key} ");
            }

            resultString = progressString;
            ResultForm.Text = resultString;
        }

        private void SaveResultHandler(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|C# file (*.cs)|*.cs";

            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, resultString);
        }

        private void SaveDictionaryHandler(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|C# file (*.cs)|*.cs";

            var byteString = dictionaryString.EncodeTo64();

            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllBytes(saveFileDialog.FileName, byteString);
        }
    }
}
