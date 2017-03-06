using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AlaAssi
{
    public class TextAPI
    {
    }

    public class BatchInput
    {
        public List<DocumentInput> documents { get; set; }
    }
    public class DocumentInput
    {
        public double id { get; set; }
        public string text { get; set; }
    }
    public class BatchResult
    {
        public List<DocumentResult> documents { get; set; }
    }
    public class DocumentResult
    {
        public double score { get; set; }
        public string id { get; set; }
    }
}