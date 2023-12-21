using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class Seagull : MonoBehaviour
{
    public AudioClip[] sounds;
    public float soundFrequency;
    public float minSpeed;
    public float turnSpeed;
    public float randomFreq;
    public float randomForce;
    public float toOriginForce;
    public float toOriginRange;
    public float damping;
    public float gravity;
    public float avoidanceRadius;
    public float avoidanceForce;
    public float followVelocity;
    public float followRadius;
    public float bankTurn;
    public bool raycast;
    public float bounce;

    private SeagullFlightPath target;
    private Transform origin;
    private Vector3 velocity;
    private Vector3 normalizedVelocity;
    private Vector3 randomPush;
    private Vector3 originPush;
    private Vector3 gravPush;
    private RaycastHit hit;
    private Transform[] objects;
    private Seagull[] otherSeagulls;
    private Animation animationComponent;
    private Transform transformComponent;
    private bool gliding;
    private float bank;
    private AnimationState glide;

    public virtual void Start()
    {
        this.randomFreq = 1f / this.randomFreq;
        this.gameObject.tag = this.transform.parent.gameObject.tag;
        this.animationComponent = (Animation) this.GetComponentInChildren(typeof(Animation));
        this.animationComponent.Blend("fly");
        this.animationComponent["fly"].normalizedTime = Random.value;
        this.glide = this.animationComponent["glide"];
        this.origin = this.transform.parent;
        this.target = (SeagullFlightPath) this.origin.GetComponent(typeof(SeagullFlightPath));
        this.transform.parent = this.transform.parent.parent; // null; /**/
        this.transformComponent = this.transform;
        Component[] tempSeagulls = new Component[0];
        if (this.transform.parent)
        {
            tempSeagulls = this.transform.parent.GetComponentsInChildren(typeof(Seagull));
        }
        this.objects = new Transform[tempSeagulls.Length];
        this.otherSeagulls = new Seagull[tempSeagulls.Length];
        int i = 0;
        while (i < tempSeagulls.Length)
        {
            this.objects[i] = tempSeagulls[i].transform;
            this.otherSeagulls[i] = (Seagull) tempSeagulls[i];
            i++;
        }
        this.StartCoroutine(this.UpdateRandom());
    }

    public virtual IEnumerator UpdateRandom()
    {
        while (true)
        {
            this.randomPush = Random.insideUnitSphere * this.randomForce;
            yield return new WaitForSeconds(this.randomFreq + Random.Range(-this.randomFreq / 2, this.randomFreq / 2));
        }
    }

    public virtual void Update()
    {
        float speed = this.velocity.magnitude;
        Vector3 avoidPush = Vector3.zero;
        Vector3 avgPoint = Vector3.zero;
        int count = 0;
        float f = 0f;
        Vector3 myPosition = this.transformComponent.position;
        int i = 0;
        Vector3 forceV = Vector3.zero;
        float d = 0f;
        while (i < this.objects.Length)
        {
            Transform o = this.objects[i];
            if (o != this.transformComponent)
            {
                Vector3 otherPosition = o.position;
                avgPoint = avgPoint + otherPosition;
                count++;
                forceV = myPosition - otherPosition;
                d = forceV.magnitude;
                if (d < this.followRadius)
                {
                    if (d < this.avoidanceRadius)
                    {
                        f = 1f - (d / this.avoidanceRadius);
                        if (d > 0)
                        {
                            avoidPush = avoidPush + (((forceV / d) * f) * this.avoidanceForce);
                        }
                    }
                    f = d / this.followRadius;
                    Seagull otherSealgull = this.otherSeagulls[i];
                    avoidPush = avoidPush + ((otherSealgull.normalizedVelocity * f) * this.followVelocity);
                }
            }
            i++;
        }

        Vector3 toAvg = Vector3.zero;
        if (count > 0)
        {
            avoidPush = avoidPush / count;
            toAvg = (avgPoint / count) - myPosition;
        }
        else
        {
            toAvg = Vector3.zero;
        }
        forceV = (this.origin.position + this.target.offset) - myPosition;
        d = forceV.magnitude;
        f = d / this.toOriginRange;
        if (d > 0)
        {
            this.originPush = ((forceV / d) * f) * this.toOriginForce;
        }
        if ((speed < this.minSpeed) && (speed > 0))
        {
            this.velocity = (this.velocity / speed) * this.minSpeed;
        }
        
        Vector3 wantedVel = this.velocity;
        wantedVel = wantedVel - ((wantedVel * this.damping) * Time.deltaTime);
        wantedVel = wantedVel + (this.randomPush * Time.deltaTime);
        wantedVel = wantedVel + (this.originPush * Time.deltaTime);
        wantedVel = wantedVel + (avoidPush * Time.deltaTime);
        wantedVel = wantedVel + ((toAvg.normalized * this.gravity) * Time.deltaTime);
        Vector3 diff = this.transformComponent.InverseTransformDirection(wantedVel - this.velocity).normalized;
        this.bank = Mathf.Lerp(this.bank, diff.x, Time.deltaTime * 0.8f);
        this.velocity = Vector3.RotateTowards(this.velocity, wantedVel, this.turnSpeed * Time.deltaTime, 100f);
        this.transformComponent.rotation = Quaternion.LookRotation(this.velocity);
        this.transformComponent.Rotate(0, 0, -this.bank * this.bankTurn);
        // Raycast
        float distance = speed * Time.deltaTime;
        if ((this.raycast && (distance > 0f)) && Physics.Raycast(myPosition, this.velocity, out this.hit, distance))
        {
            this.velocity = Vector3.Reflect(this.velocity, this.hit.normal) * this.bounce;
        }
        else
        {
            this.transformComponent.Translate(this.velocity * Time.deltaTime, Space.World);
        }
        // Animation Controls
        if (speed > 0)
        {
            float up = (this.velocity / speed).y;
            if (this.gliding && (up > 0))
            {
                this.gliding = false;
                this.animationComponent.Blend("glide", 0f, 0.2f);
                this.animationComponent.Blend("fly", 1f, 0.2f);
            }
            if (!this.gliding && (up < -0.2f))
            {
                this.gliding = true;
                this.animationComponent.Blend("glide", 1f, 0.2f);
                this.animationComponent.Blend("fly", 0f, 0.2f);
                this.glide.speed = 0;
            }
        }
        // Sounds
        if (SeagullSoundHeat.heat < Mathf.Pow(Random.value, (1 / this.soundFrequency) / Time.deltaTime))
        {
            AudioSource.PlayClipAtPoint(this.sounds[Random.Range(0, this.sounds.Length)], myPosition, 0.9f);
            SeagullSoundHeat.heat = SeagullSoundHeat.heat + ((1 / this.soundFrequency) / 10);
        }
        this.normalizedVelocity = this.velocity.normalized;
    }

    public Seagull()
    {
        this.sounds = new AudioClip[0];
        this.soundFrequency = 1f;
        this.bounce = 0.8f;
    }

}