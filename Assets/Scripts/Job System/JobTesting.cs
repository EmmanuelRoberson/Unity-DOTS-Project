using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
using Unity.Burst;
using UnityEngine.Jobs;

public class JobTesting : MonoBehaviour
{

    [SerializeField] private bool useJobs;
    [SerializeField] private Transform unit;
    private List<Unit> unitList;

    public class Unit
    {
        public Transform transform;
        public float moveY;
    }

    private void Start()
    {
        unitList = new List<Unit>();
        for (int i = 0; i < 1000; i++)
        {
            Transform unitTransform = Instantiate(unit,
                new Vector3(UnityEngine.Random.Range(-8f, 8f), UnityEngine.Random.Range(-5f, 5f)), Quaternion.identity);
            
            unitList.Add(new Unit(){transform = unitTransform, moveY = UnityEngine.Random.Range(1, 2f)});
        }
    }
    private void Update()
    {
        //float startTime = Time.realtimeSinceStartup;

        if (useJobs)
        {
            //var positionArray = new NativeArray<float3>(unitList.Count, Allocator.TempJob);
            var moveYArray = new NativeArray<float>(unitList.Count, Allocator.TempJob);
            var transformAccessArray = new TransformAccessArray(unitList.Count);

            for (int i = 0; i < unitList.Count; i++)
            {
                //positionArray[i] = unitList[i].transform.position;
                moveYArray[i] = unitList[i].moveY;
                transformAccessArray .Add(unitList[i].transform);
            }

            
            /*
            ReallyToughParallelJob rtpj = new ReallyToughParallelJob()
            {
                deltaTime = Time.deltaTime,
                positionArray = positionArray,
                moveYArray = moveYArray
            };
            
                        
            var parallelJobHandle = rtpj.Schedule(unitList.Count, 100);
            parallelJobHandle.Complete();
            */

            ReallyToughParallelJobTransforms reallyToughPJT = new ReallyToughParallelJobTransforms()
            {
                deltaTime = Time.deltaTime,
                moveYArray = moveYArray
            };

            var transformParallelJobHandle = reallyToughPJT.Schedule(transformAccessArray);
            transformParallelJobHandle.Complete();
            
            for (int i = 0; i < unitList.Count; i++)
            {
                //unitList[i].transform.position = positionArray[i];
                unitList[i].moveY = moveYArray[i];
            }
            
            //positionArray.Dispose();
            moveYArray.Dispose();
            transformAccessArray.Dispose();
            
            Debug.Log(useJobs);
        }
        else
        {
            foreach (var unit in unitList)
            {
                unit.transform.position += new Vector3(0, unit.moveY * Time.smoothDeltaTime);
                if (unit.transform.position.y > 5f)
                {
                    unit.moveY = -math.abs(unit.moveY);
                }

                if (this.unit.transform.position.y < -5f)
                {
                    unit.moveY = +math.abs(unit.moveY);
                }
            }
            
            Debug.Log(useJobs);
        }
        
        /*
        if (useJobs)
        {
            NativeList<JobHandle> jobHandleList = new NativeList<JobHandle>(Allocator.Temp);
            for (int i = 0; i < 10; i++)
            {
                JobHandle reallyToughTaskJobHandle = ReallyToughTaskJob();
                jobHandleList.Add(reallyToughTaskJobHandle);
            }
            
            JobHandle.CompleteAll(jobHandleList); //pauses the main thread unitl job has been completed
            jobHandleList.Dispose();
        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                ReallyToughTask();
            }
        }
        */

        //Debug.Log(((Time.realtimeSinceStartup - startTime) * 1000f) + "ms");
    }

    private void ReallyToughTask()
    {
        //this is meant to represent a tough task, like pathfinding or complex calculation
        float value = 0f;
        for (int i = 0; i < 50000; i++)
        {
            value = math.exp10(math.sqrt(value));
        }
    }
    
    private JobHandle ReallyToughTaskJob()
    {
        ReallyToughJob job = new ReallyToughJob();
        
        //schedule tells the job system to schedule this job to be completed by an available thread when possible
        //it also returns the job handle which is important to keep track of
        return job.Schedule();

    }
}

[BurstCompile]
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

[BurstCompile]
public struct ReallyToughParallelJob : IJobParallelFor
{
    public NativeArray<float> moveYArray;
    public NativeArray<float3> positionArray;
    [ReadOnly] public float deltaTime; //readonly attribure only makes sense when applied to native fields
                                        // it allows multiple different jobs to work concurrently on the same data
                                        // non-native fields are always just copies and not references
                                        //so in here adding readonly to deltaTiime is unnecessary
    
    public void Execute(int index)
    {
        positionArray[index] += new float3(0, moveYArray[index] * deltaTime, 0f);
        if (positionArray[index].y > 5f)
        {
            moveYArray[index] = -math.abs(moveYArray[index]);
        }

        if (positionArray[index].y < -5f)
        {
            moveYArray[index] = +math.abs(moveYArray[index]);
        }
    }
}

public struct ReallyToughParallelJobTransforms : IJobParallelForTransform
{
    public NativeArray<float> moveYArray;
    [ReadOnly] public float deltaTime;
    public void Execute(int index, TransformAccess transformAccess)
    {
        transformAccess.position += new Vector3(0, moveYArray[index] * deltaTime, 0f);
        
        if (transformAccess.position.y > 5f)
        {
            moveYArray[index] = -math.abs(moveYArray[index]);
        }

        if (transformAccess.position.y < -5f)
        {
            moveYArray[index] = +math.abs(moveYArray[index]);
        }
    }
}
