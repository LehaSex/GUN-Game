using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootCrate : MonoBehaviour
{
    private Animator animator;
    private int[] Values = {1, 3, 5, 6};
   
 
    int GetRandomValue()
    {
    return Values[Random.Range(0, Values.Length)];
    }
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    IEnumerator RemoveCrate(GameObject _go)
    {
        yield return new WaitForSeconds(1f);
        Destroy(_go);
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player"){
            Physics.IgnoreCollision(other, this.gameObject.GetComponentInChildren<BoxCollider>(), true);
            other.GetComponentInChildren<PlayerWeapon>().SetNewId(GetRandomValue());
            StartCoroutine(RemoveCrate(this.gameObject));
            animator.SetBool("Colided", true);
        }
    }
}
