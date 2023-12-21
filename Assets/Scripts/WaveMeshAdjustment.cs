using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class WaveMeshAdjustment : MonoBehaviour
{
    public float offset;
    public Collider col;
    public virtual void Start()
    {
        RaycastHit hit = default(RaycastHit);
        Vector3 dir = default(Vector3);
        MeshFilter filter = (MeshFilter) this.GetComponent(typeof(MeshFilter));
        Mesh mesh = filter.mesh;
        Transform mTransform = this.transform;
        Vector3[] vertices = mesh.vertices;
        int i = 1;
        while (i < (vertices.Length - 1)) // i - 1 == terrain side        // i == water side
        {
            dir = vertices[i - 1] - vertices[i];
            if ((mTransform.TransformDirection(dir) != Vector3.zero) && this.col.Raycast(new Ray(mTransform.TransformPoint(vertices[i]), mTransform.TransformDirection(dir)), out hit, 30f))
            {
                Vector3 hitPoint = mTransform.InverseTransformPoint(hit.point);
                Vector3 shorePos = hitPoint + (dir / 3);
                shorePos.y = shorePos.y + 15;
                if (this.col.Raycast(new Ray(mTransform.TransformPoint(shorePos), -Vector3.up), out hit, 30f))
                {
                    hitPoint = mTransform.InverseTransformPoint(hit.point);
                }
                hitPoint.y = hitPoint.y + this.offset;
                if (hitPoint.y > 1.5f)
                {
                    hitPoint.y = 0;
                }
                vertices[i - 1] = hitPoint;
            }
            i = i + 2;
        }
        mesh.vertices = vertices;
        filter.mesh = mesh;
    }

}