using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScriptSecond : MonoBehaviour
{
    Collider parent;
    void Start()
    {
       parent  = transform.parent.gameObject.GetComponent<MeshCollider>();
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player" || other.tag == "LocalPlayer" || other.tag == "RemotePlayer"){

                Physics.IgnoreCollision(other, parent, true);
        }
    }
}
