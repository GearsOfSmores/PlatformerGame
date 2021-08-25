using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
   

    [Header("Horizontal Movement")]
    public float moveSpeed = 10f;
    public Vector2 direction;
    public bool facingRight = true;
    private float slideTimer;
    private float slideDelay = 0.25f;
    public float slideSpeed = 5f;
    public bool isSliding = false;
    [SerializeField] private float rollSlideSpeed = 0f;
   
    public bool isRolling = false;
    private float animationDelayRoll = 0f;
    private float animationDelaySlide = 0f;



    [Header("Vertical Movement")]
    public float jumpSpeed = 15f;
    public float jumpDelay = 0.25f;
    private float jumpTimer;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public LayerMask groundLayer;
    public GameObject characterHolder;
    public BoxCollider2D regularColli;
    public BoxCollider2D slideColli;
    private SpriteRenderer spriteRend;
   

    [Header("Physics")]
    public float maxSpeed = 7f;
    public float linearDrag = 4f;
    public float gravity = 1f;
    public float fallMultiplier = 5f;
    public float slideDrag = 0f;
    public float decreaseSlide = 0f;
    

    [Header("Collision")]
    public bool onGround = false;
    public float groundLength = 0.6f;
    public Vector3 colliderOffset;



    private void Awake()
    {
        spriteRend = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
     
        HandleMovenment();
        HandleSlide();
        HandleDodgeRoll();

       
    }
   
    void FixedUpdate()
    {
       

        if (slideTimer > Time.time && onGround)
         {
           // preformSlide();
         }
      
        if (jumpTimer > Time.time && onGround)
         {
            Jump();
         }
        moveCharacter(direction.x);
        modifyPhysics();
        


    }
    private void HandleMovenment()
    {
        bool wasOnGround = onGround;
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer); ///|| Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (!wasOnGround && onGround)
        {
            StartCoroutine(JumpSqueeze(1.25f, 0.8f, 0.05f));
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpTimer = Time.time + jumpDelay;
        }

        if (Input.GetButtonDown("Jump") && isSliding == true)
        {
            animator.SetBool("slide", false);

        }
        
        if (Input.GetButtonDown("Jump") && isRolling == true)
        {
            
            animator.SetBool("Roll", false);

        }
        if (Input.GetButtonDown("Slide") && isRolling == true)
        {
            animator.SetBool("Roll", false);

            
        }
        if (Input.GetButtonDown("Roll") && isSliding == true)
        {
            animator.SetBool("slide", false);
            

        }



    }

        private void HandleDodgeRoll()
    {
        if (Input.GetButtonDown("Roll")&& onGround)
        {
           
            isRolling = true;
            animator.SetBool("roll", true);
            if (facingRight && direction.x > .4f)
            {
                rb.AddForce(Vector2.right * rollSlideSpeed);
                }
                else if (!facingRight && direction.x < -.4f)
                {
                    rb.AddForce(-Vector2.right * rollSlideSpeed);
                }
                else if(facingRight && direction.x <= .4f)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    rb.AddForce(Vector2.right * 30f, ForceMode2D.Impulse);
                }
                else if(!facingRight && direction.x >=-.4f)
                {
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    rb.AddForce(-Vector2.right *30f , ForceMode2D.Impulse);
                }



            StartCoroutine("stopRoll");

        }

    }
            
    

    

    private void HandleSlide()
    {
        if (Input.GetButtonDown("Slide") && onGround)
        {
         
            slideTimer = Time.time + slideDelay;
            isSliding = true;
            animator.SetBool("slide", true);


            if (facingRight)
            {
                rb.AddForce(Vector2.right * slideSpeed);
            }
            else
            {
                rb.AddForce(-Vector2.right * slideSpeed);
            }
            
            
            StartCoroutine("stopSlide");
        }
        
    }
   
    void moveCharacter(float horizontal)
    {
        rb.AddForce(Vector2.right * horizontal * moveSpeed);

        if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight))
        {
            Flip();
        }
        if (Mathf.Abs(rb.velocity.x) > maxSpeed && isSliding == false && isRolling == false)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
        animator.SetFloat("horizontal", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("vertical", rb.velocity.y);
    }
    void Jump()
    {
        //rb.velocity = new Vector2(rb.velocity.x,jumpSpeed);
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);

        jumpTimer = 0;
        StartCoroutine(JumpSqueeze(0.5f, 1.2f, 0.1f));
    }
    void modifyPhysics()
    {
        bool changingDirections = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);

        if (onGround)
        {

            if (Mathf.Abs(direction.x) < 0.4f || changingDirections)
            {
                rb.drag = linearDrag;
            }

            else if (isSliding == true)
            {
                rb.drag = 2f;
            }
            else if (isRolling == true)
            {
                rb.drag = 1f;
            }

            else
            {
                rb.drag = 0;
            }

            rb.gravityScale = 0;

        }

        else
        {
            rb.gravityScale = gravity;
            rb.drag = linearDrag * 0.15f;
            if (rb.velocity.y < 0)
            {
                rb.gravityScale = gravity * fallMultiplier;
            }
            else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb.gravityScale = gravity * (fallMultiplier / 2);
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }


    IEnumerator stopRoll()
    {
        animationDelayRoll += Time.time;

        yield return new WaitForSeconds(.5f);
        animator.SetBool("roll", false);
        //animator.SetBool("Roll", false);
        // regularColli.enabled = true;
        // slideColli.enabled = false;
        isRolling = false;

    }
    IEnumerator stopSlide()
    {
        animationDelaySlide += Time.time;

        yield return new WaitForSeconds(.8f);

        animator.SetBool("slide", false);
        // regularColli.enabled = true;
        // slideColli.enabled = false;
        isSliding = false;
        



    }
    IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds)
    {
        Vector3 originalSize = Vector3.one;
        Vector3 newSize = new Vector3(xSqueeze, ySqueeze, originalSize.z);
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            spriteRend.transform.localScale = Vector3.Lerp(originalSize, newSize, t);
            yield return null;
        }
        t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            spriteRend.transform.localScale = Vector3.Lerp(newSize, originalSize, t);
            yield return null;
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        //  Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
    }

    public bool canAttack()
    {

        return Input.GetAxis("Horizontal") == 0 && onGround;
    }

}
