using System;

namespace Armageddon.Backend.Attributes
{
    public class FieldAttribute : Attribute
    {
        public string CustomType;

        // No longer needed as we use C# instead of TypeScript
        [Obsolete("No longer needed as we use C# instead of TypeScript")]
        public bool Optional;
    }
}
