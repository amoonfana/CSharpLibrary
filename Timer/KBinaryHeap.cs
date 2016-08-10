/**
 * @file    KBinaryHeap.cs
 * @author  Longwei Lai
 * 
 * @brief   二叉堆实现
 *          所有涉及值交换的地方从O3级别优化至O1级别
 */

using System;
using System.Linq;
using System.Collections.Generic;

#region List Extra Impl
/// <summary>
/// List&lt;&gt;扩展
/// <para>实现Resize方法</para>
/// </summary>
public static class ListExtra
{
    /// <summary>
    /// List&lt;&gt;.Resize()方法实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="toSize"></param>
    /// <param name="dftValue"></param>
    public static void Resize<T>(this List<T> list, int toSize, T dftValue = default(T))
    {
        int curSize = list.Count;
        if (toSize < curSize)
        {
            list.RemoveRange(toSize, curSize - toSize);
        }
        else if (toSize > curSize)
        {
            if (toSize > list.Capacity)
                list.Capacity = toSize;

            list.AddRange(Enumerable.Repeat(dftValue, toSize - curSize));
        }
    }
}
#endregion

namespace ConsoleTimer
{
    /// <summary>
    /// 二叉堆实现
    /// <para>已执行exchange最优化</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KBinaryHeap<T>  where T : class, IComparable<T>
    {
        #region 构造
        /// <summary>
        /// 构造一个指定初始容量的二叉堆
        /// </summary>
        /// <param name="capacity">初始容量</param>
        public KBinaryHeap(int capacity = 128)
        {
            _curSize = 0;

            _elems = new List<T>(capacity + 1);
            _elems.Resize(capacity + 1);
        }

        /// <summary>
        /// 从未排序的list的构造二叉堆
        /// </summary>
        /// <param name="elems">元素列表</param>
        public KBinaryHeap(List<T> elems)
        {
            _curSize = elems.Count;
            _elems = new List<T>(elems.Count + 1);
            _elems.Add(default(T));
            _elems.AddRange(elems);
        }
        #endregion

        #region count/capacity
        /// <summary>
        /// 当前堆大小
        /// </summary>
        public int count
        {
            get { return _curSize; }
        }

        /// <summary>
        /// 堆容量
        /// </summary>
        public int capacity
        {
            get { return _elems.Count - 1; }
        }
        #endregion

        #region top/TryGetTop
        /// <summary>
        /// 堆顶元素,如果堆为空,将抛出异常
        /// <para>异常类:IndexOutOfRangeException</para>
        /// </summary>
        public T top
        {
            get
            {
                if (_curSize == 0)
                    throw new IndexOutOfRangeException("BineryHeap empty");

                return _elems[1];
            }
        }

        /// <summary>
        /// 尝试取得堆顶元素
        /// </summary>
        /// <returns>如果堆为空,返回null,否则返回堆顶元素</returns>
        public T TryGetTop()
        {
            if (_curSize == 0)
                return default(T);

            return _elems[1];
        }
        #endregion

        #region Add/Remove
        /// <summary>
        /// 往堆中增加一个元素
        /// </summary>
        /// <param name="elem">元素</param>
        public void Add(T elem)
        {
            if (_curSize == _elems.Count - 1)
                _elems.Resize(_elems.Count * 2);

            int hole = ++_curSize;
            for (; hole > 1 && elem.CompareTo(_elems[hole / 2]) < 0; hole /= 2)
                _elems[hole] = _elems[hole / 2];

            _elems[hole] = elem;
        }

        /// <summary>
        /// 从堆顶移除一个元素
        /// </summary>
        /// <returns>成功返回true,否则返回false</returns>
        public bool RemoveTop()
        {
            if (_curSize == 0)
                return false;

            _elems[1] = _elems[_curSize];
            _elems[_curSize--] = null; // Make sure heap element reference clear.

            this._PercolateDown(1);
            return true;
        }

        /// <summary>
        /// 从堆顶移除一个元素
        /// </summary>
        /// <param name="topElem">如果移除成功,删除的元素将存储至此out参数</param>
        /// <returns>成功返回true,否则返回false</returns>
        public bool RemoveTop(out T topElem)
        {
            if (_curSize == 0)
            {
                topElem = default(T);
                return false;
            }

            topElem = _elems[1];

            _elems[1] = _elems[_curSize];
            _elems[_curSize--] = null; // Make sure heap element reference clear.

            this._PercolateDown(1);
            return true;
        }

        /// <summary>
        /// 移除指定位置元素
        /// </summary>
        /// <param name="hole">hole</param>
        /// <returns>成功返回true,否则返回false</returns>
        public bool RemoveElem(int hole)
        {
            if (_curSize == 0)
                return false;
            else if (hole <= 0 || hole > _curSize)
                return false;

            _elems.RemoveAt(hole);

            _curSize -= 1;
            this._BuildHeap();

            return true;
        }

        /// <summary>
        /// 移除指定元素
        /// </summary>
        /// <param name="elem">元素</param>
        /// <returns>成功返回true,否则返回false</returns>
        public bool RemoveElem(T elem)
        {
            if (_curSize == 0)
                return false;

            for (int i = 1; i <= _curSize; i++)
            {
                if (_elems[i].Equals(elem))
                    return this.RemoveElem(i);
            }

            return false;
        }
        #endregion

        #region Clear
        /// <summary>
        /// 清除BinaryHeap
        /// </summary>
        public void Clear()
        {
            _curSize = 0;

            _elems.Clear();
            _elems.Resize(_curSize + 1);
        }
        #endregion

        #region Percolate Down/Build Heap
        /// <summary>
        /// 堆内部实现:执行元素下滤
        /// </summary>
        /// <param name="hole">起始下滤位置</param>
        private void _PercolateDown(int hole)
        {
            int child;
            T tmp = _elems[hole];

            for (; hole * 2 <= _curSize; hole = child)
            {
                child = hole * 2;
                if (child != _curSize && 
                    _elems[child + 1].CompareTo(_elems[child]) < 0)
                    ++child;

                if (_elems[child].CompareTo(tmp) < 0)
                    _elems[hole] = _elems[child];
                else
                    break;
            }

            _elems[hole] = tmp;
        }

        /// <summary>
        /// 堆内部实现:重构堆(从第一个非leaf节点开始下滤)
        /// </summary>
        private void _BuildHeap()
        {
            for (int i = _curSize / 2; i > 0; i--)
                this._PercolateDown(i);
        }
        #endregion

        #region Data members
        private int _curSize;
        private List<T> _elems;
        #endregion
    }
}
