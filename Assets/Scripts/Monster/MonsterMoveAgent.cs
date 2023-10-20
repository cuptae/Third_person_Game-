using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent))]
public class MonsterMoveAgent : MonoBehaviour
{
    public List<Transform> wayPoints;
    public int nextIdx;

    private NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent= GetComponent<NavMeshAgent>();
        //목적지에 가까워질수록 속도를 줄이는 옵션을 비활성
        agent.autoBraking = false;

        var group = GameObject.Find("WayPointGroup");
        if(group != null)
        {
            group.GetComponentsInChildren<Transform>(wayPoints);
            wayPoints.RemoveAt(0);
        }
        MoveWayPoint();
    }
    private void Update()
    {
        //NavMeshAgent가 이동하고 있고 목적지에 도착했는지 여부를 게산
        if(agent.velocity.sqrMagnitude>=0.2f*0.2f&&agent.remainingDistance<=0.5f)
        {
            //다음 목적지의 배열 첨자를 계산
            nextIdx = ++nextIdx % wayPoints.Count;
            //다음 목적지로 이동 명령을 수행
            MoveWayPoint();
        }

    }

    private void MoveWayPoint()
    {
        //최단거리 경로계산이 끝나지 않았으면 다음을 수행하지 않음
        if (agent.isPathStale) return;

        //다음 목적지를 wayPoints 배열에서 추출한 위치로 다음 목적지를 지정
        agent.destination = wayPoints[nextIdx].position;
        //네비게이션 기능을 활성화해서 이동을 시작함
        agent.isStopped = false;
    }
}
