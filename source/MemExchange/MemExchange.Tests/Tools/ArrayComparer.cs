using System.Collections;

namespace MemExchange.Tests.Tools
{
    public class ArrayComparer
    {
        public static bool AreEqual<T>(T a1, T a2)
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(a1, a2);
        }
    }
}
