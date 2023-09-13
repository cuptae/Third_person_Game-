using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Target; //ī�޶� ���� Ÿ��
    public float FollowSpeed=10; //ī�޶� ���� �ӵ�
    public float Sensitivity=100f; //ī�޶� �ΰ���
    public float clampAngle = 70f; // ī�޶� �ִ� ����

    private float rotX; //ī�޶� Yȸ����
    private float rotY; //ī�޶� Xȸ����

    public Transform realCamera; //Main Camera
    public Vector3 dirNormalized; //ī�޶��� ����
    public Vector3 finalDir; //���� ������ ����
    public float minDistance; //ī�޶�� Ÿ�ٰ��� �ּҰŸ�
    public float maxDistance; //ī�޶�� Ÿ�ٰ��� �ִ�Ÿ�
    public float finalDistance; //���� �Ÿ�
    public float smoothness = 10f; //ī�޶�

    private void Start()
    {
        rotX = transform.localRotation.eulerAngles.x; //���� ���� ������Ʈ�� X �� ���� ȸ�� ������ rotX�� ����
        rotY = transform.localRotation.eulerAngles.y; //���� ���� ������Ʈ�� Y �� ���� ȸ�� ������ rotT�� ����

        dirNormalized = realCamera.localPosition.normalized; //realCamera�� ���� ������ ���͸� ������ �����ϰ� ����ȭ �Ͽ� ���� ���� ���͸� ����
        finalDistance = realCamera.localPosition.magnitude; //realCamera�� ���������� ������ ���̸� ����

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
        rotX = Mathf.Clamp(rotX, -clampAngle-20, clampAngle);

        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
        transform.rotation = rot;
    }
    private void LateUpdate()
    {
        //Ÿ���� ���󰡰� ��
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
