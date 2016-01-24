using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericStructures
{
    // Levels start as min levels, then alternate.
    // Min should always be root,
    // max will always be one of the min's two children.
    class MinMaxHeap<T>
    {

        enum LevelType
        {
            Min,
            Max
        }

        class Node
        {
            T mData;
            int mPriority;

            public Node(T item, int priority)
            {
                mData = item;
                mPriority = priority;
            }

            public int Priority
            {
                get
                {
                    return mPriority;
                }
            }

            internal void SetData(T item)
            {
                mData = item;
            }

            public override string ToString()
            {
                return "(" + mPriority + ") : " + mData.ToString();
            }

            internal void UpdateData(Node other)
            {
                mData = other.mData;
                mPriority = other.mPriority;
            }

            internal void UpdateData(T item, int priority)
            {
                mData = item;
                mPriority = priority;
            }

            public T Data
            {
                get { return mData; }
            }

            internal void Swap(Node node)
            {
                T tmpData = this.mData;
                int tmpPriority = this.mPriority;
                this.mData = node.mData;
                this.mPriority = node.mPriority;
                node.mData = tmpData;
                node.mPriority = tmpPriority;
            }
        }


        public bool DebugTraces = false;
        public int Count
        {
            get { return mCount; }
        }
        public int DebugCount
        {
            get { return mDebugCount; }
        }

        private int mCount;
        private int mDebugCount;

        // How low-level are we taking this implementation? Do I have to handle array resizing, etc, or is it
        // OK to use an automatically-resizing array for this?
        // Assuming the test is not trying to determine if I know how to increase the size of an array and move the contents
        // over. 
        // Note: after I finished the implementation, I wish I had used a plain array, to be honest.
        System.Collections.ArrayList mNodes;

        // If set to true, we'll make sure the heap is still valid every Remove operation. Debug-only.
        public bool DebugCheckHeapProperty;

        public MinMaxHeap()
        {
            mNodes = new System.Collections.ArrayList();
            mNodes.Add(new Node(default(T), 0)); // the 0-th element is not counted. Makes rest of math easier as well.
        }

        public void Add(T item, int priority)
        {
            if (mNodes.Count == 1)
            {
                mNodes.Add(new Node(item, priority));
                mCount++;
                return;
            }

            int arrIdx = mNodes.Count;
            int parentIdx = arrIdx / 2;
            mNodes.Add(new Node(item, priority));
            mCount++;
            Node iter = (Node)mNodes[arrIdx];
            Node parent = (Node)mNodes[parentIdx];
            int comparison = NodeCompare(iter, parent);

            switch (Level(parentIdx))
            {
                case LevelType.Min:
                    if (comparison < 0)
                    {
                        AssignData(arrIdx, parentIdx);

                        VerifyMin(parentIdx, item, priority);
                    }
                    else
                    {
                        VerifyMax(arrIdx, item, priority);
                    }
                    break;
                case LevelType.Max:
                default: //Just for safety. There should not be a need for this
                    if (comparison > 0)
                    {
                        AssignData(arrIdx, parentIdx);
                        VerifyMax(parentIdx, item, priority);
                    }
                    else
                    {
                        VerifyMin(arrIdx, item, priority);
                    }
                    break;
            }
        }

        private void AssignData(int dest, int source)
        {
            ((Node)mNodes[dest]).UpdateData((Node)mNodes[source]);
        }

        private void VerifyMax(int arrIdx, T item, int priority)
        {
            int grandparentIdx = arrIdx / 4;

            while (grandparentIdx > 0)
            {
                // TODO if it's the same priority, update grandparent's data
                // stop going up the tree and fix up the tree from this node DOWNWARD
                if (NodePriorityCompare(priority, ((Node)mNodes[grandparentIdx]).Priority) > 0)
                {
                    AssignData(arrIdx, grandparentIdx);
                    arrIdx = grandparentIdx;
                    grandparentIdx /= 4;
                }
                else
                {
                    break;
                }
            }
            ((Node)mNodes[arrIdx]).UpdateData(item, priority);
        }

        private void VerifyMin(int arrIdx, T item, int priority)
        {
            int grandparentIdx = arrIdx / 4;

            while (grandparentIdx > 0)
            {
                // TODO if it's the same priority, update grandparent's data
                // stop going up the tree and fix up the tree from this node DOWNWARD
                if (NodePriorityCompare(priority, ((Node)mNodes[grandparentIdx]).Priority) < 0)
                {
                    AssignData(arrIdx, grandparentIdx);
                    arrIdx = grandparentIdx;
                    grandparentIdx /= 4;
                }
                else
                {
                    break;
                }
            }
            ((Node)mNodes[arrIdx]).UpdateData(item, priority);
        }

        private LevelType Level(int parentIdx)
        {
            // We can calculate how far deep into the heap we are by 
            // taking the floor(lg (parentIdx)) 
            return (int)Math.Floor(Math.Log(parentIdx, 2)) % 2 == 0 ? LevelType.Min : LevelType.Max;
        }

        public T RemoveMin()
        {
            T result = default(T);
            if (mCount == 0)
            {
                // Just throw an exception? T might not be nullable or else I'd return null
                return result;
            }
            AssignData(0, 1);
            int lastIdx = mCount;
            Node last = (Node)mNodes[lastIdx];
            mNodes.RemoveAt(mCount);
            mCount--;
            if (mCount == 0)
            {
                // nothing? we're done, I think. We just popped the root off.
            }
            else if (mCount == 1)
            {
                ((Node)mNodes[1]).UpdateData(last);
            }
            else
            {

                int i = 0;
                for (i = 1, lastIdx = mCount / 2; i <= lastIdx;)
                {
                    int m = MinimumChildGrandChild(i, mCount);
                    if (NodeCompare(last, (Node)mNodes[m]) <= 0)
                    {
                        // Case 1. Must be inserted into root
                        break;
                    }

                    // Case 2b or 2c
                    AssignData(i, m);

                    if (m <= (2 * i) + 1)
                    {
                        // 2b
                        i = m;
                        break;
                    }
                    // 2c. m should be grandchild of i
                    int parentIdx = m / 2;
                    if (NodeCompare(last, (Node)mNodes[parentIdx]) > 0)
                    {
                        // Swap nodes
                        last.Swap((Node)mNodes[parentIdx]);
                    }
                    i = m;
                }
                ((Node)mNodes[i]).UpdateData(last);
            }

            // This is here PURELY FOR DEBUGGING, ensuring that we actually still have a correct heap.
            // This can be disabled for speed optimizations, but it's been invaluable in checking where I've messed up some comparisons
            if (!CheckHeapProperty())
            {
                throw new Exception("BROKE HEAP PROPERTY! Check your steps and cry when you find that missing <");
            }
            return ((Node)mNodes[0]).Data;
        }

        private int MinimumChildGrandChild(int idx, int totalSize)
        {
            int[] searchNodes =
            {
                // children
                idx*2,
                idx*2 + 1,

                // grandchildren
                (idx*2) * 2,
                (idx*2) * 2 + 1,

                (idx*2 + 1) * 2,
                (idx*2 + 1) * 2+ 1
            };

            int minIdx = -1;
            int minPriority = 0;
            for (int i = 0; i < searchNodes.Count(); i++)
            {
                if (searchNodes[i] <= totalSize)
                {
                    if (minIdx < 0 || NodePriorityCompare(((Node)mNodes[searchNodes[i]]).Priority, minPriority) < 0)
                    {
                        minIdx = searchNodes[i];
                        minPriority = ((Node)mNodes[searchNodes[i]]).Priority;
                    }
                }
                else
                {
                    break;
                }
            }
            return minIdx;

        }

        public T RemoveMax()
        {
            T result = default(T);
            // It'll always be within the top 3.
            int maxIdx = 1;
            switch (mCount)
            {
                case 0: return result;
                case 1:
                    // The root is the max
                    maxIdx = 1;
                    break;
                case 2:
                    // The root has only one child, root must be min, child is max
                    maxIdx = 2;
                    break;
                default:
                    // Root has two children, max must be one of those
                    // Get the largest of the ones in the max level
                    if (NodeCompare(2, 3) > 0)
                    {
                        maxIdx = 2;
                    }
                    else
                    {
                        maxIdx = 3;
                    }
                    break;
            }
            // Now we know which of the 3 top nodes is the max value.
            AssignData(0, maxIdx);
            int lastIdx = mCount;
            Node last = (Node)mNodes[lastIdx];
            mNodes.RemoveAt(mCount);
            mCount--;

            if (mCount == 0)
            {
                // nothing? we're done, I think. We just popped the root (and only node) off.
            }
            else if (mCount < 3)
            {
                // It's just the root and the (at most) two max children. Special case this.
                if (maxIdx == 2 && mCount == 2)
                {
                    //Oh, we removed the LEFT max and left the RIGHT child dangling. Ok, fix this by moving the right root child into the left slot
                    ((Node)mNodes[maxIdx]).UpdateData(last);
                }
                else
                {
                    // we're good. Do nothing.
                }

            }
            else
            {
                // aawwwwww fudge.
                // Ok, we KNOW that the max was removed from a 'max' level, whatever it might have been.

                int i = 0;
                for (i = maxIdx, lastIdx = mCount / 2; i <= lastIdx;)
                {
                    int m = MaximumChildGrandChild(i, mCount);
                    if (NodeCompare(last, (Node)mNodes[m]) >= 0)
                    {
                        // Case 1. Must be inserted into root
                        break;
                    }

                    AssignData(i, m);

                    if (m <= (2 * i) + 1)
                    {
                        i = m;
                        break;
                    }

                    int parentIdx = m / 2;
                    if (NodeCompare(last, (Node)mNodes[parentIdx]) < 0)
                    {
                        // Swap nodes
                        last.Swap((Node)mNodes[parentIdx]);
                    }
                    i = m;
                }
                ((Node)mNodes[i]).UpdateData(last);
            }
            // This is here PURELY FOR DEBUGGING, ensuring that we actually still have a correct heap.
            // This can be disabled for speed optimizations, but it's been invaluable in checking where I've messed up some comparisons
            if (!CheckHeapProperty())
            {
                throw new Exception("BROKE HEAP PROPERTY! Check your steps and cry when you find that missing = ");
            }

            return ((Node)mNodes[0]).Data;
        }

        private int MaximumChildGrandChild(int idx, int totalSize)
        {
            int[] searchNodes =
            {
                // children
                idx*2,
                idx*2 + 1,

                // grandchildren
                (idx*2) * 2,
                (idx*2) * 2 + 1,

                (idx*2 + 1) * 2,
                (idx*2 + 1) * 2+ 1
            };

            int maxIdx = -1;
            int maxPriority = 0;
            for (int i = 0; i < searchNodes.Count(); i++)
            {
                if (searchNodes[i] <= totalSize)
                {
                    if (maxIdx < 0 || NodePriorityCompare(((Node)mNodes[searchNodes[i]]).Priority, maxPriority) > 0)
                    {
                        maxIdx = searchNodes[i];
                        maxPriority = ((Node)mNodes[searchNodes[i]]).Priority;
                    }
                }
                else
                {
                    break;
                }
            }
            return maxIdx;
        }

        //--------------

        // Returns 0 if they're equal
        // Note: the purpose of the NodeCompare functions are for a single place where 
        //      we can adjust comparisons as well as a part of the way to get the rough complexity 
        //      when debugging.
        private int NodeCompare(Node i, Node j, bool countForDebug = true)
        {
            if (countForDebug)
            {
                mDebugCount++;
            }
            return i.Priority.CompareTo(j.Priority);
        }
        // i and j are node indexes, NOT priorities
        private int NodeCompare(int i, int j, bool countForDebug = true)
        {
            Node iChild = (Node)mNodes[i];
            Node jChild = (Node)mNodes[j];

            return NodeCompare(iChild, jChild, countForDebug);
        }

        private int NodePriorityCompare(int priority1, int priority2, bool countForDebug = true)
        {
            if (countForDebug)
            {
                mDebugCount++;
            }
            return priority1.CompareTo(priority2);
        }

        // Print the heap as a tree for visualization
        public string PrintTree(int idx = 1, int indent = 0)
        {
            string result = "";
            string strIdent = "";
            for (int i = 0; i < indent; i++)
            {
                strIdent += " ";
            }
            strIdent += "[" + indent + "][" + idx + "]";
            if (idx >= mNodes.Count)
            {
                //Console.WriteLine(strIdent + "- null");
            }
            else
            {
                result = (strIdent + "- " + ((Node)mNodes[idx]).ToString() + "\n");
                result += PrintTree(idx * 2, indent + 1);
                result += PrintTree((idx * 2) + 1, indent + 1);
            }
            return result;
        }

        // Peek at the min/max without returning them. For debugging in our case
        public T FindMin()
        {
            if (mCount == 0)
            {
                // Exception?
                return default(T);
            }
            return ((Node)mNodes[1]).Data;
        }
        public T FindMax()
        {
            T result = default(T);
            // It'll always be within the top 3.
            if (mCount == 0)
            {
                return result;
            }
            switch (mCount)
            {
                case 0: return result;
                case 1:
                    result = ((Node)mNodes[1]).Data;
                    break;
                case 2:
                    result = ((Node)mNodes[2]).Data;
                    break;
                default:
                    // Get the largest of the ones in the max level
                    if (NodeCompare(2, 3) > 0)
                    {
                        result = ((Node)mNodes[2]).Data;
                    }
                    else
                    {
                        result = ((Node)mNodes[3]).Data;
                    }
                    break;
            }
            return result;
        }


        // For DEBUGGING ONLY
        public bool CheckHeapProperty()
        {
            if (mCount <= 1 || !DebugCheckHeapProperty)
            {
                return true;
            }
            return CheckHeapPropertyMin(1);
        }
        private int FindSmallestInTree(int idx)
        {
            int leftChild = idx * 2;
            int rightChild = leftChild + 1;

            int smallest = ((Node)mNodes[idx]).Priority;
            if (mCount >= leftChild)
            {
                int leftSmallest = FindSmallestInTree(leftChild);
                if (leftSmallest < smallest)
                {
                    smallest = leftSmallest;
                }
            }
            if (mCount >= rightChild)
            {
                int rightSmallest = FindSmallestInTree(rightChild);
                if (rightSmallest < smallest)
                {
                    smallest = rightSmallest;
                }
            }
            return smallest;
        }
        private int FindLargestInTree(int idx)
        {
            int leftChild = idx * 2;
            int rightChild = leftChild + 1;

            int largest = ((Node)mNodes[idx]).Priority;
            if (mCount >= leftChild)
            {
                int leftLargest = FindLargestInTree(leftChild);
                if (leftLargest > largest)
                {
                    largest = leftLargest;
                }
            }
            if (mCount >= rightChild)
            {
                int rightLargest = FindLargestInTree(rightChild);
                if (rightLargest > largest)
                {
                    largest = rightLargest;
                }
            }
            return largest;
        }
        private bool CheckHeapPropertyMin(int idx)
        {
            int leftChild = idx * 2;
            int rightChild = leftChild + 1;

            int priority = ((Node)mNodes[idx]).Priority;
            int smallest = FindSmallestInTree(idx);
            bool valid = priority <= smallest;

            if (mCount >= leftChild)
            {
                valid = valid && CheckHeapPropertyMax(leftChild);
            }
            if (mCount >= rightChild)
            {
                valid = valid && CheckHeapPropertyMax(rightChild);
            }
            if (!valid)
            {
                throw new Exception("CheckHeapPropertyMin failed on idx " + idx);
            }
            return valid;
        }

        private bool CheckHeapPropertyMax(int idx)
        {
            int leftChild = idx * 2;
            int rightChild = leftChild + 1;
            int priority = ((Node)mNodes[idx]).Priority;
            int largest = FindLargestInTree(idx);
            bool valid = priority >= largest;

            if (mCount >= leftChild)
            {
                valid = valid && CheckHeapPropertyMin(leftChild);
            }
            if (mCount >= rightChild)
            {
                valid = valid && CheckHeapPropertyMin(rightChild);
            }

            if (!valid)
            {
                throw new Exception("CheckHeapPropertyMax failed on idx " + idx);
            }

            return valid;
        }

    }
}
