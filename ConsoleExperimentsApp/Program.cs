// See https://aka.ms/new-console-template for more information

using ConsoleExperimentsApp.Experiments;

Console.WriteLine(".Net C# Experiments");

//RxExperiments.Run();    

//SerilogExperiments.Run();

await EntityFrameworkExperiments.Run();

Console.ReadLine();
