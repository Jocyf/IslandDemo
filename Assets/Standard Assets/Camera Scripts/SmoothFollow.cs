using UnityEngine;
using System.Collections;

/*
This camera smoothes out rotation around the y-axis and height.
Horizontal Distance to the target is always fixed.

There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.

For every of those smoothed values we calculate the wanted value and the current value.
Then we smooth it using the Lerp function.
Then we apply the smoothed values to the transform's position.
*/
// The target we are following
// The distance in the x-z plane to the target
// the height we want the camera to be above the target
// How much we 
// Place the script in the Camera-Control group in the component menu
[System.Serializable]
[UnityEngine.AddComponentMenu("Camera-Control/Smooth Follow")]
public partial class SmoothFollow : MonoBehaviour
{
    public Transform target;
    public float distance;
    public float height;
    public float heightDamping;
    public float rotationDamping;
    private float wantedRotationAngle;
    private float wantedHeight;
    private float currentRotationAngle;
    private float currentHeight;
    private Quaternion currentRotation;
    public virtual void LateUpdate()
    {
        // Early out if we don't have a target
        if (!this.target)
        {
            return;
        }
        // Calculate the current rotation angles
        this.wantedRotationAngle = this.target.eulerAngles.y;
        this.wantedHeight = this.target.position.y + this.height;
        this.currentRotationAngle = this.transform.eulerAngles.y;
        this.currentHeight = this.transform.position.y;
        // Damp the rotation around the y-axis
        this.currentRotationAngle = Mathf.LerpAngle(this.currentRotationAngle, this.wantedRotationAngle, this.rotationDamping * Time.deltaTime);
        // Damp the height
        this.currentHeight = Mathf.Lerp(this.currentHeight, this.wantedHeight, this.heightDamping * Time.deltaTime);
        // Convert the angle into a rotation
        // The quaternion interface uses radians not degrees so we need to convert from degrees to radians
        this.currentRotation = Quaternion.Euler(0, this.currentRotationAngle, 0);
        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        this.transform.position = this.target.position;
        this.transform.position = this.transform.position - ((this.currentRotation * Vector3.forward) * this.distance);

        {
            float _17 = // Set the height of the camera
            this.currentHeight;
            Vector3 _18 = this.transform.position;
            _18.y = _17;
            this.transform.position = _18;
        }
        // Always look at the target
        this.transform.LookAt(this.target);
    }

    public SmoothFollow()
    {
        this.distance = 10f;
        this.height = 5f;
        this.heightDamping = 2f;
        this.rotationDamping = 3f;
    }

}