using UnityEngine;
using System.Collections;

public enum HeronStatus
{
    Idle = 0,
    Walking = 1,
    Running = 2
}

[System.Serializable]
public partial class Heron : MonoBehaviour
{
    public float acceleration;
    public float turning;
    public float maxIdleTime;
    public float seekPlayerTime;
    public float scaredTime;
    public float fishingTime;
    public float shyDistance;
    public float scaredDistance;
    public float strechNeckProbability;
    public float fishWalkSpeed;
    public float walkSpeed;
    public float runSpeed;
    private HeronStatus status;
    private float fishWalkAnimSpeed;
    private float walkAnimSpeed;
    private float runAnimSpeed;
    private float minHeight;
    private float maxHeight;
    private HeronCollider[] colliders;
    private float hitTestDistanceIncrement;
    private float hitTestDistanceMax;
    private float hitTestTimeIncrement;
    private Transform myT;
    private Animation anim;
    private Transform leftKnee;
    private Transform leftAnkle;
    private Transform leftFoot;
    private Transform rightKnee;
    private Transform rightAnkle;
    private Transform rightFoot;
    private Transform player;
    private TerrainData terrain;
    private Vector3 offsetMoveDirection;
    private Vector3 usedMoveDirection;
    private Vector3 velocity;
    private Vector3 forward;
    private bool strechNeck;
    private bool fishing;
    private float lastSpeed;
    public virtual void Start()
    {
        this.forward = this.transform.forward;
        GameObject obj = GameObject.FindWithTag("Player");
        this.player = obj.transform;
        this.myT = this.transform;
        Terrain terr = Terrain.activeTerrain;
        if (terr)
        {
            this.terrain = terr.terrainData;
        }
        this.anim = (Animation) this.GetComponentInChildren(typeof(Animation));
        this.anim["Walk"].speed = this.walkSpeed;
        this.anim["Run"].speed = this.runSpeed;
        this.anim["FishingWalk"].speed = this.fishWalkSpeed;
        this.leftKnee = this.myT.Find("HeronAnimated/MasterMover/RootDummy/Root/Lhip/knee2");
        this.leftAnkle = this.leftKnee.Find("ankle2");
        this.leftFoot = this.leftAnkle.Find("foot2");
        this.rightKnee = this.myT.Find("HeronAnimated/MasterMover/RootDummy/Root/Rhip/knee3");
        this.rightAnkle = this.rightKnee.Find("ankle3");
        this.rightFoot = this.rightAnkle.Find("foot3");
        this.colliders = (HeronCollider[]) UnityEngine.Object.FindObjectsOfType(typeof(HeronCollider));
        this.StartCoroutine(this.MainLoop());
        this.StartCoroutine(this.MoveLoop());
        this.StartCoroutine(this.AwareLoop());
    }

    public virtual IEnumerator MainLoop()
    {
        while (true)
        {
            yield return this.StartCoroutine(this.SeekPlayer());
            yield return this.StartCoroutine(this.Idle());
            yield return this.StartCoroutine(this.Fish());
        }
    }

    public virtual IEnumerator SeekPlayer()
    {
        float time = 0f;
        while (time < this.seekPlayerTime)
        {
            Vector3 moveDirection = this.player.position - this.myT.position;
            if (moveDirection.magnitude < this.shyDistance)
            {
                yield return null;
                yield break;
            }
            moveDirection.y = 0;
            moveDirection = (moveDirection.normalized + (this.myT.forward * 0.5f)).normalized;
            this.offsetMoveDirection = this.GetPathDirection(this.myT.position, moveDirection);
            if (this.offsetMoveDirection != Vector3.zero)
            {
                this.status = HeronStatus.Walking;
            }
            else
            {
                this.status = HeronStatus.Idle;
            }
            yield return new WaitForSeconds(this.hitTestTimeIncrement);
            time = time + this.hitTestTimeIncrement;
        }
    }

