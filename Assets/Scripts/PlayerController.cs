using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    Animator _animator; //애니메이터
    Camera _camera; //메인 카메라
    CharacterController _controller; // 캐릭터 컨트롤러

    public float speed = 5f; //걷는 속도
    public float runSpeed = 8f; //뛰는 속도
    public float finalSpeed; // 최종 결정된 속도

    public bool toggleCameraRotation; //배틀그라운드 알트키처럼 카메라 시점 고정 토글
    public bool run; //뛰는지 안뛰는지 판단

    public float smoothness = 10f; //카메라 회전 부드러움

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
