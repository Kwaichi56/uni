using System.Xml.Serialization;

namespace UniversityApp
{
    public enum Gender
    {
        [XmlEnum("М")]
        Male,
        [XmlEnum("Ж")]
        Female
    }
}