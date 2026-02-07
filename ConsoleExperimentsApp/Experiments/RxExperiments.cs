using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleExperimentsApp.Experiments
{
    public static class RxExperiments
    {
        public static Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("RxExperiments");
            Console.ResetColor();

            ObservableTimer();
            RangeExample();
            MergMap();
            ConcatMapExample();
            SwitchMapExample();

            // 30 additional Rx.NET examples
            ObservableReturn();
            ObservableEmpty();
            ObservableThrow();
            WhereExample();
            TakeExample();
            SkipExample();
            DistinctExample();
            DistinctUntilChangedExample();
            SelectExample();
            BufferExample();
            WindowExample();
            ScanExample();
            MergeExample();
            ZipExample();
            CombineLatestExample();
            StartWithExample();
            DelayExample();
            ThrottleExample();
            DebounceExample();
            SampleExample();
            AggregateExample();
            CountExample();
            SumExample();
            CatchExample();
            RetryExample();
            DoExample();
            FinallyExample();
            DefaultIfEmptyExample();
            TakeWhileExample();
            SkipWhileExample();
            SubjectExample();
            BehaviorSubjectExample();
            ReplaySubjectExample();
            AsyncSubjectExample();
            PublishExample();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Press Enter to exit...");
            Console.ResetColor();
            return Task.CompletedTask;
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

        private static void MergMap()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("MergeMap Example:");
            Console.ResetColor();

            IObservable<string> sources = new[] { "A", "B", "C" }.ToObservable();

            IObservable<string> result = sources.SelectMany(source =>
                Observable.Range(1, 3)
                    .Select(i => $"{source}{i}")
                    .Delay(TimeSpan.FromMilliseconds(200 * new Random().Next(1, 4)))
            );

            result.Subscribe(x => 
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"MergeMap result: {x}");
                Console.ResetColor();
            });
        }

        private static void ConcatMapExample()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("ConcatMap Example (maintains order):");
            Console.ResetColor();

            IObservable<string> sources = new[] { "X", "Y", "Z" }.ToObservable();

            IObservable<string> result = sources
                .Select(source =>
                    Observable.Range(1, 3)
                        .Select(i => $"{source}{i}")
                        .Delay(TimeSpan.FromMilliseconds(300))
                )
                .Concat();

            result.Subscribe(x =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"ConcatMap result: {x}");
                Console.ResetColor();
            });
        }

        private static void SwitchMapExample()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("SwitchMap Example (switches to latest):");
            Console.ResetColor();

            IObservable<string> sources = Observable.Interval(TimeSpan.FromMilliseconds(800))
                .Take(3)
                .Select(i => $"Source{i + 1}");

            IObservable<string> result = sources.Select(source =>
                Observable.Interval(TimeSpan.FromMilliseconds(300))
                    .Take(5)
                    .Select(i => $"{source}-Item{i + 1}")
            ).Switch();

            result.Subscribe(x => 
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"SwitchMap result: {x}");
                Console.ResetColor();
            });
        }

        private static void ObservableTimer()
        {
            IObservable<long> ticks = Observable.Timer(
                dueTime: TimeSpan.Zero,
                period: TimeSpan.FromSeconds(1))
                .Take(10);

            ticks.Subscribe(x => 
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Tick: ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(x);
                Console.ResetColor();
            });
        }

        private static void ObservableReturn()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Observable.Return Example:");
            Console.ResetColor();

            Observable.Return("Single Value").Subscribe(x => 
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Return: {x}");
                Console.ResetColor();
            });
        }

        private static void ObservableEmpty()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("Observable.Empty Example:");
            Console.ResetColor();

            Observable.Empty<string>().Subscribe(
                onNext: x => Console.WriteLine($"Next: {x}"),
                onCompleted: () => 
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("Empty completed");
                    Console.ResetColor();
                });
        }

        private static void ObservableThrow()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Observable.Throw Example:");
            Console.ResetColor();

            Observable.Throw<string>(new InvalidOperationException("Test error")).Subscribe(
                onNext: x => Console.WriteLine($"Next: {x}"),
                onError: ex => 
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Error: {ex.Message}");
                    Console.ResetColor();
                });
        }

        private static void WhereExample()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Where (Filter) Example:");
            Console.ResetColor();

            Observable.Range(1, 10)
                .Where(x => x % 2 == 0)
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Even number: {x}");
                    Console.ResetColor();
                });
        }

        private static void TakeExample()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Take Example:");
            Console.ResetColor();

            Observable.Range(1, 100)
                .Take(5)
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Take: {x}");
                    Console.ResetColor();
                });
        }

        private static void SkipExample()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Skip Example:");
            Console.ResetColor();

            Observable.Range(1, 10)
                .Skip(5)
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Skip: {x}");
                    Console.ResetColor();
                });
        }

        private static void DistinctExample()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Distinct Example:");
            Console.ResetColor();

            new[] { 1, 2, 2, 3, 3, 3, 4 }.ToObservable()
                .Distinct()
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"Distinct: {x}");
                    Console.ResetColor();
                });
        }

        private static void DistinctUntilChangedExample()
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("DistinctUntilChanged Example:");
            Console.ResetColor();

            new[] { 1, 1, 2, 2, 2, 3, 1, 1 }.ToObservable()
                .DistinctUntilChanged()
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine($"DistinctUntilChanged: {x}");
                    Console.ResetColor();
                });
        }

        private static void SelectExample()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Select (Map) Example:");
            Console.ResetColor();

            Observable.Range(1, 5)
                .Select(x => x * x)
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"Squared: {x}");
                    Console.ResetColor();
                });
        }

        private static void BufferExample()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Buffer Example:");
            Console.ResetColor();

            Observable.Range(1, 10)
                .Buffer(3)
                .Subscribe(buffer => 
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($"Buffer: [{string.Join(", ", buffer)}]");
                    Console.ResetColor();
                });
        }

        private static void WindowExample()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Window Example:");
            Console.ResetColor();

            Observable.Range(1, 10)
                .Window(3)
                .Subscribe(window =>
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write("Window: ");
                    window.Subscribe(
                        x => Console.Write($"{x} "),
                        () => Console.WriteLine());
                    Console.ResetColor();
                });
        }

        private static void ScanExample()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Scan Example:");
            Console.ResetColor();

            Observable.Range(1, 5)
                .Scan((acc, x) => acc + x)
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine($"Running sum: {x}");
                    Console.ResetColor();
                });
        }

        private static void MergeExample()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Merge Example:");
            Console.ResetColor();

            var obs1 = Observable.Range(1, 3).Select(x => $"A{x}");
            var obs2 = Observable.Range(1, 3).Select(x => $"B{x}");

            obs1.Merge(obs2)
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Merged: {x}");
                    Console.ResetColor();
                });
        }

        private static void ZipExample()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Zip Example:");
            Console.ResetColor();

            var numbers = Observable.Range(1, 5);
            var letters = new[] { "A", "B", "C", "D", "E" }.ToObservable();

            numbers.Zip(letters, (n, l) => $"{l}{n}")
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Zipped: {x}");
                    Console.ResetColor();
                });
        }

        private static void CombineLatestExample()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("CombineLatest Example:");
            Console.ResetColor();

            var numbers = Observable.Interval(TimeSpan.FromMilliseconds(500)).Take(3);
            var letters = Observable.Interval(TimeSpan.FromMilliseconds(700)).Select(x => (char)('A' + x)).Take(3);

            numbers.CombineLatest(letters, (n, l) => $"{l}{n}")
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"CombineLatest: {x}");
                    Console.ResetColor();
                });
        }

        private static void StartWithExample()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("StartWith Example:");
            Console.ResetColor();

            Observable.Range(5, 3)
                .StartWith(1, 2, 3, 4)
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"StartWith: {x}");
                    Console.ResetColor();
                });
        }

        private static void DelayExample()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Delay Example:");
            Console.ResetColor();

            Observable.Range(1, 3)
                .Delay(TimeSpan.FromMilliseconds(500))
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Delayed: {x} at {DateTime.Now:HH:mm:ss.fff}");
                    Console.ResetColor();
                });
        }

        private static void ThrottleExample()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Throttle Example:");
            Console.ResetColor();

            Observable.Interval(TimeSpan.FromMilliseconds(100))
                .Take(20)
                .Throttle(TimeSpan.FromMilliseconds(300))
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"Throttled: {x}");
                    Console.ResetColor();
                });
        }

        private static void DebounceExample()
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Debounce (Throttle) Example:");
            Console.ResetColor();

            var subject = new Subject<string>();
            subject.AsObservable().Throttle(TimeSpan.FromMilliseconds(300))
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine($"Debounced: {x}");
                    Console.ResetColor();
                });

            Task.Run(async () =>
            {
                subject.OnNext("A");
                await Task.Delay(100);
                subject.OnNext("B");
                await Task.Delay(100);
                subject.OnNext("C");
                await Task.Delay(400);
                subject.OnNext("D");
                subject.OnCompleted();
            });
        }

        private static void SampleExample()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Sample Example:");
            Console.ResetColor();

            Observable.Interval(TimeSpan.FromMilliseconds(100))
                .Take(50)
                .Sample(TimeSpan.FromMilliseconds(300))
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine($"Sampled: {x}");
                    Console.ResetColor();
                });
        }

        private static void AggregateExample()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Aggregate Example:");
            Console.ResetColor();

            Observable.Range(1, 5)
                .Aggregate((acc, x) => acc * x)
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine($"Factorial of 5: {x}");
                    Console.ResetColor();
                });
        }

        private static void CountExample()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Count Example:");
            Console.ResetColor();

            Observable.Range(1, 10)
                .Where(x => x % 2 == 0)
                .Count()
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine($"Even numbers count: {x}");
                    Console.ResetColor();
                });
        }

        private static void SumExample()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Sum Example:");
            Console.ResetColor();

            Observable.Range(1, 10)
                .Sum()
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine($"Sum of 1-10: {x}");
                    Console.ResetColor();
                });
        }

        private static void CatchExample()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Catch Example:");
            Console.ResetColor();

            Observable.Range(1, 5)
                .Select(x => x == 3 ? throw new Exception("Error at 3") : x)
                .Catch(Observable.Return(999))
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine($"Catch: {x}");
                    Console.ResetColor();
                });
        }

        private static void RetryExample()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Retry Example:");
            Console.ResetColor();

            int attempt = 0;
            Observable.Defer(() =>
            {
                attempt++;
                return attempt < 3 ? 
                    Observable.Throw<int>(new Exception($"Attempt {attempt} failed")) :
                    Observable.Return(attempt);
            })
            .Retry(3)
            .Subscribe(
                x => 
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Retry succeeded on attempt: {x}");
                    Console.ResetColor();
                },
                ex => 
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Retry failed: {ex.Message}");
                    Console.ResetColor();
                });
        }

        private static void DoExample()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Do (Side Effect) Example:");
            Console.ResetColor();

            Observable.Range(1, 3)
                .Do(x => 
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Side effect: Processing {x}");
                    Console.ResetColor();
                })
                .Select(x => x * 2)
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Final result: {x}");
                    Console.ResetColor();
                });
        }

        private static void FinallyExample()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Finally Example:");
            Console.ResetColor();

            Observable.Range(1, 3)
                .Finally(() => 
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Finally: Cleanup executed");
                    Console.ResetColor();
                })
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Finally: {x}");
                    Console.ResetColor();
                });
        }

        private static void DefaultIfEmptyExample()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("DefaultIfEmpty Example:");
            Console.ResetColor();

            Observable.Empty<int>()
                .DefaultIfEmpty(42)
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"DefaultIfEmpty: {x}");
                    Console.ResetColor();
                });
        }

        private static void TakeWhileExample()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("TakeWhile Example:");
            Console.ResetColor();

            Observable.Range(1, 10)
                .TakeWhile(x => x < 5)
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"TakeWhile: {x}");
                    Console.ResetColor();
                });
        }

        private static void SkipWhileExample()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("SkipWhile Example:");
            Console.ResetColor();

            Observable.Range(1, 10)
                .SkipWhile(x => x < 5)
                .Subscribe(x => 
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"SkipWhile: {x}");
                    Console.ResetColor();
                });
        }

        private static void SubjectExample()
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("Subject Example:");
            Console.ResetColor();

            var subject = new Subject<string>();

            subject.Subscribe(x => 
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine($"Observer 1: {x}");
                Console.ResetColor();
            });

            subject.Subscribe(x => 
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine($"Observer 2: {x}");
                Console.ResetColor();
            });

            subject.OnNext("Hello");
            subject.OnNext("World");
            subject.OnCompleted();
        }

        private static void BehaviorSubjectExample()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("BehaviorSubject Example:");
            Console.ResetColor();

            var subject = new BehaviorSubject<string>("Initial");

            subject.Subscribe(x => 
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"BehaviorSubject Observer 1: {x}");
                Console.ResetColor();
            });

            subject.OnNext("Second");

            subject.Subscribe(x => 
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"BehaviorSubject Observer 2: {x}");
                Console.ResetColor();
            });

            subject.OnNext("Third");
        }

        private static void ReplaySubjectExample()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("ReplaySubject Example:");
            Console.ResetColor();

            var subject = new ReplaySubject<string>();

            subject.OnNext("First");
            subject.OnNext("Second");

            subject.Subscribe(x => 
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($"ReplaySubject Observer: {x}");
                Console.ResetColor();
            });

            subject.OnNext("Third");
        }

        private static void AsyncSubjectExample()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("AsyncSubject Example:");
            Console.ResetColor();

            var subject = new AsyncSubject<string>();

            subject.Subscribe(x => 
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"AsyncSubject Observer: {x}");
                Console.ResetColor();
            });

            subject.OnNext("First");
            subject.OnNext("Second");
            subject.OnNext("Last");
            subject.OnCompleted(); // Only "Last" will be emitted
        }

        private static void PublishExample()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("Publish (Hot Observable) Example:");
            Console.ResetColor();

            var published = Observable.Range(1, 5)
                .Do(x => 
                {
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine($"Source emitting: {x}");
                    Console.ResetColor();
                })
                .Publish();

            published.Subscribe(x => 
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine($"Observer 1: {x}");
                Console.ResetColor();
            });

            published.Subscribe(x => 
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine($"Observer 2: {x}");
                Console.ResetColor();
            });

            published.Connect(); // Start emitting
        }
    }
}
