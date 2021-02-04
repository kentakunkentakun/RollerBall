using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
public class RollerAgentRun : Agent
{
    Rigidbody rBody;
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        GetComponent<Renderer>().material.color=Color.blue;
    }

    public Transform Target;
    public int step = 0;
    public override void OnEpisodeBegin()
    {   
        step = 0;
        if(this.transform.localPosition.y<0)
        {
            this.rBody.angularVelocity=Vector3.zero;
            this.rBody.velocity=Vector3.zero;
            this.transform.localPosition=new Vector3(0,0.5f,0);
        }
        Target.localPosition = new Vector3(Random.value*8-4,0.5f,Random.value*8-4);
    }
public override void CollectObservations(VectorSensor sensor)
{
    // Target and Agent positions
    sensor.AddObservation(Target.localPosition);
    sensor.AddObservation(this.transform.localPosition);
    // Agent velocity
    sensor.AddObservation(rBody.velocity.x);
    sensor.AddObservation(rBody.velocity.z);
}
public float forceMultiplier = 10;
public override void OnActionReceived(ActionBuffers actionBuffers)
{   
    step += 1;
    // Actions, size = 2
    Vector3 controlSignal = Vector3.zero;
    controlSignal.x = actionBuffers.ContinuousActions[0];
    controlSignal.z = actionBuffers.ContinuousActions[1];
    rBody.AddForce(controlSignal * forceMultiplier);
    // Rewards
    float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);
    // Reached target
    if (distanceToTarget < 1.42f)
    {
        SetReward(-1.0f);
        EndEpisode();
    }
    // Fell off platform
    else if (this.transform.localPosition.y < 0)
    {
        EndEpisode();
    }

    else if(step == 1000)
    {
        SetReward(1.0f);
        EndEpisode();
    }
}
}