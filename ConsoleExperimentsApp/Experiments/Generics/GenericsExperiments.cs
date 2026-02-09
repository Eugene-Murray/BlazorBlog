using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleExperimentsApp.Experiments.Generics
{
    public class GenericsExperiments
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== Generics Experiments ===");
            Console.WriteLine("Description: Demonstrating type-safe generic programming with classes, methods, constraints, and variance.");
            Console.ResetColor();

            Console.WriteLine("\n--- 1. Generic Classes ---");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Demonstrates creating type-safe generic classes that can work");
            Console.WriteLine("   with different types while maintaining compile-time type checking.");
            Console.ResetColor();
            GenericClassExperiment();

            Console.WriteLine("\n--- 2. Generic Methods ---");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Shows how to create methods that work with multiple types using");
            Console.WriteLine("   type parameters, allowing reusable code with type safety.");
            Console.ResetColor();
            GenericMethodExperiment();

            Console.WriteLine("\n--- 3. Generic Constraints ---");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Demonstrates using constraints (where clauses) to restrict type parameters");
            Console.WriteLine("   to specific base classes, interfaces, or requirements like new(), struct, or class.");
            Console.ResetColor();
            GenericConstraintsExperiment();

            Console.WriteLine("\n--- 4. Covariance and Contravariance ---");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Explains covariance (out) for returning derived types and contravariance");
            Console.WriteLine("   (in) for accepting base types, enabling flexible type conversions in generic interfaces.");
            Console.ResetColor();
            CovarianceContravarianceExperiment();

            Console.WriteLine("\n--- 5. Multiple Type Parameters ---");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Shows how to use multiple type parameters in a single generic class");
            Console.WriteLine("   or method, enabling complex type relationships and operations.");
            Console.ResetColor();
            MultipleTypeParametersExperiment();

            Console.WriteLine("\n--- 6. Generic Collections ---");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("   Description: Demonstrates built-in generic collection types like List<T>, Dictionary<TKey, TValue>,");
            Console.WriteLine("   and Queue<T> that provide type safety and better performance than non-generic versions.");
            Console.ResetColor();
            GenericCollectionsExperiment();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nPress Enter to exit...");
            Console.ResetColor();
        }

        private static void GenericClassExperiment()
        {
            var intRepo = new Repository<int>();
            intRepo.Add(1);
            intRepo.Add(2);
            intRepo.Add(3);
            Console.WriteLine($"Integer Repository Count: {intRepo.Count}");
            Console.WriteLine($"First Item: {intRepo.Get(0)}");

            var stringRepo = new Repository<string>();
            stringRepo.Add("Apple");
            stringRepo.Add("Banana");
            stringRepo.Add("Cherry");
            Console.WriteLine($"String Repository Count: {stringRepo.Count}");
            Console.WriteLine($"All Items: {string.Join(", ", stringRepo.GetAll())}");
        }

        private static void GenericMethodExperiment()
        {
            int[] numbers = { 1, 2, 3, 4, 5 };
            string[] words = { "Hello", "World", "Generics" };

            Console.WriteLine("Swap integers: ");
            int a = 10, b = 20;
            Console.WriteLine($"Before: a={a}, b={b}");
            Swap(ref a, ref b);
            Console.WriteLine($"After: a={a}, b={b}");

            Console.WriteLine("\nPrint array:");
            PrintArray(numbers);
            PrintArray(words);

            Console.WriteLine($"\nMax value: {GetMax(15, 25)}");
            Console.WriteLine($"Max string: {GetMax("apple", "zebra")}");
        }

        private static void GenericConstraintsExperiment()
        {
            var personProcessor = new EntityProcessor<PersonGeneric>();
            var person = new PersonGeneric { Id = 1, Name = "John Doe" };
            personProcessor.Process(person);

            var calculator = new Calculator<int>();
            Console.WriteLine($"Sum: {calculator.Add(5, 3)}");

            var listFactory = new Factory<List<string>>();
            var list = listFactory.Create();
            Console.WriteLine($"Created instance: {list.GetType().Name}");
        }

        private static void CovarianceContravarianceExperiment()
        {
            Console.WriteLine("Covariance (out):");
            IProducer<DogGeneric> dogProducer = new AnimalGenericProducer();
            DogGeneric dog = dogProducer.Produce();
            Console.WriteLine($"Produced: {dog.GetType().Name} - {dog.MakeSound()}");

            IProducer<AnimalGeneric> animalProducer = dogProducer;
            AnimalGeneric animal = animalProducer.Produce();
            Console.WriteLine($"Produced as Animal: {animal.GetType().Name} - {animal.MakeSound()}");

            Console.WriteLine("\nContravariance (in):");
            IConsumer<AnimalGeneric> animalConsumer = new AnimalGenericConsumer();
            IConsumer<DogGeneric> dogConsumer = animalConsumer;
            dogConsumer.Consume(new DogGeneric());
        }

        private static void MultipleTypeParametersExperiment()
        {
            var pair1 = new Pair<int, string>(1, "One");
            Console.WriteLine($"Pair 1: Key={pair1.Key}, Value={pair1.Value}");

            var pair2 = new Pair<string, DateTime>("Today", DateTime.Now);
            Console.WriteLine($"Pair 2: Key={pair2.Key}, Value={pair2.Value:yyyy-MM-dd HH:mm:ss}");

            var result = new Result<string, int>("Success", 200);
            Console.WriteLine($"Result: Status={result.Status}, Code={result.Code}");
        }

        private static void GenericCollectionsExperiment()
        {
            var stack = new Stack<int>();
            stack.Push(1);
            stack.Push(2);
            stack.Push(3);
            Console.WriteLine($"Stack Pop: {stack.Pop()}");

            var dict = new Dictionary<string, int>
            {
                ["One"] = 1,
                ["Two"] = 2,
                ["Three"] = 3
            };
            Console.WriteLine($"Dictionary lookup 'Two': {dict["Two"]}");

            var queue = new Queue<string>();
            queue.Enqueue("First");
            queue.Enqueue("Second");
            queue.Enqueue("Third");
            Console.WriteLine($"Queue Dequeue: {queue.Dequeue()}");
        }

        private static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        private static void PrintArray<T>(T[] array)
        {
            Console.WriteLine($"[{string.Join(", ", array)}]");
        }

        private static T GetMax<T>(T a, T b) where T : IComparable<T>
        {
            return a.CompareTo(b) > 0 ? a : b;
        }
    }

    public class Repository<T>
    {
        private readonly List<T> _items = new();

        public void Add(T item) => _items.Add(item);
        public T Get(int index) => _items[index];
        public IEnumerable<T> GetAll() => _items;
        public int Count => _items.Count;
    }

    public class EntityProcessor<T> where T : IEntity
    {
        public void Process(T entity)
        {
            Console.WriteLine($"Processing entity with ID: {entity.Id}");
        }
    }

    public class Calculator<T> where T : struct
    {
        public T Add(T a, T b)
        {
            dynamic x = a;
            dynamic y = b;
            return x + y;
        }
    }

    public class Factory<T> where T : new()
    {
        public T Create() => new T();
    }

    public interface IEntity
    {
        int Id { get; set; }
    }

    public class PersonGeneric : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public interface IProducer<out T>
    {
        T Produce();
    }

    public interface IConsumer<in T>
    {
        void Consume(T item);
    }

    public class AnimalGeneric
    {
        public virtual string MakeSound() => "Some sound";
    }

    public class DogGeneric : AnimalGeneric
    {
        public override string MakeSound() => "Woof!";
    }

    public class AnimalGenericProducer : IProducer<DogGeneric>
    {
        public DogGeneric Produce() => new DogGeneric();
    }

    public class AnimalGenericConsumer : IConsumer<AnimalGeneric>
    {
        public void Consume(AnimalGeneric item)
        {
            Console.WriteLine($"Consuming: {item.GetType().Name} - {item.MakeSound()}");
        }
    }

    public class Pair<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public Pair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    public class Result<TStatus, TCode>
    {
        public TStatus Status { get; set; }
        public TCode Code { get; set; }

        public Result(TStatus status, TCode code)
        {
            Status = status;
            Code = code;
        }
    }
}
