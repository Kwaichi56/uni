using System;
using System.ComponentModel.DataAnnotations;

namespace lab2_1
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ValidAverageScoreAttribute : ValidationAttribute
    {
        private readonly double _min;

        public ValidAverageScoreAttribute(double min = 0.0)
        {
            _min = min;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult(ErrorMessage ?? "Значение обязательно");

            if (value is double score)
            {
                if (score < _min || score > 10.0)
                    return new ValidationResult(
                        ErrorMessage ?? $"Средний балл должен быть от {_min} до 10.0");

                double rounded = Math.Round(score, 1, MidpointRounding.AwayFromZero);
                if (Math.Abs(rounded - score) > 1e-9)
                    return new ValidationResult("Средний балл: не более 1 знака после запятой");

                return ValidationResult.Success;
            }

            return new ValidationResult("Некорректный тип данных");
        }
    }
}