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

    public float speed = 5f; //�ȴ� �ӵ�
    public float runSpeed = 8f; //�ٴ� �ӵ�
    public float finalSpeed; // ���� ������ �ӵ�
    public float rotateSpeed; // ĳ���� ȸ�� �ӵ�
    public float JumpSpeed=3f; // ĳ���� ���� �ӵ�
    public float Gravity = 9.8f;// �߷� ��
    public float ySpeed;


    public bool isGround; //���� ���� ������ �Ǵ�
    public bool isJump; //���� ������ �Ǵ�
    public bool run; //�ٴ��� �ȶٴ��� �Ǵ�

    public float smoothness = 10f; //ī�޶� ȸ�� �ε巯��

    Vector3 moveDir; //�̵�����


    private void Start()
    {
        _animator= this.GetComponent<Animator>(); //ĳ������ �ִϸ����� ������Ʈ�� ������ ����
        _camera= Camera.main; // ����ī�޶�� ���� ����
        _controller= this.GetComponent<CharacterController>(); //ĳ������ ĳ���� ��Ʈ�ѷ� ������Ʈ�� ������ ����
    }
    private void FixedUpdate()
    {              
        InputMovement();
        Jump();
    }

    void InputMovement()
    {
        //���� ����Ʈ Ű�� ĳ���Ͱ� �ٴ��� �ȶٴ��� ��ź
        if (Input.GetKey(KeyCode.LeftShift))
        {
            run = true;
        }
        else
        {
            run = false;
        }


        //bool ������ run�� Ʈ���̸� finalSpeed�� runSpeed�� �ƴ϶�� �׳� speed��
        finalSpeed = (run) ? runSpeed : speed;
        //Vector2 ���� moveInput�� Horizontal�� Vertical�� ���ο� Vector2 �� ����ؼ� �ʱ�ȭ �Ͽ� �̵��ϴ� �� Ȯ�� 
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"),0, Input.GetAxisRaw("Vertical"));
        // moveInput�� ũ�Ⱑ 0�� �ƴ϶�� ismove�� true�� ĳ���Ͱ� �̵��ϴ� ������ �Ǵ�
        bool isMove = moveInput.magnitude != 0;

        //lookForward�� ĳ������ �������� ĳ������ ������ ī�޶��� �յ� ����� ���� ��
        Vector3 lookForward = new Vector3(_camera.transform.forward.x, 0f, _camera.transform.forward.z).normalized;
        //lookRight���� ĳ������ �������� ī�޶���  �¿����� ���� ��
        Vector3 lookRight = new Vector3(_camera.transform.right.x, 0f, _camera.transform.right.z).normalized;
        //���� �������� lookForward�� lookRight�� moveInput�� ���Ͽ� �̵� ���� ����
        moveDir = lookForward * moveInput.z + lookRight * moveInput.x;
        
        //percent�� �ִϸ��̼� ���� ��ġ run�� ture�̸� 1�̰� false��� 0.5�� �ٴ� �ִϸ��̼ǰ� �ȴ� �ִϸ��̼� ����
        float percent = ((run) ? 1f : 0.5f) * moveInput.magnitude;
        _animator.SetFloat("Blend", percent, 0.1f, Time.deltaTime);

        if(isMove)
        {
            // ��ǥ ȸ�� ���� ���
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            // �ε巯�� ȸ�� ���� (lerp�� ����Ͽ� ���� ȸ������ ��ǥ ȸ������ �ε巴�� ��ȯ)
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime *rotateSpeed);
            // �̵�
            _controller.Move(moveDir * finalSpeed * Time.deltaTime);
           
        }
    }
    void Jump()
    {
        Ray ray = new Ray(transform.position, Vector3.down); //���̸� �Ʒ� �������� ���� ����� ����ִ��� �Ǵ�
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 0.1f))
        {
            // ����� �浹�� ���
            isGround = true;
        }
        else
        {
            // ����� �浹���� ���� ��� �߷� ����
            ySpeed -= Gravity * Time.deltaTime;
            isGround = false;
        }


        if (isGround&&Input.GetKey(KeyCode.Space))
        {
            // ���� �ְ� ���� ��ư�� ������ ���� ���� ����
            ySpeed = JumpSpeed;
        }

        // ����
        Vector3 move = new Vector3(0, ySpeed, 0) * Time.deltaTime;
        _controller.Move(move);
    }
}
