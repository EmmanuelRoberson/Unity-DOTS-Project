using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

//components are meant to only hold data, not behaviour
public struct LevelComponent : IComponentData
{
    public float level;
}
