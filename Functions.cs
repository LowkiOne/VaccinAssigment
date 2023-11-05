using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using Vaccination;

namespace VaccinAssigment
{
    public class Functions
    {
        public string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            string[] result = PersonsToList(input, doses, vaccinateChildren);

            return result;
        }

        private string[] PersonsToList(string[] input, int doses, bool ageLimit)
        {
            List<string> peopleToFilter = input.ToList();
            List<Person> personsToFilter = new List<Person>();

            foreach (string person in peopleToFilter)
            {
                string[] entries = person.Split(',');

                string personalNumber = entries[0];
                string lastName = entries[1];
                string firstName = entries[2];
                int healthcareEmployee = int.Parse(entries[3]);
                int riskGroup = int.Parse(entries[4]);
                int infection = int.Parse(entries[5]);

                var people = new Person
                {
                    PersonalNumber = personalNumber,
                    LastName = lastName,
                    FirstName = firstName,
                    HealthcareEmployee = healthcareEmployee,
                    RiskGroup = riskGroup,
                    Infection = infection,
                };
                personsToFilter.Add(people);
            }
            string[] result = ReformPersonalNumber(personsToFilter, doses, ageLimit);
            return result;
        }

        private string[] ReformPersonalNumber(List<Person> persons, int doses, bool age)
        {
            List<Person> updatePersonalNumber = new List<Person>();

            foreach (var person in persons)
            {
                string personalNumber = person.PersonalNumber;

                if (personalNumber.Length == 10)
                {
                    personalNumber = $"19{personalNumber.Substring(0, 6)}-{personalNumber.Substring(6)}";
                }
                else if (personalNumber.Length == 11)
                {
                    personalNumber = $"19{personalNumber}";
                }
                else if (personalNumber.Length == 12)
                {
                    personalNumber = $"{personalNumber.Substring(0, 8)}-{personalNumber.Substring(8)}";
                }

                Person updatedPerson = new Person
                {
                    PersonalNumber = personalNumber,
                    LastName = person.LastName,
                    FirstName = person.FirstName,
                    HealthcareEmployee = person.HealthcareEmployee,
                    RiskGroup = person.RiskGroup,
                    Infection = person.Infection
                };
                updatePersonalNumber.Add(updatedPerson);
            }
            string[] result = FilterPersons(updatePersonalNumber, age);
            return result;
        }
        private string[] FilterPersons(List<Person> persons, bool age)
        {
            int defaultVaccinDose = 2;
            List<VaccinPerson> vaccinPeople = new List<VaccinPerson>();

            List<Person> FilterPersons = persons
                .Where(x => !age && BirthDate(x.PersonalNumber) <= 18)
                .OrderBy(x => x.PersonalNumber)
                .ThenBy(x => x.HealthcareEmployee > 0)
                .ThenBy(x => BirthDate(x.PersonalNumber) >= 65)
                .ThenBy(x => x.RiskGroup > 0)
                .ToList();

            foreach (Person person in FilterPersons)
            {
                VaccinPerson ChangeForm = new VaccinPerson
                {
                    VPersonalNumber = person.PersonalNumber,
                    VLastName = person.LastName,
                    VFirstName = person.FirstName,
                    VVaccinDose = defaultVaccinDose - person.Infection
                };
                vaccinPeople.Add(ChangeForm);
            }

            var personsToFile = vaccinPeople;

            string[] csvLines =
            personsToFile.Select(x =>
            $"{x.VPersonalNumber}, {x.VLastName}, {x.VFirstName}, {x.VVaccinDose}")
            .ToArray();

            return csvLines;
        }

        private int BirthDate(string personalNumber)
        {
            DateTime today = DateTime.Today;

            int year = int.Parse(personalNumber.Substring(0, 4));
            int month = int.Parse(personalNumber.Substring(4, 2));
            int day = int.Parse(personalNumber.Substring(6, 2));

            DateTime birthDate = new DateTime(year, month, day);
            int result = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-result))
            {
                result--;
            }

            return result;
        }
    }
}
