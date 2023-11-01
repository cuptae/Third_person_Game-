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
        TRACE,
        ATTACK,
        DIE
    }
    public State state = State.PATROL;

    private Transform playerTr;
    private Transform enemyTr;

    public float attackDist = 5.0f;

    public float traceDist = 10.0f;

    public bool isDie = false;

    private WaitForSeconds ws;

    private MonsterMoveAgent moveAgent;
    private void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("Player");

        if(player != null)
        {
            playerTr = player.GetComponent<Transform>();
        }
        enemyTr = GetComponent<Transform>();
        moveAgent= GetComponent<MonsterMoveAgent>();
        ws = new WaitForSeconds(0.3f);
    }

    private void OnEnable()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());
    }

    IEnumerator CheckState()
    {
        while(!isDie)
        {
            if(state == State.DIE)
            {
                yield break;
            }

            float dist = Vector3.Distance(playerTr.position, enemyTr.position); 

            if(dist<=attackDist)
            {
                state = State.ATTACK;
            }
            else if(dist<=traceDist)
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
    IEnumerator Action()
    {
        while(!isDie)
        {
            yield return ws;

            switch(state)
            {
                case State.PATROL:
                    moveAgent.patrolling = true;
                    break;
                case State.TRACE:
                    moveAgent.traceTarget = playerTr.position;
                    break;
                case State.ATTACK:
                    moveAgent.Stop();
                    break;
                case State.DIE:
                    moveAgent.Stop();
                    break;
            }
        }
    }
}
