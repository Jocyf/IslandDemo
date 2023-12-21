using UnityEngine;
using System.Collections;

[System.Serializable]
// We are grounded, so recalculate movedirection directly from axes
// Apply gravity
// Move the controller
[UnityEngine.RequireComponent(typeof(CharacterController))]
public partial class FPSWalker : MonoBehaviour
{
    public float speed;
    public float jumpSpeed;
    public float gravity;
    private Vector3 moveDirection;
    private bool grounded;
    public virtual void FixedUpdate()
    {
        if (this.grounded)
        {
            this.moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            this.moveDirection = this.transform.TransformDirection(this.moveDirection);
            this.moveDirection = this.moveDirection * this.speed;
            if (Input.GetButton("Jump"))
            {
                this.moveDirection.y = this.jumpSpeed;
            }
        }
        this.moveDirection.y = this.moveDirection.y - (this.gravity * Time.deltaTime);
        CharacterController controller = (CharacterController) this.GetComponent(typeof(CharacterController));
        CollisionFlags flags = controller.Move(this.moveDirection * Time.deltaTime);
        this.grounded = (flags & CollisionFlags.CollidedBelow) != (CollisionFlags) 0;
    }

    public FPSWalker()
    {
        this.speed = 6f;
        this.jumpSpeed = 8f;
        this.gravity = 20f;
        this.moveDirection = Vector3.zero;
    }

}