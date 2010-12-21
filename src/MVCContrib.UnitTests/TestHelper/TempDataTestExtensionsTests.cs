using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using MvcContrib.TestHelper;
using NUnit.Framework;

namespace MvcContrib.UnitTests.TestHelper
{
    [TestFixture]
    public class TempDataTestExtensionsTests
    {
        [Test]
        public void PassesWhenKeyKept()
        {
            string key = "testkey";
            TempDataDictionary tempData = new TempDataDictionary();
            tempData.Add(key, new object());
            tempData.Keep(key);
            tempData.AssertKept(key);
        }

        [Test]
        [ExpectedException(typeof(MvcContrib.TestHelper.AssertionException), ExpectedMessage="Key 'testkey' not kept.")]
        public void FailsWhenKeyNotKept()
        {
            string key = "testkey";
            TempDataDictionary tempData = new TempDataDictionary();
            tempData.Add(key, new object());
            tempData.Keep("nottestkey");
            tempData.AssertKept(key);
        }
    }
}
