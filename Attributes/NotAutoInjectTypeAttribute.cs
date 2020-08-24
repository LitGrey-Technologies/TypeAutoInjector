using System;

namespace TypeAutoInjector.Attributes
{
    public class NotAutoInjectTypeAttribute : Attribute
    {
        private readonly Type _type;

        public NotAutoInjectTypeAttribute(Type type)
        {
            _type = type;
        }
    }
}