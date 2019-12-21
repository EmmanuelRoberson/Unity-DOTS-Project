using Unity.Entities;
using UnityEngine;

public class LevelUpSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        //the code will run on every entity with the 'LevelComponent' component
        Entities.ForEach((ref LevelComponent levelComponent) => { levelComponent.level += 1f * Time.deltaTime; });
    }
}
