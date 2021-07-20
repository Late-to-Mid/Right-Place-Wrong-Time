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


        // Start is called before the first frame update
        void Start()
        {
            m_Health = GetComponent<Health>();

            weaponAnimator = GetComponent<PlayerWeaponsManager>().weapon.weaponAnimator;
        }

        public void Melee()
        {
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
