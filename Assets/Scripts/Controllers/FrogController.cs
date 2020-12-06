using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogController : LandingScript
{
    public float jumpForce;
    public float landTime;
    public float hopDelay;
    public enum MovementDirections { Left = 0, Right = 1, LeftAndRight = 2 }
    public MovementDirections direction = MovementDirections.Left;
    public List<Collider2D> ignoreColliders;
    public List<GameObject> ignoreGameObjects;
    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject gO in ignoreGameObjects)
            ignoreColliders.AddRange(gO.GetComponentsInChildren<Collider2D>());

        foreach (Collider2D col in ignoreColliders)
            Physics2D.IgnoreCollision(this.GetComponentInChildren<Collider2D>(), col);
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<RestrictAngles>().restrictAngle(0);
        if(Time.time - landTime >= hopDelay)
            jump();
    }

    public void OnCollisionEnter2D(Collision2D col) {
        GetComponent<RestrictAngles>().collisionUpdate(col);
    }

    private void jump() {
        if (direction == MovementDirections.Left || direction == MovementDirections.Right)
            jump(direction);
        else
            jump((MovementDirections)Random.Range(0, 2));
    }

    private void jump(MovementDirections jumpDirection) {
        if (GetComponent<RestrictAngles>().touchingGround)
        {
            if (jumpDirection == MovementDirections.Left)
            {
                Vector3 scale = this.transform.localScale;
                scale.x = Mathf.Abs(scale.x);
                this.transform.localScale = scale;

                GetComponent<Rigidbody2D>().AddForce(new Vector3(-1 * jumpForce, 2.5F * jumpForce));
            }
            else if (jumpDirection == MovementDirections.Right)
            {
                Vector3 scale = this.transform.localScale;
                scale.x = -Mathf.Abs(scale.x);
                this.transform.localScale = scale;

                GetComponent<Rigidbody2D>().AddForce(new Vector3(1 * jumpForce, 2.5F * jumpForce));
            }

            GetComponentInChildren<Animator>().Play("jump", 0);
            GetComponent<RestrictAngles>().leaveGround();
        }
    }

    override
    public void land() {
        GetComponentInChildren<Animator>().Play("land", 0);
        GetComponent<RestrictAngles>().touchingGround = true;
        landTime = Time.time;
    }
}
