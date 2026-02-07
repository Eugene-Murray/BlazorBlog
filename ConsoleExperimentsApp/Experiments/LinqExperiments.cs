using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ConsoleExperimentsApp.Experiments
{
    public class LinqExperiments
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== Linq Experiments ===");
            Console.ResetColor();

            // Sample data
            var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var names = new[] { "Alice", "Bob", "Charlie", "David", "Eve", "Frank", "Grace", "Henry" };
            var people = new[]
            {
                new Person { Id = 1, Name = "Alice", Age = 28, City = "New York", Salary = 75000 },
                new Person { Id = 2, Name = "Bob", Age = 35, City = "London", Salary = 82000 },
                new Person { Id = 3, Name = "Charlie", Age = 22, City = "New York", Salary = 55000 },
                new Person { Id = 4, Name = "David", Age = 45, City = "Paris", Salary = 95000 },
                new Person { Id = 5, Name = "Eve", Age = 28, City = "London", Salary = 78000 },
                new Person { Id = 6, Name = "Frank", Age = 38, City = "Tokyo", Salary = 88000 },
                new Person { Id = 7, Name = "Grace", Age = 29, City = "New York", Salary = 72000 },
                new Person { Id = 8, Name = "Henry", Age = 42, City = "Paris", Salary = 91000 }
            };

            var orders = new[]
            {
                new Order { Id = 1, PersonId = 1, Product = "Laptop", Amount = 1200 },
                new Order { Id = 2, PersonId = 1, Product = "Mouse", Amount = 25 },
                new Order { Id = 3, PersonId = 2, Product = "Keyboard", Amount = 75 },
                new Order { Id = 4, PersonId = 3, Product = "Monitor", Amount = 350 },
                new Order { Id = 5, PersonId = 2, Product = "Laptop", Amount = 1500 },
                new Order { Id = 6, PersonId = 4, Product = "Tablet", Amount = 450 }
            };

            // Example 1: Where - Filter even numbers
            Console.WriteLine("\n1. Where - Filter even numbers:");
            var evenNumbers = numbers.Where(n => n % 2 == 0);
            Console.WriteLine(string.Join(", ", evenNumbers));

            // Example 2: Select - Project to new form
            Console.WriteLine("\n2. Select - Project to squares:");
            var squares = numbers.Select(n => n * n);
            Console.WriteLine(string.Join(", ", squares));

            // Example 3: Select with index
            Console.WriteLine("\n3. Select with index:");
            var indexedNames = names.Select((name, index) => $"{index + 1}. {name}");
            Console.WriteLine(string.Join(", ", indexedNames));

            // Example 4: SelectMany - Flatten nested collections
            Console.WriteLine("\n4. SelectMany - Flatten nested collections:");
            var nestedNumbers = new[] { new[] { 1, 2 }, new[] { 3, 4 }, new[] { 5, 6 } };
            var flattened = nestedNumbers.SelectMany(arr => arr);
            Console.WriteLine(string.Join(", ", flattened));

            // Example 5: OrderBy - Sort ascending
            Console.WriteLine("\n5. OrderBy - Sort people by age:");
            var sortedByAge = people.OrderBy(p => p.Age);
            foreach (var p in sortedByAge.Take(3))
                Console.WriteLine($"  {p.Name}: {p.Age} years");

            // Example 6: OrderByDescending - Sort descending
            Console.WriteLine("\n6. OrderByDescending - Sort by salary (highest first):");
            var sortedBySalary = people.OrderByDescending(p => p.Salary);
            foreach (var p in sortedBySalary.Take(3))
                Console.WriteLine($"  {p.Name}: ${p.Salary:N0}");

            // Example 7: ThenBy - Multiple sort criteria
            Console.WriteLine("\n7. ThenBy - Sort by city, then by name:");
            var multiSort = people.OrderBy(p => p.City).ThenBy(p => p.Name);
            foreach (var p in multiSort.Take(4))
                Console.WriteLine($"  {p.City}: {p.Name}");

            // Example 8: GroupBy - Group by city
            Console.WriteLine("\n8. GroupBy - Group people by city:");
            var groupedByCity = people.GroupBy(p => p.City);
            foreach (var group in groupedByCity)
                Console.WriteLine($"  {group.Key}: {group.Count()} people");

            // Example 9: Join - Inner join
            Console.WriteLine("\n9. Join - People with their orders:");
            var joined = people.Join(orders,
                person => person.Id,
                order => order.PersonId,
                (person, order) => new { person.Name, order.Product, order.Amount });
            foreach (var item in joined.Take(3))
                Console.WriteLine($"  {item.Name} bought {item.Product} for ${item.Amount}");

            // Example 10: GroupJoin - Left outer join
            Console.WriteLine("\n10. GroupJoin - People with all their orders:");
            var groupJoined = people.GroupJoin(orders,
                person => person.Id,
                order => order.PersonId,
                (person, personOrders) => new { person.Name, OrderCount = personOrders.Count() });
            foreach (var item in groupJoined.Take(4))
                Console.WriteLine($"  {item.Name} has {item.OrderCount} order(s)");

            // Example 11: First - Get first element
            Console.WriteLine("\n11. First - First person:");
            var firstPerson = people.First();
            Console.WriteLine($"  {firstPerson.Name}");

            // Example 12: FirstOrDefault - With predicate
            Console.WriteLine("\n12. FirstOrDefault - First person from Tokyo:");
            var tokyoPerson = people.FirstOrDefault(p => p.City == "Tokyo");
            Console.WriteLine($"  {tokyoPerson?.Name ?? "None"}");

            // Example 13: Last - Get last element
            Console.WriteLine("\n13. Last - Last person:");
            var lastPerson = people.Last();
            Console.WriteLine($"  {lastPerson.Name}");

            // Example 14: Single - Get single matching element
            Console.WriteLine("\n14. SingleOrDefault - Person with Id 5:");
            var specificPerson = people.SingleOrDefault(p => p.Id == 5);
            Console.WriteLine($"  {specificPerson?.Name}");

            // Example 15: Any - Check if any element matches
            Console.WriteLine("\n15. Any - Are there people over 40?");
            var hasOver40 = people.Any(p => p.Age > 40);
            Console.WriteLine($"  {hasOver40}");

            // Example 16: All - Check if all elements match
            Console.WriteLine("\n16. All - Are all people adults (18+)?");
            var allAdults = people.All(p => p.Age >= 18);
            Console.WriteLine($"  {allAdults}");

            // Example 17: Contains - Check if collection contains element
            Console.WriteLine("\n17. Contains - Does numbers contain 5?");
            var containsFive = numbers.Contains(5);
            Console.WriteLine($"  {containsFive}");

            // Example 18: Count - Count elements
            Console.WriteLine("\n18. Count - Number of people from New York:");
            var nyCount = people.Count(p => p.City == "New York");
            Console.WriteLine($"  {nyCount}");

            // Example 19: Sum - Sum numeric values
            Console.WriteLine("\n19. Sum - Total of all salaries:");
            var totalSalaries = people.Sum(p => p.Salary);
            Console.WriteLine($"  ${totalSalaries:N0}");

            // Example 20: Average - Calculate average
            Console.WriteLine("\n20. Average - Average age:");
            var avgAge = people.Average(p => p.Age);
            Console.WriteLine($"  {avgAge:F1} years");

            // Example 21: Min - Find minimum
            Console.WriteLine("\n21. Min - Youngest age:");
            var minAge = people.Min(p => p.Age);
            Console.WriteLine($"  {minAge} years");

            // Example 22: Max - Find maximum
            Console.WriteLine("\n22. Max - Highest salary:");
            var maxSalary = people.Max(p => p.Salary);
            Console.WriteLine($"  ${maxSalary:N0}");

            // Example 23: Aggregate - Custom aggregation
            Console.WriteLine("\n23. Aggregate - Concatenate names:");
            var allNames = names.Take(3).Aggregate((acc, name) => acc + ", " + name);
            Console.WriteLine($"  {allNames}");

            // Example 24: Distinct - Remove duplicates
            Console.WriteLine("\n24. Distinct - Unique cities:");
            var cities = people.Select(p => p.City).Distinct();
            Console.WriteLine($"  {string.Join(", ", cities)}");

            // Example 25: Union - Combine collections (distinct)
            Console.WriteLine("\n25. Union - Combine two number sets:");
            var set1 = new[] { 1, 2, 3, 4 };
            var set2 = new[] { 3, 4, 5, 6 };
            var union = set1.Union(set2);
            Console.WriteLine($"  {string.Join(", ", union)}");

            // Example 26: Intersect - Common elements
            Console.WriteLine("\n26. Intersect - Common numbers:");
            var intersection = set1.Intersect(set2);
            Console.WriteLine($"  {string.Join(", ", intersection)}");

            // Example 27: Except - Difference
            Console.WriteLine("\n27. Except - Numbers in set1 but not in set2:");
            var difference = set1.Except(set2);
            Console.WriteLine($"  {string.Join(", ", difference)}");

            // Example 28: Take - Take first N elements
            Console.WriteLine("\n28. Take - First 3 numbers:");
            var firstThree = numbers.Take(3);
            Console.WriteLine($"  {string.Join(", ", firstThree)}");

            // Example 29: Skip - Skip first N elements
            Console.WriteLine("\n29. Skip - Skip first 5 numbers:");
            var skipFive = numbers.Skip(5);
            Console.WriteLine($"  {string.Join(", ", skipFive)}");

            // Example 30: TakeWhile - Take while condition is true
            Console.WriteLine("\n30. TakeWhile - Take while less than 6:");
            var takeWhile = numbers.TakeWhile(n => n < 6);
            Console.WriteLine($"  {string.Join(", ", takeWhile)}");

            // Example 31: SkipWhile - Skip while condition is true
            Console.WriteLine("\n31. SkipWhile - Skip while less than 6:");
            var skipWhile = numbers.SkipWhile(n => n < 6);
            Console.WriteLine($"  {string.Join(", ", skipWhile)}");

            // Example 32: Reverse - Reverse order
            Console.WriteLine("\n32. Reverse - Numbers in reverse:");
            var reversed = numbers.Reverse();
            Console.WriteLine($"  {string.Join(", ", reversed)}");

            // Example 33: Zip - Combine two sequences
            Console.WriteLine("\n33. Zip - Combine numbers and names:");
            var zipped = numbers.Take(3).Zip(names, (n, name) => $"{n}:{name}");
            Console.WriteLine($"  {string.Join(", ", zipped)}");

            // Example 34: ToDictionary - Convert to dictionary
            Console.WriteLine("\n34. ToDictionary - People indexed by Id:");
            var peopleDict = people.Take(3).ToDictionary(p => p.Id, p => p.Name);
            Console.WriteLine($"  {string.Join(", ", peopleDict.Select(kvp => $"{kvp.Key}={kvp.Value}"))}");

            // Example 35: ToLookup - Create lookup (one-to-many)
            Console.WriteLine("\n35. ToLookup - People grouped by age:");
            var ageLookup = people.ToLookup(p => p.Age);
            foreach (var ageGroup in ageLookup.Take(2))
                Console.WriteLine($"  Age {ageGroup.Key}: {string.Join(", ", ageGroup.Select(p => p.Name))}");

            // Example 36: Range - Generate sequence
            Console.WriteLine("\n36. Range - Generate numbers 1-5:");
            var range = Enumerable.Range(1, 5);
            Console.WriteLine($"  {string.Join(", ", range)}");

            // Example 37: Repeat - Repeat element
            Console.WriteLine("\n37. Repeat - Repeat 'X' 5 times:");
            var repeated = Enumerable.Repeat("X", 5);
            Console.WriteLine($"  {string.Join(", ", repeated)}");

            // Example 38: Empty - Empty sequence
            Console.WriteLine("\n38. Empty - Create empty int sequence:");
            var empty = Enumerable.Empty<int>();
            Console.WriteLine($"  Count: {empty.Count()}");

            // Example 39: DefaultIfEmpty - Provide default for empty sequence
            Console.WriteLine("\n39. DefaultIfEmpty - Empty sequence with default:");
            var emptyWithDefault = empty.DefaultIfEmpty(-1);
            Console.WriteLine($"  {string.Join(", ", emptyWithDefault)}");

            // Example 40: OfType - Filter by type
            Console.WriteLine("\n40. OfType - Filter mixed collection:");
            object[] mixed = { 1, "two", 3, "four", 5 };
            var onlyInts = mixed.OfType<int>();
            Console.WriteLine($"  Integers: {string.Join(", ", onlyInts)}");

            // Example 41: Cast - Cast all elements
            Console.WriteLine("\n41. Cast - Cast object array to int:");
            object[] objNumbers = { 1, 2, 3 };
            var castedInts = objNumbers.Cast<int>();
            Console.WriteLine($"  {string.Join(", ", castedInts)}");

            // Example 42: Chunk - Split into chunks (C# 10+)
            Console.WriteLine("\n42. Chunk - Split numbers into chunks of 3:");
            var chunks = numbers.Chunk(3);
            foreach (var chunk in chunks)
                Console.WriteLine($"  [{string.Join(", ", chunk)}]");

            // Example 43: DistinctBy - Distinct by property (C# 10+)
            Console.WriteLine("\n43. DistinctBy - Distinct people by age:");
            var distinctByAge = people.DistinctBy(p => p.Age);
            foreach (var p in distinctByAge.Take(4))
                Console.WriteLine($"  {p.Name} ({p.Age})");

            // Example 44: MinBy - Find element with minimum property (C# 10+)
            Console.WriteLine("\n44. MinBy - Person with lowest salary:");
            var minBySalary = people.MinBy(p => p.Salary);
            Console.WriteLine($"  {minBySalary?.Name}: ${minBySalary?.Salary:N0}");

            // Example 45: MaxBy - Find element with maximum property (C# 10+)
            Console.WriteLine("\n45. MaxBy - Person with highest salary:");
            var maxBySalary = people.MaxBy(p => p.Salary);
            Console.WriteLine($"  {maxBySalary?.Name}: ${maxBySalary?.Salary:N0}");

            // Example 46: Complex query - Method syntax
            Console.WriteLine("\n46. Complex query - People from NY or London, age > 25, ordered by salary:");
            var complexQuery = people
                .Where(p => (p.City == "New York" || p.City == "London") && p.Age > 25)
                .OrderBy(p => p.Salary)
                .Select(p => new { p.Name, p.City, p.Age, p.Salary });
            foreach (var p in complexQuery)
                Console.WriteLine($"  {p.Name} ({p.City}): ${p.Salary:N0}");

            // Example 47: Query syntax
            Console.WriteLine("\n47. Query syntax - Same as above:");
            var queryExpression = from p in people
                                  where (p.City == "New York" || p.City == "London") && p.Age > 25
                                  orderby p.Salary
                                  select new { p.Name, p.City, p.Age, p.Salary };
            foreach (var p in queryExpression)
                Console.WriteLine($"  {p.Name} ({p.City}): ${p.Salary:N0}");

            // Example 48: Let clause in query syntax
            Console.WriteLine("\n48. Let clause - Calculate bonus:");
            var withBonus = from p in people
                           let bonus = p.Salary * 0.1m
                           select new { p.Name, p.Salary, Bonus = bonus };
            foreach (var p in withBonus.Take(3))
                Console.WriteLine($"  {p.Name}: Salary ${p.Salary:N0}, Bonus ${p.Bonus:N0}");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nPress Enter to exit...");
            Console.ResetColor();
        }

        private class Person
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public int Age { get; set; }
            public string City { get; set; } = string.Empty;
            public decimal Salary { get; set; }
        }

        private class Order
        {
            public int Id { get; set; }
            public int PersonId { get; set; }
            public string Product { get; set; } = string.Empty;
            public decimal Amount { get; set; }
        }
    }
}
