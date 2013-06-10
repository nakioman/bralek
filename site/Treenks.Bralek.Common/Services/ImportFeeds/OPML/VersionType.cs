using System;
using System.Xml.Serialization;

namespace Treenks.Bralek.Common.Services.ImportFeeds.OPML
{
    [Serializable]
    public enum VersionType {
    
        [XmlEnum("1.0")]
        Item10,
        [XmlEnum("2.0")]
        Item20,
    }
}