    public virtual IEnumerator Idle()
    {
        this.strechNeck = false;
        float time = 0f;
        while (time < this.seekPlayerTime)
        {
            if (time > 0.6f)
            {
                this.strechNeck = true;
            }
            this.status = HeronStatus.Idle;
            this.offsetMoveDirection = Vector3.zero;
            yield return new WaitForSeconds(this.hitTestTimeIncrement);
            time = time + this.hitTestTimeIncrement;
        }
    }

    public virtual IEnumerator Scared()
    {
        float dist = (this.player.position - this.myT.position).magnitude;
        if (dist > this.scaredDistance)
        {
            yield break;
        }
        float time = 0f;
        while (time < this.scaredTime)
        {
            Vector3 moveDirection = this.myT.position - this.player.position;
            if (moveDirection.magnitude > (this.shyDistance * 1.5f))
            {
                yield return null;
                yield break;
            }
            moveDirection.y = 0;
            moveDirection = (moveDirection.normalized + (this.myT.forward * 0.5f)).normalized;
            this.offsetMoveDirection = this.GetPathDirection(this.myT.position, moveDirection);
            if (this.offsetMoveDirection != Vector3.zero)
            {
                this.status = HeronStatus.Running;
            }
            else
            {
                this.status = HeronStatus.Idle;
            }
            yield return new WaitForSeconds(this.hitTestTimeIncrement);
            time = time + this.hitTestTimeIncrement;
        }
    }

    public virtual IEnumerator Fish()
    {
        Vector3 direction = default(Vector3);
        float height = this.terrain.GetInterpolatedHeight(this.myT.position.x / this.terrain.size.x, this.myT.position.z / this.terrain.size.z);
        this.status = HeronStatus.Walking;
        Vector3 randomDir = Random.onUnitSphere;
        if (height > 40)
        {
            this.maxHeight = 40;
            this.offsetMoveDirection = this.GetPathDirection(this.myT.position, randomDir);
            yield return new WaitForSeconds(0.5f);
            if (this.velocity.magnitude > 0.01f)
            {
                direction = this.myT.right * (Random.value > 0.5f ? -1 : 1);
            }
        }
        if (height > 38)
        {
            this.maxHeight = 38;
            this.offsetMoveDirection = this.GetPathDirection(this.myT.position, randomDir);
            yield return new WaitForSeconds(1);
            if (this.velocity.magnitude > 0.01f)
            {
                direction = this.myT.right * (Random.value > 0.5f ? -1 : 1);
            }
        }
        if (height > 36.5f)
        {
            this.maxHeight = 36.5f;
            this.offsetMoveDirection = this.GetPathDirection(this.myT.position, randomDir);
            yield return new WaitForSeconds(1.5f);
            if (this.velocity.magnitude > 0.01f)
            {
                direction = this.myT.right * (Random.value > 0.5f ? -1 : 1);
            }
        }
        while (height > 35)
        {
            this.maxHeight = 35;
            yield return new WaitForSeconds(0.5f);
            if (this.velocity.magnitude > 0.01f)
            {
                direction = this.myT.right * (Random.value > 0.5f ? -1 : 1);
            }
            this.offsetMoveDirection = this.GetPathDirection(this.myT.position, randomDir);
            height = this.terrain.GetInterpolatedHeight(this.myT.position.x / this.terrain.size.x, this.myT.position.z / this.terrain.size.z);
        }
        this.fishing = true;
        this.status = HeronStatus.Walking;
        yield return new WaitForSeconds(this.fishingTime / 3);
        this.status = HeronStatus.Idle;
        yield return new WaitForSeconds(this.fishingTime / 6);
        this.status = HeronStatus.Walking;
        yield return new WaitForSeconds(this.fishingTime / 3);
        this.status = HeronStatus.Idle;
        yield return new WaitForSeconds(this.fishingTime / 6);
        this.fishing = false;
        this.maxHeight = 42;
    }

