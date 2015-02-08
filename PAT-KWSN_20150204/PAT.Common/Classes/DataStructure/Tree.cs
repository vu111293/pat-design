using System.Collections.Generic;
namespace PAT.ModelChecking.DataStructure
{
    public struct MyNode
    {
        public int value;
        public Tail tail;
    }

    public class Tree
    {
        private const int NOT_EXIST = -1;
        private MyNode root;
        private int count;
        public Tree()
        {
            count = 0;
            root = new MyNode();
            root.tail = new Tail();
            root.value = -1;
        }
        public int Count
        {
            get
            {
                return count;
            }
        }

        public bool Visit(List<int> value)
        {
            MyNode temp = root;
            bool result = false;
            VisitTemp(value, ref temp, ref result);
            //each branch will use the leaf node (value=int.min, tail=null) to mark its end

            //in case: addd(1,2,3)->add(1,2)
            if (result || (!(temp.tail[0].value == int.MinValue && temp.tail[0].tail == null)))
            {
                count++;
                MyNode leafNode = new MyNode();
                leafNode.value = int.MinValue;
                leafNode.tail = null;
                temp.tail.Insert(0, leafNode);
                return true;
            }
            else
            {
                return false;
            }

        }
      

        public void VisitWithID(List<int> value, int ID)
        {
            MyNode temp = root;
            bool result = false;
            VisitTemp(value, ref temp, ref result);
            MyNode leafNode = new MyNode();
            leafNode.value = ID;
            leafNode.tail = null;
            temp.tail.Insert(0, leafNode);
            count++;
            return;
        }
        private int BinarySearch(Tail array, int value)
        {
            int begin = 0;
            int end = array.Count - 1;
            do
            {
                if (array[begin].value > value)
                    return ~begin;
                if (array[begin].value == value)
                    return begin;
                if (array[end].value < value)
                    return ~(end + 1);
                if (array[end].value == value)
                    return (end);
                int middle = (begin + end) / 2;
                int middleValue = array[middle].value;
                if (middleValue < value)
                {
                    begin = middle + 1;
                    middle = (begin + end) / 2;
                }
                else if (middleValue > value)
                {
                    end = middle - 1;
                    middle = (begin + end) / 2;
                }
                if (middleValue == value)
                    return middle;
            } while (begin < end);

            if (array[begin].value > value)
                return ~(begin);
            else if (array[begin].value < value)
                return ~(begin + 1);
            else
                return begin;
        }

        public bool ContainsKey(List<int> value)
        {
            MyNode temp = root;
            for (int i = 0; i < value.Count; i++)
            {
                if (temp.tail.Count == 0)
                {
                    return false;
                }
                else
                {
                    int pos = BinarySearch(temp.tail, value[i]);
                    if (pos >= 0)
                        temp = temp.tail[pos];
                    else
                    {
                        return false;
                    }
                }
            }

            for (int i = 0; i < temp.tail.Count; i++)
            {
                if (temp.tail[i].tail == null)
                    return true;
            }
            return false;

        }

        public int GetID(List<int> value)
        {
            MyNode temp = root;
            for (int i = 0; i < value.Count; i++)
            {
                if (temp.tail.Count == 0)
                {
                    return NOT_EXIST;
                }
                else
                {
                    int pos = BinarySearch(temp.tail, value[i]);
                    if (pos >= 0)
                        temp = temp.tail[pos];
                    else
                    {
                        return NOT_EXIST;
                    }
                }
            }

            for (int i = 0; i < temp.tail.Count; i++)
            {
                if (temp.tail[i].tail == null)
                    return temp.tail[i].value;
            }
            return NOT_EXIST;
        }
        //if LTL, each key has value so we do not need use the leaf node (int.min, null)
        private void VisitTemp(List<int> value, ref MyNode temp, ref bool result)
        {
            for (int i = 0; i < value.Count; i++)
            {
                if (temp.tail.Count == 0)
                {
                    result = true;

                    MyNode childNode = new MyNode();
                    childNode.value = value[i];
                    childNode.tail = new Tail();

                    temp.tail.Add(childNode);
                    temp = temp.tail[0];
                }
                else
                {
                    int pos = BinarySearch(temp.tail, value[i]);
                    if (pos >= 0)
                        temp = temp.tail[pos];
                    else
                    {
                        result = true;

                        MyNode childNode = new MyNode();
                        childNode.value = value[i];
                        childNode.tail = new Tail();

                        temp.tail.Insert(~pos, childNode);
                        temp = temp.tail[~pos];
                    }
                }
            }
        }
    }
}