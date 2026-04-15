using System.Xml.Serialization;

namespace lab2_1
{
    public enum Gender
    {
        [XmlEnum("М")]
        Male,
        [XmlEnum("Ж")]
        Female
    }
}