using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace MontyHall.Extensions
{
    internal static class ExtRandomness
    {
        /* NOTE: extension method to be used like LINQ */
        internal static void ShuffleCrypto<T>(this IList<T> list)
        {
            lock (list)
            {
                using var rngProvider = RandomNumberGenerator.Create();
                var box = new byte[1];
                for (int i = list.Count - 1; i > 0; i--)
                {
                    do rngProvider.GetBytes(box);
                    while (!(box[0] < byte.MaxValue));
                    int k = box[0] % i;
                    (list[i], list[k]) = (list[k], list[i]);
                }
            }
        }

        /* RO: read-only */
        internal static List<T> ShuffleCryptoRO<T>(in List<T> listToShuffle)
        {
            var list = new List<T>(listToShuffle);

            using var rngProvider = RandomNumberGenerator.Create();
            var box = new byte[1];
            for (int i = list.Count - 1; i > 0; i--)
            {
                do rngProvider.GetBytes(box);
                while (!(box[0] < byte.MaxValue));
                int k = box[0] % i;
                (list[i], list[k]) = (list[k], list[i]);
            }

            return list;
        }
    }
}
