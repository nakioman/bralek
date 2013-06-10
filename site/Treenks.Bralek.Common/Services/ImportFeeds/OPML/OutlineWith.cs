using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Treenks.Bralek.Common.Services.ImportFeeds.OPML
{
    [Serializable]
    public class OutlineWith : Outline {
        public OutlineWith() {
            Type = "text";
        }

        [XmlElement("outline")]
        public OutlineWith[] Outline { get; set; }

        [XmlAttribute("type"), DefaultValue("text")]
        public string Type { get; set; }
    }
}