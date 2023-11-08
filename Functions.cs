using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using Vaccination;
using System.Runtime.CompilerServices;

namespace VaccinAssigment
{
    public class Functions
    {
        private static List<VaccinPerson> vaccinPeople = new List<VaccinPerson>();
        private static int dosesUsed = 0;
        public string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            List<Person> persons = PersonsToList(input, vaccinateChildren);
            CalculateDoses(doses);
            var result = ListToCSV(persons);

            return result;
        }

        private void CalculateDoses(int doses)
        {
            int remaningdoses = doses;

            for(int i = 0; i < vaccinPeople.Count; i++)
            {
                if (remaningdoses == 0)
                    break;
                if (vaccinPeople[i].VVaccinDoseTaken == 1 && vaccinPeople[i].VVaccinDoseNeeded == 2)
                {
                    remaningdoses--;
                    vaccinPeople[i].VVaccinDoseTaken++;
                    dosesUsed++;
                }
                else if (remaningdoses == 1)
                {
                    i = -1;
                }
                else
                {
                    remaningdoses--;
                    vaccinPeople[i].VVaccinDoseTaken++;
                    dosesUsed++;
                }
                
            }
        }
        private string[] ListToCSV(List<Person> persons)
        {
            var personsToFile = vaccinPeople;

            string[] csvLines =
            personsToFile.Select(x =>
            $"{x.VPersonalNumber},{x.VLastName},{x.VFirstName},{x.VVaccinDoseNeeded}")
            .ToArray();

            return csvLines;
        }
        private List<Person> PersonsToList(string[] input, bool age)
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
            var reformList = ReformPersonalNumber(personsToFilter, age);
            var result = FilterPersons(reformList, age);

            return result;
        }

        private List<Person> ReformPersonalNumber(List<Person> persons, bool age)
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
            
            return updatePersonalNumber;
        }
        private List<Person> FilterPersons(List<Person> persons, bool age)
        {
            int defaultVaccinDose = 2;

            List<Person> filterPersons = KidsFilter(persons, age);
            List<Person> healthFilter = filterPersons
                .Where(x => x.HealthcareEmployee > 0)
                .ToList();
            List<Person> pensionFilter = filterPersons
                .Where(x => BirthDate(x.PersonalNumber) >= 65 && x.HealthcareEmployee < 1)
                .ToList();
            List<Person> riskGroupFilter = filterPersons
                .Where(x => x.RiskGroup > 0 && BirthDate(x.PersonalNumber) < 65 && x.HealthcareEmployee < 1)
                .ToList();
            List<Person> otherFilter = filterPersons
                .Where(x => BirthDate(x.PersonalNumber) < 65 && x.HealthcareEmployee < 1 && x.RiskGroup < 1)
                .ToList();
            List<Person> filterDone = healthFilter.Concat(pensionFilter).Concat(riskGroupFilter).Concat(otherFilter).ToList();
            //break out?
            foreach (var person in filterDone)
            {
                VaccinPerson ChangeForm = new VaccinPerson
                {
                    VPersonalNumber = person.PersonalNumber,
                    VLastName = person.LastName,
                    VFirstName = person.FirstName,
                    VVaccinDoseNeeded = defaultVaccinDose - person.Infection
                };
                vaccinPeople.Add(ChangeForm);
            }

            return filterDone;
        }

        private List<Person> KidsFilter(List<Person> person, bool age)
        {
            List<Person> filterPersons = person
                .OrderBy(x => x.PersonalNumber).ToList();

            if (!age)
            {
                 filterPersons = person
                    .Where(x => BirthDate(x.PersonalNumber) >= 18)
                    .OrderBy(x => x.PersonalNumber).ToList();
            }
            return filterPersons;
        }

        public int DosesUsed()
        {
            return dosesUsed;
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
