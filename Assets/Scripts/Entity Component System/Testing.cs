
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Rendering;
using Random = UnityEngine.Random;

public class Testing : MonoBehaviour
{
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;
    private void Awake()
    {
        EntityManager entityManager = World.Active.EntityManager;

        //making archetypes
        EntityArchetype entityArchetype = entityManager.CreateArchetype(
            typeof(LevelComponent),
            typeof(Translation),
            typeof(RenderMesh),
            typeof(LocalToWorld), //used by renderer to calculate how it should be viewed
            typeof(MoveSpeedComponent)
            );
        
        
        NativeArray<Entity> entityArray = new NativeArray<Entity>(50000, Allocator.Temp); 
        entityManager.CreateEntity(entityArchetype, entityArray);
        
        //goes through the native array and sets the component data
        for (int i = 0; i < entityArray.Length; i++)
        {
            Entity entity = entityArray[i];
            entityManager.SetComponentData(entity, new LevelComponent {level = Random.Range(10, 20) });
            entityManager.SetComponentData(entity, new MoveSpeedComponent{ moveSpeed = Random.Range(1f, 2f) });
            entityManager.SetComponentData(entity,
                new Translation {Value = new float3(Random.Range(-8, 8f), Random.Range(-5, 5f), 0)});
            
            entityManager.SetSharedComponentData(entity,
                new RenderMesh { mesh = _mesh, material = _material });
        }
        
        //dispose when done with native array
        entityArray.Dispose();
    }
}
