using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerScripts
{
    public class PlayerMelee : MonoBehaviour
    {
        public float damage = 10f;
        public float healAmount = 50f;
        public LayerMask hittableLayers = -1;
        [Tooltip("Optional weapon animator for OnShoot animations")]
        Animator weaponAnimator;

        Health m_Health;

        const string k_AnimMeleeParameter = "Melee";

        float m_TimeActivated;

        public float cooldown = 0.25f;


        // Start is called before the first frame update
        void Start()
        {
            m_Health = GetComponent<Health>();

            weaponAnimator = GetComponent<PlayerWeaponsManager>().weapon.weaponAnimator;

            m_TimeActivated = -cooldown;
        }

        public void Melee()
        {
            if (Time.time > m_TimeActivated + cooldown)
            {
                m_TimeActivated = Time.time;
                weaponAnimator.SetTrigger(k_AnimMeleeParameter);
                Collider[] colliders = Physics.OverlapBox(transform.position + transform.forward * 1f, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, hittableLayers, QueryTriggerInteraction.Ignore);
                foreach (Collider collider in colliders) 
                {
                    if (collider.GetComponent<Damageable>() != null)
                    {
                        Damageable damageable = collider.GetComponent<Damageable>();
                        damageable.InflictDamage(damage, false, gameObject);
                        m_Health.Heal(healAmount);

                    }
                }
            }
        }
    }
}
