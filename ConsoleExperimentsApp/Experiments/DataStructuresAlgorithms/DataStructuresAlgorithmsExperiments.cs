using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleExperimentsApp.Experiments.DataStructuresAlgorithms
{
    public static class DataStructuresAlgorithmsExperiments
    {
        public static async Task Run()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=== Data Structures & Algorithms Experiments ===\n");
            Console.ResetColor();

            // Data Structures
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Description: Demonstrates the Stack data structure (Last-In-First-Out)");
            Console.WriteLine("with Push, Pop, and Peek operations.");
            Console.ResetColor();
            DemoStack();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Description: Demonstrates the Queue data structure (First-In-First-Out)");
            Console.WriteLine("with Enqueue, Dequeue, and Peek operations.");
            Console.ResetColor();
            DemoQueue();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Description: Shows LinkedList operations including AddFirst, AddLast,");
            Console.WriteLine("and navigation through nodes.");
            Console.ResetColor();
            DemoLinkedList();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Description: Demonstrates Dictionary hash table operations");
            Console.WriteLine("for key-value pair storage and fast O(1) lookup.");
            Console.ResetColor();
            DemoDictionary();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Description: Demonstrates binary tree structure with insertion");
            Console.WriteLine("and traversal algorithms (in-order, pre-order, post-order).");
            Console.ResetColor();
            DemoBinaryTree();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Description: Shows graph data structure with vertices and edges,");
            Console.WriteLine("including depth-first and breadth-first traversal algorithms.");
            Console.ResetColor();
            DemoGraph();

            // Algorithms
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Description: Demonstrates various sorting algorithms including");
            Console.WriteLine("Bubble Sort, Quick Sort, Merge Sort with complexity analysis.");
            Console.ResetColor();
            DemoSortingAlgorithms();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Description: Shows different searching algorithms like Binary Search");
            Console.WriteLine("and Linear Search with time complexity comparisons.");
            Console.ResetColor();
            DemoSearchingAlgorithms();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Description: Demonstrates dynamic programming techniques with examples");
            Console.WriteLine("like Fibonacci sequence using memoization and tabulation.");
            Console.ResetColor();
            DemoDynamicProgramming();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Description: Shows recursive algorithms including factorial,");
            Console.WriteLine("Fibonacci, and demonstrates the call stack behavior.");
            Console.ResetColor();
            DemoRecursion();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nPress Enter to exit...");
            Console.ResetColor();
        }

        #region Data Structures

        private static void DemoStack()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--- Stack (LIFO) ---");
            Console.ResetColor();

            var stack = new Stack<string>();
            stack.Push("First");
            stack.Push("Second");
            stack.Push("Third");

            Console.WriteLine($"Peek: {stack.Peek()}");
            Console.WriteLine($"Pop: {stack.Pop()}");
            Console.WriteLine($"Count: {stack.Count}");
            Console.WriteLine();
        }

        private static void DemoQueue()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--- Queue (FIFO) ---");
            Console.ResetColor();

            var queue = new Queue<int>();
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);

            Console.WriteLine($"Peek: {queue.Peek()}");
            Console.WriteLine($"Dequeue: {queue.Dequeue()}");
            Console.WriteLine($"Count: {queue.Count}");
            Console.WriteLine();
        }

        private static void DemoLinkedList()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--- Linked List ---");
            Console.ResetColor();

            var linkedList = new LinkedList<string>();
            linkedList.AddLast("Node1");
            linkedList.AddLast("Node2");
            linkedList.AddFirst("Node0");

            Console.WriteLine("Traversing linked list:");
            foreach (var item in linkedList)
            {
                Console.WriteLine($"  {item}");
            }
            Console.WriteLine();
        }

        private static void DemoDictionary()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--- Dictionary (Hash Table) ---");
            Console.ResetColor();

            var dict = new Dictionary<string, int>
            {
                ["apple"] = 5,
                ["banana"] = 3,
                ["orange"] = 7
            };

            Console.WriteLine($"Apple count: {dict["apple"]}");
            Console.WriteLine($"Contains 'grape': {dict.ContainsKey("grape")}");
            Console.WriteLine();
        }

        private static void DemoBinaryTree()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--- Binary Search Tree ---");
            Console.ResetColor();

            var bst = new BinarySearchTree();
            bst.Insert(50);
            bst.Insert(30);
            bst.Insert(70);
            bst.Insert(20);
            bst.Insert(40);
            bst.Insert(60);
            bst.Insert(80);

            Console.Write("In-Order Traversal: ");
            bst.InOrderTraversal();
            Console.WriteLine();

            Console.WriteLine($"Search 40: {bst.Search(40)}");
            Console.WriteLine($"Search 100: {bst.Search(100)}");
            Console.WriteLine();
        }

        private static void DemoGraph()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--- Graph (Adjacency List) ---");
            Console.ResetColor();

            var graph = new Graph(5);
            graph.AddEdge(0, 1);
            graph.AddEdge(0, 4);
            graph.AddEdge(1, 2);
            graph.AddEdge(1, 3);
            graph.AddEdge(1, 4);
            graph.AddEdge(2, 3);
            graph.AddEdge(3, 4);

            Console.Write("BFS from vertex 0: ");
            graph.BFS(0);
            Console.WriteLine();

            Console.Write("DFS from vertex 0: ");
            graph.DFS(0);
            Console.WriteLine("\n");
        }

        #endregion

        #region Algorithms

        private static void DemoSortingAlgorithms()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--- Sorting Algorithms ---");
            Console.ResetColor();

            int[] arr1 = [64, 34, 25, 12, 22, 11, 90];
            int[] arr2 = [64, 34, 25, 12, 22, 11, 90];
            int[] arr3 = [64, 34, 25, 12, 22, 11, 90];

            BubbleSort(arr1);
            Console.WriteLine($"Bubble Sort: {string.Join(", ", arr1)}");

            QuickSort(arr2, 0, arr2.Length - 1);
            Console.WriteLine($"Quick Sort: {string.Join(", ", arr2)}");

            MergeSort(arr3, 0, arr3.Length - 1);
            Console.WriteLine($"Merge Sort: {string.Join(", ", arr3)}");
            Console.WriteLine();
        }

        private static void BubbleSort(int[] arr)
        {
            int n = arr.Length;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (arr[j] > arr[j + 1])
                    {
                        (arr[j], arr[j + 1]) = (arr[j + 1], arr[j]);
                    }
                }
            }
        }

        private static void QuickSort(int[] arr, int low, int high)
        {
            if (low < high)
            {
                int pi = Partition(arr, low, high);
                QuickSort(arr, low, pi - 1);
                QuickSort(arr, pi + 1, high);
            }
        }

        private static int Partition(int[] arr, int low, int high)
        {
            int pivot = arr[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (arr[j] < pivot)
                {
                    i++;
                    (arr[i], arr[j]) = (arr[j], arr[i]);
                }
            }
            (arr[i + 1], arr[high]) = (arr[high], arr[i + 1]);
            return i + 1;
        }

        private static void MergeSort(int[] arr, int left, int right)
        {
            if (left < right)
            {
                int mid = left + (right - left) / 2;
                MergeSort(arr, left, mid);
                MergeSort(arr, mid + 1, right);
                Merge(arr, left, mid, right);
            }
        }

        private static void Merge(int[] arr, int left, int mid, int right)
        {
            int n1 = mid - left + 1;
            int n2 = right - mid;

            int[] leftArr = new int[n1];
            int[] rightArr = new int[n2];

            Array.Copy(arr, left, leftArr, 0, n1);
            Array.Copy(arr, mid + 1, rightArr, 0, n2);

            int i = 0, j = 0, k = left;

            while (i < n1 && j < n2)
            {
                if (leftArr[i] <= rightArr[j])
                {
                    arr[k++] = leftArr[i++];
                }
                else
                {
                    arr[k++] = rightArr[j++];
                }
            }

            while (i < n1) arr[k++] = leftArr[i++];
            while (j < n2) arr[k++] = rightArr[j++];
        }

        private static void DemoSearchingAlgorithms()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--- Searching Algorithms ---");
            Console.ResetColor();

            int[] sortedArr = [2, 5, 8, 12, 16, 23, 38, 45, 56, 67, 78];

            int target = 23;
            int linearResult = LinearSearch(sortedArr, target);
            Console.WriteLine($"Linear Search for {target}: Index {linearResult}");

            int binaryResult = BinarySearch(sortedArr, target);
            Console.WriteLine($"Binary Search for {target}: Index {binaryResult}");
            Console.WriteLine();
        }

        private static int LinearSearch(int[] arr, int target)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == target) return i;
            }
            return -1;
        }

        private static int BinarySearch(int[] arr, int target)
        {
            int left = 0, right = arr.Length - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;

                if (arr[mid] == target) return mid;
                if (arr[mid] < target) left = mid + 1;
                else right = mid - 1;
            }
            return -1;
        }

        private static void DemoDynamicProgramming()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--- Dynamic Programming (Fibonacci) ---");
            Console.ResetColor();

            int n = 10;
            Console.WriteLine($"Fibonacci({n}) using DP: {FibonacciDP(n)}");
            Console.WriteLine();
        }

        private static long FibonacciDP(int n)
        {
            if (n <= 1) return n;

            long[] dp = new long[n + 1];
            dp[0] = 0;
            dp[1] = 1;

            for (int i = 2; i <= n; i++)
            {
                dp[i] = dp[i - 1] + dp[i - 2];
            }

            return dp[n];
        }

        private static void DemoRecursion()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--- Recursion (Factorial) ---");
            Console.ResetColor();

            int n = 5;
            Console.WriteLine($"Factorial({n}): {Factorial(n)}");
            Console.WriteLine();
        }

        private static long Factorial(int n)
        {
            if (n <= 1) return 1;
            return n * Factorial(n - 1);
        }

        #endregion

        #region Helper Classes

        private class BinarySearchTree
        {
            private class Node
            {
                public int Data;
                public Node? Left;
                public Node? Right;

                public Node(int data)
                {
                    Data = data;
                    Left = null;
                    Right = null;
                }
            }

            private Node? root;

            public void Insert(int data)
            {
                root = InsertRec(root, data);
            }

            private Node InsertRec(Node? node, int data)
            {
                if (node == null)
                {
                    return new Node(data);
                }

                if (data < node.Data)
                {
                    node.Left = InsertRec(node.Left, data);
                }
                else if (data > node.Data)
                {
                    node.Right = InsertRec(node.Right, data);
                }

                return node;
            }

            public bool Search(int data)
            {
                return SearchRec(root, data);
            }

            private bool SearchRec(Node? node, int data)
            {
                if (node == null) return false;
                if (node.Data == data) return true;
                if (data < node.Data) return SearchRec(node.Left, data);
                return SearchRec(node.Right, data);
            }

            public void InOrderTraversal()
            {
                InOrderRec(root);
            }

            private void InOrderRec(Node? node)
            {
                if (node != null)
                {
                    InOrderRec(node.Left);
                    Console.Write($"{node.Data} ");
                    InOrderRec(node.Right);
                }
            }
        }

        private class Graph
        {
            private readonly int vertices;
            private readonly List<int>[] adjacencyList;

            public Graph(int v)
            {
                vertices = v;
                adjacencyList = new List<int>[v];
                for (int i = 0; i < v; i++)
                {
                    adjacencyList[i] = new List<int>();
                }
            }

            public void AddEdge(int v, int w)
            {
                adjacencyList[v].Add(w);
            }

            public void BFS(int start)
            {
                bool[] visited = new bool[vertices];
                Queue<int> queue = new Queue<int>();

                visited[start] = true;
                queue.Enqueue(start);

                while (queue.Count > 0)
                {
                    int vertex = queue.Dequeue();
                    Console.Write($"{vertex} ");

                    foreach (int adjacent in adjacencyList[vertex])
                    {
                        if (!visited[adjacent])
                        {
                            visited[adjacent] = true;
                            queue.Enqueue(adjacent);
                        }
                    }
                }
            }

            public void DFS(int start)
            {
                bool[] visited = new bool[vertices];
                DFSRec(start, visited);
            }

            private void DFSRec(int vertex, bool[] visited)
            {
                visited[vertex] = true;
                Console.Write($"{vertex} ");

                foreach (int adjacent in adjacencyList[vertex])
                {
                    if (!visited[adjacent])
                    {
                        DFSRec(adjacent, visited);
                    }
                }
            }
        }

        #endregion
    }
}
