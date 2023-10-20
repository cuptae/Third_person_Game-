using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    public enum State
    {
        PATROL,
        IDLE,
        TRACE,
        ATTACK,
        DIE
    }
    public State state = State.IDLE;

    private Transform trasnform;
    private Transform playerTransform;

    public float attackDistance = 1.0f;
    public float traceDistance = 10.0f;
    
    public bool isDie = false;

    private WaitForSeconds ws;

    private void Awake()
    {
        var player = GameObject.FindGameObjectsWithTag("Player");

        if(player !=null)
        {
            playerTransform = GetComponent<Transform>();
        }

        trasnform= GetComponent<Transform>();

        ws = new WaitForSeconds(0.3f);
    }
    //OnEnable: 스크립트가 활성화 될 떄마다 실행되는 함수 Awake다음에 호출 Start보다 먼저 호출
    private void OnEnable()
    {
        StartCoroutine(CheckState());
    }
    IEnumerator CheckState()
    {
        while(!isDie)
        {
            if (state == State.DIE) yield break;

            float dist = Vector3.Distance(playerTransform.position, trasnform.position);

            if(dist <= attackDistance)
            {
                state = State.ATTACK;
            }
            else if(dist <= traceDistance)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.PATROL;
            }

            yield return ws;
        }
    }
}
