using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vaccination
{
    public class VaccinInputs
    {
        private List<Person> people = new List<Person>();
        private string personCSVPath = @"D:\2023\Progamering\C#\Inlamning4\NyaFiler\Test.csv";
        private int vaccinAmount = 0;
        private bool ageLimit = false;

        public void AddPerson
            (
            string personalNumber,
            string lastName,
            string firstName,
            int healthcareEmployee,
            int riskGroup,
            int infection
            )
        {
            Person person = new Person
            {
                PersonalNumber = personalNumber,
                LastName = lastName,
                FirstName = firstName,
                HealthcareEmployee = healthcareEmployee,
                RiskGroup = riskGroup,
                Infection = infection
            };
            people.Add(person);
        }

        public List<Person> PeopleList()
        {
            return people;
        }

        public void AddToCSVInput()
        {
            var peopleList = PeopleList();

            string[] csvLines =
            peopleList.Select(person => 
            $"{person.PersonalNumber}, {person.LastName}, {person.FirstName}, {person.HealthcareEmployee}, {person.RiskGroup}, {person.Infection}")
            .ToArray();

            File.WriteAllLines(personCSVPath, csvLines);
        }
        public string[] ReadCSVInputFile()
        {
            if(!File.Exists(personCSVPath))
            {
                using (File.Create(personCSVPath)) { }
            }

            string[] lines = File.ReadAllLines(personCSVPath);

            foreach (string line in lines)
            {
                string[] entries = line.Split(',');

                string personalNumber = entries[0];
                string lastName = entries[1];
                string firstName = entries[2];
                int healthcareEmployee = int.Parse(entries[3]);
                int riskGroup = int.Parse(entries[4]);
                int infection = int.Parse(entries[5]);

                people.Add(new Person
                {
                    PersonalNumber = personalNumber,
                    LastName = lastName,
                    FirstName = firstName,
                    HealthcareEmployee = healthcareEmployee,
                    RiskGroup = riskGroup,
                    Infection = infection
                });
            }
            return lines;
        }

        public void ChangeDoseAmount(int doses)
        {
            vaccinAmount = doses;
        }
        public int DosesAmount()
        {
            return vaccinAmount;
        }

        public void ChangeAgeLimit(int change)
        {
            if(change > 0)
            {
                ageLimit = true;
            }
            else 
            { 
                ageLimit = false; 
            }
        }
        public bool AgeLimit()
        {
            return ageLimit;
        }

    }
    public class VaccinFilters
    {
        private List<VaccinPerson> vaccinPeople = new List<VaccinPerson>();
        private string vaccinPersonCSVPath = @"D:\2023\Progamering\C#\Inlamning4\NyaFiler\Vaccinations.csv";

        private List<Person> convertToList = new List<Person>();

        public List<VaccinPerson> VaccinPeopleList()
        {
            return vaccinPeople;
        }
        public string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            FilterPeople(input);
            // Replace with your own code.
            return new string[0];
        }

        public void FilterPeople(string[] input)
        {
            List<string> peopleToFilter = input.ToList();

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
                convertToList.Add(people);
            }
     
            ReformPersonalNumber(convertToList);
        }

        public List<Person> ReformPersonalNumber(List<Person> people)
        {
            // Ändrar personnummer till format 19901125-1111 efter varje filter
            // Arbetar inom vården filter > äldre 65 filter > riskgrupp filter > alla andra
            List<Person> updatePersonalNumber = new List<Person>();

            foreach (var person in people)
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

            OrderByAge(updatePersonalNumber);

            return updatePersonalNumber;
        }
        public List<Person> OrderByAge(List<Person> people)
        {
            List<Person> OrderAge = people.OrderBy(person => person.PersonalNumber).ToList();

            HealthCareEmployeeFilter(OrderAge);

            return OrderAge;
        }

        public List<Person> HealthCareEmployeeFilter(List<Person> people)
        {
            //Först filter alla som är anställda inom vården
            List<Person> result = people
                .Where(people => people.HealthcareEmployee > 0).ToList();
            
            DosesPerPerson(result);
            return result;
        }

        public void DosesPerPerson(List<Person> people)
        {
            int vaccinDoses = 2;

            foreach (var person in people)
            {
                VaccinPerson updatedPerson = new VaccinPerson
                {
                    VPersonalNumber = person.PersonalNumber,
                    VLastName = person.LastName,
                    VFirstName = person.FirstName,
                    VVaccinDose = vaccinDoses - person.Infection
                };
                vaccinPeople.Add(updatedPerson);
            }
        }
    }
}
