using System;

namespace ExampleCodeGenApp.ViewModels
{
    [Flags]
    public enum PortDataType
    {
        Unknown = 0,
        Number = 1 << 0,
        String = 1 << 1,
        Boolean = 1 << 2,
        Measurement = 1 << 3,
        Part = 1 << 4,

        NumberCollection = Number | Collection,
        StringCollection = String | Collection,
        BooleanCollection = Boolean | Collection,
        MeasurementCollection = Measurement | Collection,
        PartCollection = Part | Collection,

        Collection = 1 << 10,
    }

    public static class PortDataTypeExtensions
    {
        public static bool IsCollection(this PortDataType a)
        {
            return a.HasFlag(PortDataType.Collection);
        }
    }
}
