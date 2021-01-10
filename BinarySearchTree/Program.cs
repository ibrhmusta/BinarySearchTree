using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BinarySearchTree
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] dizi = { 10000, 100000, 1000000, 2500000 };
            int sayi = 0;
            BinaryTree<int> numbers = new BinaryTree<int>(new BinaryTreeNode<int>(7));

            Random rastgele = new Random();
            for (int i = 0; i < dizi[0]; i++)
            {
                sayi = rastgele.Next(dizi[0]);
                numbers.Add(sayi);
            }
            

            Console.WriteLine("{0} eleman aktarıldı", numbers.Count);

            foreach (int number in numbers)
                Console.WriteLine(number);

            Console.WriteLine("{0} değeri koleksiyonda {1}",48,numbers.Contains(5)?"Var":"Yok");

            Console.WriteLine("Max {0} Min {1}",numbers.MaxValue,numbers.MinValue);
            
            int[] array = new int[numbers.Count];
            numbers.CopyTo(array, 0);
            Console.ReadKey();
        }
    }

    public class BinaryTreeNode<T>
    {
        public T Value { get; set; }
        public BinaryTreeNode<T> ParentNode { get; set; }
        public BinaryTreeNode<T> LeftNode { get; set; }
        public BinaryTreeNode<T> RightNode { get; set; }
        public bool IsRoot { get { return ParentNode == null; } }
        public bool IsLeaf { get { return LeftNode == null && RightNode == null; } }

        public BinaryTreeNode(T RealValue)
        {
            Value = RealValue;
        }

        public BinaryTreeNode(T RealValue, BinaryTreeNode<T> Parent)
        {
            Value = RealValue;
            ParentNode = Parent;
        }

        public BinaryTreeNode(T RealValue, BinaryTreeNode<T> Parent, BinaryTreeNode<T> Left, BinaryTreeNode<T> Right)
        {
            Value = RealValue;
            RightNode = Right;
            LeftNode = Left;
            ParentNode = Parent;
        }
    }

    public class BinaryTree<T> 
        : ICollection<T>, IEnumerable<T> 
        where T : IComparable<T>
    {
        public BinaryTreeNode<T> RootNode { get; set; }
        public int NodeCount { get; set; }
        public bool IsEmpty { get { return RootNode == null; } }

        // Ağaç içerisindeki en küçük değerli elemanı döndürür
        public T MinValue
        {
            get
            {
                if (IsEmpty)
                    throw new Exception("Ağaç içerisinde hiç bir eleman yok");
                BinaryTreeNode<T> tempNode = RootNode;

                while (tempNode.LeftNode != null) // Sol dallarda değer olduğu sürece dolaş
                    tempNode = tempNode.LeftNode;
                
                return tempNode.Value;
            }
        }

        // Ağaç içerisindeki en büyük değerli elemanı döndürür
        public T MaxValue
        {
            get
            {
                if (IsEmpty)
                    throw new Exception("Ağaç içerisinde hiç bir eleman yok");

                BinaryTreeNode<T> tempNode = RootNode;
                while (tempNode.RightNode != null) // Sağ dallarda değer olduğu sürece dolaş
                    tempNode = tempNode.RightNode;
                
                return tempNode.Value;
            }
        }

        // Kaç eleman olduğunu döndürür
        public int Count
        {
            get { return NodeCount+1; }
        }

        public BinaryTree(BinaryTreeNode<T> Root)
        {
            RootNode = Root;
            NodeCount = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {            
            foreach (BinaryTreeNode<T> tempNode in Traversal(RootNode))
                yield return tempNode.Value; // Çok şükürki 2.0 ile gelen yield keyword' ü var :)
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (BinaryTreeNode<T> tempNode in Traversal(RootNode))
                yield return tempNode.Value;
        }

        // Koleksiyona eleman ekleyebilmek için kullanılır
        public void Add(T SourceItem)
        {
            if (RootNode == null)
            {
                RootNode = new BinaryTreeNode<T>(SourceItem);
                NodeCount++;
            }
            else if(Contains(SourceItem))
                return;
            else
                Insert(SourceItem);
        }

        public void Clear()
        {
            RootNode = null;
        }

        // Koleksiyonda T tipinden olan eleman olup olmadığını araştırır
        public bool Contains(T SourceItem)
        {
            if (IsEmpty)
                return false;

            BinaryTreeNode<T> tempNode = RootNode;
            while (tempNode != null)
            {
                int comparedValue = tempNode.Value.CompareTo(SourceItem);

                if (comparedValue == 0)
                    return true;
                else if (comparedValue < 0)
                    tempNode = tempNode.LeftNode;
                else
                    tempNode = tempNode.RightNode;
            }

            return false;
        }

        // Koleksiyon içeriğinin aynı tipten bir Array' e kopyalar
        public void CopyTo(T[] TargetArray, int IndexNo)
        {
            T[] tempArray = new T[NodeCount+1];
            int Counter = 0;
            foreach (T value in this)
            {
                tempArray[Counter] = value;
                Counter++;
            }
            Array.Copy(tempArray, 0, TargetArray, IndexNo, this.NodeCount);
        }

        // Koleksiyondan eleman çıkartmak için kullanılır
        public bool Remove(T SourceItem)
        {
            BinaryTreeNode<T> item = Find(SourceItem);
            if (item == null)
                return false;

            List<T> values = new List<T>();
            foreach (BinaryTreeNode<T> tempNode in Traversal(item.LeftNode))
                values.Add(tempNode.Value);

            foreach (BinaryTreeNode<T> tempNode in Traversal(item.RightNode))
                values.Add(tempNode.Value);

            if (item.ParentNode.LeftNode == item)
                item.ParentNode.LeftNode = null;
            else
                item.ParentNode.RightNode = null;

            item.ParentNode = null;
            foreach (T value in values)
                Add(value);

            return true;
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        BinaryTreeNode<T> Find(T SourceItem)
        {
            foreach (BinaryTreeNode<T> item in Traversal(RootNode))
                if (item.Value.Equals(SourceItem))
                    return item;

            return null;
        }

        // InOrder modeline göre elemanlar dolaşılır.
        IEnumerable<BinaryTreeNode<T>> Traversal(BinaryTreeNode<T> Node)
        {
            if (Node.LeftNode != null)
                foreach (BinaryTreeNode<T> leftNode in Traversal(Node.LeftNode))
                    yield return leftNode;
            
            yield return Node;
            
            if (Node.RightNode != null)
                foreach (BinaryTreeNode<T> rightNode in Traversal(Node.RightNode))
                    yield return rightNode;
        }

        void Insert(T SourceItem)
        {
            BinaryTreeNode<T> tempNode = RootNode;
            bool found = false;
            while (!found)
            {
                int comparedValue = tempNode.Value.CompareTo(SourceItem);
                if (comparedValue < 0)
                {
                    if (tempNode.LeftNode == null)
                    {
                        tempNode.LeftNode = new BinaryTreeNode<T>(SourceItem, tempNode);
                        NodeCount++;
                        return;
                    }
                    else
                    {
                        tempNode = tempNode.LeftNode;
                    }
                }
                else if (comparedValue > 0)
                {
                    if (tempNode.RightNode == null)
                    {
                        tempNode.RightNode = new BinaryTreeNode<T>(SourceItem, tempNode);
                        NodeCount++;
                        return;
                    }
                    else
                    {
                        tempNode = tempNode.RightNode;
                    }
                }
                else
                {
                    return;
                }
            }
        }
    }
}
