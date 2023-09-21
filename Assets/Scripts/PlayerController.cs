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
    Animator _animator; //�ִϸ�����
    Camera _camera; //���� ī�޶�
    CharacterController _controller; // ĳ���� ��Ʈ�ѷ�

    #region Moving variable
    public float speed; //�ȴ� �ӵ�
    public float runSpeed; //�ٴ� �ӵ�
    public float finalSpeed; // ���� ������ �ӵ�
    public float rotateSpeed; // ĳ���� ȸ�� �ӵ�
    public float JumpSpeed=10f; // ĳ���� ���� �ӵ�
    public float Gravity = 9.8f;// �߷� ��
    public float ySpeed; // ���� ���� ��ġ
    private float lastGroundSpeed; // ���󿡼��� �ӵ�


    public bool isMove; //�����̴��� �ƴ��� �Ǵ�
    public bool isGround; //���� ���� ������ �Ǵ�
    public bool run; //�ٴ��� �ȶٴ��� �Ǵ�
    public bool Moveable;//������ �� �ִ� ��Ȳ���� �Ǵ�

    Vector3 moveDir; //�̵�����
    #endregion

    public float Damage;
    public float AttackDelay = 1.5f;
    public float AttackSpeed = 1f;

    public int Combo = 0;

    public bool IsAttack;


    private void Start()
    {
        _animator= this.GetComponent<Animator>(); //ĳ������ �ִϸ����� ������Ʈ�� ������ ����
        _camera= Camera.main; // ����ī�޶�� ���� ����
        _controller= this.GetComponent<CharacterController>(); //ĳ������ ĳ���� ��Ʈ�ѷ� ������Ʈ�� ������ ����
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
        // �߷� ����
        ySpeed -= Gravity * Time.deltaTime;
        //ySpeed�� �ּڰ��� ���Ͽ� ������ �߷��� ������� �ʵ�����
        ySpeed = Mathf.Max(ySpeed, -5f);

        Ray ray = new Ray(transform.position, Vector3.down); // ���̸� �Ʒ� �������� ���� ����� ����ִ��� �Ǵ�
        RaycastHit hit;
        Debug.DrawRay(transform.position, Vector3.down, Color.red);
        if (Physics.Raycast(ray, out hit, 0.1f))
        {
            // ����� �浹�� ���
            isGround = true;
            _animator.SetBool("IsGrounded", true);
            _animator.SetBool("IsJumping", false);
            _animator.SetBool("IsFalling", false);
            if (isGround) // ���� �ִ� ��쿡�� ���� ���ǵ带 ����
            {
                lastGroundSpeed = finalSpeed;
            }
        }
        else
        {
            // ����� �浹���� ���� ���
            isGround = false;
            _animator.SetBool("IsGrounded", false);
            if (!isGround)
            {
                if (ySpeed < 9.8)
                {
                    // ySpeed�� ���� �����Ӻ��� �۾����� �� Falling �ִϸ��̼� ����
                    _animator.SetBool("IsFalling", true);
                }
            }
        }

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Landing"))
        {
            //  ���� �ִϸ��̼��� ���� ������ ���� �� ������ ����
            Moveable = false;
        }
        if (Moveable)
        {
            if (isGround && Input.GetKeyDown(KeyCode.Space))
            {
                // Moveable�� true�̰� ���� �ְ� ���� ��ư�� ������ ���� ���� ����
                ySpeed = JumpSpeed;
                _animator.SetBool("IsJumping", true);
                Debug.Log("����");
            }
        }
        // ����
        Vector3 move = new Vector3(0, ySpeed * Time.deltaTime, 0);
        _controller.Move(move);
    }
    void InputMovement()
    {
        //���� ����Ʈ Ű�� ĳ���Ͱ� �ٴ��� �ȶٴ��� �Ǵ�
        if (Input.GetKey(KeyCode.LeftShift))
        {
            run = true;
        }
        else
        {
            run = false;
        }
      
        finalSpeed = (run) ? runSpeed : speed; //bool ������ run�� Ʈ���̸� finalSpeed�� runSpeed�� �ƴ϶�� �׳� speed��

        //���߿����� ���󿡼��� ������ �ӵ��� ����
        if (!isGround)
        {
            finalSpeed = lastGroundSpeed;
        }

        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"),0, Input.GetAxisRaw("Vertical"));
        //���� ���� �̵� ������ ����
        
        isMove = moveInput.magnitude != 0; // moveInput�� ũ�Ⱑ 0�� �ƴ϶�� ismove�� true�� ĳ���Ͱ� �̵��ϴ� ������ �Ǵ�
        
        Vector3 lookForward = new Vector3(_camera.transform.forward.x, 0f, _camera.transform.forward.z).normalized;
       
        Vector3 lookRight = new Vector3(_camera.transform.right.x, 0f, _camera.transform.right.z).normalized;
        
        moveDir = lookForward * moveInput.z + lookRight * moveInput.x;//���� �������� lookForward�� lookRight�� moveInput�� ���Ͽ� �̵� ���� ����

        //percent�� �ִϸ��̼� ���� ��ġ run�� ture�̸� 1�̰� false��� 0.5�� �ٴ� �ִϸ��̼ǰ� �ȴ� �ִϸ��̼� ����
        float percent = ((run) ? 1f : 0f) * moveInput.magnitude;
        _animator.SetFloat("Blend", percent, 0.1f, Time.deltaTime);

        if (isMove&&Moveable)
        {
            _animator.SetBool("IsMoving", true);
            // ��ǥ ȸ�� ���� ���
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            // �ε巯�� ȸ�� ���� (lerp�� ����Ͽ� ���� ȸ������ ��ǥ ȸ������ �ε巴�� ��ȯ)
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
            // �̵�
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
