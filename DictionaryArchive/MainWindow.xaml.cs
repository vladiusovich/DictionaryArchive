using DictionaryArchive.Archive;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        string dictionaryPath = $"dictionary.txt";

        private byte[] inputBytes;
        private string inputFile = string.Empty;
        string dictionaryString = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void openFileHandler(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    inputFile = File.ReadAllText(openFileDialog.FileName, Encoding.Unicode);

                    SourceString.Text = inputFile;

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

        private void openEncodeFileHandler(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                inputBytes = File.ReadAllBytes(openFileDialog.FileName);
                dictionaryString = File.ReadAllText(openFileDialog.FileName + ".dic");
                archiveDictionary.SetDictionary(dictionaryString);

                string resultEncode = "";
                foreach (var s in inputBytes)
                {
                    resultEncode += s;
                }
                SourceString.Text = resultEncode;

                WordDictionary.Text = archiveDictionary.DictonaryToJSON();
                DictionarySize.Text += archiveDictionary.Dictonary.Count;
            }
        }

  
        private void SaveDictionaryHandler(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt|C# file (*.cs)|*.cs";

            var dictonaryToJSON = archiveDictionary.DictonaryToJSON();
                File.WriteAllText(dictionaryPath, dictonaryToJSON);
        }

        private void Compress_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            if (saveFileDialog.ShowDialog() == true)
            {
                var result = archiveDictionary.Compress(inputFile);

                File.WriteAllBytes(saveFileDialog.FileName, result.EncodeBytes.ToArray());
                File.WriteAllText(saveFileDialog.FileName + ".dic", result.Dictionary);

                Close();
            }

            //else
            //{
            //    //LengthSorce.Text += archiveDictionary.SourceString.Length;
            //    LengthResult.Text += archiveDictionary.EncodeBytes.Count;

            //    string resultEncode = "";
            //    foreach (var s in archiveDictionary.EncodeBytes)
            //    {
            //        resultEncode += s;
            //    }

            //    ResultForm.Text = resultEncode;

            //    WordDictionary.Text = string.Empty;
            //    WordDictionary.Text = archiveDictionary.DictonaryToJSON();

            //    DictionarySize.Text = string.Empty;
            //    DictionarySize.Text += archiveDictionary.Dictonary.Count;
            //}


        }

        private void Decode_Click(object sender, RoutedEventArgs e)
        {
            var result = archiveDictionary.Decode(inputBytes);

            ResultForm.Text = result;
        }
    }
}
