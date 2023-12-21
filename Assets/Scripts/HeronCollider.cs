using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class HeronCollider : MonoBehaviour
{
    public float radius;
    public Vector3 position;
    public virtual void Awake()
    {
        this.position = this.transform.position;
    }

    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.32f, 0.55f, 0.76f, 0.7f);
        Gizmos.DrawWireSphere(this.transform.position, this.radius);
    }

}