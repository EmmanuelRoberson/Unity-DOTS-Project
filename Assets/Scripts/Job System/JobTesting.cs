using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class JobTesting : MonoBehaviour
{
    private void Update()
    {
        float startTime = Time.realtimeSinceStartup;
        
       JobHandle reallyToughTaskJobHandle =  ReallyToughTaskJob();
       reallyToughTaskJobHandle.Complete();//pauses the main thread unitl job jhss been completer
       
        Debug.Log(((Time.realtimeSinceStartup - startTime) * 1000f) + "ms");
    }

    private JobHandle ReallyToughTaskJob()
    {
        ReallyToughJob job = new ReallyToughJob();
        
        //schedule tells the job system to schedule this job to be completed by an available thread when possible
        //it also returns the job handle which is important to keep track of
        return job.Schedule();

    }
}

public struct ReallyToughJob : IJob
{
    public void Execute()
    {
        //this is meant to represent a tough task, like pathfinding or complex calculation
        float value = 0f;
        for (int i = 0; i < 50000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }
}
