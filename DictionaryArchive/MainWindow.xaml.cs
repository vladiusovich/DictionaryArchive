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
        ArchiveDictionary archiveDictionary = new ArchiveDictionary();

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
            }

            archiveDictionary.SourceString = inputString;

            var encodeSuccess = archiveDictionary.Encode();

            WordDictionary.Text = archiveDictionary.DictonaryToJSON();
            DictionarySize.Text += archiveDictionary.Dictonary.Count;

            LengthSorce.Text += archiveDictionary.SourceString.Length;
            LengthResult.Text += archiveDictionary.EncodeString.Length;

            //WordCount.Text += allWords.Count;

            ResultForm.Text = archiveDictionary.EncodeString;
        }

        private void SaveResultHandler(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|C# file (*.cs)|*.cs";

            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, archiveDictionary.EncodeString);
        }

        private void SaveDictionaryHandler(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|C# file (*.cs)|*.cs";

            var byteString = archiveDictionary.DictonaryToString().EncodeTo64();

            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllBytes(saveFileDialog.FileName, byteString);
        }
    }
}
