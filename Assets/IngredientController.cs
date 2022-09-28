using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientController : MonoBehaviour
{
    public float stayDuration = 0;
    private GameObject followingPaw;
    private bool isDropped = false;
    
    void OnMouseDown() {
        GameObject paw = GameObject.Find("Left");
        paw.SendMessage("InitiateGrab", gameObject);
    }

    void Update() {
        if (followingPaw != null) {
            gameObject.transform.position = followingPaw.transform.position;
        }
    }

    public void Grab(GameObject paw) {
        followingPaw = paw;
    }

    public virtual void Drop() {
        Invoke("SendPawBack", stayDuration);
    }

    void SendPawBack() {
        followingPaw.SendMessage("Return");
        followingPaw = null;
        FinalizeDrop();
    }
    
    public virtual void FinalizeDrop() {
        Rigidbody2D rb2d = gameObject.GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 1;
        isDropped = true;
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (isDropped && coll.gameObject.CompareTag("Cauldron")) {
            GameObject.Destroy(gameObject);
        }
    }
}
