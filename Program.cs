using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Vaccination
{
    public class Program
    {
        private static bool running = true;
        public static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            VaccinInputs vaccinInputs = new VaccinInputs();
            VaccinFilters vaccinFilters = new VaccinFilters();
            
            vaccinInputs.AddPerson("200610012341", "Doe", "John", 0, 1, 0);
            vaccinInputs.AddPerson("200611011234", "Smith", "Jane", 0, 1, 1);
            vaccinInputs.AddPerson("200611251111", "Ern", "Dan", 0, 0, 0);
            vaccinInputs.AddToCSVInput();


            vaccinFilters.CreateVaccinationOrder(vaccinInputs.ReadCSVInputFile(), vaccinInputs.DosesAmount(), vaccinInputs.AgeLimit());

            List<VaccinPerson> vaccinPeople = vaccinFilters.VaccinPeopleList();
 
            foreach (var person in vaccinPeople)
            {
                Console.WriteLine($"Personal Number: {person.VPersonalNumber}");
                Console.WriteLine($"First Name: {person.VFirstName}");
                Console.WriteLine($"Last Name: {person.VLastName}");
                Console.WriteLine($"Doses: {person.VVaccinDose}");
            }
             
            Console.WriteLine(vaccinInputs.DosesAmount() - vaccinFilters.DosesUsed());

        }
        
        public static void CreatePerson()
        {

        }
        public static void VaccinAmount()
        {
            VaccinInputs vaccinInputs = new VaccinInputs();

            Console.Write("Ändra antalet vaccindoser: ");
            int vaccinAmount = int.Parse(Console.ReadLine());

            vaccinInputs.ChangeDoseAmount(vaccinAmount);

        }
        public static void AgeLimit()
        {
            VaccinInputs vaccinInputs = new VaccinInputs();

            int ageLimit = ShowOption("Sätt åldersgräns?");

            vaccinInputs.ChangeAgeLimit(ageLimit);
        }
        public static void PriorityOrder()
        {

        }
        public static void ShowPriorityOrder()
        {

        }
        public static void IndataChange()
        {

        }
        public static void OutdataChange()
        {

        }
        public static void Exit()
        {

        }

        public static void MainMenu()
        {
            VaccinInputs vaccinInputs = new VaccinInputs();
            VaccinFilters vaccinFilters = new VaccinFilters();

            int vaccinResult = vaccinInputs.DosesAmount() - vaccinFilters.DosesUsed();

            while (running)
            {
                Console.WriteLine("Huvudmeny\n");
                Console.WriteLine($"Antal vaccindoser: {vaccinResult}");
                Console.WriteLine($"Åldersgräns 18 år: {vaccinInputs.DisplayAgeLimit}");
                Console.WriteLine($"Indata:");
                Console.WriteLine($"Utdata: ");

                int option = ShowMenu("\nAlternativ", new[]
                {
                    "Lägg till person",
                    "Ändra antal vaccindoser",
                    "Ändra åldersgräns",
                    "Skapa prioritetsordning",
                    "Visa prioritetsordning",
                    "Ändra indata sökväg",
                    "Ändra utdata sökväg",
                    "Avsluta"
                });
                Console.Clear();

                Action Navigate = option switch
                {
                    0 => new Action(CreatePerson),
                    1 => new Action(VaccinAmount),
                    2 => new Action(AgeLimit),
                    3 => new Action(PriorityOrder),
                    4 => new Action(ShowPriorityOrder),
                    5 => new Action(IndataChange),
                    6 => new Action(OutdataChange),
                    7 => new Action(Exit),
                };
                Navigate.Invoke();
            }
        }

        // Create the lines that should be saved to a CSV file after creating the vaccination order.
        //
        // Parameters:
        //
        // input: the lines from a CSV file containing population information
        // doses: the number of vaccine doses available
        // vaccinateChildren: whether to vaccinate people younger than 18


        public static int ShowOption(string prompt)
        {
            List<string> option = new List<string>();
            option.Add("Nej");
            option.Add("Ja");

            return ShowMenu(prompt, option);
        }
        public static int ShowMenu(string prompt, IEnumerable<string> options)
        {
            if (options == null || options.Count() == 0)
            {
                throw new ArgumentException("Cannot show a menu for an empty list of options.");
            }

            Console.WriteLine(prompt);

            // Hide the cursor that will blink after calling ReadKey.
            Console.CursorVisible = false;

            // Calculate the width of the widest option so we can make them all the same width later.
            int width = options.Max(option => option.Length);

            int selected = 0;
            int top = Console.CursorTop;
            for (int i = 0; i < options.Count(); i++)
            {
                // Start by highlighting the first option.
                if (i == 0)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                var option = options.ElementAt(i);
                // Pad every option to make them the same width, so the highlight is equally wide everywhere.
                Console.WriteLine("- " + option.PadRight(width));

                Console.ResetColor();
            }
            Console.CursorLeft = 0;
            Console.CursorTop = top - 1;

            ConsoleKey? key = null;
            while (key != ConsoleKey.Enter)
            {
                key = Console.ReadKey(intercept: true).Key;

                // First restore the previously selected option so it's not highlighted anymore.
                Console.CursorTop = top + selected;
                string oldOption = options.ElementAt(selected);
                Console.Write("- " + oldOption.PadRight(width));
                Console.CursorLeft = 0;
                Console.ResetColor();

                // Then find the new selected option.
                if (key == ConsoleKey.DownArrow)
                {
                    selected = Math.Min(selected + 1, options.Count() - 1);
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    selected = Math.Max(selected - 1, 0);
                }

                // Finally highlight the new selected option.
                Console.CursorTop = top + selected;
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                string newOption = options.ElementAt(selected);
                Console.Write("- " + newOption.PadRight(width));
                Console.CursorLeft = 0;
                // Place the cursor one step above the new selected option so that we can scroll and also see the option above.
                Console.CursorTop = top + selected - 1;
                Console.ResetColor();
            }

            // Afterwards, place the cursor below the menu so we can see whatever comes next.
            Console.CursorTop = top + options.Count();

            // Show the cursor again and return the selected option.
            Console.CursorVisible = true;
            return selected;
        }
    }
}