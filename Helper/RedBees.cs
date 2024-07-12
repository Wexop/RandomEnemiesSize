using UnityEngine;

namespace RandomEnemiesSize;

public class RedBees
{
    public GameObject GameObject;
    public Vector3 baseScale = Vector3.one;
    public Vector3 SizedScale = Vector3.one;
    public int lastState = -1;
    public float multiplier = 1f;
}