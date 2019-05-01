using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cycloid.Extensions
{
    public static class ClassExtensions
    {
        public static bool Between(this DateTime d, DateTime start, DateTime end)
        {
            return d >= start && d <= end;
        }
    }
}
