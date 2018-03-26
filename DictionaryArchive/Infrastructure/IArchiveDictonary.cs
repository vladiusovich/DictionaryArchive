using System;
using System.Collections.Generic;

namespace DictionaryArchive.Infrastructure
{
    public interface IArchiveDictonary
    {
        bool Encode();
        bool Decode(string encodeString);
        string DictonaryToJSON();
        string EncodeString { get; }
        string SourceString { get; set; }
        Dictionary<int, string> Dictonary { get; }
    }
}
