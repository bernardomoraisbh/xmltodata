using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace XmlManager
{
    public class Data
    {
        [XmlElement("RowData")]
        public List<RowData> Rows { get; set; }

        [XmlElement("ObjectData")]
        public List<ObjectData> Objects { get; set; }

        [XmlElement("ArrayData")]
        public List<ArrayData> Arrays { get; set; }

        [XmlElement("Error")]
        public string Error { get; set; }

        [XmlAttribute(AttributeName = "noun")]
        public string Noun { get; set; }

        [XmlAttribute(AttributeName = "verb")]
        public string Verb { get; set; }

        public Data()
        {
            Rows = new List<RowData>();
            Objects = new List<ObjectData>();
            Arrays = new List<ArrayData>();
        }

        public Data(IEnumerable<RowData> Rows, string Noun)
        {
            this.Rows = Rows.ToList();
            this.Noun = Noun;
        }

        public static List<CellData> GetCellData(object[] row)
        {
            List<CellData> Cells = new List<CellData>();
            for (int i = 0; i < row.Length; i++)
            {
                CellData cell = new CellData();
                cell.Index = i;
                cell.Value = row[i].ToString();
                Cells.Add(cell);
            }
            return Cells;
        }
    }
}
