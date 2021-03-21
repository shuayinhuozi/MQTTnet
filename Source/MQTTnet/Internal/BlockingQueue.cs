﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace MQTTnet.Internal
{
    public class BlockingQueue<TItem>
    {
        private readonly object _syncRoot = new object();
        private readonly LinkedList<TItem> _items = new LinkedList<TItem>();
        private readonly ManualResetEvent _gate = new ManualResetEvent(false);

        public int Count
        {
            get
            {
                lock (_syncRoot)
                {
                    return _items.Count;
                }
            }
        }

        public void Enqueue(TItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            lock (_syncRoot)
            {
                _items.AddLast(item);
                _gate.Set();
            }
        }

        public TItem Dequeue()
        {
            while (true)
            {
                lock (_syncRoot)
                {
                    if (_items.Count > 0)
                    {
                        var item = _items.First.Value;
                        _items.RemoveFirst();

                        return item;
                    }

                    if (_items.Count == 0)
                    {
                        _gate.Reset();
                    }
                }

                _gate.WaitOne();
            }
        }

        public TItem RemoveFirst()
        {
            lock (_syncRoot)
            {
                var item = _items.First;
                _items.RemoveFirst();

                return item.Value;
            }
        }

        public void Clear()
        {
            lock (_syncRoot)
            {
                _items.Clear();
            }
        }
    }
}