    public virtual IEnumerator AwareLoop()
    {
        while (true)
        {
            float dist = (this.player.position - this.myT.position).magnitude;
            if ((dist < this.scaredDistance) && (this.status != HeronStatus.Running))
            {
                this.StopCoroutine("Fish");
                this.maxHeight = 42;
                this.StopCoroutine("Idle");
                this.strechNeck = false;
                this.StopCoroutine("SeekPlayer");
                this.StartCoroutine(this.Scared());
            }
            yield return null;
        }
    }

    public virtual IEnumerator MoveLoop()
    {
        while (true)
        {
            float deltaTime = Time.deltaTime;
            float targetSpeed = 0f;
            if ((this.status == HeronStatus.Walking) && (this.offsetMoveDirection.magnitude > 0.01f))
            {
                if (!this.fishing)
                {
                    targetSpeed = this.walkAnimSpeed * this.walkSpeed;
                    this.anim.CrossFade("Walk", 0.4f);
                }
                else
                {
                    targetSpeed = this.fishWalkAnimSpeed * this.fishWalkSpeed;
                    this.anim.CrossFade("FishingWalk", 0.4f);
                }
            }
            else
            {
                if (this.status == HeronStatus.Running)
                {
                    targetSpeed = this.runAnimSpeed * this.runSpeed;
                    this.anim.CrossFade("Run", 0.4f);
                }
                else
                {
                    if (!this.fishing)
                    {
                        targetSpeed = 0;
                        if (!this.strechNeck)
                        {
                            this.anim.CrossFade("IdleHold", 0.4f);
                        }
                        else
                        {
                            this.anim.CrossFade("IdleStrechNeck", 0.4f);
                        }
                    }
                    else
                    {
                        targetSpeed = 0;
                        this.anim.CrossFade("IdleFishing", 0.4f);
                    }
                }
            }
            this.usedMoveDirection = Vector3.Lerp(this.usedMoveDirection, this.offsetMoveDirection, deltaTime * 0.7f);
            this.velocity = Vector3.RotateTowards(this.velocity, this.offsetMoveDirection * targetSpeed, this.turning * deltaTime, this.acceleration * deltaTime);
            this.velocity.y = 0;
            if (this.velocity.magnitude > 0.01f)
            {
                if (this.lastSpeed < 0.01f)
                {
                    this.velocity = this.forward * 0.1f;
                }
                else
                {
                    this.forward = this.velocity.normalized;
                }
            }
            this.transform.position = this.transform.position + (this.velocity * deltaTime);
            this.transform.rotation = Quaternion.LookRotation(this.forward);
            this.lastSpeed = this.velocity.magnitude;
            yield return null;
        }
    }

    public virtual Vector3 GetPathDirection(Vector3 curPos, Vector3 wantedDirection)
    {
        Vector3 awayFromCollision = this.TestPosition(curPos);
        if (awayFromCollision != Vector3.zero)
        {
             //Debug.DrawRay(myT.position, awayFromCollision.normalized * 20, Color.yellow);
            return awayFromCollision.normalized;
        }
        else
        {
        }
         ///Debug.DrawRay(myT.position, Vector3.up * 5, Color.yellow);
        Vector3 right = Vector3.Cross(wantedDirection, Vector3.up);
        float currentLength = this.TestDirection(this.myT.position, wantedDirection);
        if (currentLength > this.hitTestDistanceMax)
        {
            return wantedDirection;
        }
        else
        {
            float sideAmount = 1 - Mathf.Clamp01(currentLength / 50);
            Vector3 rightDirection = Vector3.Lerp(wantedDirection, right, sideAmount * sideAmount);
            float rightLength = this.TestDirection(this.myT.position, rightDirection);
            Vector3 leftDirection = Vector3.Lerp(wantedDirection, -right, sideAmount * sideAmount);
            float leftLength = this.TestDirection(this.myT.position, leftDirection);
            if (((rightLength > leftLength) && (rightLength > currentLength)) && (rightLength > this.hitTestDistanceIncrement))
            {
                return rightDirection.normalized;
            }
            if (((leftLength > rightLength) && (leftLength > currentLength)) && (leftLength > this.hitTestDistanceIncrement))
            {
                return leftDirection.normalized;
            }
        }
        if (currentLength > this.hitTestDistanceIncrement)
        {
            return wantedDirection;
        }
        return Vector3.zero;
    }

