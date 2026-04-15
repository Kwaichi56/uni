using System;
using System.Xml.Serialization;

namespace UniversityApp
{
    [System.Serializable]
    [XmlRoot("Student")]
    public class Student
    {
        [XmlElement("FullName")]
        public string FullName { get; set; }

        [XmlElement("Age")]
        public int Age { get; set; }

        [XmlElement("Speciality")]
        public string Speciality { get; set; }

        [XmlElement("BirthDate")]
        public DateTime BirthDate { get; set; }

        [XmlElement("Course")]
        public int Course { get; set; }

        [XmlElement("Group")]
        public string Group { get; set; }

        [XmlElement("AverageScore")]
        public double AverageScore { get; set; }

        [XmlElement("Gender")]
        public Gender Gender { get; set; }

        [XmlElement("Address")]
        public Address Address { get; set; }

        [XmlElement("Job")]
        public Job Job { get; set; }

        public Student() { } // для сериализации

        public override string ToString()
        {
            return $"{FullName} ({Group}) - ср.балл: {AverageScore:F2}";
        }
    }
}