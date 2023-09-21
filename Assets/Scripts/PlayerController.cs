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

    #region Moving variable
    public float speed; //걷는 속도
    public float runSpeed; //뛰는 속도
    public float finalSpeed; // 최종 결정된 속도
    public float rotateSpeed; // 캐릭터 회전 속도
    public float JumpSpeed=10f; // 캐릭터 점프 속도
    public float Gravity = 9.8f;// 중력 힘
    public float ySpeed; // 현재 점프 위치
    private float lastGroundSpeed; // 지상에서의 속도


    public bool isMove; //움직이는지 아닌지 판단
    public bool isGround; //땅에 접지 중인지 판단
    public bool run; //뛰는지 안뛰는지 판단
    public bool Moveable;//움직일 수 있는 상황인지 판단

    Vector3 moveDir; //이동방향
    #endregion

    public float Damage;
    public float AttackDelay = 1.5f;
    public float AttackSpeed = 1f;

    public int Combo = 0;

    public bool IsAttack;


    private void Start()
    {
        _animator= this.GetComponent<Animator>(); //캐릭터의 애니메이터 컴포턴트를 변수에 저장
        _camera= Camera.main; // 메인카메라로 변수 설정
        _controller= this.GetComponent<CharacterController>(); //캐릭터의 캐릭터 컨트롤러 컴포넌트를 변수에 저장
    }
    private void FixedUpdate()
    {           
        InputMovement();     
        
    }
    private void Update()
    {
        Attack();
        Jump();
        
    }
    void Jump()
    {
        // 중력 적용
        ySpeed -= Gravity * Time.deltaTime;
        //ySpeed의 최솟값을 정하여 과도한 중력이 적용되지 않도록함
        ySpeed = Mathf.Max(ySpeed, -5f);

        Ray ray = new Ray(transform.position, Vector3.down); // 레이를 아랫 방향으로 쏴서 지면과 닿아있는지 판단
        RaycastHit hit;
        Debug.DrawRay(transform.position, Vector3.down, Color.red);
        if (Physics.Raycast(ray, out hit, 0.1f))
        {
            // 지면과 충돌한 경우
            isGround = true;
            _animator.SetBool("IsGrounded", true);
            _animator.SetBool("IsJumping", false);
            _animator.SetBool("IsFalling", false);
            if (isGround) // 땅에 있는 경우에만 이전 스피드를 저장
            {
                lastGroundSpeed = finalSpeed;
            }
        }
        else
        {
            // 지면과 충돌하지 않은 경우
            isGround = false;
            _animator.SetBool("IsGrounded", false);
            if (!isGround)
            {
                if (ySpeed < 9.8)
                {
                    // ySpeed가 이전 프레임보다 작아졌을 때 Falling 애니메이션 적용
                    _animator.SetBool("IsFalling", true);
                }
            }
        }

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Landing"))
        {
            //  랜딩 애니메이션이 진행 중이지 않을 때 움직임 제한
            Moveable = false;
        }
        if (Moveable)
        {
            if (isGround && Input.GetKeyDown(KeyCode.Space))
            {
                // Moveable이 true이고 땅에 있고 점프 버튼이 눌렸을 때만 점프 실행
                ySpeed = JumpSpeed;
                _animator.SetBool("IsJumping", true);
                Debug.Log("점프");
            }
        }
        // 점프
        Vector3 move = new Vector3(0, ySpeed * Time.deltaTime, 0);
        _controller.Move(move);
    }
    void InputMovement()
    {
        //좌측 시프트 키로 캐릭터가 뛰는지 안뛰는지 판단
        if (Input.GetKey(KeyCode.LeftShift))
        {
            run = true;
        }
        else
        {
            run = false;
        }
      
        finalSpeed = (run) ? runSpeed : speed; //bool 변수인 run이 트루이면 finalSpeed를 runSpeed로 아니라면 그냥 speed로

        //공중에서는 지상에서의 마지막 속도로 고정
        if (!isGround)
        {
            finalSpeed = lastGroundSpeed;
        }

        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"),0, Input.GetAxisRaw("Vertical"));
        //수직 수평 이동 방향을 저장
        
        isMove = moveInput.magnitude != 0; // moveInput의 크기가 0이 아니라면 ismove를 true로 캐릭터가 이동하는 것으로 판단
        
        Vector3 lookForward = new Vector3(_camera.transform.forward.x, 0f, _camera.transform.forward.z).normalized;
       
        Vector3 lookRight = new Vector3(_camera.transform.right.x, 0f, _camera.transform.right.z).normalized;
        
        moveDir = lookForward * moveInput.z + lookRight * moveInput.x;//최종 방향으로 lookForward와 lookRight와 moveInput을 곱하여 이동 방향 결정

        //percent는 애니메이션 블랜드 수치 run이 ture이면 1이고 false라면 0.5로 뛰는 애니메이션과 걷는 애니메이션 구분
        float percent = ((run) ? 1f : 0f) * moveInput.magnitude;
        _animator.SetFloat("Blend", percent, 0.1f, Time.deltaTime);

        if (isMove&&Moveable)
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

    void Attack()
    {
        if(Input.GetMouseButtonDown(0))
        {
            IsAttack = true;
            if(IsAttack &&AttackDelay>0)
            {
                if(Combo==0)
                {
                    _animator.SetBool("IsAttack", true);
                    Combo = 1;
                    AttackDelay = 0.5f;
                }
                else if(Combo == 1)
                {
                    _animator.SetInteger("Combo",1);
                    Combo = 2;
                    AttackDelay = 0.5f;
                }
                else if(Combo == 2)
                {
                    _animator.SetInteger("Combo", 2);
                    AttackDelay = 0.5f;
                }
            }
        }
        if (AttackDelay <= 0)
        {
            IsAttack = false;
            _animator.SetBool("IsAttack", false);
            _animator.SetInteger("Combo", 0);
            AttackDelay = 0.5f;
            Combo= 0;
        }


        if (IsAttack)
        {
            Moveable = false;
            AttackDelay -= Time.deltaTime;
        }
        else
        {
            Moveable= true;
        }
    } 
}
