using UnityEngine;
public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon Prefabs")]
    public GameObject Colt;
    public GameObject Glock;
    public GameObject Makarov;
    public GameObject Deagle;
    public GameObject Uzi;
    public GameObject Tec9;
    public GameObject Beretta;
    
    [Header("Current Weapon")]
    [SerializeField]public int id = 0;
    [SerializeField]public int newid;
    public GameObject current;
    public Weapon Weapon;
    public bool attackHeld;
    /// <summary>
    /// Атаковать текущим оружием
    /// </summary>
    private void Start() {
        newid = id;
        SetWeapon(id);
    }

    void Update()
    {   
        Weapon.AttackHeld(attackHeld);
        if (newid != id)
        {
            SetWeapon(newid);
            id = newid;
        }
    }

    public void SetNewId(int value)
    {
        newid = value;
    }

/*     private void OnValidate() {
        SetWeapon(id);
    } */
    
    public void GetWeapPrefab(GameObject newWeapon)
    {
        current.SetActive(false); 
        current = newWeapon;
        string type = newWeapon.name;
        Weapon = newWeapon.GetComponent(type) as Weapon;
        current.SetActive(true); 
    }

    public void SetWeapon(int weapid)
    {
    switch (weapid)
    {
    case 0:
        GetWeapPrefab(Colt);
       break;
    case 1:
        GetWeapPrefab(Glock);
       break;
    case 2: 
        GetWeapPrefab(Makarov);
       break;
    case 3: 
        GetWeapPrefab(Deagle);
       break;
    case 4:
        GetWeapPrefab(Beretta);
       break;
    case 5: 
        GetWeapPrefab(Tec9);
       break;
    case 6: 
        GetWeapPrefab(Uzi);
       break;
    }
    }
    public void SetAttackHeld(bool value)
    {
        attackHeld = value;
    }
    public void Attack()
    {
        Weapon.Attack();
    }
}