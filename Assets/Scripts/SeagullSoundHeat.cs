using UnityEngine;
using System.Collections;

[System.Serializable]
public partial class SeagullSoundHeat : MonoBehaviour
{
    public static float heat;

    public virtual void Update()
    {
        if (SeagullSoundHeat.heat > 0) { SeagullSoundHeat.heat -= Time.deltaTime; }
    }

}