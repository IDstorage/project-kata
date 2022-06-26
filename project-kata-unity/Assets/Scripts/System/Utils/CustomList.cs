using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anomaly 
{
    public class CustomList<T> {
        public T data;
        public CustomList<T> next = null;
        public bool isHead = false;

        public bool IsNull => ReferenceEquals(data, null);

        public static CustomList<T> Create(T initData = default(T)) {
            CustomList<T> head = new CustomList<T>();
            head.isHead = true;
            head.data = initData;
            return head;
        }

        public void Add(CustomList<T> other) {
            if (other == null) return;
            var last = this;
            while (last.next != null) last = last.next;
            last.next = other;
            other.isHead = false;
        }

        public void Add(T other) {
            CustomList<T> node = new CustomList<T>();
            node.data = other;
            Add(node);
        }

        public static void Remove(CustomList<T> head, CustomList<T> node) {
            if (head == null || node == null) return;

            CustomList<T> iter = head.next;

            if (ReferenceEquals(head, node)) {
                head.next.isHead = true;
                head = null;
                return;
            }

            while (iter != null) {
                if (ReferenceEquals(iter.next, node)) {
                    iter.next = node.next;
                    node = null; node.next = null;
                    break;
                }
                iter = iter.next;
            }
        }
    }

}