using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckLib
{
    public class SingletonOption
    {
        public interface IOption { }
        public sealed class AutoCreate : IOption { }
        public sealed class NotAutoCreate : IOption { }
    }
}
