using System;
using System.Collections.Generic;

namespace DictionaryArchive.Infrastructure
{
    public interface IArchiveDictonary
    {
        bool Encode();
        bool Decode(byte[] encodeBytes);
        string DictonaryToJSON();
        byte[] EncodeString { get; }
        string SourceString { get; set; }
        Dictionary<ushort, string> Dictonary { get; }
    }
}
