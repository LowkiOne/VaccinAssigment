﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using VaccinAssigment;

public class Vaccin
{
    private static bool running = true;
    private static int vaccinAmount = 0;
    private static bool ageLimit = false;
    private static string ageDisplay = "Nej";

    private static string inputCSVPath = @"D:\2023\Progamering\C#\Inlamning4\NyaFiler\Test.csv";
    private static string outdataCSVPath = @"D:\2023\Progamering\C#\Inlamning4\NyaFiler\Vaccinations.csv";
    public static void Main()
    {
        while (running)
        {

            Console.WriteLine("Huvudmeny\n");
            Console.WriteLine($"Antal vaccindoser: {vaccinAmount}");
            Console.WriteLine($"Åldersgräns 18 år: {ageDisplay}");
            Console.WriteLine($"Indata: {inputCSVPath}");
            Console.WriteLine($"Utdata: {outdataCSVPath}");

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
                0 => new Action(AddVaccinAmount),
                1 => new Action(ChangeAgeLimit),
                2 => new Action(CreatePriorityOrder),
                3 => new Action(IndataFileSearchChange),
                4 => new Action(OutDataFileSearchChange),
                5 => new Action(Exit)
            };
            Navigate.Invoke();
        }
    }

    public static void AddVaccinAmount()
    {
        while (true)
        {
            Console.Write("Ändra antalet vaccindoser: ");
            string inputAmount = Console.ReadLine();

            if (inputAmount != null && int.TryParse(inputAmount, out int result))
            {
                vaccinAmount += result;
                break;
            }
            else
            {
                Console.WriteLine("Skriv en hel siffra");
            }
        }
    }

    public static void ChangeAgeLimit()
    {
        int option = ShowMenu("Ändra Åldersgräns", new[]
        {
            "Sätt åldersgräns",
            "Ingen åldersgräns"
        });
        if (option == 1)
        {
            ageLimit = true;
            ageDisplay = "Ja";
            Console.WriteLine("Ändrad till vaccinera personer under 18 år");
        }
        else
        {
            ageLimit = false;
            ageDisplay = "Nej";
            Console.WriteLine("Ändrad till vaccinera inte personer under 18 år");
        }
    }

    public static void CreatePriorityOrder()
    {
        Functions functions = new Functions();
        string[] input = File.ReadAllLines(inputCSVPath);

        try
        {
            ErrorHandle(input);

            string[] outPut = functions.CreateVaccinationOrder(input, vaccinAmount, ageLimit);
             
            if(vaccinAmount >= functions.DosesUsed())
            {
                vaccinAmount -= functions.DosesUsed();
            }
            else
            {
                vaccinAmount -= vaccinAmount;
                Console.WriteLine("");
            }

            if (File.Exists(outdataCSVPath))
            {
                int option = ShowMenu("Vill du skriva över filen?", new[]
                {
                    "Ja",
                    "Nej"
                });
                if (option == 0)
                {
                    File.WriteAllLines(outdataCSVPath, outPut);
                    Console.WriteLine("Ordern är skapd");
                }
                else
                {
                    return;
                }
            }
            else
            {
                File.Create(outdataCSVPath);
                File.WriteAllLines(outdataCSVPath, outPut);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static void IndataFileSearchChange()
    {
        while (true)
        {
            Console.Write("Välj ny sökväg för Indata: ");
            string changeFileInputPath = Console.ReadLine();

            inputCSVPath = changeFileInputPath;

            if (File.Exists(inputCSVPath))
            {
                break;
            }
            else
            {
                Console.WriteLine("Filen exiterar inte, testa igen!");
            }
        }
    }

    public static void OutDataFileSearchChange()
    {
        while (true)
        {
            Console.Write("Välj ny sökväg för Utdata: ");
            string changeFileOutputPath = Console.ReadLine();

            outdataCSVPath = changeFileOutputPath;

            if (!Directory.Exists(Path.GetDirectoryName(outdataCSVPath)))
            {
                Console.WriteLine("Mappen existerar inte, testa igen!");
            }
            else
            {
                if (!File.Exists(outdataCSVPath))
                {
                    File.Create(outdataCSVPath);
                }
                Console.WriteLine($"Ny sökväg: {changeFileOutputPath}");

                break;
            }
        }
    }

    private static void Exit()
    {
        running = false;
    }

    public static bool IsAllDigits(string input)
    {
        int counter = 0;

        foreach (char c in input)
        {
            if (c == '-' && counter < 1)
            {
                counter++;
            }
            else if (!char.IsDigit(c))
            {
                return false;
            }
        }
        return true;
    }

    public static void ErrorHandle(string[] inputs)
    {
        List<string> errorMessages = new List<string>();

        foreach (string line in inputs)
        {
            string[] input = line.Split(',');

            if (input.Length != 6)
            {
                errorMessages.Add("För få värden som är separerade med ,");
            }

            string personalNumberCheck = @"^\d{8}-\d{4}$|^\d{6}-\d{4}$|^\d{12}$|^\d{10}$";

            if (!Regex.IsMatch(input[0], personalNumberCheck) || !IsAllDigits(input[0]) || input[0] == null)
            {
                errorMessages.Add($"Index 0.{input[0]} Felaktigt format på personummer. Accepterade format: YYYYMMDD-NNNN, YYMMDD-NNNN, YYYYMMDDNNNN, YYMMDDNNNN");
            }

            string names = "^[a-zA-ZåäöÅÄÖ]+$";

            if (!Regex.IsMatch(input[1], names) || input[1] == null)
            {
                errorMessages.Add("Index 1. Efternamnetamnet är felaktigt. Accepterade tecken: A-Ö, a-ö");
            }

            if (!Regex.IsMatch(input[2], names) || input[2] == null)
            {
                errorMessages.Add("Index 2. Förnamnet är felaktig. Accepterade tecken: A-Ö, a-ö");
            }

            string value = "^[01]$";

            if (!Regex.IsMatch(input[3], value) || input[3] == null)
            {
                errorMessages.Add("Index 3. Innehåller felaktigt värde. Accepterade värden: 0, 1");
            }

            if (!Regex.IsMatch(input[4], value) || input[4] == null)
            {
                errorMessages.Add("Index 4. Innehåller felaktigt värde. Accepterade värden: 0, 1");
            }

            if (!Regex.IsMatch(input[5], value) || input[5] == null)
            {
                errorMessages.Add("Index 5. Innehåller felaktigt värde. Accepterade värden: 0, 1");
            }
        }

        if (errorMessages.Count > 0)
        {
            throw new FormatException(string.Join(Environment.NewLine, errorMessages));
        }
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