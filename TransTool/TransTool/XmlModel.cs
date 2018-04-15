using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TransTool
{
    [XmlRoot("root")]
    public class XmlModel
    {
        public Config Config { get; set; }
    }

    public class Config
    {
        [XmlAttribute]
        public string Application { get; set; }
        [XmlAttribute]
        public string Language { get; set; }

        [XmlElement("Row")]
        public Row[] Rows { get; set; }
    }

    [XmlRoot("Row")]
    public class Row
    {
        [XmlAttribute]
        public string TextID { get; set; }

        public string ENGLISH { get; set; }

        public string CHINESE_SIMPLIFIED { get; set; }

        public string CHINESE_Traditional { get; set; }

    }
}
