using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Transforms;

public class GameHandler : MonoBehaviour
{

    public Mesh quadMesh;
    public Material walkingSpriteSheetMaterial;
    private void Awake()
    {
        EntityManager entityManager = World.Active.EntityManager;
        var entityArchetype = entityManager.CreateArchetype(typeof(Translation));

        Entity entity = entityManager.CreateEntity(entityArchetype);
    }
}
