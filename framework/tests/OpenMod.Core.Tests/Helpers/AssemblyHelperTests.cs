using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OpenMod.Core.Tests.Helpers
{
    [TestClass]
    public class AssemblyHelperTests
    {
        private const string c_CosturaResourceNameStart = "costura.";
        private const string c_CosturaResourceNameEnd = ".dll.compressed";
        private const int c_MinCosturaResourceNameCharactersLenght = 19;

        [TestMethod]
        [DataRow("costura.microsoft.extensions.caching.abstractions.dll.compressed", true)]
        [DataRow("costura.microsoft.extensions.caching.abstractions", false)]
        [DataRow("microsoft.extensions.caching.abstractions.dll.compressed", false)]
        public void IsCosturaResource_AreEqual_WhenExpectedResultSameAsResult(string resourceName, bool expectedResult)
        {
            // Act.
            var result = IsCosturaResource(resourceName);

            // Assert.
            Assert.AreEqual(expectedResult, result);
        }

        private static bool IsCosturaResource(string resourceName)
        {
            if (!string.IsNullOrWhiteSpace(resourceName))
            {
                if (resourceName.Length > c_MinCosturaResourceNameCharactersLenght
                    && resourceName.StartsWith(c_CosturaResourceNameStart)
                    && resourceName.EndsWith(c_CosturaResourceNameEnd))
                {
                    return true;
                }
            }
            return false;
        }
    }
}