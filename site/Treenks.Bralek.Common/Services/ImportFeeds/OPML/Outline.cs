using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Treenks.Bralek.Common.Services.ImportFeeds.OPML
{
    [Serializable]
    [XmlInclude(typeof(OutlineWith))]    
    public class Outline
    {
        public Outline()
        {
            IsComment = false;
            IsBreakpoint = false;
        }

        [XmlAttribute("text")]
        public string Text { get; set; }

        [XmlAttribute("isComment"), DefaultValue(false)]
        public bool IsComment { get; set; }

        [XmlAttribute("isBreakpoint"), DefaultValue(false)]
        public bool IsBreakpoint { get; set; }

        [XmlAttribute("created")]
        public string Created { get; set; }

        [XmlAttribute("category")]
        public string Category { get; set; }

        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlAttribute("url", DataType = "anyURI")]
        public string Url { get; set; }

        [XmlAttribute("htmlUrl", DataType = "anyURI")]
        public string HtmlUrl { get; set; }

        [XmlAttribute("xmlUrl", DataType = "anyURI")]
        public string XmlUrl { get; set; }

        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("language")]
        public string Language { get; set; }
    }
}