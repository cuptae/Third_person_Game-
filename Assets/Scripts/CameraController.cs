using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Target; //카메라가 따라갈 타겟
    public float FollowSpeed=10; //카메라가 따라갈 속도
    public float Sensitivity=100f; //카메라 민감도
    public float clampAngle = 70f; // 카메라 최대 각도

    private float rotX; //카메라 Y회전축
    private float rotY; //카메라 X회전축

    public Transform realCamera; //Main Camera
    public Vector3 dirNormalized; //카메라의 방향
    public Vector3 finalDir; //최종 정해진 방향
    public float minDistance; //카메라와 타겟간의 최소거리
    public float maxDistance; //카메라와 타겟간의 최대거리
    public float finalDistance; //최종 거리
    public float smoothness = 10f; //카메라

    private void Start()
    {
        rotX = transform.localRotation.eulerAngles.x; //현재 게임 오브젝트의 X 축 로컬 회전 각도를 rotX에 저장
        rotY = transform.localRotation.eulerAngles.y; //현재 게임 오브젝트의 Y 축 로컬 회전 각도를 rotT에 저장

        dirNormalized = realCamera.localPosition.normalized; //realCamera의 로컬 포지션 벡터를 변수에 저장하고 정규화 하여 방향 단위 벡터를 구함
        finalDistance = realCamera.localPosition.magnitude; //realCamera의 로컬포지션 벡터의 길이를 구함

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        rotX += -(Input.GetAxis("Mouse Y")) * Sensitivity * Time.deltaTime; 
        rotY += Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime;
        //축을 기준으로 회전하는 것이기 때문에 마우스 방향은 축과 반대 방향으로 이동 따라서 rotX rotY는 서로 반대되는 마우스의 
        //방향을 받음

        //로테이션 최대값 계산
        //Mathf.Clamp(값,최소값,최대값)
        rotX = Mathf.Clamp(rotX, -clampAngle-20, clampAngle);

        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
        transform.rotation = rot;
    }
    private void LateUpdate()
    {
        //타겟을 따라가게 함
        transform.position = Vector3.MoveTowards(transform.position, Target.position , FollowSpeed * Time.deltaTime);

        //TransformPoint : 트랜스폼의 포지션을 로컬포지션에서 월드 스페이스로 바꿔줌
        finalDir = transform.TransformPoint(dirNormalized * maxDistance);

        RaycastHit hit;

        if(Physics.Linecast(transform.position,finalDir,out hit))
        {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            finalDistance = maxDistance;
        }
        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalized * finalDistance, Time.deltaTime * smoothness);
    }
}
