using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class SeagullFlightPath : MonoBehaviour
{
    public float flySpeed;
    public float highFlyHeight;
    public float normalFlyHeight;
    public float lowFlyHeight;
    public float flyDownSpeed;
    public float circleRadius;
    public float circleSpeed;
    public float circleTime;
    public float awayTime;
    public Vector3 offset;

    private Transform myT;
    private Transform player;
    private Vector3 awayDir;
    private float flyHeight;
    private Collider col;
    private RaycastHit hit;
    private float distToTarget;
    private float lastHeight;
    private float height;
    private Vector3 terrainSize;
    private TerrainData terrainData;
    private float dTime;


    public virtual void Start()
    {
        this.terrainData = Terrain.activeTerrain.terrainData;
        this.terrainSize = this.terrainData.size;
        this.col = Terrain.activeTerrain.GetComponent<Collider>();
        this.myT = this.transform;
        this.player = GameObject.FindWithTag("Player").transform;
        this.StartCoroutine(this.MainRoutine());
    }

    public virtual IEnumerator MainRoutine()
    {
        while (true)
        {
            yield return this.StartCoroutine(this.ReturnToPlayer());
            yield return this.StartCoroutine(this.CirclePlayer());
            yield return this.StartCoroutine(this.FlyAway());
        }
    }

    public virtual IEnumerator ReturnToPlayer()
    {
        this.distToTarget = 100f;
        while (this.distToTarget > 10)
        {
            Vector3 toPlayer = this.player.position - this.myT.position;
            toPlayer.y = 0;
            this.distToTarget = toPlayer.magnitude;
            Vector3 targetPos = Vector3.zero;
            if (this.distToTarget > 0)
            {
                targetPos = this.transform.position + ((toPlayer / this.distToTarget) * 10);
            }
            else
            {
                targetPos = Vector3.zero;
            }
            targetPos.y = this.terrainData.GetInterpolatedHeight(targetPos.x / this.terrainSize.x, targetPos.z / this.terrainSize.z);
            Vector3 normal = this.terrainData.GetInterpolatedNormal(targetPos.x / this.terrainSize.x, targetPos.z / this.terrainSize.z);
            this.offset = new Vector3(normal.x * 40, 0, normal.z * 40);
            this.flyHeight = this.distToTarget > 80 ? this.highFlyHeight : this.lowFlyHeight;
            if (this.distToTarget > 0)
            {
                this.Move(targetPos - this.transform.position);
            }
            yield return new WaitForSeconds(this.dTime);
        }
    }

    public virtual IEnumerator CirclePlayer()
    {
        float time = 0f;
        while (time < this.circleTime)
        {
            Vector3 circlingPos = this.player.position + new Vector3(Mathf.Cos(Time.time * this.circleSpeed) * this.circleRadius, 0, Mathf.Sin(Time.time * this.circleSpeed) * this.circleRadius);
            circlingPos.y = this.terrainData.GetInterpolatedHeight(circlingPos.x / this.terrainSize.x, circlingPos.z / this.terrainSize.z);
            Vector3 normal = this.terrainData.GetInterpolatedNormal(circlingPos.x / this.terrainSize.x, circlingPos.z / this.terrainSize.z);
            this.offset = new Vector3(normal.x * 40, 0, normal.z * 40);
            this.flyHeight = this.normalFlyHeight;
            this.Move(circlingPos - this.myT.position);
            time = time + this.dTime;
            yield return new WaitForSeconds(this.dTime);
        }
    }

    public virtual IEnumerator FlyAway()
    {
        float radians = (Random.value * 2) * Mathf.PI;
        this.awayDir = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians));
        float time = 0f;
        while (time < this.awayTime)
        {
            Vector3 away = this.player.position + (this.awayDir * 1000);
            away.y = 0;
            Vector3 toAway = away - this.transform.position;
            this.distToTarget = toAway.magnitude;
            Vector3 targetPos = Vector3.zero;
            if (this.distToTarget > 0)
            {
                targetPos = this.transform.position + ((toAway / this.distToTarget) * 10);
            }
            else
            {
                targetPos = Vector3.zero;
            }
            targetPos.y = this.terrainData.GetInterpolatedHeight(targetPos.x / this.terrainSize.x, targetPos.z / this.terrainSize.z);
            Vector3 normal = this.terrainData.GetInterpolatedNormal(targetPos.x / this.terrainSize.x, targetPos.z / this.terrainSize.z);
            this.offset = new Vector3(normal.x * 40, 0, normal.z * 40);
            this.flyHeight = this.highFlyHeight;
            this.Move(targetPos - this.transform.position);
            time = time + this.dTime;
            yield return new WaitForSeconds(this.dTime);
        }
    }

    public virtual void Move(Vector3 delta)
    {
        delta.y = 0;
        delta = (delta.normalized * this.flySpeed) * this.dTime;
        Vector3 newPos = new Vector3(this.myT.position.x + delta.x, 1000, this.myT.position.z + delta.z);
        float newHeight = 0f;
        if (this.col.Raycast(new Ray(newPos, -Vector3.up), out this.hit, 2000))
        {
            newHeight = this.hit.point.y;
        }
        else
        {
            newHeight = 0f;
        }
        if (newHeight < this.lastHeight)
        {
            this.height = Mathf.Lerp(this.height, newHeight, this.flyDownSpeed * this.dTime);
        }
        else
        {
            this.height = newHeight;
        }
        this.lastHeight = newHeight;
        this.myT.position = new Vector3(newPos.x, Mathf.Clamp(this.height, 35.28f, 1000f) + this.flyHeight, newPos.z);
    }

    public SeagullFlightPath()
    {
        this.flySpeed = 15f;
        this.highFlyHeight = 80f;
        this.normalFlyHeight = 40f;
        this.lowFlyHeight = 20f;
        this.flyDownSpeed = 0.1f;
        this.circleRadius = 60f;
        this.circleSpeed = 0.2f;
        this.circleTime = 15f;
        this.awayTime = 20f;
        this.dTime = 0.1f;
    }

}