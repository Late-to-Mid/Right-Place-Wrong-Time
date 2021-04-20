using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class CharacterAbility : MonoBehaviour
{
    [Tooltip("Dummy to be placed that enemies will shoot at")]
    public GameObject playerDummyObject;
    [Tooltip("Time stealth lasts")]
    [Range(0f, 10f)]
    public float timeLength = 5f;
    [Tooltip("Hitbox of player, to be deactivated on ability use")]
    public GameObject playerHitbox;

    float m_TimeActivated;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerHitbox.activeSelf == false && Time.time > m_TimeActivated + timeLength)
        {
            playerHitbox.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.T) && playerHitbox.activeSelf)
        {
            playerHitbox.SetActive(false);
            m_TimeActivated = Time.time;
        }
    }
}
