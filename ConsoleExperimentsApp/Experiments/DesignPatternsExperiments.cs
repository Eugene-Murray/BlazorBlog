using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleExperimentsApp.Experiments
{
    public static class DesignPatternsExperiments
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== Gang of Four Design Patterns Examples ===");
            Console.ResetColor();

            await RunCreationalPatterns();
            await RunStructuralPatterns();
            await RunBehavioralPatterns();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Press Enter to exit...");
            Console.ResetColor();
        }

        private static async Task RunCreationalPatterns()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n=== CREATIONAL PATTERNS ===");
            Console.ResetColor();

            // 1. Singleton Pattern
            Console.WriteLine("\n1. Singleton Pattern:");
            var singleton1 = Singleton.Instance;
            var singleton2 = Singleton.Instance;
            Console.WriteLine($"Same instance: {ReferenceEquals(singleton1, singleton2)}");

            // 2. Factory Method Pattern
            Console.WriteLine("\n2. Factory Method Pattern:");
            var dogCreator = new DogCreator();
            var catCreator = new CatCreator();
            var dog = dogCreator.CreateAnimal();
            var cat = catCreator.CreateAnimal();
            dog.MakeSound();
            cat.MakeSound();

            // 3. Abstract Factory Pattern
            Console.WriteLine("\n3. Abstract Factory Pattern:");
            var modernFactory = new ModernFurnitureFactory();
            var chair = modernFactory.CreateChair();
            var table = modernFactory.CreateTable();
            chair.SitOn();
            table.PlaceItem();

            // 4. Builder Pattern
            Console.WriteLine("\n4. Builder Pattern:");
            var house = new HouseBuilder()
                .SetFoundation("Concrete")
                .SetWalls("Brick")
                .SetRoof("Tile")
                .Build();
            Console.WriteLine($"Built house: {house}");

            // 5. Prototype Pattern
            Console.WriteLine("\n5. Prototype Pattern:");
            var originalDocument = new Document("Original", "Content here");
            var clonedDocument = originalDocument.Clone();
            Console.WriteLine($"Original: {originalDocument}");
            Console.WriteLine($"Clone: {clonedDocument}");
        }

        private static async Task RunStructuralPatterns()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n=== STRUCTURAL PATTERNS ===");
            Console.ResetColor();

            // 1. Adapter Pattern
            Console.WriteLine("\n1. Adapter Pattern:");
            var adaptee = new Adaptee();
            var adapter = new Adapter(adaptee);
            adapter.Request();

            // 2. Bridge Pattern
            Console.WriteLine("\n2. Bridge Pattern:");
            var redCircle = new Circle(new RedColor());
            var blueSquare = new Square(new BlueColor());
            redCircle.Draw();
            blueSquare.Draw();

            // 3. Composite Pattern
            Console.WriteLine("\n3. Composite Pattern:");
            var root = new CompositeComponent("Root");
            var branch1 = new CompositeComponent("Branch1");
            var leaf1 = new LeafComponent("Leaf1");
            var leaf2 = new LeafComponent("Leaf2");
            root.Add(branch1);
            branch1.Add(leaf1);
            branch1.Add(leaf2);
            root.Operation();

            // 4. Decorator Pattern
            Console.WriteLine("\n4. Decorator Pattern:");
            var coffee = new SimpleCoffee();
            var milkCoffee = new MilkDecorator(coffee);
            var sugarMilkCoffee = new SugarDecorator(milkCoffee);
            Console.WriteLine($"Coffee cost: ${sugarMilkCoffee.GetCost()}, Description: {sugarMilkCoffee.GetDescription()}");

            // 5. Facade Pattern
            Console.WriteLine("\n5. Facade Pattern:");
            var facade = new ComputerFacade();
            facade.Start();

            // 6. Flyweight Pattern
            Console.WriteLine("\n6. Flyweight Pattern:");
            var factory = new TreeTypeFactory();
            var tree1 = factory.GetTreeType("Oak", "Green", "OakTexture");
            var tree2 = factory.GetTreeType("Pine", "Dark Green", "PineTexture");
            tree1.Render(10, 20);
            tree2.Render(30, 40);

            // 7. Proxy Pattern
            Console.WriteLine("\n7. Proxy Pattern:");
            var proxy = new ImageProxy("photo.jpg");
            proxy.Display(); // First call loads the image
            proxy.Display(); // Second call uses cached image
        }

        private static async Task RunBehavioralPatterns()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n=== BEHAVIORAL PATTERNS ===");
            Console.ResetColor();

            // 1. Chain of Responsibility Pattern
            Console.WriteLine("\n1. Chain of Responsibility Pattern:");
            var manager = new Manager();
            var director = new Director();
            var vp = new VP();
            manager.SetSuccessor(director);
            director.SetSuccessor(vp);
            manager.HandleRequest(new PurchaseRequest(500, "Office Supplies"));
            manager.HandleRequest(new PurchaseRequest(5000, "New Computer"));
            manager.HandleRequest(new PurchaseRequest(50000, "Company Car"));

            // 2. Command Pattern
            Console.WriteLine("\n2. Command Pattern:");
            var light = new Light();
            var turnOn = new TurnOnCommand(light);
            var turnOff = new TurnOffCommand(light);
            var remote = new RemoteControl();
            remote.SetCommand(turnOn);
            remote.PressButton();
            remote.SetCommand(turnOff);
            remote.PressButton();

            // 3. Interpreter Pattern
            Console.WriteLine("\n3. Interpreter Pattern:");
            var context = new Context();
            context.SetVariable("x", 5);
            context.SetVariable("y", 3);
            var expression = new AddExpression(
                new VariableExpression("x"),
                new VariableExpression("y"));
            Console.WriteLine($"x + y = {expression.Interpret(context)}");

            // 4. Iterator Pattern
            Console.WriteLine("\n4. Iterator Pattern:");
            var collection = new ConcreteAggregate();
            collection.Add("Item 1");
            collection.Add("Item 2");
            collection.Add("Item 3");
            var iterator = collection.CreateIterator();
            while (iterator.HasNext())
            {
                Console.WriteLine($"Iterator: {iterator.Next()}");
            }

            // 5. Mediator Pattern
            Console.WriteLine("\n5. Mediator Pattern:");
            var mediator = new ConcreteMediator();
            var colleague1 = new ConcreteColleague1(mediator);
            var colleague2 = new ConcreteColleague2(mediator);
            mediator.SetColleague1(colleague1);
            mediator.SetColleague2(colleague2);
            colleague1.Send("Hello from Colleague1");
            colleague2.Send("Hello from Colleague2");

            // 6. Memento Pattern
            Console.WriteLine("\n6. Memento Pattern:");
            var originator = new Originator();
            var caretaker = new Caretaker();
            originator.SetState("State 1");
            caretaker.SaveMemento(originator.CreateMemento());
            originator.SetState("State 2");
            Console.WriteLine($"Current state: {originator.GetState()}");
            originator.RestoreMemento(caretaker.GetMemento());
            Console.WriteLine($"Restored state: {originator.GetState()}");

            // 7. Observer Pattern
            Console.WriteLine("\n7. Observer Pattern:");
            var subject = new ConcreteSubject();
            var observer1 = new ConcreteObserver("Observer1");
            var observer2 = new ConcreteObserver("Observer2");
            subject.Attach(observer1);
            subject.Attach(observer2);
            subject.SetState("New State");

            // 8. State Pattern
            Console.WriteLine("\n8. State Pattern:");
            var trafficLight = new TrafficLightContext();
            trafficLight.Request(); // Red -> Green
            trafficLight.Request(); // Green -> Yellow
            trafficLight.Request(); // Yellow -> Red

            // 9. Strategy Pattern
            Console.WriteLine("\n9. Strategy Pattern:");
            var shoppingCart = new ShoppingCart();
            shoppingCart.SetPaymentStrategy(new CreditCardPayment());
            shoppingCart.Checkout(100);
            shoppingCart.SetPaymentStrategy(new PayPalPayment());
            shoppingCart.Checkout(200);

            // 10. Template Method Pattern
            Console.WriteLine("\n10. Template Method Pattern:");
            var tea = new Tea();
            var coffee = new Coffee();
            Console.WriteLine("Making tea:");
            tea.PrepareRecipe();
            Console.WriteLine("Making coffee:");
            coffee.PrepareRecipe();

            // 11. Visitor Pattern
            Console.WriteLine("\n11. Visitor Pattern:");
            var elements = new List<IElement> { new ConcreteElementA(), new ConcreteElementB() };
            var visitor = new ConcreteVisitor();
            foreach (var element in elements)
            {
                element.Accept(visitor);
            }
        }
    }

    #region Creational Patterns

    // 1. Singleton Pattern
    public class Singleton
    {
        private static Singleton? _instance;
        private static readonly object _lock = new object();

        private Singleton() { }

        public static Singleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new Singleton();
                    }
                }
                return _instance;
            }
        }
    }

    // 2. Factory Method Pattern
    public abstract class Animal
    {
        public abstract void MakeSound();
    }

    public class Dog : Animal
    {
        public override void MakeSound() => Console.WriteLine("Woof!");
    }

    public class Cat : Animal
    {
        public override void MakeSound() => Console.WriteLine("Meow!");
    }

    public abstract class AnimalCreator
    {
        public abstract Animal CreateAnimal();
    }

    public class DogCreator : AnimalCreator
    {
        public override Animal CreateAnimal() => new Dog();
    }

    public class CatCreator : AnimalCreator
    {
        public override Animal CreateAnimal() => new Cat();
    }

    // 3. Abstract Factory Pattern
    public interface IChair
    {
        void SitOn();
    }

    public interface ITable
    {
        void PlaceItem();
    }

    public class ModernChair : IChair
    {
        public void SitOn() => Console.WriteLine("Sitting on modern chair");
    }

    public class ModernTable : ITable
    {
        public void PlaceItem() => Console.WriteLine("Placing item on modern table");
    }

    public interface IFurnitureFactory
    {
        IChair CreateChair();
        ITable CreateTable();
    }

    public class ModernFurnitureFactory : IFurnitureFactory
    {
        public IChair CreateChair() => new ModernChair();
        public ITable CreateTable() => new ModernTable();
    }

    // 4. Builder Pattern
    public class House
    {
        public string Foundation { get; set; } = string.Empty;
        public string Walls { get; set; } = string.Empty;
        public string Roof { get; set; } = string.Empty;

        public override string ToString() => $"House with {Foundation} foundation, {Walls} walls, and {Roof} roof";
    }

    public class HouseBuilder
    {
        private House _house = new House();

        public HouseBuilder SetFoundation(string foundation)
        {
            _house.Foundation = foundation;
            return this;
        }

        public HouseBuilder SetWalls(string walls)
        {
            _house.Walls = walls;
            return this;
        }

        public HouseBuilder SetRoof(string roof)
        {
            _house.Roof = roof;
            return this;
        }

        public House Build() => _house;
    }

    // 5. Prototype Pattern
    public class Document : ICloneable
    {
        public string Title { get; set; }
        public string Content { get; set; }

        public Document(string title, string content)
        {
            Title = title;
            Content = content;
        }

        public Document Clone() => new Document(Title + " (Copy)", Content);

        object ICloneable.Clone() => Clone();

        public override string ToString() => $"Document: {Title} - {Content}";
    }

    #endregion

    #region Structural Patterns

    // 1. Adapter Pattern
    public class Adaptee
    {
        public void SpecificRequest() => Console.WriteLine("Specific request from Adaptee");
    }

    public interface ITarget
    {
        void Request();
    }

    public class Adapter : ITarget
    {
        private readonly Adaptee _adaptee;

        public Adapter(Adaptee adaptee)
        {
            _adaptee = adaptee;
        }

        public void Request() => _adaptee.SpecificRequest();
    }

    // 2. Bridge Pattern
    public interface IColor
    {
        void ApplyColor();
    }

    public class RedColor : IColor
    {
        public void ApplyColor() => Console.WriteLine("Applying red color");
    }

    public class BlueColor : IColor
    {
        public void ApplyColor() => Console.WriteLine("Applying blue color");
    }

    public abstract class Shape
    {
        protected IColor _color;

        protected Shape(IColor color)
        {
            _color = color;
        }

        public abstract void Draw();
    }

    public class Circle : Shape
    {
        public Circle(IColor color) : base(color) { }

        public override void Draw()
        {
            Console.Write("Drawing circle. ");
            _color.ApplyColor();
        }
    }

    public class Square : Shape
    {
        public Square(IColor color) : base(color) { }

        public override void Draw()
        {
            Console.Write("Drawing square. ");
            _color.ApplyColor();
        }
    }

    // 3. Composite Pattern
    public abstract class Component
    {
        protected string _name;

        public Component(string name)
        {
            _name = name;
        }

        public abstract void Add(Component component);
        public abstract void Remove(Component component);
        public abstract void Operation();
    }

    public class LeafComponent : Component
    {
        public LeafComponent(string name) : base(name) { }

        public override void Add(Component component) => throw new NotSupportedException();
        public override void Remove(Component component) => throw new NotSupportedException();
        public override void Operation() => Console.WriteLine($"Leaf {_name} operation");
    }

    public class CompositeComponent : Component
    {
        private readonly List<Component> _children = new List<Component>();

        public CompositeComponent(string name) : base(name) { }

        public override void Add(Component component) => _children.Add(component);
        public override void Remove(Component component) => _children.Remove(component);

        public override void Operation()
        {
            Console.WriteLine($"Composite {_name} operation");
            foreach (var child in _children)
            {
                child.Operation();
            }
        }
    }

    // 4. Decorator Pattern
    public interface ICoffee
    {
        double GetCost();
        string GetDescription();
    }

    public class SimpleCoffee : ICoffee
    {
        public double GetCost() => 2.0;
        public string GetDescription() => "Simple coffee";
    }

    public abstract class CoffeeDecorator : ICoffee
    {
        protected ICoffee _coffee;

        public CoffeeDecorator(ICoffee coffee)
        {
            _coffee = coffee;
        }

        public virtual double GetCost() => _coffee.GetCost();
        public virtual string GetDescription() => _coffee.GetDescription();
    }

    public class MilkDecorator : CoffeeDecorator
    {
        public MilkDecorator(ICoffee coffee) : base(coffee) { }

        public override double GetCost() => base.GetCost() + 0.5;
        public override string GetDescription() => base.GetDescription() + ", milk";
    }

    public class SugarDecorator : CoffeeDecorator
    {
        public SugarDecorator(ICoffee coffee) : base(coffee) { }

        public override double GetCost() => base.GetCost() + 0.3;
        public override string GetDescription() => base.GetDescription() + ", sugar";
    }

    // 5. Facade Pattern
    public class CPU
    {
        public void Start() => Console.WriteLine("CPU started");
    }

    public class Memory
    {
        public void Load() => Console.WriteLine("Memory loaded");
    }

    public class HardDrive
    {
        public void Read() => Console.WriteLine("Hard drive reading");
    }

    public class ComputerFacade
    {
        private readonly CPU _cpu = new CPU();
        private readonly Memory _memory = new Memory();
        private readonly HardDrive _hardDrive = new HardDrive();

        public void Start()
        {
            Console.WriteLine("Computer facade starting...");
            _cpu.Start();
            _memory.Load();
            _hardDrive.Read();
            Console.WriteLine("Computer started successfully!");
        }
    }

    // 6. Flyweight Pattern
    public class TreeType
    {
        private readonly string _name;
        private readonly string _color;
        private readonly string _sprite;

        public TreeType(string name, string color, string sprite)
        {
            _name = name;
            _color = color;
            _sprite = sprite;
        }

        public void Render(int x, int y)
        {
            Console.WriteLine($"Rendering {_name} tree ({_color}) at ({x}, {y})");
        }
    }

    public class TreeTypeFactory
    {
        private static readonly Dictionary<string, TreeType> _treeTypes = new Dictionary<string, TreeType>();

        public TreeType GetTreeType(string name, string color, string sprite)
        {
            string key = $"{name}-{color}-{sprite}";
            if (!_treeTypes.ContainsKey(key))
            {
                _treeTypes[key] = new TreeType(name, color, sprite);
                Console.WriteLine($"Created new TreeType: {key}");
            }
            return _treeTypes[key];
        }
    }

    // 7. Proxy Pattern
    public interface IImage
    {
        void Display();
    }

    public class RealImage : IImage
    {
        private readonly string _filename;

        public RealImage(string filename)
        {
            _filename = filename;
            LoadImageFromDisk();
        }

        private void LoadImageFromDisk()
        {
            Console.WriteLine($"Loading image: {_filename}");
        }

        public void Display()
        {
            Console.WriteLine($"Displaying image: {_filename}");
        }
    }

    public class ImageProxy : IImage
    {
        private RealImage? _realImage;
        private readonly string _filename;

        public ImageProxy(string filename)
        {
            _filename = filename;
        }

        public void Display()
        {
            _realImage ??= new RealImage(_filename);
            _realImage.Display();
        }
    }

    #endregion

    #region Behavioral Patterns

    // 1. Chain of Responsibility Pattern
    public class PurchaseRequest
    {
        public double Amount { get; }
        public string Purpose { get; }

        public PurchaseRequest(double amount, string purpose)
        {
            Amount = amount;
            Purpose = purpose;
        }
    }

    public abstract class Approver
    {
        protected Approver? _successor;

        public void SetSuccessor(Approver successor)
        {
            _successor = successor;
        }

        public abstract void HandleRequest(PurchaseRequest request);
    }

    public class Manager : Approver
    {
        public override void HandleRequest(PurchaseRequest request)
        {
            if (request.Amount < 1000)
            {
                Console.WriteLine($"Manager approved ${request.Amount} for {request.Purpose}");
            }
            else
            {
                _successor?.HandleRequest(request);
            }
        }
    }

    public class Director : Approver
    {
        public override void HandleRequest(PurchaseRequest request)
        {
            if (request.Amount < 10000)
            {
                Console.WriteLine($"Director approved ${request.Amount} for {request.Purpose}");
            }
            else
            {
                _successor?.HandleRequest(request);
            }
        }
    }

    public class VP : Approver
    {
        public override void HandleRequest(PurchaseRequest request)
        {
            Console.WriteLine($"VP approved ${request.Amount} for {request.Purpose}");
        }
    }

    // 2. Command Pattern
    public interface ICommand
    {
        void Execute();
    }

    public class Light
    {
        public void TurnOn() => Console.WriteLine("Light is ON");
        public void TurnOff() => Console.WriteLine("Light is OFF");
    }

    public class TurnOnCommand : ICommand
    {
        private readonly Light _light;

        public TurnOnCommand(Light light)
        {
            _light = light;
        }

        public void Execute() => _light.TurnOn();
    }

    public class TurnOffCommand : ICommand
    {
        private readonly Light _light;

        public TurnOffCommand(Light light)
        {
            _light = light;
        }

        public void Execute() => _light.TurnOff();
    }

    public class RemoteControl
    {
        private ICommand? _command;

        public void SetCommand(ICommand command)
        {
            _command = command;
        }

        public void PressButton()
        {
            _command?.Execute();
        }
    }

    // 3. Interpreter Pattern
    public interface IExpression
    {
        int Interpret(Context context);
    }

    public class Context
    {
        private readonly Dictionary<string, int> _variables = new Dictionary<string, int>();

        public void SetVariable(string name, int value)
        {
            _variables[name] = value;
        }

        public int GetVariable(string name)
        {
            return _variables.TryGetValue(name, out int value) ? value : 0;
        }
    }

    public class VariableExpression : IExpression
    {
        private readonly string _name;

        public VariableExpression(string name)
        {
            _name = name;
        }

        public int Interpret(Context context)
        {
            return context.GetVariable(_name);
        }
    }

    public class AddExpression : IExpression
    {
        private readonly IExpression _left;
        private readonly IExpression _right;

        public AddExpression(IExpression left, IExpression right)
        {
            _left = left;
            _right = right;
        }

        public int Interpret(Context context)
        {
            return _left.Interpret(context) + _right.Interpret(context);
        }
    }

    // 4. Iterator Pattern
    public interface IIterator
    {
        bool HasNext();
        object Next();
    }

    public interface IAggregate
    {
        IIterator CreateIterator();
    }

    public class ConcreteIterator : IIterator
    {
        private readonly ConcreteAggregate _aggregate;
        private int _current = 0;

        public ConcreteIterator(ConcreteAggregate aggregate)
        {
            _aggregate = aggregate;
        }

        public bool HasNext()
        {
            return _current < _aggregate.Count;
        }

        public object Next()
        {
            return _aggregate.GetItem(_current++);
        }
    }

    public class ConcreteAggregate : IAggregate
    {
        private readonly List<object> _items = new List<object>();

        public int Count => _items.Count;

        public void Add(object item)
        {
            _items.Add(item);
        }

        public object GetItem(int index)
        {
            return _items[index];
        }

        public IIterator CreateIterator()
        {
            return new ConcreteIterator(this);
        }
    }

    // 5. Mediator Pattern
    public interface IMediator
    {
        void SendMessage(string message, Colleague colleague);
    }

    public abstract class Colleague
    {
        protected IMediator _mediator;

        public Colleague(IMediator mediator)
        {
            _mediator = mediator;
        }

        public abstract void Send(string message);
        public abstract void Receive(string message);
    }

    public class ConcreteColleague1 : Colleague
    {
        public ConcreteColleague1(IMediator mediator) : base(mediator) { }

        public override void Send(string message)
        {
            _mediator.SendMessage(message, this);
        }

        public override void Receive(string message)
        {
            Console.WriteLine($"Colleague1 received: {message}");
        }
    }

    public class ConcreteColleague2 : Colleague
    {
        public ConcreteColleague2(IMediator mediator) : base(mediator) { }

        public override void Send(string message)
        {
            _mediator.SendMessage(message, this);
        }

        public override void Receive(string message)
        {
            Console.WriteLine($"Colleague2 received: {message}");
        }
    }

    public class ConcreteMediator : IMediator
    {
        private ConcreteColleague1? _colleague1;
        private ConcreteColleague2? _colleague2;

        public void SetColleague1(ConcreteColleague1 colleague1)
        {
            _colleague1 = colleague1;
        }

        public void SetColleague2(ConcreteColleague2 colleague2)
        {
            _colleague2 = colleague2;
        }

        public void SendMessage(string message, Colleague colleague)
        {
            if (colleague == _colleague1)
            {
                _colleague2?.Receive(message);
            }
            else
            {
                _colleague1?.Receive(message);
            }
        }
    }

    // 6. Memento Pattern
    public class Memento
    {
        private readonly string _state;

        public Memento(string state)
        {
            _state = state;
        }

        public string GetState()
        {
            return _state;
        }
    }

    public class Originator
    {
        private string _state = string.Empty;

        public void SetState(string state)
        {
            _state = state;
        }

        public string GetState()
        {
            return _state;
        }

        public Memento CreateMemento()
        {
            return new Memento(_state);
        }

        public void RestoreMemento(Memento memento)
        {
            _state = memento.GetState();
        }
    }

    public class Caretaker
    {
        private Memento? _memento;

        public void SaveMemento(Memento memento)
        {
            _memento = memento;
        }

        public Memento GetMemento()
        {
            return _memento ?? throw new InvalidOperationException("No memento saved");
        }
    }

    // 7. Observer Pattern
    public interface IObserver
    {
        void Update(string state);
    }

    public interface ISubject
    {
        void Attach(IObserver observer);
        void Detach(IObserver observer);
        void Notify();
    }

    public class ConcreteSubject : ISubject
    {
        private readonly List<IObserver> _observers = new List<IObserver>();
        private string _state = string.Empty;

        public void Attach(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void Notify()
        {
            foreach (var observer in _observers)
            {
                observer.Update(_state);
            }
        }

        public void SetState(string state)
        {
            _state = state;
            Notify();
        }
    }

    public class ConcreteObserver : IObserver
    {
        private readonly string _name;

        public ConcreteObserver(string name)
        {
            _name = name;
        }

        public void Update(string state)
        {
            Console.WriteLine($"{_name} received update: {state}");
        }
    }

    // 8. State Pattern
    public interface ITrafficLightState
    {
        void Handle(TrafficLightContext context);
    }

    public class RedState : ITrafficLightState
    {
        public void Handle(TrafficLightContext context)
        {
            Console.WriteLine("Red light -> Green light");
            context.SetState(new GreenState());
        }
    }

    public class GreenState : ITrafficLightState
    {
        public void Handle(TrafficLightContext context)
        {
            Console.WriteLine("Green light -> Yellow light");
            context.SetState(new YellowState());
        }
    }

    public class YellowState : ITrafficLightState
    {
        public void Handle(TrafficLightContext context)
        {
            Console.WriteLine("Yellow light -> Red light");
            context.SetState(new RedState());
        }
    }

    public class TrafficLightContext
    {
        private ITrafficLightState _state;

        public TrafficLightContext()
        {
            _state = new RedState();
        }

        public void SetState(ITrafficLightState state)
        {
            _state = state;
        }

        public void Request()
        {
            _state.Handle(this);
        }
    }

    // 9. Strategy Pattern
    public interface IPaymentStrategy
    {
        void Pay(double amount);
    }

    public class CreditCardPayment : IPaymentStrategy
    {
        public void Pay(double amount)
        {
            Console.WriteLine($"Paid ${amount} using Credit Card");
        }
    }

    public class PayPalPayment : IPaymentStrategy
    {
        public void Pay(double amount)
        {
            Console.WriteLine($"Paid ${amount} using PayPal");
        }
    }

    public class ShoppingCart
    {
        private IPaymentStrategy? _paymentStrategy;

        public void SetPaymentStrategy(IPaymentStrategy strategy)
        {
            _paymentStrategy = strategy;
        }

        public void Checkout(double amount)
        {
            _paymentStrategy?.Pay(amount);
        }
    }

    // 10. Template Method Pattern
    public abstract class CaffeineBeverage
    {
        public void PrepareRecipe()
        {
            BoilWater();
            Brew();
            PourInCup();
            AddCondiments();
        }

        private void BoilWater()
        {
            Console.WriteLine("Boiling water");
        }

        private void PourInCup()
        {
            Console.WriteLine("Pouring into cup");
        }

        protected abstract void Brew();
        protected abstract void AddCondiments();
    }

    public class Tea : CaffeineBeverage
    {
        protected override void Brew()
        {
            Console.WriteLine("Steeping the tea");
        }

        protected override void AddCondiments()
        {
            Console.WriteLine("Adding lemon");
        }
    }

    public class Coffee : CaffeineBeverage
    {
        protected override void Brew()
        {
            Console.WriteLine("Dripping coffee through filter");
        }

        protected override void AddCondiments()
        {
            Console.WriteLine("Adding sugar and milk");
        }
    }

    // 11. Visitor Pattern
    public interface IVisitor
    {
        void VisitConcreteElementA(ConcreteElementA element);
        void VisitConcreteElementB(ConcreteElementB element);
    }

    public interface IElement
    {
        void Accept(IVisitor visitor);
    }

    public class ConcreteElementA : IElement
    {
        public void Accept(IVisitor visitor)
        {
            visitor.VisitConcreteElementA(this);
        }

        public void OperationA()
        {
            Console.WriteLine("ConcreteElementA operation");
        }
    }

    public class ConcreteElementB : IElement
    {
        public void Accept(IVisitor visitor)
        {
            visitor.VisitConcreteElementB(this);
        }

        public void OperationB()
        {
            Console.WriteLine("ConcreteElementB operation");
        }
    }

    public class ConcreteVisitor : IVisitor
    {
        public void VisitConcreteElementA(ConcreteElementA element)
        {
            Console.WriteLine("Visitor processing ConcreteElementA");
            element.OperationA();
        }

        public void VisitConcreteElementB(ConcreteElementB element)
        {
            Console.WriteLine("Visitor processing ConcreteElementB");
            element.OperationB();
        }
    }

    #endregion
}
