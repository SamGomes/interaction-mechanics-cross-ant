using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
    public GameObject gameManager;

    public GameObject myQueen;
    Animator queenAnimator;

    public GameObject mySpawner;
    public ParticleSystem myParticleSystem;
    Animator myAnimator;

    public GameObject foodProjectile;
    private Sprite foodProjectileSprite;


    private float movementSpeed;
    private float currMovementSpeed;

    private float lifetime = 10;
    private float differenceFromQueen;

    private float throwingStartTime = -1;

    public Utilities.OutputRestriction outputRestriction;

    // Use this for initialization
    void Start()
    {
        this.movementSpeed = 5;

        myAnimator = this.gameObject.GetComponent<Animator>();
        Vector3 spawnPosition = this.transform.position;
        differenceFromQueen = myQueen.transform.position.x - spawnPosition.x;
        Destroy(gameObject, lifetime);

        if(differenceFromQueen < 0)
        {
            movementSpeed = -movementSpeed;
            this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        this.queenAnimator = myQueen.GetComponent<Animator>();
        currMovementSpeed = movementSpeed;

    }

    // Update is called once per frame
    void Update()
    {
        if (outputRestriction == Utilities.OutputRestriction.NONE)
        {
            return;
        }

        //STAR POWER
        if (outputRestriction == Utilities.OutputRestriction.STARPOWER)
        {
            myParticleSystem.Play();
        }
        
        
        //EAT ANIMATION
        if (outputRestriction == Utilities.OutputRestriction.EAT)
        {
            
            this.transform.Translate(Vector3.right * currMovementSpeed * Time.deltaTime);
            if (throwingStartTime < 0)
            {
                return;
            }
            else
            {
                float currenTime = Time.time;
                float difference = currenTime - throwingStartTime;
                if (difference > 4)
                {
                    this.myAnimator.SetTrigger("walk");
                    gameObject.GetComponents<AudioSource>()[1].Play();
                    if (queenAnimator != null)
                    {
                        queenAnimator.SetTrigger("rest");
                    }

                    throwingStartTime = -1;
                    this.gameObject.GetComponent<SpriteRenderer>().flipX = !this.gameObject.GetComponent<SpriteRenderer>().flipX;
                    currMovementSpeed = -movementSpeed;
                }
            }
        }
    }


    public void SetCargo(string currTargetWord)
    {
        GameObject child = gameObject.transform.GetChild(0).gameObject;
        SpriteRenderer childRenderer = child.GetComponent<SpriteRenderer>();
        childRenderer.sprite = (Sprite)Resources.Load("Textures/FoodItems/" + currTargetWord, typeof(Sprite));
        foodProjectileSprite = childRenderer.sprite;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other != mySpawner.GetComponent<Collider2D>())
        {
            return;
        }

        this.myAnimator.SetTrigger("throw");
        GameObject child = gameObject.transform.GetChild(0).gameObject;
        Destroy(child);
        Instantiate(foodProjectile, gameObject.transform.position, Quaternion.identity).GetComponent<SpriteRenderer>().sprite = foodProjectileSprite;

        if (queenAnimator != null)
        {
            queenAnimator.SetTrigger("beginEating");
            gameObject.GetComponents<AudioSource>()[0].Play();
        }

        currMovementSpeed = 0;
        throwingStartTime = Time.time;
    }

   
}
