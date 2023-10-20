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
        //�������� ����������� �ӵ��� ���̴� �ɼ��� ��Ȱ��
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
        //NavMeshAgent�� �̵��ϰ� �ְ� �������� �����ߴ��� ���θ� �Ի�
        if(agent.velocity.sqrMagnitude>=0.2f*0.2f&&agent.remainingDistance<=0.5f)
        {
            //���� �������� �迭 ÷�ڸ� ���
            nextIdx = ++nextIdx % wayPoints.Count;
            //���� �������� �̵� ����� ����
            MoveWayPoint();
        }

    }

    private void MoveWayPoint()
    {
        //�ִܰŸ� ��ΰ���� ������ �ʾ����� ������ �������� ����
        if (agent.isPathStale) return;

        //���� �������� wayPoints �迭���� ������ ��ġ�� ���� �������� ����
        agent.destination = wayPoints[nextIdx].position;
        //�׺���̼� ����� Ȱ��ȭ�ؼ� �̵��� ������
        agent.isStopped = false;
    }
}
