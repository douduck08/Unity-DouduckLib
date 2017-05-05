using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouduckGame {
    public interface IModular {
        void ModuleInitialize ();
        void ModuleUninitialize ();
    }
}