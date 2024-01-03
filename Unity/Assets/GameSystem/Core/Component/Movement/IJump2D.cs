using System;
using UnityEngine;

namespace UnityLayer
{
    public interface IJump2D
    {
        void Execute();
        event Action OnExecute;
    }
}