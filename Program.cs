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

            MainMenu();
        }
        
        public static void VaccinAmount()
        {
            VaccinInputs vaccinInputs = new VaccinInputs();

            Console.Write("Ändra antalet vaccindoser: ");
            int vaccinAmount = int.Parse(Console.ReadLine());

            Console.Clear();

            vaccinInputs.ChangeDoseAmount(vaccinAmount);
            vaccinInputs.AgeLimitReturn();
        }
        public static void AgeLimit()
        {
            VaccinInputs vaccinInputs = new VaccinInputs();

            int ageLimit = ShowOption("Ta bort åldergräns?");

            Console.Clear();

            vaccinInputs.ChangeAgeLimit(ageLimit);
            vaccinInputs.AgeLimitReturn();
        }
        public static void PriorityOrder()
        {
            VaccinInputs vaccinInputs = new VaccinInputs();
            FilterPerson personFilter = new FilterPerson();

            personFilter.CreateVaccinationOrder(vaccinInputs.ReadCSVInputFile(), vaccinInputs.DosesAmount(), vaccinInputs.AgeLimitReturn());
        }
        public static void IndataChange()
        {
            VaccinInputs vaccinInput = new VaccinInputs();

            while (true)
            {
                Console.Write("Välj ny sökväg för Indata: ");
                string changeFileInputPath = Console.ReadLine();

                vaccinInput.IndataChange(changeFileInputPath);

                if (File.Exists(vaccinInput.HandleFile()))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Filen exiterar inte, testa igen!");
                }
            }
        }
        public static void OutdataChange()
        {
            VaccinInputs vaccinInInput = new VaccinInputs();
            FilterPerson vaccinFilters = new FilterPerson();
            while (true)
            {
                Console.Write("Välj ny sökväg för Utdata: ");
                string changeFileOutputPath = Console.ReadLine();

                vaccinInInput.OutdataChange(changeFileOutputPath);
                if(Directory.Exists(vaccinFilters.HandleOutFile()))
                {
                    Console.WriteLine($"Ny sökväg: {changeFileOutputPath}");
                    if (!File.Exists(vaccinFilters.HandleOutFile()))
                    {
                        using (File.Create(vaccinFilters.HandleOutFile())) { }
                    }
                    break;
                }
                else
                {
                    Console.WriteLine("Mappen existerar inte, testa igen!");
                }
            }
        }
        public static void Exit()
        {
            running = false;
        }

        public static void MainMenu()
        {
            while (running)
            {
                VaccinInputs vaccinInputs = new VaccinInputs();

                Console.WriteLine("Huvudmeny\n");
                Console.WriteLine($"Antal vaccindoser: {vaccinInputs.DosesAmount()}");
                Console.WriteLine($"Åldersgräns 18 år: {vaccinInputs.DisplayAgeLimit()}");
                Console.WriteLine($"Indata: ");
                Console.WriteLine($"Utdata: ");

                int option = ShowMenu("\nAlternativ", new[]
                {
                    "Ändra antal vaccindoser",
                    "Ändra åldersgräns",
                    "Skapa prioritetsordning",
                    "Ändra indata sökväg",
                    "Ändra utdata sökväg",
                    "Avsluta"
                });
                Console.Clear();

                Action Navigate = option switch
                {
                    0 => new Action(VaccinAmount),
                    1 => new Action(AgeLimit),
                    2 => new Action(PriorityOrder),
                    3 => new Action(IndataChange),
                    4 => new Action(OutdataChange),
                    5 => new Action(Exit),
                };
                Navigate.Invoke();
            }
        }
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