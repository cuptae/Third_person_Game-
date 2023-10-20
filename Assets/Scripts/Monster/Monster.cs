using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public enum State
{
    Battle,
    Peace
}

public class Monster : MonoBehaviour
{
    [SerializeField]
    private MonsterData _Data;

    public string Name;

    public float speed;
    public float runspeed;
    public float finalspeed;
    public float sight;
    public float Hp;
    public float Damage;
    public float PatrolDelay = 2.0f;

    private Vector3 SpawnPoint;
    private Vector3 previousPosition;
    private Vector3 currentPosition;

    public int MaxPatrolradius = 5;
    public int MinPatrolradius = 3;

    public bool isMove;
    public bool isPatrol;

    private State state;
    public type Type;

    private Animator _animator;
    private Transform _transform;


    Rigidbody rigid;
    private void Start()
    {
        _transform = GetComponent<Transform>();
        previousPosition = transform.position;
        SpawnPoint = transform.position;
        rigid = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        state = State.Peace;   
        if (_Data != null)
        {
            Name = _Data.Monster_Name;
            speed = _Data.Speed;
            runspeed = _Data.RunSpeed;
            sight = _Data.Sight;
            Hp = _Data.HP;
            Damage = _Data.Damage;
            Type = _Data.Type;
        }
        StartCoroutine(Patrol());
    }
    private void Update()
    {
    }
    private void FixedUpdate()
    {
        Rotation();
    }
    IEnumerator Patrol()
    {
        if (Type != type.Boss)
        {
            while (state == State.Peace)
            {
                Vector3 randomDirection = Random.insideUnitSphere * MaxPatrolradius;
                randomDirection += SpawnPoint;

                //바닥 랜덤한 위치에 ray를 발사하여 그곳으로 이동
                RaycastHit hit;

                if (Vector3.Distance(transform.position, SpawnPoint) < MaxPatrolradius)
                {
                    if (Physics.Raycast(randomDirection, Vector3.down, out hit, Mathf.Infinity))
                    {
                        Vector3 targetPosition = hit.point;
                        Debug.DrawLine(randomDirection, hit.point, Color.green);
                        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                        {
                            rigid.MovePosition(Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime));
                            yield return null;
                        }
                    }
                    else
                    {
                        Vector3 targetPosition = transform.position + new Vector3(0, 0.1f, 0);
                        Debug.DrawLine(randomDirection, hit.point, Color.green);
                        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                        {
                            rigid.MovePosition(Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime));
                            yield return null;
                        }
                    }
                }
                yield return new WaitForSeconds(PatrolDelay);
            }
        }
    }
    private void Rotation()
    {
        currentPosition = transform.position;
        // 현재 위치를 previousPosition으로 업데이트
        previousPosition = currentPosition;

        // 이동 방향 벡터 계산
        Vector3 movementDirection = (currentPosition - previousPosition).normalized;
        // 움직임이 없는 경우 회전하지 않음
        if (movementDirection != Vector3.zero)
        {
            // 이동 방향을 바탕으로 목표 회전 각도 계산
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);

            // Y 축 회전만 사용하고 X 및 Z 회전을 고정
            targetRotation.eulerAngles = new Vector3(0f, targetRotation.eulerAngles.y, 0f);

            // 부드러운 회전 적용
            rigid.MoveRotation(targetRotation); // Rigidbody의 MoveRotation 사용
        }
    }
}