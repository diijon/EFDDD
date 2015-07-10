using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDDD.Core
{
    public static class Extensions
    {
        public static object SetToGuidIfNullOrEmpty(this object _this, Guid guid)
        {
            if (_this is Guid)
            {
                if ((Guid)_this == Guid.Empty)
                {
                    _this = guid;
                }
            }
            else
            {
                _this = guid;
            }

            return _this;
        }
    }
}
