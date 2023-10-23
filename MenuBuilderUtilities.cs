using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CC.SoundSystem
{
    public static class MenuBuilderUtilities
    {

        [System.Serializable]
        public class MenuComponentException : System.Exception
        {
            public MenuComponentException() { }
            public MenuComponentException(string message) : base(message) { }
            public MenuComponentException(string message, System.Exception inner) : base(message, inner) { }
            protected MenuComponentException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
    }
}
