// See https://aka.ms/new-console-template for more information

using ConsoleExperimentsApp.Experiments;

Console.ForegroundColor = ConsoleColor.White;
Console.WriteLine("######## .Net C# Experiments ##########");
Console.WriteLine();
Console.ResetColor();

bool running = true;

while (running)
{
    DisplayMenu();
    string? choice = Console.ReadLine();
    Console.WriteLine();

    switch (choice)
    {
        case "1":
            RxExperiments.Run();
            break;
        case "2":
            SerilogExperiments.Run();
            break;
        case "3":
            await EntityFrameworkExperiments.Run();
            break;
        case "4":
            TPLExperiments.Run();
            break;
        case "5":
            AspectCodingExperiments.Run();
            break;
        case "6":
            AutoMapperExperiments.Run();
            break;
        case "7":
            BackgroundWorkExperiments.Run();
            break;
        case "8":
            ChannelsExperiments.Run();
            break;
        case "9":
            CQRSExperiments.Run();
            break;
        case "10":
            DesignPatternsExperiments.Run();
            break;
        case "11":
            EnumerableCollectionsExperiments.Run();
            break;
        case "12":
            ExtensionMethodExperiments.Run();
            break;
        case "13":
            GarbageCollectionExperiments.Run();
            break;
        case "14":
            GenericsExperiments.Run();
            break;
        case "15":
            LinqExperiments.Run();
            break;
        case "16":
            MemorySpanExperiments.Run();
            break;
        case "17":
            MLNetExperiments.Run();
            break;
        case "18":
            ReflectionExperiments.Run();
            break;
        case "0":
        case "exit":
        case "quit":
            running = false;
            Console.WriteLine("Exiting...");
            break;
        default:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid choice. Please try again.");
            Console.ResetColor();
            break;
    }

    if (running)
    {
        Console.WriteLine();
        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
        Console.Clear();
    }
}

static void DisplayMenu()
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("═══════════════════════════════════════");
    Console.WriteLine("    Select an Experiment to Run");
    Console.WriteLine("═══════════════════════════════════════");
    Console.ResetColor();
    Console.WriteLine(" 1.  Rx Experiments");
    Console.WriteLine(" 2.  Serilog Experiments");
    Console.WriteLine(" 3.  Entity Framework Experiments");
    Console.WriteLine(" 4.  TPL Experiments");
    Console.WriteLine(" 5.  Aspect Coding Experiments");
    Console.WriteLine(" 6.  AutoMapper Experiments");
    Console.WriteLine(" 7.  Background Work Experiments");
    Console.WriteLine(" 8.  Channels Experiments");
    Console.WriteLine(" 9.  CQRS Experiments");
    Console.WriteLine(" 10. Design Patterns Experiments");
    Console.WriteLine(" 11. Enumerable Collections Experiments");
    Console.WriteLine(" 12. Extension Method Experiments");
    Console.WriteLine(" 13. Garbage Collection Experiments");
    Console.WriteLine(" 14. Generics Experiments");
    Console.WriteLine(" 15. LINQ Experiments");
    Console.WriteLine(" 16. Memory/Span Experiments");
    Console.WriteLine(" 17. ML.NET Experiments");
    Console.WriteLine(" 18. Reflection Experiments");
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine(" 0.  Exit");
    Console.ResetColor();
    Console.WriteLine("═══════════════════════════════════════");
    Console.Write("Enter your choice: ");
}
