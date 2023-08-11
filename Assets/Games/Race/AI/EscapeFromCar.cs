using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class EscapeFromCar : Agent
{
    [SerializeField] private GameObject _car;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Animator _amogusAnimator;
    [SerializeField] private AudioSource _AmogusDeath;
    [SerializeField] private float rotationDemo;
    private RayPerceptionSensorComponent3D _rayPerceptionSensor;
    private float _startEpisodeTime;
    private float _distanceBetweenCarAndAgent;
    private float _angleBetweenCarAndAgent;
    [SerializeField] private bool train;
    //private int _animIDSpeed;

    public override void CollectObservations(VectorSensor sensor)
    {
        _distanceBetweenCarAndAgent = Vector3.Distance(_car.transform.position, transform.position);
        //Debug.Log(_distanceBetweenCarAndAgent + " — DISTANCE");
        sensor.AddObservation(_distanceBetweenCarAndAgent);
        Vector3 directionToTarget = _car.transform.position - transform.position;
        _angleBetweenCarAndAgent = Vector3.SignedAngle(transform.forward, directionToTarget, transform.up);
        sensor.AddObservation(_angleBetweenCarAndAgent);

        /*if (train)
        {
            sensor.AddObservation(_moveSpeed);
        } else
        {
            sensor.AddObservation(1.35f);
        }*/
        
        var rayOutputs = RayPerceptionSensor.Perceive(_rayPerceptionSensor.GetRayPerceptionInput()).RayOutputs;
        int lengthOfRayOutputs = rayOutputs.Length;
        float[] rayHitDistances = new float[lengthOfRayOutputs];
        for (int i = 0; i < lengthOfRayOutputs; i++)
        {
            GameObject goHit = rayOutputs[i].HitGameObject;
            if (goHit != null)
            {
                var rayDirection = rayOutputs[i].EndPositionWorld - rayOutputs[i].StartPositionWorld;
                var scaledRayLength = rayDirection.magnitude;
                float rayHitDistance = rayOutputs[i].HitFraction * scaledRayLength;
                rayHitDistances[i] = rayHitDistance;
            }
            else
            {
                rayHitDistances[i] = 1000;
            }
        }
        sensor.AddObservation(rayHitDistances);
    }

    private IEnumerator TeleportCarForTraining()
    {
        yield return new WaitForSeconds(Random.Range(12, 26));
        _car.transform.position = new Vector3(_car.transform.position.x + (3.5f * (transform.position.x - _car.transform.position.x)), -2.5f, _car.transform.position.z + (3.5f * (transform.position.z - _car.transform.position.z)));
        StartCoroutine(TeleportCarForTraining());
    }

    private void Start()
    {
        _amogusAnimator.SetFloat("Speed", _moveSpeed);
        _rayPerceptionSensor = GetComponent<RayPerceptionSensorComponent3D>();
        if (train)
        {
            
            //_animIDSpeed = Animator.StringToHash("Speed");
            _startEpisodeTime = 0;
            StartCoroutine(RewardByTime());
            StartCoroutine(CheckForHitAndAddPenalty());
            //StartCoroutine(TeleportCarForTraining());
        } else
        {
            StartCoroutine(AgentForsedRotation());
        }
    }

    private IEnumerator AgentForsedRotation()
    {
        yield return new WaitForSeconds(0.05f);
        rotationDemo = _angleBetweenCarAndAgent;
        if (_angleBetweenCarAndAgent > -90 & _angleBetweenCarAndAgent < 90)
        {
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y + 180f, 0f);
            
        }
        else
        {

            var rayOutputs = RayPerceptionSensor.Perceive(_rayPerceptionSensor.GetRayPerceptionInput()).RayOutputs;

            //bool 

            GameObject goHit = rayOutputs[0].HitGameObject;
            if (goHit != null)
            {
                var rayDirection = rayOutputs[0].EndPositionWorld - rayOutputs[0].StartPositionWorld;
                var scaledRayLength = rayDirection.magnitude;
                float rayHitDistance = rayOutputs[0].HitFraction * scaledRayLength;
                if (rayHitDistance < 3.5)
                {
                    float rotateYFor;
                    if (_angleBetweenCarAndAgent > 0)
                    {
                        if (_angleBetweenCarAndAgent > 160)
                        {
                            rotateYFor = -130;
                        }
                        else
                        {
                            rotateYFor = -60;
                        }
                    }
                    else
                    {
                        if (_angleBetweenCarAndAgent < -160)
                        {
                            rotateYFor = 130;
                        }
                        else
                        {
                            rotateYFor = 60;
                        }
                    }
                    transform.rotation = Quaternion.Euler(0f, rotateYFor, 0f);
                }
            }
        }
        StartCoroutine(AgentForsedRotation());
    }

    private IEnumerator RewardByTime()
    {
        yield return new WaitForSeconds(0.25f);

        if (_angleBetweenCarAndAgent > 90 | _angleBetweenCarAndAgent < -90)
        {
            AddReward(_distanceBetweenCarAndAgent / 10);
        }
        _startEpisodeTime = _startEpisodeTime + 0.25f;
        StartCoroutine(RewardByTime());

    }


    private void Update()
    {
        if (_distanceBetweenCarAndAgent > 300) 
        {
            _amogusAnimator.SetFloat("Speed", 0);
            return;
        }
        
        _amogusAnimator.SetFloat("Speed", _moveSpeed);
        transform.Translate(Vector3.forward * _moveSpeed * Time.deltaTime);


    }

    public override void OnEpisodeBegin()
    {
        if (train)
        {
            _startEpisodeTime = 0;

            transform.localPosition = new Vector3(Random.Range(-1100, 150), -3f, 405 /*Random.Range(390, 421)*/);
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            Vector3 randomDirection = Random.insideUnitSphere;
            Vector3 spawnPosition = transform.position + new Vector3(randomDirection.x * 60, 0, randomDirection.x * 13);
            _car.transform.position = spawnPosition/*new Vector3(Random.Range(-1100, 150), -3f, Random.Range(390, 421))*/;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> actionSegment = actionsOut.ContinuousActions;

        if (Input.GetKey(KeyCode.RightArrow))
        {
            actionSegment[0] = 1f;
            Debug.Log("Right");
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            actionSegment[0] = -1f;
            Debug.Log("Left");
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.Equals(_car.name))
        {
            //Debug.Log(hallo);
            if (train)
            {
                EndEpisodeForAI();
            }
            else
            {
                Rigidbody rbCar = _car.GetComponent<Rigidbody>();
                rbCar.velocity = rbCar.velocity * 4 / 5;
                _AmogusDeath.Play();
                Destroy(gameObject);
            }
        } 
    }

    private void EndEpisodeForAI()
    {
        /*            float episodeDuration = Time.time - _startEpisodeTime;*/
        AddReward(-100 / _startEpisodeTime);
        Debug.Log(GetCumulativeReward() + "   ");
        EndEpisode();
        
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (_distanceBetweenCarAndAgent > 300)
        {
            return;
        }
        float rotateY = actions.ContinuousActions[0] * 90f * Time.deltaTime + transform.eulerAngles.y;
        //Debug.Log(actions.ContinuousActions[0] + "     " + actions.ContinuousActions[0] * 80f * Time.deltaTime + "     " + rotateY);

        //Debug.Log(actions.ContinuousActions[0] + "  " + rotateY);
        transform.rotation = Quaternion.Euler(0f, rotateY, 0f);
        //transform.Rotate(0, rotateY, 0);
        //transform.rotation = Quaternion.Euler(0f, transform.rotation.y, 0f);

        //CheckForHitAndAddPenalty();
    }

    private IEnumerator CheckForHitAndAddPenalty()
    {
        var rayOutputs = RayPerceptionSensor.Perceive(_rayPerceptionSensor.GetRayPerceptionInput()).RayOutputs;
        int lengthOfRayOutputs = rayOutputs.Length;
        bool slow = false;
        for (int i = 0; i < lengthOfRayOutputs; i++)
        {
            GameObject goHit = rayOutputs[i].HitGameObject;
            if (goHit != null)
            {
                var rayDirection = rayOutputs[i].EndPositionWorld - rayOutputs[i].StartPositionWorld;
                var scaledRayLength = rayDirection.magnitude;
                float rayHitDistance = rayOutputs[i].HitFraction * scaledRayLength;


                string dispStr = "";
                dispStr = dispStr + "__RayPerceptionSensor - HitInfo__:\r\n";
                dispStr = dispStr + "GameObject name: " + goHit.name + "\r\n";
                dispStr = dispStr + "Hit _distanceBetweenCarAndAgent of Ray: " + rayHitDistance + "\r\n";
                dispStr = dispStr + "GameObject tag: " + goHit.tag + "\r\n";
                
                
                    /*if (rayHitDistance < 3f)
                    {*/
                    
                if (rayHitDistance < 4f)
                {
                    AddReward(-0.1f);
                    slow = true;
                }
                    //}

                    //AddReward(-(0.02f / rayHitDistance));
                    //Debug.Log("hit " + -(0.5f / rayHitDistance));
                    //EndEpisode();
                
            }
        }
        if (slow)
        {
            _moveSpeed = 1f;
        }
        else
        {
            _moveSpeed = 1.35f;
        }
        yield return new WaitForSeconds(0.75f);
        StartCoroutine(CheckForHitAndAddPenalty());
    }
    /*
      SetReward(+-1f);
      EndEpisode();
     */
}
