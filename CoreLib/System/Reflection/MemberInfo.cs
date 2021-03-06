////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Apache License 2.0 (Apache)
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////namespace System.Reflection
namespace System.Reflection
{

    using System;

    [Serializable()]
    public abstract class MemberInfo
    {
        public abstract MemberTypes MemberType
        {
            get;
        }

        public abstract String Name
        {
            get;
        }

        public abstract Type DeclaringType
        {
            get;
        }
    }
}


