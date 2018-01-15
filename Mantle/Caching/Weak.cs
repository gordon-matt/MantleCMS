//using System;

//namespace Mantle.Caching
//{
//    public class Weak<T>
//    {
//        private readonly WeakReference target;

//        public Weak(T target)
//        {
//            this.target = new WeakReference(target);
//        }

//        public Weak(T target, bool trackResurrection)
//        {
//            this.target = new WeakReference(target, trackResurrection);
//        }

//        public T Target
//        {
//            get { return (T)target.Target; }
//            set { target.Target = value; }
//        }
//    }
//}