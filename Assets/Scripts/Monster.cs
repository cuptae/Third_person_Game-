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

    public int Patrolradius = 5;
    
    public bool isMove;

    private State state;
    private Animator _animator;

    Rigidbody rigid;
    private void Start()
    {
        previousPosition= transform.position;
        SpawnPoint = transform.position;
        rigid = GetComponent<Rigidbody>();
        _animator= GetComponent<Animator>();
        state = State.Peace;
        if(_Data!=null)
        {
            Name = _Data.Monster_Name;
            speed = _Data.Speed;
            runspeed = _Data.RunSpeed;
            sight= _Data.Sight;
            Hp =_Data.HP; 
            Damage = _Data.Damage;
        }      
        StartCoroutine(Patrol());

    }
    private void Update()
    {
        MoveAnimation();
    }
    private void FixedUpdate()
    {
        Rotation();
    }


    IEnumerator Patrol()
    {
        //while (state == State.Peace && _Data.Type != type.Boss)
        //{
        //    Vector3 targetPosition;

        //    targetPosition = new Vector3(Random.Range(-Patrolradius, Patrolradius), 0, Random.Range(-Patrolradius, Patrolradius)) + transform.position;
        //    while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        //    {
        //        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        //        yield return null;
        //    }
        //    yield return new WaitForSeconds(PatrolDelay);

        //    // targetPosition을 다시 설정하지 않고 계속해서 패트롤을 수행합니다.
        //}
        while (state == State.Peace && _Data.Type != type.Boss)
        {
            Vector3 Movepoint = Random.insideUnitSphere * Patrolradius;
            Movepoint += transform.position;

            // 레이캐스트를 발사하여 유효한 목표 지점을 찾습니다.
            RaycastHit hit;
            if (Physics.Raycast(Movepoint, Vector3.down, out hit, Mathf.Infinity))
            {
                // 레이에 닿은 지점을 목표 지점으로 설정합니다.
                Vector3 targetPosition = hit.point;

                while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, finalspeed * Time.deltaTime);
                    yield return null;
                }
            }
            else
            {
                // 레이에 아무 것도 닿지 않은 경우, 현재 위치에서 약간 위로 올려서 이동하게 됩니다.
                Vector3 targetPosition = transform.position + new Vector3(0, 0.1f, 0);

                while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
                    yield return null;
                }
            }

            if (Vector3.Distance(transform.position, SpawnPoint) > Patrolradius)
            {
                while (Vector3.Distance(transform.position, SpawnPoint) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, SpawnPoint, speed * Time.deltaTime);
                    yield return null;
                }
            }

            yield return new WaitForSeconds(PatrolDelay);
            
        }
    }

    private void Rotation()
    {
         currentPosition= transform.position;

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
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.fixedDeltaTime);
        }

        // 현재 위치를 previousPosition으로 업데이트
        previousPosition = currentPosition;
    }

    private void MoveAnimation()
    {
        if(state == State.Peace)
        {
           finalspeed = speed;
            
        }
        else if(state == State.Battle)
        {
            finalspeed = runspeed;
        }

        if(state == State.Battle)
        {
            
        }

    }
    private void Search()
    {
   
    }
}
