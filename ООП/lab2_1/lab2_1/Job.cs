using System.Xml.Serialization;

namespace lab2_1
{
    [System.Serializable]
    public class Job
    {
        [XmlElement("Company")]
        public string Company { get; set; }

        [XmlElement("Position")]
        public string Position { get; set; }

        [XmlElement("Experience")]
        public int Experience { get; set; } // стаж в годах

        public Job() { } // для сериализации

        public override string ToString()
        {
            return $"{Company} - {Position} ({Experience} лет)";
        }
    }
}