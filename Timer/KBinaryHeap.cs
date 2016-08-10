/**
 * @file    KBinaryHeap.cs
 * @author  Longwei Lai
 * 
 * @brief   �����ʵ��
 *          �����漰ֵ�����ĵط���O3�����Ż���O1����
 */

using System;
using System.Linq;
using System.Collections.Generic;

#region List Extra Impl
/// <summary>
/// List&lt;&gt;��չ
/// <para>ʵ��Resize����</para>
/// </summary>
public static class ListExtra
{
    /// <summary>
    /// List&lt;&gt;.Resize()����ʵ��
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
    /// �����ʵ��
    /// <para>��ִ��exchange���Ż�</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KBinaryHeap<T>  where T : class, IComparable<T>
    {
        #region ����
        /// <summary>
        /// ����һ��ָ����ʼ�����Ķ����
        /// </summary>
        /// <param name="capacity">��ʼ����</param>
        public KBinaryHeap(int capacity = 128)
        {
            _curSize = 0;

            _elems = new List<T>(capacity + 1);
            _elems.Resize(capacity + 1);
        }

        /// <summary>
        /// ��δ�����list�Ĺ�������
        /// </summary>
        /// <param name="elems">Ԫ���б�</param>
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
        /// ��ǰ�Ѵ�С
        /// </summary>
        public int count
        {
            get { return _curSize; }
        }

        /// <summary>
        /// ������
        /// </summary>
        public int capacity
        {
            get { return _elems.Count - 1; }
        }
        #endregion

        #region top/TryGetTop
        /// <summary>
        /// �Ѷ�Ԫ��,�����Ϊ��,���׳��쳣
        /// <para>�쳣��:IndexOutOfRangeException</para>
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
        /// ����ȡ�öѶ�Ԫ��
        /// </summary>
        /// <returns>�����Ϊ��,����null,���򷵻ضѶ�Ԫ��</returns>
        public T TryGetTop()
        {
            if (_curSize == 0)
                return default(T);

            return _elems[1];
        }
        #endregion

        #region Add/Remove
        /// <summary>
        /// ����������һ��Ԫ��
        /// </summary>
        /// <param name="elem">Ԫ��</param>
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
        /// �ӶѶ��Ƴ�һ��Ԫ��
        /// </summary>
        /// <returns>�ɹ�����true,���򷵻�false</returns>
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
        /// �ӶѶ��Ƴ�һ��Ԫ��
        /// </summary>
        /// <param name="topElem">����Ƴ��ɹ�,ɾ����Ԫ�ؽ��洢����out����</param>
        /// <returns>�ɹ�����true,���򷵻�false</returns>
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
        /// �Ƴ�ָ��λ��Ԫ��
        /// </summary>
        /// <param name="hole">hole</param>
        /// <returns>�ɹ�����true,���򷵻�false</returns>
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
        /// �Ƴ�ָ��Ԫ��
        /// </summary>
        /// <param name="elem">Ԫ��</param>
        /// <returns>�ɹ�����true,���򷵻�false</returns>
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
        /// ���BinaryHeap
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
        /// ���ڲ�ʵ��:ִ��Ԫ������
        /// </summary>
        /// <param name="hole">��ʼ����λ��</param>
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
        /// ���ڲ�ʵ��:�ع���(�ӵ�һ����leaf�ڵ㿪ʼ����)
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
