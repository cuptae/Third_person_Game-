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
    public Vector3 dirNormalized; // ����
    public Vector3 finalDir; //���� ������ ����
    public float minDistance; //ī�޶�� Ÿ�ٰ��� �ּҰŸ�
    public float maxDistance; //ī�޶�� Ÿ�ٰ��� �ִ�Ÿ�
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
        //���� �������� ȸ���ϴ� ���̱� ������ ���콺 ������ ��� �ݴ� �������� �̵� ���� rotX rotY�� ���� �ݴ�Ǵ� ���콺�� 
        //������ ����

        //�����̼� �ִ밪 ���
        //Mathf.Clamp(��,�ּҰ�,�ִ밪)
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
        transform.rotation = rot;
    }
    private void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, Target.position , FollowSpeed * Time.deltaTime);

        //TransformPoint : Ʈ�������� �������� ���������ǿ��� ���� �����̽��� �ٲ���
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
