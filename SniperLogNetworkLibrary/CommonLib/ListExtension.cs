using System.Reflection.Emit;
using System.Reflection;

namespace SniperLogNetworkLibrary.CommonLib
{
    /// <summary>
    /// Extensions for <see cref="List{T}"/> class.
    /// </summary>
    public static class ListExtension
    {
        /// <summary>
        /// Gets the internal array of the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// https://stackoverflow.com/questions/4972951/listt-to-t-without-copying
        private static class ArrayAccessor<T>
        {
            public static Func<List<T>, T[]> Getter;

            static ArrayAccessor()
            {
                DynamicMethod? dm = new DynamicMethod("get", MethodAttributes.Static | MethodAttributes.Public, CallingConventions.Standard, typeof(T[]), [typeof(List<T>)], typeof(ArrayAccessor<T>), true);
                ILGenerator gen = dm.GetILGenerator();

                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, typeof(List<T>).GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance));
                gen.Emit(OpCodes.Ret);

                Getter = (Func<List<T>, T[]>)dm.CreateDelegate(typeof(Func<List<T>, T[]>));
            }
        }

        /// <summary>
        /// Extension method to get the internal array used by the list.
        /// </summary>
        /// <typeparam name="T">Type of the array.</typeparam>
        /// <param name="list">This list.</param>
        /// <returns>Internal array used by the list.</returns>
        public static T[] GetInternalArray<T>(this List<T> list)
        {
            return ArrayAccessor<T>.Getter(list);
        }
    }
}