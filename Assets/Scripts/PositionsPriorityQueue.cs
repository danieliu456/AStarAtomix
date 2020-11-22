using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class PositionsPriorityQueue
    {
        public List<Positions> Queue = new List<Positions>();
        int size = -1;
        public int Length { get => Queue.Count; }
        private int leftChild(int i) => (i * 2 + 1);
        private int rightChild(int i) => (i * 2 + 2);

        public void Enqueue(Positions positions)
        {
            Queue.Add(positions);
            size += 1;
            BuildMinHeap(size);
        }

        private void BuildMinHeap(int i) {
            // (i - 1) / 2 parrent index
            // i current index
            while (i >= 0 && Queue[(i - 1) / 2].F > Queue[i].F)
            {
                Swap(i, (i - 1) / 2);
                i = (i - 1) / 2;
            }
        }

        private void MinHeapify(int i) {
            int leftChild = this.leftChild(i);
            int rightChild = this.rightChild(i);

            int lowest = i;

            if (leftChild <= size && Queue[lowest].F > Queue[leftChild].F)
                lowest = leftChild;
            if (rightChild <= size && Queue[lowest].F > Queue[rightChild].F)
                lowest = rightChild;

            if (lowest != i)
            {
                Swap(lowest, i);
                MinHeapify(lowest);
            }
        }


        public Positions Dequeue()
        {
            if (size > -1)
            {
                var min = Queue[0];
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

        public void UpdatePriority(Positions positions)
        {
            for (int i = 0; i < Queue.Count; i++)
            {
                if (positions.Equals(Queue[i]))
                {
                    Queue[i] = positions;
                    BuildMinHeap(i);
                    MinHeapify(i);
                }
            }
        }

        public Positions Exists(Positions obj) {
            foreach (var position in Queue)
            {
                if (position.Equals(obj))
                    return position;
            }
            return null;
        }
    }
}
