using UnityEngine;
using Nakama.TinyJson;
using System.Collections.Generic;
/// <summary>
/// Базовый класс для пушек
/// </summary>
public abstract class Weapon : MonoBehaviour
{
    [Header("Weapon Info")]
    [SerializeField]public string WeaponName;
    [SerializeField]public int ammo;
    [SerializeField]public float power;
    [SerializeField]public bool auto;
    [SerializeField]public bool inf; //infinite
    [SerializeField]public float AttackInterval; //rate of fire
    [SerializeField]public float spread;
    [SerializeField]public float BulletSpeed;
    [SerializeField]public weaponType type;
    public enum weaponType {Pistol, Rifle};
    
    [Header("Others")]
    public GameObject ShotParticlePrefab;
    public AudioSource audioData;
    public AudioClip shotSound;
    
    [Header("For Debug")]
    [SerializeField]private bool helding;
    [SerializeField]protected int current_ammo;
    [SerializeField]private float attackTimer;
     
    private void Update()
    {
        attackTimer -= Time.deltaTime;
        if (auto && helding)
        {
            Attack();
        }
    }

    protected string GetWeaponData(string weapon, string value)
		{
			Dictionary<string, string> weapondata = value.FromJson<Dictionary<string, string>>();

			if (weapondata.ContainsKey(weapon))
			{
				return weapondata[weapon].ToString();
			}

			return "Error";
	}

    protected void SetWeaponParams(string value)
    {
        if (PlayerPrefs.HasKey("WeaponData"))
        {
            //string weapondata = "{" + GetWeaponData(value, PlayerPrefs.GetString("WeaponData")) + "}";
            string weapondata = GetWeaponData(value, PlayerPrefs.GetString("WeaponData"));
            WeaponName = GetWeaponData("name", weapondata);
            inf = (GetWeaponData("inf", weapondata) == "true");
            auto = (GetWeaponData("auto", weapondata) == "true");
            ammo = int.Parse(GetWeaponData("ammo", weapondata));
            AttackInterval = float.Parse(GetWeaponData("speed", weapondata));
            power = float.Parse(GetWeaponData("power", weapondata));
            spread = float.Parse(GetWeaponData("spread", weapondata));
            BulletSpeed = float.Parse(GetWeaponData("bulletspeed", weapondata));
        }

    }


    /// <summary>
    /// Атаковать.
    /// </summary>
    public void Attack()
    {
            if (attackTimer <= 0)
            {
            HandleAttack();
            --current_ammo;
            PlayAttackParticle();
            attackTimer = AttackInterval;
            }
    }

    public void AttackHeld(bool value)
    {
        helding = value;
    }

    /// <summary>
    /// Для дочернего класса для логики атаки.
    /// </summary>
    protected abstract void HandleAttack();
    protected abstract void PlayAttackParticle();
}
