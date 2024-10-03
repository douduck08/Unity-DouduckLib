using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonOption
{
    public interface IOptopn { }
    public sealed class AutoCreate : IOptopn { }
    public sealed class NotAutoCreate : IOptopn { }
}
