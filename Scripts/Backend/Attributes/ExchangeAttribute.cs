using System;

namespace Armageddon.Backend.Attributes
{
    public class ExchangeAttribute : Attribute
    {
        // /// <summary>
        // ///     Remove suffix Object at backend side.
        // /// </summary>
        // public bool RemoveSuffixObject;

        public bool AssignEnumValue;
        public bool AddConvertExtension = true;
    }
}
