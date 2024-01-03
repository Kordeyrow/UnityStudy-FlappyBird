using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderOwnerContainer : MonoBehaviour, IColliderOwnerContainer
{
    [SerializeField] GameObject owner;

    public GameObject Owner => owner;
}
