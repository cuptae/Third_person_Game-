using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster",menuName ="MonsterData",order = 0)]
public class MonsterData : ScriptableObject
{
    [SerializeField]
    private string monster_Name;
    public string Monster_Name { get { return monster_Name; } }
    [SerializeField]
    private float hp;
    public float HP { get { return hp; } }

    [SerializeField]
    private float damage;
    public float Damage { get { return damage; } }
    [SerializeField]
    private float speed;
    public float Speed { get { return speed; } }
    [SerializeField]
    private float sight; 
    public float Sight { get { return sight; } }
}