    public virtual float TestDirection(Vector3 position, Vector3 direction)
    {
        float length = 0f;
        while (true)
        {
            length = length + this.hitTestDistanceIncrement;
            if (length > this.hitTestDistanceMax)
            {
                return length;
            }
            Vector3 testPos = position + (direction * length);
            float height = this.terrain.GetInterpolatedHeight(testPos.x / this.terrain.size.x, testPos.z / this.terrain.size.z);
            if ((height > this.maxHeight) || (height < this.minHeight))
            {
                break;
            }
            else
            {
                bool hit = false;
                int i = 0;
                while (i < this.colliders.Length)
                {
                    HeronCollider collider = this.colliders[i];
                    float x = collider.position.x - testPos.x;
                    float z = collider.position.z - testPos.z;
                    if (x < 0)
                    {
                        x = -x;
                    }
                    if (z < 0)
                    {
                        z = -z;
                    }
                    if ((z + x) < collider.radius)
                    {
                        hit = true;
                        break;
                    }
                    i++;
                }
                if (hit)
                {
                    break;
                }
            }
        }
        return length;
    }

    public virtual Vector3 TestPosition(Vector3 testPos)
    {
        Vector3 moveDir = default(Vector3);
        Vector3 hieghtPos = testPos;
        float height = this.terrain.GetInterpolatedHeight(testPos.x / this.terrain.size.x, testPos.z / this.terrain.size.z);
        if ((height > this.maxHeight) || (height < this.minHeight))
        {
            float heightDiff = 100f;
            float optimalHeight = (this.maxHeight * 0.5f) + (this.minHeight * 0.5f);
            bool found = false;
            float mult = 1f;
            while (!found && (mult < 5))
            {
                float rotation = 0f;
                while (rotation < 360)
                {
                    Vector3 forwardDir = Quaternion.Euler(0, rotation, 0) * Vector3.forward;
                    Vector3 forwardPos = testPos + (((forwardDir * this.hitTestDistanceIncrement) * mult) * 3);
                    //Debug.DrawRay(forwardPos, Vector3.up, Color(0.9, 0.1, 0.1, 0.7));
                    float forwardHeight = this.terrain.GetInterpolatedHeight(forwardPos.x / this.terrain.size.x, forwardPos.z / this.terrain.size.z);
                    float diff = Mathf.Abs(forwardHeight - optimalHeight);
                    if (((forwardHeight < this.maxHeight) && (forwardHeight > this.minHeight)) && (heightDiff > diff))
                    {
                         //Debug.DrawRay(forwardPos, Vector3.up, Color.green);
                        found = true;
                        heightDiff = diff;
                        hieghtPos = forwardPos;
                    }
                    rotation = rotation + 45;
                }
                mult = mult + 0.5f;
            }
        }
        Vector3 move = hieghtPos - testPos;
        if (move.magnitude > 0.01f)
        {
             //print("height");
            moveDir = move.normalized;
        }
        else
        {
             //print("noheight");
            moveDir = Vector3.zero;
        }
        int i = 0;
        while (i < this.colliders.Length)
        {
            HeronCollider collider = this.colliders[i];
            float x = collider.position.x - testPos.x;
            float z = collider.position.z - testPos.z;
            if (x < 0)
            {
                x = -x;
            }
            if (z < 0)
            {
                z = -z;
            }
            if ((z + x) < collider.radius)
            {
                moveDir = moveDir + (testPos - collider.position).normalized;
                break;
            }
            i++;
        }
        return moveDir;
    }

