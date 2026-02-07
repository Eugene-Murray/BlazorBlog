using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ConsoleExperimentsApp.Experiments
{
    #region Helper Classes and Attributes

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class CustomDescriptionAttribute : Attribute
    {
        public string Description { get; set; }
        public CustomDescriptionAttribute(string description)
        {
            Description = description;
        }
    }

    [CustomDescription("Sample person class for reflection")]
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        private string _secret = "Hidden value";

        [CustomDescription("Email property")]
        public string Email { get; set; }

        public Person() { }

        public Person(string name, string email)
        {
            Name = name;
            Email = email;
        }

        [CustomDescription("Greet method")]
        public string Greet(string greeting)
        {
            return $"{greeting}, {Name}!";
        }

        private void PrivateMethod()
        {
            Console.WriteLine("This is a private method");
        }

        public static void StaticMethod()
        {
            Console.WriteLine("This is a static method");
        }
    }

    public class Employee : Person
    {
        public string Department { get; set; }
        public decimal Salary { get; set; }
    }

    public class GenericContainer<T>
    {
        public T Value { get; set; }
        public void Display() => Console.WriteLine($"Value: {Value}");
    }

    public interface IProcessor
    {
        void Process();
    }

    public class DataProcessor : IProcessor
    {
        public void Process() => Console.WriteLine("Processing data");
    }

    #endregion

    public class ReflectionExperiments
    {
        public static void Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== Reflection Experiments ===");
            Console.ResetColor();

            Example1_GetTypeInformation();
            Example2_GetAllProperties();
            Example3_GetAllMethods();
            Example4_GetAllFields();
            Example5_CreateInstanceDynamically();
            Example6_GetAndSetPropertyValues();
            Example7_InvokeMethodDynamically();
            Example8_GetCustomAttributes();
            Example9_GetConstructors();
            Example10_InvokePrivateMethod();
            Example11_WorkWithGenericTypes();
            Example12_GetAssemblyInformation();
            Example13_GetInterfaceImplementations();
            Example14_GetTypeHierarchy();
            Example15_GetPropertyAttributes();
            Example16_GetMethodParameters();
            Example17_CheckTypeRelationships();
            Example18_GetAllTypesInAssembly();
            Example19_CreateGenericInstance();
            Example20_GetEvents();
            Example21_GetNestedTypes();
            Example22_GetMemberAccessModifiers();
            Example23_CloneObjectUsingReflection();
            Example24_CompareObjectsUsingReflection();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nPress Enter to exit...");
            Console.ResetColor();
        }

        static void Example1_GetTypeInformation()
        {
            PrintHeader("Example 1: Get Type Information");

            Person person = new Person { Name = "John", Email = "john@example.com" };
            Type type = person.GetType();

            Console.WriteLine($"Type Name: {type.Name}");
            Console.WriteLine($"Full Name: {type.FullName}");
            Console.WriteLine($"Namespace: {type.Namespace}");
            Console.WriteLine($"Is Class: {type.IsClass}");
            Console.WriteLine($"Is Public: {type.IsPublic}");
            Console.WriteLine($"Assembly: {type.Assembly.GetName().Name}");
        }

        static void Example2_GetAllProperties()
        {
            PrintHeader("Example 2: Get All Properties");

            Type type = typeof(Person);
            PropertyInfo[] properties = type.GetProperties();

            Console.WriteLine($"Properties of {type.Name}:");
            foreach (var prop in properties)
            {
                Console.WriteLine($"  - {prop.Name} ({prop.PropertyType.Name}) " +
                    $"[CanRead: {prop.CanRead}, CanWrite: {prop.CanWrite}]");
            }
        }

        static void Example3_GetAllMethods()
        {
            PrintHeader("Example 3: Get All Methods");

            Type type = typeof(Person);
            MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            Console.WriteLine($"Public Instance Methods of {type.Name}:");
            foreach (var method in methods)
            {
                if (!method.IsSpecialName)
                {
                    var parameters = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                    Console.WriteLine($"  - {method.ReturnType.Name} {method.Name}({parameters})");
                }
            }
        }

        static void Example4_GetAllFields()
        {
            PrintHeader("Example 4: Get All Fields (Including Private)");

            Type type = typeof(Person);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            Console.WriteLine($"Fields of {type.Name}:");
            foreach (var field in fields)
            {
                Console.WriteLine($"  - {field.Name} ({field.FieldType.Name}) " +
                    $"[IsPrivate: {field.IsPrivate}, IsPublic: {field.IsPublic}]");
            }
        }

        static void Example5_CreateInstanceDynamically()
        {
            PrintHeader("Example 5: Create Instance Dynamically");

            Type type = typeof(Person);

            object instance1 = Activator.CreateInstance(type);
            Console.WriteLine($"Created instance using default constructor: {instance1.GetType().Name}");

            object instance2 = Activator.CreateInstance(type, "Alice", "alice@example.com");
            Person person = (Person)instance2;
            Console.WriteLine($"Created instance with parameters: Name={person.Name}, Email={person.Email}");
        }

        static void Example6_GetAndSetPropertyValues()
        {
            PrintHeader("Example 6: Get and Set Property Values");

            Person person = new Person();
            Type type = person.GetType();

            PropertyInfo nameProp = type.GetProperty("Name");
            nameProp.SetValue(person, "Bob");
            Console.WriteLine($"Set Name to: {nameProp.GetValue(person)}");

            PropertyInfo emailProp = type.GetProperty("Email");
            emailProp.SetValue(person, "bob@example.com");
            Console.WriteLine($"Set Email to: {emailProp.GetValue(person)}");
        }

        static void Example7_InvokeMethodDynamically()
        {
            PrintHeader("Example 7: Invoke Method Dynamically");

            Person person = new Person { Name = "Charlie" };
            Type type = person.GetType();

            MethodInfo greetMethod = type.GetMethod("Greet");
            object result = greetMethod.Invoke(person, new object[] { "Hello" });
            Console.WriteLine($"Method invocation result: {result}");

            MethodInfo staticMethod = type.GetMethod("StaticMethod");
            staticMethod.Invoke(null, null);
        }

        static void Example8_GetCustomAttributes()
        {
            PrintHeader("Example 8: Get Custom Attributes");

            Type type = typeof(Person);

            var classAttributes = type.GetCustomAttributes(typeof(CustomDescriptionAttribute), false);
            foreach (CustomDescriptionAttribute attr in classAttributes)
            {
                Console.WriteLine($"Class Description: {attr.Description}");
            }

            MethodInfo greetMethod = type.GetMethod("Greet");
            var methodAttributes = greetMethod.GetCustomAttributes(typeof(CustomDescriptionAttribute), false);
            foreach (CustomDescriptionAttribute attr in methodAttributes)
            {
                Console.WriteLine($"Method Description: {attr.Description}");
            }
        }

        static void Example9_GetConstructors()
        {
            PrintHeader("Example 9: Get Constructors");

            Type type = typeof(Person);
            ConstructorInfo[] constructors = type.GetConstructors();

            Console.WriteLine($"Constructors of {type.Name}:");
            foreach (var ctor in constructors)
            {
                var parameters = string.Join(", ", ctor.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                Console.WriteLine($"  - {type.Name}({parameters})");
            }
        }

        static void Example10_InvokePrivateMethod()
        {
            PrintHeader("Example 10: Invoke Private Method");

            Person person = new Person();
            Type type = person.GetType();

            MethodInfo privateMethod = type.GetMethod("PrivateMethod", BindingFlags.NonPublic | BindingFlags.Instance);
            Console.Write("Invoking private method: ");
            privateMethod.Invoke(person, null);
        }

        static void Example11_WorkWithGenericTypes()
        {
            PrintHeader("Example 11: Work with Generic Types");

            Type genericType = typeof(GenericContainer<>);
            Console.WriteLine($"Is Generic Type Definition: {genericType.IsGenericTypeDefinition}");
            Console.WriteLine($"Generic Type: {genericType.Name}");

            Type constructedType = genericType.MakeGenericType(typeof(int));
            Console.WriteLine($"Constructed Type: {constructedType.Name}");

            object instance = Activator.CreateInstance(constructedType);
            PropertyInfo valueProp = constructedType.GetProperty("Value");
            valueProp.SetValue(instance, 42);
            Console.WriteLine($"Generic Value: {valueProp.GetValue(instance)}");
        }

        static void Example12_GetAssemblyInformation()
        {
            PrintHeader("Example 12: Get Assembly Information");

            Assembly assembly = Assembly.GetExecutingAssembly();
            Console.WriteLine($"Assembly Name: {assembly.GetName().Name}");
            Console.WriteLine($"Version: {assembly.GetName().Version}");
            Console.WriteLine($"Location: {assembly.Location}");
            Console.WriteLine($"Full Name: {assembly.FullName}");
        }

        static void Example13_GetInterfaceImplementations()
        {
            PrintHeader("Example 13: Get Interface Implementations");

            Type type = typeof(DataProcessor);
            Type[] interfaces = type.GetInterfaces();

            Console.WriteLine($"Interfaces implemented by {type.Name}:");
            foreach (var iface in interfaces)
            {
                Console.WriteLine($"  - {iface.Name}");
            }

            Console.WriteLine($"Implements IProcessor: {typeof(IProcessor).IsAssignableFrom(type)}");
        }

        static void Example14_GetTypeHierarchy()
        {
            PrintHeader("Example 14: Get Type Hierarchy");

            Type type = typeof(Employee);
            Console.WriteLine($"Type Hierarchy for {type.Name}:");

            Type currentType = type;
            while (currentType != null)
            {
                Console.WriteLine($"  - {currentType.Name}");
                currentType = currentType.BaseType;
            }
        }

        static void Example15_GetPropertyAttributes()
        {
            PrintHeader("Example 15: Get Property Attributes");

            Type type = typeof(Person);
            PropertyInfo emailProp = type.GetProperty("Email");

            var attributes = emailProp.GetCustomAttributes(typeof(CustomDescriptionAttribute), false);
            foreach (CustomDescriptionAttribute attr in attributes)
            {
                Console.WriteLine($"Email Property Description: {attr.Description}");
            }
        }

        static void Example16_GetMethodParameters()
        {
            PrintHeader("Example 16: Get Method Parameters");

            Type type = typeof(Person);
            MethodInfo greetMethod = type.GetMethod("Greet");
            ParameterInfo[] parameters = greetMethod.GetParameters();

            Console.WriteLine($"Parameters of {greetMethod.Name} method:");
            foreach (var param in parameters)
            {
                Console.WriteLine($"  - {param.ParameterType.Name} {param.Name} " +
                    $"[Position: {param.Position}, HasDefaultValue: {param.HasDefaultValue}]");
            }
        }

        static void Example17_CheckTypeRelationships()
        {
            PrintHeader("Example 17: Check Type Relationships");

            Type employeeType = typeof(Employee);
            Type personType = typeof(Person);

            Console.WriteLine($"Is Employee a subclass of Person: {employeeType.IsSubclassOf(personType)}");
            Console.WriteLine($"Is Person assignable from Employee: {personType.IsAssignableFrom(employeeType)}");
            Console.WriteLine($"Is Employee assignable from Person: {employeeType.IsAssignableFrom(personType)}");
        }

        static void Example18_GetAllTypesInAssembly()
        {
            PrintHeader("Example 18: Get All Types in Assembly");

            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes()
                .Where(t => t.Namespace == "ConsoleExperimentsApp.Experiments" && !t.IsNested)
                .Take(5)
                .ToArray();

            Console.WriteLine($"Sample types in ConsoleExperimentsApp.Experiments namespace:");
            foreach (var type in types)
            {
                Console.WriteLine($"  - {type.Name} [IsClass: {type.IsClass}, IsInterface: {type.IsInterface}]");
            }
        }

        static void Example19_CreateGenericInstance()
        {
            PrintHeader("Example 19: Create Generic Instance");

            Type openGenericType = typeof(List<>);
            Type closedGenericType = openGenericType.MakeGenericType(typeof(string));

            object listInstance = Activator.CreateInstance(closedGenericType);
            MethodInfo addMethod = closedGenericType.GetMethod("Add");

            addMethod.Invoke(listInstance, new object[] { "Item 1" });
            addMethod.Invoke(listInstance, new object[] { "Item 2" });
            addMethod.Invoke(listInstance, new object[] { "Item 3" });

            PropertyInfo countProp = closedGenericType.GetProperty("Count");
            Console.WriteLine($"List Count: {countProp.GetValue(listInstance)}");
        }

        static void Example20_GetEvents()
        {
            PrintHeader("Example 20: Get Events");

            Type type = typeof(System.ComponentModel.BackgroundWorker);
            EventInfo[] events = type.GetEvents().Take(3).ToArray();

            Console.WriteLine($"Sample Events in {type.Name}:");
            foreach (var evt in events)
            {
                Console.WriteLine($"  - {evt.Name} ({evt.EventHandlerType.Name})");
            }
        }

        static void Example21_GetNestedTypes()
        {
            PrintHeader("Example 21: Get Nested Types");

            Type type = typeof(Environment);
            Type[] nestedTypes = type.GetNestedTypes(BindingFlags.Public).Take(3).ToArray();

            Console.WriteLine($"Sample Nested Types in {type.Name}:");
            foreach (var nestedType in nestedTypes)
            {
                Console.WriteLine($"  - {nestedType.Name}");
            }
        }

        static void Example22_GetMemberAccessModifiers()
        {
            PrintHeader("Example 22: Get Member Access Modifiers");

            Type type = typeof(Person);

            Console.WriteLine("Properties with access modifiers:");
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var getMethod = prop.GetGetMethod(true);
                var setMethod = prop.GetSetMethod(true);
                Console.WriteLine($"  - {prop.Name}: Get={GetAccessModifier(getMethod)}, Set={GetAccessModifier(setMethod)}");
            }
        }

        static void Example23_CloneObjectUsingReflection()
        {
            PrintHeader("Example 23: Clone Object Using Reflection");

            Person original = new Person 
            { 
                Id = 1, 
                Name = "Original", 
                Email = "original@example.com" 
            };

            Person clone = new Person();
            Type type = typeof(Person);

            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    prop.SetValue(clone, prop.GetValue(original));
                }
            }

            Console.WriteLine($"Original: Id={original.Id}, Name={original.Name}, Email={original.Email}");
            Console.WriteLine($"Clone: Id={clone.Id}, Name={clone.Name}, Email={clone.Email}");
            Console.WriteLine($"Are they the same instance? {ReferenceEquals(original, clone)}");
        }

        static void Example24_CompareObjectsUsingReflection()
        {
            PrintHeader("Example 24: Compare Objects Using Reflection");

            Person person1 = new Person { Id = 1, Name = "John", Email = "john@example.com" };
            Person person2 = new Person { Id = 1, Name = "John", Email = "john@example.com" };
            Person person3 = new Person { Id = 2, Name = "Jane", Email = "jane@example.com" };

            Console.WriteLine($"person1 == person2 (by properties): {CompareByReflection(person1, person2)}");
            Console.WriteLine($"person1 == person3 (by properties): {CompareByReflection(person1, person3)}");
        }

        #region Helper Methods

        static void PrintHeader(string title)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n{title}");
            Console.WriteLine(new string('-', title.Length));
            Console.ResetColor();
        }

        static string GetAccessModifier(MethodInfo method)
        {
            if (method == null) return "None";
            if (method.IsPublic) return "Public";
            if (method.IsPrivate) return "Private";
            if (method.IsFamily) return "Protected";
            if (method.IsAssembly) return "Internal";
            return "Unknown";
        }

        static bool CompareByReflection(object obj1, object obj2)
        {
            if (obj1 == null || obj2 == null) return obj1 == obj2;
            if (obj1.GetType() != obj2.GetType()) return false;

            Type type = obj1.GetType();
            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (prop.CanRead)
                {
                    object value1 = prop.GetValue(obj1);
                    object value2 = prop.GetValue(obj2);

                    if (!Equals(value1, value2))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion
    }
}
