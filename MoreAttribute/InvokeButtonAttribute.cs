using UnityEngine;
using System;

// Ref: http://c.atch.co/invoke-a-method-right-from-a-unity-inspector/
namespace DouduckLib {
    [AttributeUsage (AttributeTargets.Method)]
    public class InvokeButtonAttribute : PropertyAttribute { }
}