using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Jobs;
using Unity.Jobs;

public class SpawnParallel : MonoBehaviour
{
    public GameObject sheepPrefab;

    public Transform[] allSheepTransforms;

    struct MoveJob : IJobParallelForTransform
    {
        public void Execute(int index, TransformAccess transform)
        {
            //no transform.position in here... so, we do the math
            transform.position += 0.1f * (transform.rotation * new Vector3(0, 0, 1));
            if (transform.position.z > 50)
            {
                transform.position = new Vector3(transform.position.x, 0, -50);
            }
        }
    }

    MoveJob moveJob;
    JobHandle moveHandle;
    TransformAccessArray transforms;

    const int numsheep = 15000;

    // Start is called before the first frame update
    void Start()
    {
        allSheepTransforms = new Transform[numsheep];

        for (int i = 0; i < numsheep; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
            GameObject sheep  = Instantiate(sheepPrefab, pos, Quaternion.identity);
            allSheepTransforms[i] = sheep.transform;
        }

        transforms = new TransformAccessArray(allSheepTransforms);
    }

    private void Update()
    {
        moveJob = new MoveJob { };
        moveHandle = moveJob.Schedule(transforms);
    }

    private void LateUpdate()
    {
        moveHandle.Complete();
    }
    private void OnDestroy()
    {
        transforms.Dispose();
    }

}


