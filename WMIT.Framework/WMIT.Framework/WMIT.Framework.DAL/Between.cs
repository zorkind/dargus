using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WMIT.Framework.VO;

namespace WMIT.Framework.DAL
{
    public static class Between<T>
    {
        public static bool HasValue(VO.Between<T> obj)
        {
            return WichHasValue(obj) > -1;
        }

        private static short WichHasValue(VO.Between<T> obj)
        {
            var HasMin = !EqualityComparer<T>.Default.Equals(obj.Min, default(T));
            var HasMax = !EqualityComparer<T>.Default.Equals(obj.Max, default(T));

            if (HasMin && HasMax)
            {
                if (EqualityComparer<T>.Default.Equals(obj.Min, obj.Max))
                    return 3;

                return 2;
            }

            if (HasMin && !HasMax)
                return 1;

            if (!HasMin && HasMax)
                return 0;

            return -1;
        }

        public static void Append(IParametrosBuilder pParametros, VO.Between<T> obj, string pAlias, string pField)
        {
            switch (WichHasValue(obj))
            {
                case 3:
                    pParametros.AppendEqual(obj.Min, pAlias, pField);
                    break;
                case 2:
                    pParametros.AppendBetween(obj.Min, obj.Max, pAlias, pField);
                    break;
                case 1:
                    if (obj.UseEquals)
                        pParametros.AppendBiggerOrEqualThen(obj.Min, pAlias, pField);
                    else
                        pParametros.AppendBiggerThen(obj.Min, pAlias, pField);
                    break;
                case 0:
                    if (obj.UseEquals)
                        pParametros.AppendSmallerOrEqualThen(obj.Max, pAlias, pField);
                    else
                        pParametros.AppendSmallerThen(obj.Max, pAlias, pField);
                    break;
            }
        }
    }
}
