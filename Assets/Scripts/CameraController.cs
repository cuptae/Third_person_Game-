using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Target;
    public float FollowSpeed=10;
    public float Sensitivity=100f;
    public float clampAngle = 70f;

    private float rotX;
    private float rotY;

    public Transform realCamera;
    public Vector3 dirNormalized; // 방향
    public Vector3 finalDir; //최종 정해진 방향
    public float minDistance; //카메라와 타겟간의 최소거리
    public float maxDistance; //카메라와 타겟간의 최대거리
    public float finalDistance;
    public float smoothness = 10f;

    private void Start()
    {
        rotX = transform.localRotation.eulerAngles.x;
        rotY = transform.localRotation.eulerAngles.y;

        dirNormalized = realCamera.localPosition.normalized;
        finalDistance = realCamera.localPosition.magnitude;

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
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
        transform.rotation = rot;
    }
    private void LateUpdate()
    {
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
