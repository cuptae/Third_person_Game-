using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    public float rotateSpeed; // 캐릭터 회전 속도
    public float JumpSpeed=3f; // 캐릭터 점프 속도
    public float Gravity = 9.8f;// 중력 힘
    public float ySpeed;


    public bool isGround; //땅에 접지 중인지 판단
    public bool isJump; //점프 중인지 판단
    public bool run; //뛰는지 안뛰는지 판단

    public float smoothness = 10f; //카메라 회전 부드러움

    Vector3 moveDir; //이동방향


    private void Start()
    {
        _animator= this.GetComponent<Animator>(); //캐릭터의 애니메이터 컴포턴트를 변수에 저장
        _camera= Camera.main; // 메인카메라로 변수 설정
        _controller= this.GetComponent<CharacterController>(); //캐릭터의 캐릭터 컨트롤러 컴포넌트를 변수에 저장
    }
    private void FixedUpdate()
    {              
        InputMovement();
        Jump();
    }

    void InputMovement()
    {
        //좌측 시프트 키로 캐릭터가 뛰는지 안뛰는지 판탄
        if (Input.GetKey(KeyCode.LeftShift))
        {
            run = true;
        }
        else
        {
            run = false;
        }


        
        finalSpeed = (run) ? runSpeed : speed; //bool 변수인 run이 트루이면 finalSpeed를 runSpeed로 아니라면 그냥 speed로
        
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"),0, Input.GetAxisRaw("Vertical"));
        //Vector3 변수 moveInput에 Horizontal과 Vertical로 새로운 Vector3 를 계속해서 초기화 하여 이동하는 힘 확보 
        
        bool isMove = moveInput.magnitude != 0; // moveInput의 크기가 0이 아니라면 ismove를 true로 캐릭터가 이동하는 것으로 판단
        
        Vector3 lookForward = new Vector3(_camera.transform.forward.x, 0f, _camera.transform.forward.z).normalized;
        //lookForward는 캐릭터의 방향으로 캐릭터의 방향은 카메라의 앞뒤 방향과 같게 함
        
        Vector3 lookRight = new Vector3(_camera.transform.right.x, 0f, _camera.transform.right.z).normalized;
        //lookRight또한 캐릭터의 방향으로 카메라의  좌우방향과 같게 함

        
        moveDir = lookForward * moveInput.z + lookRight * moveInput.x;//최종 방향으로 lookForward와 lookRight와 moveInput을 곱하여 이동 방향 결정

        //percent는 애니메이션 블랜드 수치 run이 ture이면 1이고 false라면 0.5로 뛰는 애니메이션과 걷는 애니메이션 구분
        float percent = ((run) ? 1f : 0f) * moveInput.magnitude;
        _animator.SetFloat("Blend", percent, 0.1f, Time.deltaTime);

        if (isMove)
        {
            _animator.SetBool("IsMoving", true);
            // 목표 회전 각도 계산
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            // 부드러운 회전 적용 (lerp를 사용하여 현재 회전에서 목표 회전으로 부드럽게 전환)
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
            // 이동
            _controller.Move(moveDir * finalSpeed * Time.deltaTime);

        }
        else if(moveInput.magnitude<=0)
        {
            _animator.SetBool("IsMoving", false);
        }
    }
    void Jump()
    {
        Ray ray = new Ray(transform.position, Vector3.down); //레이를 아랫 방향으로 쏴서 지면과 닿아있는지 판단
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 0.1f))
        {
            // 지면과 충돌한 경우
            isGround = true;
            ySpeed= 0f;
            _animator.SetBool("IsGrounded", true);
            _animator.SetBool("IsJumping", false);
            _animator.SetBool("IsFalling", false);
        }
        else
        {
            // 지면과 충돌하지 않은 경우 중력 적용
            ySpeed -= Gravity * Time.deltaTime;
            isGround = false;
            _animator.SetBool("IsGrounded", false);
        }

        if (isGround&&Input.GetKey(KeyCode.Space))
        {
            // 땅에 있고 점프 버튼이 눌렸을 때만 점프 실행
            ySpeed = JumpSpeed;
            _animator.SetBool("IsJumping", true);
        }

        if(isGround==false && ySpeed <-1)
        {
            //땅이 아니고 떨어지는 속도가 1 초과일때 Falling 애니메이션 적용
            _animator.SetBool("IsFalling", true);
        }

       
        if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Landing"))
        {
            //랜딩 에니메이션이 진행중일땐 움직임 제한

            speed = 0;
            runSpeed = 0;
        }
        else
        {
            //랜딩 애니메이션이 진행중이지 않을 땐 이전 속도로
            speed = 1.5f;
            runSpeed = 4;
        }

        // 점프
        Vector3 move = new Vector3(0, ySpeed, 0) * Time.deltaTime;
        _controller.Move(move);
    }
}