    public virtual void LateUpdate() // leg IK
    {
        float rightHeight = this.terrain.GetInterpolatedHeight(this.rightFoot.position.x / this.terrain.size.x, this.rightFoot.position.z / this.terrain.size.z);
        Vector3 rightNormal = this.terrain.GetInterpolatedNormal(this.rightFoot.position.x / this.terrain.size.x, this.rightFoot.position.z / this.terrain.size.z);
        float leftHeight = this.terrain.GetInterpolatedHeight(this.leftFoot.position.x / this.terrain.size.x, this.leftFoot.position.z / this.terrain.size.z);
        Vector3 leftNormal = this.terrain.GetInterpolatedNormal(this.leftFoot.position.x / this.terrain.size.x, this.leftFoot.position.z / this.terrain.size.z);
        if (leftHeight < rightHeight)
        {

            {
                float _3 = leftHeight;
                Vector3 _4 = this.transform.position;
                _4.y = _3;
                this.transform.position = _4;
            }
            this.leftFoot.rotation = Quaternion.LookRotation(this.leftFoot.forward, leftNormal);
            this.leftFoot.Rotate(Vector3.right * 15);
            float raise = (rightHeight - leftHeight) * 0.5f;

            {
                float _5 = this.rightKnee.position.y + raise;
                Vector3 _6 = this.rightKnee.position;
                _6.y = _5;
                this.rightKnee.position = _6;
            }

            {
                float _7 = this.rightAnkle.position.y + raise;
                Vector3 _8 = this.rightAnkle.position;
                _8.y = _7;
                this.rightAnkle.position = _8;
            }
            this.rightFoot.rotation = Quaternion.LookRotation(rightNormal, this.rightFoot.up);
            this.rightFoot.Rotate(-Vector3.right * 15);
        }
        else
        {

            {
                float _9 = rightHeight;
                Vector3 _10 = this.transform.position;
                _10.y = _9;
                this.transform.position = _10;
            }
            this.rightFoot.rotation = Quaternion.LookRotation(rightNormal, this.rightFoot.up);
            this.rightFoot.Rotate(-Vector3.right * 15);
            float raise = (leftHeight - rightHeight) * 0.5f;

            {
                float _11 = this.leftKnee.position.y + raise;
                Vector3 _12 = this.leftKnee.position;
                _12.y = _11;
                this.leftKnee.position = _12;
            }

            {
                float _13 = this.leftAnkle.position.y + raise;
                Vector3 _14 = this.leftAnkle.position;
                _14.y = _13;
                this.leftAnkle.position = _14;
            }
            this.leftFoot.rotation = Quaternion.LookRotation(this.leftFoot.forward, leftNormal);
            this.leftFoot.Rotate(Vector3.right * 15);
        }

        {
            float _15 = this.transform.position.y + 0.1f;
            Vector3 _16 = this.transform.position;
            _16.y = _15;
            this.transform.position = _16;
        }
    }

    public Heron()
    {
        this.acceleration = 5f;
        this.turning = 3f;
        this.maxIdleTime = 4f;
        this.seekPlayerTime = 6f;
        this.scaredTime = 4f;
        this.fishingTime = 30f;
        this.shyDistance = 10f;
        this.scaredDistance = 5f;
        this.strechNeckProbability = 10f;
        this.fishWalkSpeed = 1f;
        this.walkSpeed = 1f;
        this.runSpeed = 1f;
        this.status = HeronStatus.Idle;
        this.fishWalkAnimSpeed = 0.5f;
        this.walkAnimSpeed = 2f;
        this.runAnimSpeed = 9f;
        this.minHeight = 34.1f;
        this.maxHeight = 42f;
        this.hitTestDistanceIncrement = 1f;
        this.hitTestDistanceMax = 50f;
        this.hitTestTimeIncrement = 0.2f;
    }

}