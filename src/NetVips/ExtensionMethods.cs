namespace NetVips
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using Internal;

    /// <summary>
    /// Useful extension methods that we use in our codebase.
    /// </summary>
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Removes the element with the specified key from the <see cref="VOption"/>
        /// and retrieves the value to <paramref name="target"/>.
        /// </summary>
        /// <param name="self">The <see cref="VOption"/> to remove from.</param>
        /// <param name="key">>The key of the element to remove.</param>
        /// <param name="target">The target to retrieve the value to.</param>
        /// <returns><see langword="true"/> if the element is successfully removed; otherwise, <see langword="false"/>.</returns>
        internal static bool Remove(this VOption self, string key, out object target)
        {
            self.TryGetValue(key, out target);
            return self.Remove(key);
        }

        /// <summary>
        /// Merges 2 <see cref="VOption"/>s.
        /// </summary>
        /// <param name="self">The <see cref="VOption"/> to merge into.</param>
        /// <param name="merge">The <see cref="VOption"/> to merge from.</param>
        internal static void Merge(this VOption self, VOption merge)
        {
            foreach (var item in merge)
            {
                self[item.Key] = item.Value;
            }
        }

        /// <summary>
        /// Dereferences data from an unmanaged block of memory
        /// to a newly allocated managed object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to be created. This object
        /// must represent a formatted class or a structure.</typeparam>
        /// <param name="ptr">A pointer to an unmanaged block of memory.</param>
        /// <returns>A newly allocated managed object of the specified type.</returns>
        internal static T Dereference<T>(this IntPtr ptr)
        {
            return (T)Marshal.PtrToStructure(ptr, typeof(T));
        }

        /// <summary>
        /// Call a libvips operation.
        /// </summary>
        /// <param name="image">A <see cref="Image"/> used as guide.</param>
        /// <param name="operationName">Operation name.</param>
        /// <returns>A new object.</returns>
        internal static object Call(this Image image, string operationName) =>
            Operation.Call(operationName, null, image);

        /// <summary>
        /// Call a libvips operation.
        /// </summary>
        /// <param name="image">A <see cref="Image"/> used as guide.</param>
        /// <param name="operationName">Operation name.</param>
        /// <param name="args">An arbitrary number and variety of arguments.</param>
        /// <returns>A new object.</returns>
        internal static object Call(this Image image, string operationName, params object[] args) =>
            Operation.Call(operationName, null, image, args);

        /// <summary>
        /// Call a libvips operation.
        /// </summary>
        /// <param name="image">A <see cref="Image"/> used as guide.</param>
        /// <param name="operationName">Operation name.</param>
        /// <param name="kwargs">Optional arguments.</param>
        /// <returns>A new object.</returns>
        internal static object Call(this Image image, string operationName, VOption kwargs) =>
            Operation.Call(operationName, kwargs, image);

        /// <summary>
        /// Call a libvips operation.
        /// </summary>
        /// <param name="image">A <see cref="Image"/> used as guide.</param>
        /// <param name="operationName">Operation name.</param>
        /// <param name="kwargs">Optional arguments.</param>
        /// <param name="args">An arbitrary number and variety of arguments.</param>
        /// <returns>A new object.</returns>
        internal static object Call(this Image image, string operationName, VOption kwargs, params object[] args) =>
            Operation.Call(operationName, kwargs, image, args);

        /// <summary>
        /// Prepends <paramref name="image"/> to <paramref name="args"/>.
        /// </summary>
        /// <param name="args">The <see cref="Image"/> array.</param>
        /// <param name="image">The <see cref="Image"/> to prepend to <paramref name="args"/>.</param>
        /// <returns>A new object array.</returns>
        internal static object[] PrependImage<T>(this T[] args, Image image)
        {
            if (args == null)
            {
                return new object[] { image };
            }

            var newValues = new object[args.Length + 1];
            newValues[0] = image;
            Array.Copy(args, 0, newValues, 1, args.Length);
            return newValues;
        }

        /// <summary>
        /// Marshals a GLib UTF8 char* to a managed string.
        /// </summary>
        /// <param name="utf8Str">Pointer to the GLib string.</param>
        /// <param name="freePtr">If set to <see langword="true"/>, free the GLib string.</param>
        /// <param name="size">Size of the GLib string, use 0 to read until the null character.</param>
        /// <returns>The managed string.</returns>
        internal static string ToUtf8String(this IntPtr utf8Str, bool freePtr = false, int size = 0)
        {
            if (utf8Str == IntPtr.Zero)
            {
                return null;
            }

            if (size == 0)
            {
                while (Marshal.ReadByte(utf8Str, size) != 0)
                {
                    ++size;
                }
            }

            if (size == 0)
            {
                if (freePtr)
                {
                    GLib.GFree(utf8Str);
                }

                return string.Empty;
            }

            var managedArray = new byte[size];
            try
            {
                Marshal.Copy(utf8Str, managedArray, 0, size);
                return Encoding.UTF8.GetString(managedArray, 0, size);
            }
            finally
            {
                if (freePtr)
                {
                    GLib.GFree(utf8Str);
                }
            }
        }

        /// <summary>
        /// Convert bytes to human readable format.
        /// </summary>
        /// <param name="value">The number of bytes.</param>
        /// <returns>The readable format of the bytes.</returns>
        internal static string ToReadableBytes(this ulong value)
        {
            string[] sizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            var i = 0;
            decimal dValue = value;
            while (Math.Round(dValue, 2) >= 1000)
            {
                dValue /= 1024;
                i++;
            }

            return $"{dValue:n2} {sizeSuffixes[i]}";
        }

        /// <summary>
        /// Negate all elements in an array.
        /// </summary>
        /// <param name="array">An array of doubles.</param>
        /// <returns>The negated array.</returns>
        internal static double[] Negate(this double[] array)
        {
            for (var i = 0; i < array.Length; i++)
            {
                array[i] *= -1;
            }

            return array;
        }

        /// <summary>
        /// Negate all elements in an array.
        /// </summary>
        /// <remarks>
        /// It will output an array of doubles instead of integers.
        /// </remarks>
        /// <param name="array">An array of integers.</param>
        /// <returns>The negated array.</returns>
        internal static double[] Negate(this int[] array)
        {
            var doubles = new double[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                ref var value = ref doubles[i];
                value = array[i] * -1;
            }

            return doubles;
        }

        /// <summary>
        /// Invert all elements in an array.
        /// </summary>
        /// <param name="array">An array of doubles.</param>
        /// <returns>The inverted array.</returns>
        internal static double[] Invert(this double[] array)
        {
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = 1.0 / array[i];
            }

            return array;
        }

        /// <summary>
        /// Invert all elements in an array.
        /// </summary>
        /// <remarks>
        /// It will output an array of doubles instead of integers.
        /// </remarks>
        /// <param name="array">An array of integers.</param>
        /// <returns>The inverted array.</returns>
        internal static double[] Invert(this int[] array)
        {
            var doubles = new double[array.Length];
            for (var i = 0; i < array.Length; i++)
            {
                ref var value = ref doubles[i];
                value = 1.0 / array[i];
            }

            return doubles;
        }
    }
}