using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlManager
{
    public class JsonData
    {
        [XmlAttribute(AttributeName = "noun")]
        public string Noun { get; set; }

        [XmlAttribute(AttributeName = "key")]
        public string Key { get; set; }

        [XmlAttribute(AttributeName = "value")]
        public string Value { get; set; }

        [XmlElement("ObjectData")]
        public List<ObjectData> Objects { get; set; }

        [XmlElement("ArrayData")]
        public List<ArrayData> Arrays { get; set; }

        [XmlElement("ValueData")]
        public List<ValueData> Values { get; set; }

        public JsonData()
        {
            Objects = new List<ObjectData>();
            Arrays = new List<ArrayData>();
            Values = new List<ValueData>();
        }
    }

    public class ValueData : JsonData
    {
    }
    public class ArrayData : JsonData
    {
    }
    public class ObjectData : JsonData
    {
    }
}
