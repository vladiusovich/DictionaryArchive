using DictionaryArchive.Archive;
using log4net;
using log4net.Config;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private Dictionary<int, string> wordsDictionary = new Dictionary<int, string>();
        Archiver archiveDictionary = new Archiver();
        string commonDictionaryPath = $"CommonDictionary";

        private byte[] inputBytes;
        private string inputFileForCompress = string.Empty;
        string dictionaryString = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            XmlConfigurator.Configure();
        }

        private void openFileHandler(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    inputFileForCompress = File.ReadAllText(openFileDialog.FileName, Encoding.Unicode);

                }
                catch (FileNotFoundException ex)
                {
                    var fileStream = File.Create(commonDictionaryPath);
                }

                SourceString.Text = inputFileForCompress;
                WordDictionary.Text = archiveDictionary.DictonaryToJSON();
                DictionarySize.Text += archiveDictionary.GetDictionary().Count;
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
                DictionarySize.Text += archiveDictionary.GetDictionary().Count;
            }
        }

  
        private void Compress_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            if (saveFileDialog.ShowDialog() == true)
            {
                var dirrectoreyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                DirectoryInfo directoryInfo = new DirectoryInfo(dirrectoreyPath);//Assuming Test is your Folder @"D:\Test"
                FileInfo[] files = directoryInfo.GetFiles("*.dic"); //Getting Text files
                string lastCommonDictionaryPath = "";
                foreach (FileInfo file in files)
                {
                    if (file.Name.Contains(commonDictionaryPath))
                        lastCommonDictionaryPath = file.Name;
                }

                if (!string.IsNullOrEmpty(lastCommonDictionaryPath))
                {
                    var commonDic = File.ReadAllText(lastCommonDictionaryPath);
                    archiveDictionary.InitializeCommonDictionary(commonDic);
                }

                var result = archiveDictionary.Compress(inputFileForCompress);

                File.WriteAllBytes(saveFileDialog.FileName, result.EncodeBytes.ToArray());
                File.WriteAllText($"{saveFileDialog.FileName} (words = {result.DictionaryWordCount.ToString()}).dic", result.Dictionary);
                File.WriteAllText($"{commonDictionaryPath} (words = {result.CommonDictionaryWordCount.ToString()}).dic", result.CommonDictionary);

                Close();
            }
        }

        private void Decode_Click(object sender, RoutedEventArgs e)
        {
            var result = archiveDictionary.Decode(inputBytes);

            ResultForm.Text = result;
        }
    }
}
