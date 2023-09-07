using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float Damage;
    public int Combo =0;
    public float AttackDelay = 3f;
    public float comboTime = 2f;
    public float time;
    Animator _animator;

    private void Start()
    {
        _animator= GetComponent<Animator>();
    }

    private void Update()
    {

        //time += Time.deltaTime;
        if (Input.GetMouseButtonDown(0))
        {
            AttackDelay -= Time.deltaTime;
            Combo++;
            _animator.SetBool("IsAttack", true);
            if (Combo == 1 && AttackDelay <= 1.5f)
            {
                _animator.SetInteger("Combo", 1);
                AttackDelay = 3;
            }
            else if (Combo == 2 && AttackDelay <= 2.5f)
            {
                _animator.SetInteger("Combo", 2);
            }
            else if (AttackDelay == 0)
            {
                _animator.SetBool("IsAttack", false);
            }

        }


    }
}
