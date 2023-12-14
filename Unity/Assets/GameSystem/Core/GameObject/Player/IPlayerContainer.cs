using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerContainer 
{
    IPlayer? Player { get; }
    event Action<IPlayer?> OnPlayerChanged;
    void SetPlayer(IPlayer? player);
}