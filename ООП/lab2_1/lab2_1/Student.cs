using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace lab2_1
{
    [Serializable]
    [XmlRoot("Student")]
    public class Student
    {
        [Required(ErrorMessage = "ФИО обязательно для заполнения")]
        [RegularExpression(@"^[А-ЯЁа-яёA-Za-z\s\-]{2,100}$",
            ErrorMessage = "ФИО: только буквы (2-100 символов)")]
        [XmlElement("FullName")]
        public string FullName { get; set; }

        [Range(16, 60, ErrorMessage = "Возраст должен быть от 16 до 60 лет")]
        [XmlElement("Age")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Специальность обязательна")]
        [XmlElement("Speciality")]
        public string Speciality { get; set; }

        [XmlElement("BirthDate")]
        public DateTime BirthDate { get; set; }

        [Range(1, 4, ErrorMessage = "Курс должен быть от 1 до 4")]
        [XmlElement("Course")]
        public int Course { get; set; }

        [Required(ErrorMessage = "Группа обязательна")]
        [RegularExpression(@"^[А-ЯЁа-яёA-Za-z0-9\-]{1,20}$",
            ErrorMessage = "Группа: буквы, цифры, дефис (до 20 символов)")]
        [XmlElement("Group")]
        public string Group { get; set; }

        [ValidAverageScore(0.0, ErrorMessage = "Средний балл должен быть от 0.0 до 10.0")]
        [XmlElement("AverageScore")]
        public double AverageScore { get; set; }

        [XmlElement("Gender")]
        public Gender Gender { get; set; }

        [XmlElement("Address")]
        public Address Address { get; set; }

        [XmlElement("Job")]
        public Job Job { get; set; }

        public Student() { }

        public override string ToString() =>
            $"{FullName} ({Group}) - ср.балл: {AverageScore:F2}";
    }
}