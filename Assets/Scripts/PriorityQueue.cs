using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class Node<T>
    {
        public Node(T unit, int priority)
        {
            Unit = unit;
            Priority = priority;
        }

        public T Unit { get; set; }
        public int Priority { get; set; }
        public int G { get; set; }
        public int H { get; set; }
        public int F { get => G + H; }
    }

    public class PriorityQueue<T>
    {
        public List<Node<T>> Queue = new List<Node<T>>();
        int size = -1;
        public int Length { get => Queue.Count; }
        private int leftChild(int i) => (i * 2 + 1);
        private int rightChild(int i) => (i * 2 + 2);

        public void Enqueue(Node<T> node)
        {
            Queue.Add(node);
            size += 1;
            BuildMinHeap(size);
        }

        private void BuildMinHeap(int i) {
            // (i - 1) / 2 parrent index
            // i current index
            while (i >= 0 && Queue[(i - 1) / 2].Priority > Queue[i].Priority)
            {
                Swap(i, (i - 1) / 2);
                i = (i - 1) / 2;
            }
        }

        private void MinHeapify(int i) {
            int leftChild = this.leftChild(i);
            int rightChild = this.rightChild(i);

            int lowest = i;

            if (leftChild <= size && Queue[lowest].Priority > Queue[leftChild].Priority)
                lowest = leftChild;
            if (rightChild <= size && Queue[lowest].Priority > Queue[rightChild].Priority)
                lowest = rightChild;

            if (lowest != i)
            {
                Swap(lowest, i);
                MinHeapify(lowest);
            }
        }


        public T Dequeue()
        {
            if (size > -1)
            {
                var min = Queue[0].Unit;
                Queue[0] = Queue[size];
                Queue.RemoveAt(size);
                size--;
                MinHeapify(0);
                return min;
            }
            else
                throw new Exception("Empty QUEUE detected :////");
        }

        private void Swap(int source, int destination)
        {
            var temporary = Queue[destination];
            Queue[destination] = Queue[source];
            Queue[source] = temporary;
        }
    }
}
