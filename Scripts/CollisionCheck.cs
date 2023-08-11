using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionCheck : MonoBehaviour
{
    Rigidbody rigidOfOriginal;
    Rigidbody rigidOfOther;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name != "Quad")
        {

            rigidOfOther = collision.gameObject.GetComponent<Rigidbody>();
            rigidOfOther.constraints = RigidbodyConstraints.FreezeAll;
        }

        //rigidOfOther = collision.gameObject.GetComponent<Rigidbody>();
        //rigidOfOriginal = gameObject.GetComponent<Rigidbody>();
        //rigidOfOriginal.constraints = RigidbodyConstraints.None;
        //rigidOfOriginal.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionY;
        //rigidOfOther.constraints = RigidbodyConstraints.FreezeAll;

        //if(collision.gameObject.tag == "trashcan")
        //{
        //    Destroy(gameObject);
        //}
    }

    private void OnCollisionStay(Collision collision)
    {
        //rigidOfOther = collision.gameObject.GetComponent<Rigidbody>();
        //rigidOfOther.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void OnCollisionExit(Collision collision)
    {
        rigidOfOther = collision.gameObject.GetComponent<Rigidbody>();
        rigidOfOther.constraints = RigidbodyConstraints.None;
        rigidOfOther.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezePositionY;

        //rigidOfOther = collision.gameObject.GetComponent<Rigidbody>();
        //rigidOfOther.constraints = RigidbodyConstraints.FreezeAll;
        //rigidOfOriginal = gameObject.GetComponent<Rigidbody>();
        //rigidOfOriginal.constraints = RigidbodyConstraints.FreezeAll;
    }
}
