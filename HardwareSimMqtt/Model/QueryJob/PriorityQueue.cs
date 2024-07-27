using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareSimMqtt.Model.QueryJob
{
    public class PriorityQueue<T>
    {
        private SortedDictionary<int, Queue<T>> queueSortedMap
        {
            get;
            set;
        }

        private int totalCount
        { 
            get; 
            set; 
        }

        public PriorityQueue()
        {
            queueSortedMap = new SortedDictionary<int, Queue<T>>();
            totalCount = 0;
        }

        public void Enqueue(T item, int priority)
        {
            if (!queueSortedMap.ContainsKey(priority))
            {
                queueSortedMap[priority] = new Queue<T>();
            }
            queueSortedMap[priority].Enqueue(item);
            totalCount++;
        }

        public T Dequeue()
        {
            if (queueSortedMap.Count == 0)
            {
                throw new InvalidOperationException("The priority queue is empty.");
            }

            var firstKey = GetFirstKey();
            var queue = queueSortedMap[firstKey];
            var item = queue.Dequeue();
            totalCount--;

            if (queue.Count == 0)
            {
                queueSortedMap.Remove(firstKey);
            }

            return item;
        }

        public T Peek()
        {
            if (queueSortedMap.Count == 0)
            {
                throw new InvalidOperationException("The priority queue is empty.");
            }

            var firstKey = GetFirstKey();
            return queueSortedMap[firstKey].Peek();
        }

        public int Count
        {
            get { return totalCount; }
        }

        public bool IsEmpty
        {
            get { return queueSortedMap.Count == 0; }
        }

        private int GetFirstKey()
        {
            using (var enumerator = queueSortedMap.Keys.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    return enumerator.Current;
                }
            }
            throw new InvalidOperationException("The priority queue is empty.");
        }

        public override string ToString()
        {
            var result = new List<string>();
            foreach (var kvp in queueSortedMap)
            {
                result.Add($"Priority {kvp.Key}: [{string.Join(", ", kvp.Value)}]");
            }
            return string.Join(", ", result);
        }
    }
}
