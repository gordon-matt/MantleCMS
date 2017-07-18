using System;
using System.Collections;
using System.Collections.Generic;

namespace Mantle.Collections.Generic
{
    public interface IRelationship<T> : IRelationship<T, T>
    {
    }

    public interface IRelationship<TParent, TChild> : IEnumerable<IRelationship<TChild>>
    {
        TParent Parent { get; set; }

        int CurrentLevel { get; set; }
    }

    public class Relationship<T> : Relationship<T, T>, IRelationship<T>
    {
        public Relationship(T parent)
            : base(parent)
        {
        }
    }

    public class Relationship<TParent, TChild> : IRelationship<TParent, TChild>, ICollection<IRelationship<TChild>>
    {
        private readonly IList<IRelationship<TChild>> children;

        public Relationship(TParent parent)
        {
            Parent = parent;
            children = new List<IRelationship<TChild>>();
        }

        #region ICollection<TChild> Members

        public void Add(IRelationship<TChild> item)
        {
            item.CurrentLevel = CurrentLevel + 1;
            children.Add(item);
        }

        public void Clear()
        {
            children.Clear();
        }

        public bool Contains(IRelationship<TChild> item)
        {
            return children.Contains(item);
        }

        public void CopyTo(IRelationship<TChild>[] array, int arrayIndex)
        {
            children.CopyTo(array, arrayIndex);
        }

        public bool Remove(IRelationship<TChild> item)
        {
            return children.Remove(item);
        }

        public int Count
        {
            get { return children.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion ICollection<TChild> Members

        #region IRelationship<TParent,TChild> Members

        public IEnumerator<IRelationship<TChild>> GetEnumerator()
        {
            return children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TParent Parent { get; set; }

        public int CurrentLevel { get; set; }

        #endregion IRelationship<TParent,TChild> Members
    }

    public static class RelationshipExtensions
    {
        public static IList<IRelationship<T>> ToRelationship<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childSelector)
        {
            var result = new List<IRelationship<T>>();

            foreach (var item in source)
            {
                var relationship = new Relationship<T>(item);

                ToRelationship(relationship, item, childSelector);

                result.Add(relationship);
            }

            return result;
        }

        private static void ToRelationship<T>(ICollection<IRelationship<T>> parent, T item, Func<T, IEnumerable<T>> childSelector)
        {
            var childItems = childSelector(item);
            foreach (var child in childItems)
            {
                var relationship = new Relationship<T>(child);
                parent.Add(relationship);
                ToRelationship(relationship, child, childSelector);
            }
        }
    }
}