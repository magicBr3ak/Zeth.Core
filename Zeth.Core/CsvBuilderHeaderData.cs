using System.Collections.Generic;

namespace Zeth.Core
{
    public class CsvBuilderHeaderData
    {
        public string HeaderName { get; set; }
        public IEnumerable<string> HeaderData { get; set; }
    }
}
