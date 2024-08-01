using System;

namespace Kadinche.Kassets.Variable.Sample
{
    public class CustomVariable : JsonableVariable<CustomStruct>
    {
    }

    [Serializable]
    public struct CustomStruct
    {
        public bool boolField;
        public int intField;
        public float floatField;
        public string stringField;
    }
}