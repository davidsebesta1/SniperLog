using System.Reflection.Emit;
using System.Reflection;

namespace SniperLogNetworkLibrary.CommonLib
{
    public static class ListExtension
    {
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

        public static T[] GetInternalArray<T>(this List<T> list)
        {
            return ArrayAccessor<T>.Getter(list);
        }
    }
}