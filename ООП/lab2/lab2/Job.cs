using System.Xml.Serialization;

namespace UniversityApp
{
    [System.Serializable]
    public class Job
    {
        [XmlElement("Company")]
        public string Company { get; set; }

        [XmlElement("Position")]
        public string Position { get; set; }

        [XmlElement("Experience")]
        public int Experience { get; set; } 

        public Job() { }

        public override string ToString()
        {
            return $"{Company} - {Position} ({Experience} лет)";
        }
    }
}