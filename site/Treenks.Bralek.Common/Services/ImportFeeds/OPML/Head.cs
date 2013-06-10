using System;
using System.Xml.Serialization;

namespace Treenks.Bralek.Common.Services.ImportFeeds.OPML
{
    [Serializable]
    public class Head
    {
        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("dateCreated")]
        public string DateCreated { get; set; }

        [XmlElement("dateModified")]
        public string DateModified { get; set; }

        [XmlElement("ownerName")]
        public string OwnerName { get; set; }

        [XmlElement("ownerEmail")]
        public string OwnerEmail { get; set; }

        [XmlElement("ownerId", DataType = "anyURI")]
        public string OwnerId { get; set; }

        [XmlElement("docs", DataType = "anyURI")]
        public string Docs { get; set; }

        [XmlElement("expansionState")]
        public string ExpansionState { get; set; }

        [XmlElement("vertScrollState", DataType = "positiveInteger")]
        public string VertScrollState { get; set; }

        [XmlElement("windowTop", DataType = "integer")]
        public string WindowTop { get; set; }

        [XmlElement("windowLeft", DataType = "integer")]
        public string WindowLeft { get; set; }

        [XmlElement("windowBottom", DataType = "integer")]
        public string WindowBottom { get; set; }

        [XmlElement("windowRight", DataType = "integer")]
        public string WindowRight { get; set; }
    }
}