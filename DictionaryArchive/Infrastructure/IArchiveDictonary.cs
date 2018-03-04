using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryArchive.Infrastructure
{
    public interface IArchiveDictonary
    {
        bool Encode();
        bool Decode();
        string DictonaryToJSON();
        string EncodeString { get; }
        string SourceString { get; set; }
        Dictionary<int, string> Dictonary { get; }
    }
}
