using UnityEngine;

public class PickupRandomizer : MonoBehaviour
{
    [Tooltip("Prefabs of pickups this thing should spawn")]
    public Pickup[] pickups;

    // Start is called before the first frame update
    void Start()
    {
        int i = Random.Range(0, pickups.Length);

        Instantiate(pickups[i], transform.position, transform.rotation);

        Destroy(gameObject, 0);
    }
}
