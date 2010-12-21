using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MvcContrib.TestHelper
{
    public static class TempDataTestExtensions
    {
        /// <summary>
        /// Asserts that a key has been passed to TempData.Keep(key)
        /// </summary>
        /// <param name="TempData">TempData collection</param>
        /// <param name="key">The key to assert kept.</param>
        public static void AssertKept(this TempDataDictionary TempData, string key)
        {
            var keptKeysField = typeof(TempDataDictionary).GetField("_retainedKeys", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var keptKeys = keptKeysField.GetValue(TempData) as HashSet<string>;

            if (!keptKeys.Contains(key))
                throw new AssertionException(String.Format("Key '{0}' not kept.", key));
        }
    }
}
