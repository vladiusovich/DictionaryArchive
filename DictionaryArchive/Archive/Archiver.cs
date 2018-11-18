using System;
using System.Collections.Generic;
using DictionaryArchive.Models;
using log4net;
using System.Reflection;

namespace DictionaryArchive.Archive
{
    public class Archiver
    {
        private static readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private List<byte> encodeBytes = new List<byte>();

        private ArchiveDictionary archiveDictionary = new ArchiveDictionary();
        private ArchiveDictionary archiveCommonDictionary = new ArchiveDictionary();
        private Compressor compressor;
        private Decompressor decompressor;

  
        public Dictionary<string, string> GetDictionary()
        {
            return archiveDictionary.Dictonary;
        }

        public Dictionary<string, string> GetCommonDictionary()
        {
            return archiveDictionary.Dictonary;
        }


        public void InitializeCommonDictionary(string dictionaryJsonString)
        {
            //SET global dictionary
            archiveCommonDictionary.SetDictionary(dictionaryJsonString);
        }

        public CompressResultContainer Compress(string sourceString)
        {
            var result = new CompressResultContainer();
            try
            {
                archiveCommonDictionary.Initialize(sourceString);

                var isCreated = InitializeDictionary(sourceString);
                if (isCreated)
                {
                    compressor = new Compressor(archiveDictionary);
                    result.EncodeBytes = compressor.Compress(sourceString);
                }
                else
                {
                    throw new Exception("Dictonary was not create.");
                }

                result.Dictionary = DictonaryToJSON(archiveDictionary);
                result.CommonDictionary = DictonaryToJSON(archiveCommonDictionary);

                result.DictionaryWordCount = archiveDictionary.Dictonary.Count;
                result.CommonDictionaryWordCount = archiveCommonDictionary.Dictonary.Count;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return result;
        }


        public string Decode(byte[] encodeBytes)
        {
            string decodeString = "";

            try
            {
                decompressor = new Decompressor(archiveDictionary);
                decodeString = decompressor.Decode(encodeBytes);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
         

            return decodeString;
        }

        public void SetDictionary(string dictionaryJsonString)
        {
            archiveDictionary.SetDictionary(dictionaryJsonString);
        }

        public bool InitializeDictionary(string sourceString)
        {
            return archiveDictionary.Initialize(sourceString);
        }

        public string DictonaryToJSON(ArchiveDictionary ad)
        {
            return ad.DictonaryToJSON();
        }

        public string DictonaryToJSON()
        {
            return archiveDictionary.DictonaryToJSON();
        }

    }
}
