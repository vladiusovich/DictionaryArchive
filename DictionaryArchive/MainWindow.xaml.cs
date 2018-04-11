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
        string dictionaryPath = $"L:\\Магистратура\\Tests for programm\\dictionary.txt";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void openFileHandler(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            string inputString, dictionaryString;

            if (openFileDialog.ShowDialog() == true)
            {
                inputString = File.ReadAllText(openFileDialog.FileName);
                SourceString.Text = inputString;
                archiveDictionary.SourceString = inputString;
                try
                {
                    dictionaryString = File.ReadAllText(dictionaryPath);
                    archiveDictionary.OpenDictionary(dictionaryString);
                }
                catch (FileNotFoundException ex)
                {
                    var fileStream = File.Create(dictionaryPath);
                }
                finally
                {

                }

                WordDictionary.Text = archiveDictionary.DictonaryToJSON();
                DictionarySize.Text += archiveDictionary.Dictonary.Count;
            }
        }

        private void SaveResultHandler(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            //saveFileDialog.Filter = "Text file (*.txt)|*.txt|C# file (*.cs)|*.cs";

            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllBytes(saveFileDialog.FileName, archiveDictionary.EncodeString);
        }

        private void SaveDictionaryHandler(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|C# file (*.cs)|*.cs";

            var byteString = archiveDictionary.DictonaryToJSON();

            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, byteString);
        }

        private void Encode_Click(object sender, RoutedEventArgs e)
        {
            archiveDictionary.CreateDictionary(SourceString.Text);

            var encodeSuccess = archiveDictionary.Encode();

            if (!encodeSuccess)
            {
                return;
            }

            LengthSorce.Text += archiveDictionary.SourceString.Length;
            LengthResult.Text += archiveDictionary.EncodeString.Length;

            //WordCount.Text += allWords.Count;

            string resultEncode = "";
            foreach (var s in archiveDictionary.EncodeString)
            {
                resultEncode += s;
            }

            ResultForm.Text = resultEncode;
        }

        private void Decode_Click(object sender, RoutedEventArgs e)
        {
            var decodeSuccess = archiveDictionary.Decode(archiveDictionary.SourceString);

            LengthSorce.Text += archiveDictionary.SourceString.Length;
            LengthResult.Text += archiveDictionary.DecodeString.Length;

            ResultForm.Text = archiveDictionary.DecodeString;
        }
    }
}
