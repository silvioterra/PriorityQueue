using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericStructures
{
    class PriorityQueue<T>
    // If we wanted to just dump the specific priority, we could just make T have to implement IComparable and we'd be good
    // where T : IComparable
    {
        private MinMaxHeap<T> mHeapImpl;
        private bool mDebugTrace = false;

        public PriorityQueue()
        {
            mHeapImpl = new MinMaxHeap<T>();

        }

        /* *****************************************************************************
         * TEST INTERFACE IMPLEMENTATION
         * Here are the ArenaNet functions!
         * 
         * 
         ***************************************************************************** */
        // enqueue item into the PriorityQueue with the given priority
        public bool Enqueue(T item, int priority)
        {
            // NOTE! passing in a priority really shouldn't be needed... the T can be made  IComparable...
            bool success = true;
            mHeapImpl.Add(item, priority);
            return success;
        }
        // dequeue the element with minimum priority
        public T DequeueMin()
        {
            return mHeapImpl.RemoveMin();
        }
        // dequeue the element with maximum priority
        public T DequeueMax()
        {
            return mHeapImpl.RemoveMax();
        }


        // And my own debug/utility functions

        // Returns how many items there are in the queue
        public int Count
        {
            get { return mHeapImpl.Count; }
        }

        public bool DebugTrace
        {
            get
            {
                return mDebugTrace;
            }

            set
            {
                mDebugTrace = value;
                mHeapImpl.DebugTraces = value;
            }
        }

        public int DebugCount
        {
            get
            {
                return mHeapImpl.DebugCount;
            }
        }

        public bool DebugValidation { get { return mHeapImpl.DebugCheckHeapProperty; } internal set { mHeapImpl.DebugCheckHeapProperty = value; } }
    }

}
