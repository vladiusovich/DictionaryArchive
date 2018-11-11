using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryArchive.Models
{
    public class CompressResultContainer
    {
        public List<byte> EncodeBytes { get; set; }
        public string Dictionary { get; set; }
    }
}
