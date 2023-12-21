using UnityEngine;
using System.Collections;

[System.Serializable]
// We are grounded, so recalculate movedirection directly from axes
// Apply gravity
// Move the controller
[UnityEngine.RequireComponent(typeof(CharacterController))]
//myWalker.grounded=true;
[UnityEngine.RequireComponent(typeof(FPSWalker))]
public partial class FPFlyer : MonoBehaviour
{
    public float speed;
    public float jumpSpeed;
    public float gravity;
    private Vector3 moveDirection;
    private bool grounded;
    public virtual void FixedUpdate()
    {
        float my = this.moveDirection.y;
        this.moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        this.moveDirection = this.transform.TransformDirection(this.moveDirection);
        if (this.grounded)
        {
            this.moveDirection = this.moveDirection * this.speed;
        }
        else
        {
            this.moveDirection = this.moveDirection * (this.speed + (this.transform.position.y / 5));
            this.moveDirection.y = my;
        }
        if (Input.GetButton("Jump"))
        {
            this.moveDirection.y = this.jumpSpeed;
        }
        this.moveDirection.y = this.moveDirection.y - (this.gravity * Time.deltaTime);
        CharacterController controller = (CharacterController) this.GetComponent(typeof(CharacterController));
        CollisionFlags flags = controller.Move(this.moveDirection * Time.deltaTime);
        this.grounded = (flags & CollisionFlags.CollidedBelow) != (CollisionFlags) 0;
    }

    private FPSWalker myWalker;
    public float maxHeight;
    public virtual void Start()
    {
        this.myWalker = (FPSWalker) this.gameObject.GetComponent(typeof(FPSWalker));
    }

    public virtual void Update()
    {
        if (Input.GetKey("left shift"))
        {
            this.myWalker.gravity = -20;
        }
        else
        {
            this.myWalker.gravity = 10;
        }
        if (this.transform.position.y > this.maxHeight)
        {
            this.myWalker.gravity = 20;
        }
    }

    public FPFlyer()
    {
        this.speed = 6f;
        this.jumpSpeed = 8f;
        this.gravity = 20f;
        this.moveDirection = Vector3.zero;
        this.maxHeight = 250;
    }

}