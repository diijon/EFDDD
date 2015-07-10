using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EFDDD.Core;

namespace Core.Test
{
    [TestClass]
    public class ExtensionsTest
    {
        [TestClass]
        public class SetToGuidIfNullOrEmptyMethod
        {
            [TestMethod]
            public void ShouldNotEqualNull()
            {
                var expected = Guid.NewGuid();
                object actual = null;
                actual = actual.SetToGuidIfNullOrEmpty(expected);

                Assert.AreEqual(expected, actual);
            }
        }
    }
}
