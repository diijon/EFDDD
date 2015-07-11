using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EFDDD.DataModel.Migrations.Core
{
    /// <summary>
    /// Thread-safe random integer generator
    /// </summary>
    public static class RandomGen
    {
        private static RNGCryptoServiceProvider _global =
            new RNGCryptoServiceProvider();
        [ThreadStatic]
        private static Random _local;

        public static int Next()
        {
            return Next(null, null);
        }

        public static int Next(int? rangeMin, int? rangeMax)
        {
            Random inst = _local;
            if (inst == null)
            {
                byte[] buffer = new byte[4];
                _global.GetBytes(buffer);
                _local = inst = new Random(
                    BitConverter.ToInt32(buffer, 0));
            }

            if (rangeMin.HasValue && rangeMax.HasValue)
            {
                var rInt = inst.Next(rangeMin.Value, rangeMax.Value);
                return rInt;
            }

            return inst.Next();
        }
    }
}
