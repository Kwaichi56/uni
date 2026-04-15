using System.Xml.Serialization;

namespace lab2_1
{
    [System.Serializable]
    public class Address
    {
        [XmlElement("City")]
        public string City { get; set; }

        [XmlElement("Zip")]
        public string Zip { get; set; }

        [XmlElement("Street")]
        public string Street { get; set; }

        [XmlElement("House")]
        public string House { get; set; }

        [XmlElement("Apartment")]
        public string Apartment { get; set; }

        public Address() { } // для сериализации

        public override string ToString()
        {
            return $"{City}, {Street} {House}/{Apartment}";
        }
    }
}