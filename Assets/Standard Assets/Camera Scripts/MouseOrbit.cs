using UnityEngine;
using System.Collections;

[System.Serializable]
[UnityEngine.AddComponentMenu("Camera-Control/Mouse Orbit")]
public partial class MouseOrbit : MonoBehaviour
{
    public Transform target;
    public float distance;
    public float xSpeed;
    public float ySpeed;
    public int yMinLimit;
    public int yMaxLimit;
    private float x;
    private float y;
    public virtual void Start()
    {
        Vector3 angles = this.transform.eulerAngles;
        this.x = angles.y;
        this.y = angles.x;
        // Make the rigid body not change rotation
        if (this.GetComponent<Rigidbody>())
        {
            this.GetComponent<Rigidbody>().freezeRotation = true;
        }
    }

    public virtual void LateUpdate()
    {
        if (this.target)
        {
            this.x = this.x + ((Input.GetAxis("Mouse X") * this.xSpeed) * 0.02f);
            this.y = this.y - ((Input.GetAxis("Mouse Y") * this.ySpeed) * 0.02f);
            this.y = MouseOrbit.ClampAngle(this.y, this.yMinLimit, this.yMaxLimit);
            Quaternion rotation = Quaternion.Euler(this.y, this.x, 0);
            Vector3 position = (rotation * new Vector3(0f, 0f, -this.distance)) + this.target.position;
            this.transform.rotation = rotation;
            this.transform.position = position;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
        {
            angle = angle + 360;
        }
        if (angle > 360)
        {
            angle = angle - 360;
        }
        return Mathf.Clamp(angle, min, max);
    }

    public MouseOrbit()
    {
        this.distance = 10f;
        this.xSpeed = 250f;
        this.ySpeed = 120f;
        this.yMinLimit = -20;
        this.yMaxLimit = 80;
    }

}