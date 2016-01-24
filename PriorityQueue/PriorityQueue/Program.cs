using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericStructures
{
    class Program
    {
        static void Main(string[] args)
        {
            // Test the heap (actual implementation)
            //HeapTest();

            // Test the Priority Queue that uses the MinMaxHeap, tested above
            PriorityQueueTest();
        }
        static void PriorityQueueTest()
        {
            { 
                PriorityQueue<int> queueRemoveMin = new PriorityQueue<int>();
                PriorityQueue<int> queueRemoveMax = new PriorityQueue<int>();
                List<int> doubleCheckMin = new List<int>();
                List<int> doubleCheckMax = new List<int>();
                Random r = new Random();
                // Generate the list of numbers to populate the queue and to check against
                for (int i = 0; i < 20; i++)
                {
                    int randInt = r.Next(-100, 100);
                    doubleCheckMin.Add(randInt);
                }

                for (int i = 0; i < doubleCheckMin.Count; i++)
                {
                    int randInt = doubleCheckMin[i];
                    // heap.Add("" + i, i);
                    queueRemoveMin.Enqueue(randInt, randInt);
                    queueRemoveMax.Enqueue(randInt, randInt);
                    doubleCheckMax.Add(randInt);
                }
                doubleCheckMin.Sort(); // Default. Ascending
                doubleCheckMax.Sort(delegate (int x, int y)
                {
                    if (x == y) return 0;
                    if (x > y) return -1;
                    if (x < y) return 1;
                    return 0;
                });

                Console.WriteLine(" -- NOW REMOVE MIN --");

                int checkCount = 0;
                while (queueRemoveMin.Count > 0)
                {
                    int min = queueRemoveMin.DequeueMin();
                    if (doubleCheckMin[checkCount] != min)
                    {
                        throw new Exception("WRONG!");
                    }
                    checkCount++;
                    Console.WriteLine(min);
                }
                Console.WriteLine(" -- NOW REMOVE MAX --");
                checkCount = 0;
                while (queueRemoveMax.Count > 0)
                {
                    int max = queueRemoveMax.DequeueMax();
                    if (doubleCheckMax[checkCount] != max)
                    {
                        throw new Exception("WRONG!");
                    }
                    checkCount++;
                    Console.WriteLine(max);
                }
            }


            // Now for some random fun. Randomly decide what operation we're performing. 
            // Sorted list is kept alongside for double-checking validity of heap results.
            {
                PriorityQueue<int> queue = new PriorityQueue<int>();
                queue.DebugValidation = true;
                List<int> list = new List<int>();

                const int kMaxOperations = 2000;
                int numOps = 0;
                Random r = new Random();
                for (numOps = 0; numOps < kMaxOperations; numOps++)
                {
                    int randInt = r.Next(0, 4);
                    switch (randInt)
                    {
                        case 0:
                        case 1: // twice as likely to occur
                            {
                                // Add an item.
                                randInt = r.Next(-1000, 1000);
                                Console.WriteLine("Adding : " + randInt);
                                list.Add(randInt);
                                queue.Enqueue(randInt, randInt);
                                if (list.Count != queue.Count)
                                {
                                    throw new Exception("Count mismatch!");
                                }
                            }
                            break;
                        case 2:
                            {
                                // Dequeue Min
                                list.Sort();
                                if (list.Count != queue.Count)
                                {
                                    throw new Exception("Count mismatch! List= " + list.Count + ", queue = " + queue.Count);
                                }
                                if (list.Count == 0)
                                {
                                    // well, can't do much here. early break
                                    break;
                                }
                                int listMin = list[0];
                                list.RemoveAt(0);
                                int queueMin = queue.DequeueMin();
                                if (listMin != queueMin)
                                {
                                    throw new Exception("Min mismatch! List=" + listMin + ", queue=" + queueMin);
                                }
                                Console.WriteLine("DequeueMin : " + queueMin);
                            }

                            break;
                        case 3:
                            {
                                // DequeueMax
                                list.Sort(delegate (int x, int y)
                                {
                                    if (x == y) return 0;
                                    if (x > y) return -1;
                                    if (x < y) return 1;
                                    return 0;
                                });
                                if (list.Count != queue.Count)
                                {
                                    throw new Exception("Count mismatch! List= " + list.Count + ", queue = " + queue.Count);
                                }
                                if (list.Count == 0)
                                {
                                    // well, can't do much here. early break
                                    break;
                                }
                                int listMax = list[0];
                                list.RemoveAt(0);
                                int queueMax = queue.DequeueMax();
                                if (listMax != queueMax)
                                {
                                    throw new Exception("Max mismatch! List=" + listMax + ", queue=" + queueMax);
                                }
                                Console.WriteLine("DequeueMax : " + queueMax);
                            }
                            break;
                    }
                }
                Console.WriteLine("All tests passed!");

            }

        }
        static void HeapTest()
        { 
            MinMaxHeap<int> heapRemoveMin = new MinMaxHeap<int>();
            heapRemoveMin.DebugCheckHeapProperty = true;
            MinMaxHeap<int> heapRemoveMax = new MinMaxHeap<int>();
            heapRemoveMax.DebugCheckHeapProperty = true;
            List<int> doubleCheckMin = new List<int>();
            List<int> doubleCheckMax = new List<int>();
            Random r = new Random();

            // Generate the list of numbers to populate the heaps and to check against
            if (true)
            {
                for (int i = 0; i < 700; i++)
                {
                    int randInt = r.Next(-10000, 10000);
                    doubleCheckMin.Add(randInt);
                }
            }
            else
            {
                // Manually test degenerate cases
                doubleCheckMin.Add(-55);
                doubleCheckMin.Add(31);
                doubleCheckMin.Add(-93);
                //doubleCheckMin.Add(64);
            }

            for (int i = 0; i < doubleCheckMin.Count; i++)
            {
                int randInt = doubleCheckMin[i];
                // heap.Add("" + i, i);
                heapRemoveMin.Add(randInt, randInt);
                heapRemoveMax.Add(randInt, randInt);
                doubleCheckMax.Add(randInt);
            }
            doubleCheckMin.Sort(); // Default. Ascending
            doubleCheckMax.Sort(delegate (int x, int y)
            {
                if (x == y) return 0;
                if (x > y) return -1;
                if (x < y) return 1;
                return 0;
            });
            Console.WriteLine(" -- NOW REMOVE MIN --");
            int checkCount = 0;
            while (heapRemoveMin.Count > 0)
            {
                int min = heapRemoveMin.FindMin();
                if (doubleCheckMin[checkCount] != min)
                {
                    throw new Exception("WRONG!");
                }
                heapRemoveMin.RemoveMin();
                checkCount++;
                Console.WriteLine(min);
            }
            Console.WriteLine(" -- NOW REMOVE MAX --");
            checkCount = 0;
            while (heapRemoveMax.Count > 0)
            {
                //Console.WriteLine("iteration " + checkCount);
                //Console.WriteLine(heapRemoveMax.PrintTree());
                int max = heapRemoveMax.RemoveMax();
                if (doubleCheckMax[checkCount] != max)
                {
                    throw new Exception("WRONG!");
                }
                checkCount++;
                Console.WriteLine(max);
            }
        }
    }
}
