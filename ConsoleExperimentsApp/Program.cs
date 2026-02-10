// See https://aka.ms/new-console-template for more information

using ConsoleExperimentsApp.Experiments.AspectCoding;
using ConsoleExperimentsApp.Experiments.AutoMapper;
using ConsoleExperimentsApp.Experiments.Azure;
using ConsoleExperimentsApp.Experiments.BackgroundWork;
using ConsoleExperimentsApp.Experiments.Channels;
using ConsoleExperimentsApp.Experiments.CQRS;
using ConsoleExperimentsApp.Experiments.CSharpVersions;
using ConsoleExperimentsApp.Experiments.DataStructuresAlgorithms;
using ConsoleExperimentsApp.Experiments.DesignPatterns;
using ConsoleExperimentsApp.Experiments.EntityFramework;
using ConsoleExperimentsApp.Experiments.EnumerableCollections;
using ConsoleExperimentsApp.Experiments.ExtensionMethod;
using ConsoleExperimentsApp.Experiments.GarbageCollection;
using ConsoleExperimentsApp.Experiments.Generics;
using ConsoleExperimentsApp.Experiments.Linq;
using ConsoleExperimentsApp.Experiments.MediatR;
using ConsoleExperimentsApp.Experiments.MemorySpan;
using ConsoleExperimentsApp.Experiments.MLNet;
using ConsoleExperimentsApp.Experiments.NServiceBus;
using ConsoleExperimentsApp.Experiments.Polly;
using ConsoleExperimentsApp.Experiments.RabbitMQ;
using ConsoleExperimentsApp.Experiments.Reflection;
using ConsoleExperimentsApp.Experiments.Rx;
using ConsoleExperimentsApp.Experiments.Serilog;
using ConsoleExperimentsApp.Experiments.TPL;

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
            await RxExperiments.Run();
            break;
        case "2":
            await SerilogExperiments.Run();
            break;
        case "3":
            await EntityFrameworkExperiments.Run();
            break;
        case "4":
            await TPLExperiments.Run();
            break;
        case "5":
            await AspectCodingExperiments.Run();
            break;
        case "6":
            await AutoMapperExperiments.Run();
            break;
        case "7":
            await BackgroundWorkExperiments.Run();
            break;
        case "8":
            await ChannelsExperiments.Run();
            break;
        case "9":
            await CQRSExperiments.Run();
            break;
        case "10":
            await DesignPatternsExperiments.Run();
            break;
        case "11":
            await EnumerableCollectionsExperiments.Run();
            break;
        case "12":
            await ExtensionMethodExperiments.Run();
            break;
        case "13":
            await GarbageCollectionExperiments.Run();
            break;
        case "14":
            await GenericsExperiments.Run();
            break;
        case "15":
            await LinqExperiments.Run();
            break;
        case "16":
            await MemorySpanExperiments.Run();
            break;
        case "17":
            await MLNetExperiments.Run();
            break;
        case "18":
            await ReflectionExperiments.Run();
            break;
        case "19":
            await NewInCSharp12_13_14.Run();
            break;
        case "20":
            await MediatRExperiments.Run();
            break;
        case "21":
            await MLNetExperiments.Run();
            break;
        case "22":
            await PollyExperiments.Run();
            break;
        case "23":
            await CQRSExperiments.Run();
            break;
        case "24":
            await DataStructuresAlgorithmsExperiments.Run();
            break;
        case "25":
            await RabbitMQExperiments.Run();
            break;
        case "26":
            await NServiceBusExperiments.Run();
            break;
        case "27":
            await AzureServiceBusExperiments.Run();
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
    Console.WriteLine(" 19. NewInCSharp12_13_14 Experiments");
    Console.WriteLine(" 20. MediatR Experiments");
    Console.WriteLine(" 21. ML.NET Experiments");
    Console.WriteLine(" 22. Polly Experiments");
    Console.WriteLine(" 23. CQRS Experiments");
    Console.WriteLine(" 24. Data Structures & Algorithms Experiments");
    Console.WriteLine(" 25. RabbitMQ Experiments");
    Console.WriteLine(" 26. NServiceBus Experiments");
    Console.WriteLine(" 27. Azure ServiceBus Experiments");
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine(" 0.  Exit");
    Console.ResetColor();
    Console.WriteLine("═══════════════════════════════════════");
    Console.Write("Enter your choice: ");
}
