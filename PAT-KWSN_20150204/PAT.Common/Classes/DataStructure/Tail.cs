using System;
using System.Text;

namespace PAT.ModelChecking.DataStructure
{
    public class Tail
    {
        private MyNode[] _items;
        private int _size;

        public Tail()
        {
            this._items = new MyNode[0];
        }

        public void Add(MyNode item)
        {
            if (this._size == this._items.Length)
            {
                this.EnsureCapacity(this._size + 1);
            }
            this._items[this._size++] = item;
        }

        private void EnsureCapacity(int min)
        {
            if (this._items.Length < min)
            {
                int num = (this._items.Length == 0) ? 1 : (this._items.Length * 2);
                if (num < min)
                {
                    num = min;
                }
                this.Capacity = num;
            }
        }

        public void Insert(int index, MyNode item)
        {
            if (this._size == this._items.Length)
            {
                this.EnsureCapacity(this._size + 1);
            }
            if (index < this._size)
            {
                Array.Copy(this._items, index, this._items, index + 1, this._size - index);
            }
            this._items[index] = item;
            this._size++;
        }

        public int Capacity
        {
            get
            {
                return this._items.Length;
            }
            set
            {
                if (value != this._items.Length)
                {
                   
                    //if (value > 0)
                    //{
                        MyNode[] destinationArray = new MyNode[value];
                        if (this._size > 0)
                        {
                            Array.Copy(this._items, 0, destinationArray, 0, this._size);
                        }
                        this._items = destinationArray;
                    //}
                    //else
                    //{
                    //    this._items = Tail._emptyArray;
                    //}
                }
            }
        }

        public int Count
        {
            get
            {
                return this._size;
            }
        }

        public MyNode this[int index]
        {
            get
            {
                return this._items[index];
            }
            set
            {
                this._items[index] = value;
            }
        }
    }
}
