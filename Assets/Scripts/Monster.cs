using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum State
{
   Battle,
   Peace
}
public class Monster : MonoBehaviour
{
    [SerializeField]
    private MonsterData Data;

    public string Name;

    public float speed;
    public float sight;
    public float Hp;
    public float Damage;
    public float PatrolDelay = 2.0f;

    private Vector3 PatrolPoint;
    private Vector3 SpawnPoint;
    private Vector3 previousPosition;
    public float Patrolradius = 5.0f;
    
    public bool isMove;

    private State state;
    private Animator _anim;
    LayerMask monsterLayer;

    Rigidbody rigid;
    private void Start()
    {
        SpawnPoint = transform.position;
        rigid = GetComponent<Rigidbody>();
        _anim= GetComponent<Animator>();
        state = State.Peace;
        if(Data!=null)
        {
            Name = Data.Monster_Name;
            speed = Data.Speed;
            sight= Data.Sight;
            Hp =Data.HP; 
            Damage = Data.Damage;
        }
        monsterLayer = LayerMask.GetMask("Monster");
        StartCoroutine(Patrol());

    }
    private void Update()
    {
        MoveAnimation();
        Rotation();
    }

    private void FixedUpdate()
    {
        
    }



    IEnumerator Patrol()
    { 
        while(state == State.Peace)
        {
            PatrolPoint = new Vector3(Random.Range(-Patrolradius, Patrolradius), 0, Random.Range(-Patrolradius, Patrolradius));
            PatrolPoint += transform.position;
            while (Vector3.Distance(transform.position, PatrolPoint) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, PatrolPoint, speed * Time.deltaTime);
                yield return null;
            }
            if (Vector3.Distance(transform.position, SpawnPoint) > Patrolradius)
            {
                transform.position = Vector3.MoveTowards(transform.position, PatrolPoint, speed * Time.deltaTime);
            }
            yield return new WaitForSeconds(PatrolDelay);
        }       
    }

    private void MoveAnimation()
    {
        if(rigid.velocity.magnitude<=-0.1)
        {
            isMove= true;
            _anim.SetBool("IsMove", true);
        }
        else
        {
            isMove= false;
            _anim.SetBool("IsMove", false);
        }
    }
    private void Rotation()
    {
        Vector3 lookDir = PatrolPoint - transform.position;
        lookDir.y = 0; // y 방향 회전을 고려하지 않음

        if (lookDir != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 10 * Time.deltaTime);
        }
    }

    private void Search()
    {
   
    }
}
