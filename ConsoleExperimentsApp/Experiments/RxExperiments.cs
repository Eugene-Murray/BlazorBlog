using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;

namespace ConsoleExperimentsApp.Experiments
{
    public static class RxExperiments
    {
        public static void Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("RxExperiments");
            Console.ResetColor();

            ObservableTimer();
            RangeExample();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Press Enter to exit...");
            Console.ResetColor();
        }

        private static void RangeExample()
        {
            IObservable<int> numbers = Observable.Range(1, 5);
            IObservable<string> result = numbers.SelectMany(x =>
                Observable.Return($"Number: {x}").Delay(TimeSpan.FromMilliseconds(500 * x))
            );
            result.Subscribe(x => 
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(x);
                Console.ResetColor();
            });
        }

        private static void ObservableTimer()
        {
            IObservable<long> ticks = Observable.Timer(
                dueTime: TimeSpan.Zero,
                period: TimeSpan.FromSeconds(1));

            ticks.Subscribe(x => 
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Tick: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(x);
                Console.ResetColor();
            });
        }
    }
}
