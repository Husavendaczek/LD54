using System;
using System.Collections.Generic;
using System.Linq;
using SystemBase.Adapter;
using UnityEngine;

namespace SystemBase.Utils.DotNet
{
    public static class LinqExtensions
    {
        public static T NthElement<T>(this IEnumerable<T> coll, int n)
        {
            return coll.OrderBy(x => x).Skip(n - 1).FirstOrDefault();
        }

        public static T[] RandomizeInPlace<T>(this T[] list)
        {
            var random = IoC.Resolve<IUnityRandom>();
            for (var i = 0; i < list.Length; i++)
            {
                var rnd = (int)(random.Value * list.Length);
                (list[rnd], list[i]) = (list[i], list[rnd]);
            }

            return list;
        }

        public static List<T> RandomizeInPlace<T>(this List<T> list)
        {
            var random = IoC.Resolve<IUnityRandom>();
            for (var i = 0; i < list.Count; i++)
            {
                var rnd = (int)(random.Value * list.Count);
                (list[rnd], list[i]) = (list[i], list[rnd]);
            }

            return list;
        }

        public static T[] Randomize<T>(this T[] list)
        {
            var random = IoC.Resolve<IUnityRandom>();
            var result = (T[])list.Clone();
            for (var i = 0; i < result.Length; i++)
            {
                var rnd = (int)(random.Value * result.Length);
                (result[rnd], result[i]) = (result[i], result[rnd]);
            }

            return result;
        }

        public static List<T> Randomize<T>(this IEnumerable<T> list)
        {
            var random = IoC.Resolve<IUnityRandom>();
            var result = new List<T>(list);
            for (var i = 0; i < result.Count; i++)
            {
                var rnd = (int)(random.Value * result.Count);
                (result[rnd], result[i]) = (result[i], result[rnd]);
            }

            return result;
        }
        
        public static T RandomElement<T>(this ICollection<T> collection)
        {
            var random = IoC.Resolve<IUnityRandom>();
            var index = random.Range(0, collection.Count);
            return collection.ElementAt(index);
        }

        public static IEnumerable<T> AddDefaultCount<T>(this IEnumerable<T> coll, int n) where T : new()
        {
            var result = new List<T>();
            for (var i = 0; i < n; i++) result.Add(new T());

            return coll.Concat(result);
        }

        public static void Print<T>(this IEnumerable<T> list)
        {
            foreach (var element in list) Debug.Log(element);
        }

        public static void Print<T>(this IEnumerable<T> list, Func<T, string> returnStringToPrint)
        {
            foreach (var element in list) Debug.Log(returnStringToPrint(element));
        }
    }
}