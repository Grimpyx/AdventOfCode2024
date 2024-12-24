using AdventOfCode2024.Days;
using AdventOfCode2024.Utilities;
using System.Diagnostics;

Dictionary<ushort, IDayChallenge> _days = new Dictionary<ushort, IDayChallenge>
{
    { 1, new Day1() },
    { 2, new Day2() },
    { 3, new Day3() },
    { 4, new Day4() },
    { 5, new Day5() },
    { 6, new Day6() },
    { 7, new Day7() },
    { 8, new Day8() },
    { 9, new Day9() },
    { 10, new Day10() },
    { 11, new Day11() },
    { 12, new Day12() },
    { 13, new Day13() },
    { 14, new Day14() },
    { 15, new Day15() },
    { 16, new Day16() },
    { 17, new Day17() },
    { 18, new Day18() },
    { 19, new Day19() },
    //{ 20, new Day20() },
    //{ 21, new Day21() },
    //{ 22, new Day22() },
    //{ 23, new Day23() },
    //{ 24, new Day24() },
    //{ 25, new Day25() }
};

// Command loop
while (true)
{
    TextUtilities.CFW("@WhiEnter command: (type 'h' for help)\n > @Yel");
    string? input = Console.ReadLine();
    Console.ResetColor();
    if (input != null) InterpretCommand(input);
}


void InterpretCommand(string command)
{
    command = command.ToLower();

    if (command == "help" || command == "h")
    {
        TextUtilities.CFWLine(
            "  .. To start a day, write 'day[dataType][challenge]'. For example, 'day14f2'.\n" +
            "  .. dataType needs to be either 'f' for the full dataset, or 's' for the simple dataset.\n" +
            "  .. challenge needs to be either '1' for the first star challenge or '2' for the second star challenge.\n");

        return;
    }

    if (command == "quit" || command == "exit" || command == "q")
    {
        Environment.Exit(0);
    }

    if (command == "clear")
    {
        Console.Clear();
        return;
    }

    if (command.Length >= 3 && command[..3] == "day")
    {
        Console.WriteLine();
        if (!char.IsNumber(command[^1]) || !char.IsNumber(command[^3]) || !char.IsLetter(command[^2]))
        {
            TextUtilities.CFWLine("@RedFailed to interpret command. Command might not be complete.");
            return;
        }

        if(!ushort.TryParse(new string(command[..^2].Where(char.IsDigit).ToArray()), out ushort dayNumber) || !_days.ContainsKey(dayNumber))
        {
            // If invalid number
            TextUtilities.CFWLine("@RedFailed to interpret command. Not a valid day.");
            return;
        }

        IDayChallenge day = _days[dayNumber];
        char star = command[^1];
        if (star != '1' && star != '2')
        {
            TextUtilities.CFWLine($"@RedFailed to interpret command. '{star}' is not a valid part of the challenge. Use '1' or '2'.");
            return;
        }

        char fullOrSimple = command[^2];
        if (fullOrSimple != 's' && fullOrSimple != 'f')
        {
            TextUtilities.CFWLine($"@RedFailed to interpret command. '{fullOrSimple}' is not a valid dataset. Use 's' for simple data, or 'f' for the full data.");
            return;
        }

        // Run day command
        Stopwatch stopwatch = Stopwatch.StartNew();
        if (star == '1' && fullOrSimple == 'f') day.RunFirstStar(DayDataType.Full);
        else if (star == '2' && fullOrSimple == 'f') day.RunSecondStar(DayDataType.Full);
        else if (star == '1' && fullOrSimple == 's') day.RunFirstStar(DayDataType.Simple);
        else if (star == '2' && fullOrSimple == 's') day.RunSecondStar(DayDataType.Simple);
        else
        {
            TextUtilities.CFWLine($"@RedFailed to interpret command. Not correctly formatted.");
            return;
        }
        TextUtilities.CFWLine($"\n@DGyCommand took @DYe{stopwatch.ElapsedMilliseconds}@DGyms.\n");
        return;
    }

    TextUtilities.ColorWriteLine(ConsoleColor.Red, "Invalid command.");
}