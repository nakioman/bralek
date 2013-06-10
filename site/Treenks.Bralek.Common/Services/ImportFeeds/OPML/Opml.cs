using System;
using System.Xml.Serialization;

namespace Treenks.Bralek.Common.Services.ImportFeeds.OPML
{
    [Serializable]
    [XmlRoot("opml", IsNullable=false)]
    public class Opml {
        [XmlElement("head")]
        public Head Head { get; set; }

        [XmlElement("body")]
        public OutlineWith Body { get; set; }

        [XmlAttribute("version")]
        public VersionType Version { get; set; }
    }
}