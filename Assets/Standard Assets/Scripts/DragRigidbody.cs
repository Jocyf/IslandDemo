using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class DragRigidbody : MonoBehaviour
{
    public float spring;
    public float damper;
    public float drag;
    public float angularDrag;
    public float distance;
    public bool attachToCenterOfMass;
    private SpringJoint springJoint;
    public virtual void Update()
    {
        RaycastHit hit = default(RaycastHit);
         // Make sure the user pressed the mouse down
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }
        Camera mainCamera = this.FindCamera();
        // We need to actually hit an object
        if (!Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, 100))
        {
            return;
        }
        // We need to hit a rigidbody that is not kinematic
        if (!hit.rigidbody || hit.rigidbody.isKinematic)
        {
            return;
        }
        if (!this.springJoint)
        {
            GameObject go = new GameObject("Rigidbody dragger");
            Rigidbody body = go.AddComponent<Rigidbody>();
            this.springJoint = go.AddComponent<SpringJoint>();
            body.isKinematic = true;
        }
        this.springJoint.transform.position = hit.point;
        if (this.attachToCenterOfMass)
        {
            Vector3 anchor = this.transform.TransformDirection(hit.rigidbody.centerOfMass) + hit.rigidbody.transform.position;
            anchor = this.springJoint.transform.InverseTransformPoint(anchor);
            this.springJoint.anchor = anchor;
        }
        else
        {
            this.springJoint.anchor = Vector3.zero;
        }
        this.springJoint.spring = this.spring;
        this.springJoint.damper = this.damper;
        this.springJoint.maxDistance = this.distance;
        this.springJoint.connectedBody = hit.rigidbody;
        this.StartCoroutine("DragObject", hit.distance);
    }

    public virtual IEnumerator DragObject(float distance)
    {
        float oldDrag = this.springJoint.connectedBody.drag;
        float oldAngularDrag = this.springJoint.connectedBody.angularDrag;
        this.springJoint.connectedBody.drag = this.drag;
        this.springJoint.connectedBody.angularDrag = this.angularDrag;
        Camera mainCamera = this.FindCamera();
        while (Input.GetMouseButton(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            this.springJoint.transform.position = ray.GetPoint(distance);
            yield return null;
        }
        if (this.springJoint.connectedBody)
        {
            this.springJoint.connectedBody.drag = oldDrag;
            this.springJoint.connectedBody.angularDrag = oldAngularDrag;
            this.springJoint.connectedBody = null;
        }
    }

    public virtual Camera FindCamera()
    {
        if (this.GetComponent<Camera>())
        {
            return this.GetComponent<Camera>();
        }
        else
        {
            return Camera.main;
        }
    }

    public DragRigidbody()
    {
        this.spring = 50f;
        this.damper = 5f;
        this.drag = 10f;
        this.angularDrag = 5f;
        this.distance = 0.2f;
    }

}