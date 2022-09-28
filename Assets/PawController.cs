using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawController : MonoBehaviour
{
    public GameObject anchorPrefab;
    public float rotationAtEdge = 30;
    public float grabDuration = 1;
    private GameObject heldIngredient;
    private GameObject currentTarget;
    private Vector3 initialPos;
    private Quaternion initialRotation;
    private GameObject initialPosAnchor;
    private Rigidbody2D rb2d;
    private float cornerOffset;
    private GameObject nextTarget;  // Added for delayed execution of InitiateGrab


    // Start is called before the first frame update
    void Start()
    {
        initialPos = gameObject.transform.position;
        initialPosAnchor = Instantiate(anchorPrefab, gameObject.transform.position, Quaternion.identity);
        initialRotation = gameObject.transform.rotation;

        rb2d = gameObject.GetComponent<Rigidbody2D>();
        Bounds backgroundBounds = GameObject.Find("Background").GetComponent<SpriteRenderer>().sprite.bounds;
        cornerOffset = backgroundBounds.extents.x;
    }

    void InitiateGrab(GameObject target) {
        currentTarget = target;

        Vector3 currentPos = gameObject.transform.position;
        Vector3 targetPos = target.transform.position;
        float xSpeed = (targetPos.x - currentPos.x) / grabDuration;
        float ySpeed = (targetPos.y - currentPos.y) / grabDuration;
        
        // f(x) = ax+b;
        float initialRotationDegrees = initialRotation.eulerAngles.z;
        if (initialRotationDegrees > 180) {
            initialRotationDegrees -= 360;
        }

        float a = (rotationAtEdge - initialRotationDegrees) / (cornerOffset - System.Math.Abs(initialPos.x));
        float b = initialRotationDegrees - (a * System.Math.Abs(initialPos.x));

        float targetRotation = a * System.Math.Abs(targetPos.x) + b;

        float rotationSpeed = (targetRotation - rb2d.rotation) / grabDuration;

        rb2d.velocity = new Vector2(xSpeed, ySpeed);
        rb2d.angularVelocity = rotationSpeed;
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (coll.gameObject == currentTarget) {
            if (coll.gameObject.CompareTag("Ingredient")) {
                GrabIngredient(coll.gameObject);
            } else if (coll.gameObject.name == "CauldronDropPoint") {
                DropIngredient();
            } else if (coll.gameObject == initialPosAnchor) {
                Reset();
            }
        }
    }

    void GrabIngredient(GameObject ingredient) {
        heldIngredient = ingredient;
        ingredient.SendMessage("Grab", gameObject);
        GameObject cauldron = GameObject.Find("CauldronDropPoint");
        InitiateGrab(cauldron);
    }

    void DropIngredient() {
        heldIngredient.SendMessage("Drop");
        Stop();
    }

    void Return() {
        InitiateGrab(initialPosAnchor);
    }

    public void Stop() {
        rb2d.velocity = new Vector2(0, 0);
        rb2d.angularVelocity = 0;
    }

    void Reset() {
        Stop();
        rb2d.position = initialPos;
        rb2d.rotation = initialRotation.z;
        gameObject.transform.rotation = initialRotation;
        currentTarget = null;
    }
}
