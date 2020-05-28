using System.Collections.Generic;
using System.Xml.Serialization;

namespace XmlManager
{
    public class RowData
    {
        [XmlAttribute(AttributeName = "noun")]
        public string Noun { get; set; }

        [XmlElement("CellsData")]
        public List<CellData> Cells { get; set; }
        public RowData()
        {
            Cells = new List<CellData>();
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
