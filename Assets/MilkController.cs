using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilkController : IngredientController
{
    private Animator animator;

    void Start() {
        animator = gameObject.GetComponent<Animator>();
    }

    public override void Drop() {
        Invoke("SendPawBack", stayDuration);
        animator.SetBool("is_spilling", true);
    }

    public override void FinalizeDrop() {
        animator.SetBool("is_spilling", false);
        GameObject.Destroy(gameObject);
    }
}
