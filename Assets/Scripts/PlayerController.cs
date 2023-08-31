using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    Animator _animator; //�ִϸ�����
    Camera _camera; //���� ī�޶�
    CharacterController _controller; // ĳ���� ��Ʈ�ѷ�

    public float speed = 5f; //�ȴ� �ӵ�
    public float runSpeed = 8f; //�ٴ� �ӵ�
    public float finalSpeed; // ���� ������ �ӵ�

    public bool toggleCameraRotation; //��Ʋ�׶��� ��ƮŰó�� ī�޶� ���� ���� ���
    public bool run; //�ٴ��� �ȶٴ��� �Ǵ�

    public float smoothness = 10f; //ī�޶� ȸ�� �ε巯��

    private void Start()
    {
        _animator= this.GetComponent<Animator>();
        _camera= Camera.main;
        _controller= this.GetComponent<CharacterController>();
    }
    private void Update()
    {
        if(Input.GetKey(KeyCode.LeftAlt))
        {
            toggleCameraRotation=true;
        }
        else
        {
            toggleCameraRotation=false;
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            run = true;
        }
        else
        {
            run = false;
        }
        InputMovement();
    }
    private void LateUpdate()
    {
        //if (toggleCameraRotation != true)
        //{
        //    Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1, 0, 1));
        //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        //}
    }
    void InputMovement()
    {
        //Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //bool isMove = moveInput.magnitude != 0;
        //finalSpeed = (run) ? runSpeed : speed;
        //Vector3 forward = transform.TransformDirection(Vector3.forward);
        //Vector3 right = transform.TransformDirection(Vector3.right);
        //Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;
        //if (isMove)
        //{
        //    _controller.Move(moveDirection.normalized * finalSpeed * Time.deltaTime);

        //    float percent = ((run) ? 1f : 0.5f) * moveDirection.magnitude;
        //    _animator.SetFloat("Blend", percent, 0.1f, Time.deltaTime);
        //    transform.forward = moveDirection;
        //}


        finalSpeed = (run) ? runSpeed : speed;
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool isMove = moveInput.magnitude != 0;

        if (isMove)
        {
            Vector3 lookForward = new Vector3(_camera.transform.forward.x, 0f, _camera.transform.forward.z).normalized;
            Vector3 lookRight = new Vector3(_camera.transform.right.x, 0f, _camera.transform.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            float percent = ((run) ? 1f : 0.5f) * moveInput.magnitude;
            _animator.SetFloat("Blend", percent, 0.1f, Time.deltaTime);


            transform.forward = moveDir;

            transform.position += moveDir * finalSpeed * Time.deltaTime;
        }



    }
}
