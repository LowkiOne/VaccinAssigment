using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http.Headers;

namespace Vaccination
{
    public class VaccinInputs
    {
        private List<Person> people = new List<Person>();
        private string inputCSVPath = @"D:\2023\Progamering\C#\Inlamning4\NyaFiler\Test.csv";
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

            File.AppendAllLines(inputCSVPath, csvLines);
        }
        public string[] ReadCSVInputFile()
        {
            string[] lines = File.ReadAllLines(inputCSVPath);

            return lines;
        }
        public void ChangeDoseAmount(int doses)
        {
            vaccinAmount = doses;

            Console.WriteLine($"Du har lagg till: {doses} doser");
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
                Console.WriteLine("Du har tagit bort åldergränsen");
            }
            else 
            { 
                ageLimit = false;
                Console.WriteLine("Du har satt en åldergräns");
            }
        }
        public string DisplayAgeLimit()
        {
            string display = "Nej";

            if (ageLimit == true)
            {
                display = "Ja";
            }

            return display;
        }
        public bool AgeLimit()
        {
            return ageLimit;
        }
        public void IndataChange(string input)
        {
            inputCSVPath = input;
        }
        public string HandleFile()
        {
            return inputCSVPath;
        }
        public void OutdataChange(string input)
        {
            FilterPerson vaccinFilters = new FilterPerson();

            string outdataChange = input;

            Console.WriteLine($"Sökväg för utdata ändrad till: {input}");

            vaccinFilters.OutdataChange(outdataChange);
        }
    }
    public class FilterPerson
    {
        private List<VaccinPerson> vaccinPeople = new List<VaccinPerson>();
        private int dosesUse;
        private string outdataCSVPath = @"D:\2023\Progamering\C#\Inlamning4\NyaFiler\Vaccinations.csv";

        private List<Person> convertToList = new List<Person>();
        
        public int DosesUsed()
        {
            return dosesUse;
        }
        public List<VaccinPerson> VaccinPeopleList()
        {
            return vaccinPeople;
        }
        public void OutdataChange(string input)
        {
            outdataCSVPath = input;
        }
        public string HandleOutFile()
        {
            return outdataCSVPath;
        }
        public int BirthDate(string personalNumber)
        {
            if (int.TryParse(personalNumber.Substring(0, 8), out int Birth))
            {
                return Birth;
            }

            return 0;
        }
        public void DosesToPeople(int doses)
        {
            List<int> applyDoses = new List<int>();

            foreach(var person in vaccinPeople) 
            {
                if (doses > 1)
                {
                    doses--;

                    int dose = person.VVaccinDose - 1;
                    dosesUse += doses;
                    applyDoses.Add(dose);
                }
            }
            foreach(int second in applyDoses)
            {
                if(second > 0 && doses > 0)
                {
                    doses--;
                    dosesUse += doses;
                }
            }
        }
        public string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            FilterPeople(input, vaccinateChildren);
            AddToCSVOutPut();
            DosesToPeople(doses);

            return ReadCSVOutPutFile();
        }
        public void FilterPeople(string[] input, bool ageLimit)
        {
            ErrorHandle errorHandle = new ErrorHandle();

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
                convertToList.Add(people);
                var persons = new Person
                {
                    PersonalNumber = personalNumber
                };
                personsToFilter.Add(persons);
            }

            try
            {
                errorHandle.CheckPersonalNumber(personsToFilter);
                ReformPersonalNumber(convertToList, ageLimit);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ett fel på personnummer {e.Message}");
            }
        }
        public void ReformPersonalNumber(List<Person> people, bool ageLimit)
        {
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

            if(!ageLimit)
            {
                List<Person> kids = KidsPeopleFilter(updatePersonalNumber);
                OrderByAge(kids);
            }
            else
            {
                OrderByAge(updatePersonalNumber);
            }
        }
        public void OrderByAge(List<Person> people)
        {
            List<Person> OrderAge = people.OrderBy(person => person.PersonalNumber).ToList();

            HealthCareEmployeeFilter(OrderAge);
            PensionPeopleFilter(OrderAge);
            RiskGroupFilter(OrderAge);
            OtherPeopleFilter(OrderAge);
        }
        public void HealthCareEmployeeFilter(List<Person> people)
        {
            List<Person> result = people
                .Where(person => person.HealthcareEmployee > 0).ToList();

            DosesPerPerson(result);
        }
        public void PensionPeopleFilter(List<Person> people)
        {
            List<Person> result = people
                .Where(person => BirthDate(person.PersonalNumber) <= 19581029 && person.HealthcareEmployee < 1).ToList();

            DosesPerPerson(result);
        }
        public void RiskGroupFilter(List<Person> people)
        {
            List<Person> result = people
                .Where(person => person.RiskGroup > 0 &&
                person.HealthcareEmployee < 1 && BirthDate(person.PersonalNumber) >= 19581029).ToList();

            DosesPerPerson(result);
        }
        public void OtherPeopleFilter(List<Person> people)
        {
            List<Person> result = people
                .Where(person => person.HealthcareEmployee < 1 &&
                BirthDate(person.PersonalNumber) >= 19581029 &&
                person.RiskGroup < 1).ToList();

            DosesPerPerson(result);
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
        public List<Person> KidsPeopleFilter(List<Person> people)
        {
            List<Person> result = people
                .Where(person => BirthDate(person.PersonalNumber) <= 20051029).ToList();
            return result;
        }
        public string[] AddToCSVOutPut()
        {
            var peopleList = VaccinPeopleList();

            string[] csvLines =
            peopleList.Select(person =>
            $"{person.VPersonalNumber}, {person.VLastName}, {person.VFirstName}, {person.VVaccinDose}")
            .ToArray();

            File.WriteAllLines(outdataCSVPath, csvLines);
            return csvLines;
        }
        public string[] ReadCSVOutPutFile()
        {
            string[] lines = File.ReadAllLines(outdataCSVPath);

            return lines;
        }
    }
    public class ErrorHandle
    {
        public void CheckPersonalNumber(List<Person> people)
        {
            FilterPerson filterperson = new FilterPerson();

            foreach (var person in people)
            {
                bool birthDateIsNumber = person.PersonalNumber.Take(6).All(char.IsDigit);
                bool fullBirthDateIsNumber = person.PersonalNumber.Take(8).All(char.IsDigit);
                bool lastFourisNumber = person.PersonalNumber.Length >= 4 &&
                    person.PersonalNumber.Substring(person.PersonalNumber.Length - 4).All(char.IsDigit);
                char check = '-';

                int error = person.PersonalNumber.Length;

                if (error < 10)
                {
                    throw new Exception($"{person.PersonalNumber} är för kort");
                }
                else if (error > 13)
                {
                    throw new Exception($"{person.PersonalNumber} är för långt");
                }

                if (error == 11 || error == 10)
                {
                    if (!birthDateIsNumber || !lastFourisNumber)
                    {
                        throw new Exception($"{person.PersonalNumber} är inte ett personnummer");
                    }
                    if (error == 11)
                    {
                        if (person.PersonalNumber.IndexOf(check) == -1)
                        {
                            throw new Exception($"{person.PersonalNumber} är i fel format");
                        }
                    }
                }
                else if (error == 13 || error == 12)
                {
                    if (!fullBirthDateIsNumber || !lastFourisNumber)
                    {
                        throw new Exception($"{person.PersonalNumber} är inte ett personnummer");
                    }
                    if (error == 13)
                    {
                        if (person.PersonalNumber.IndexOf(check) == -1)
                        {
                            throw new Exception($"{person.PersonalNumber} är i fel format");
                        }
                    }
                }
            }
        }
    }
}
