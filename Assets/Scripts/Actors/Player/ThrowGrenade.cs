using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerScripts
{
    public class ThrowGrenade : PlayerAbilityBase
    {
        public float throwForce = 40f;
        public float cooldown = 2.5f;
        public GameObject grenade;
        public Camera playerCamera;

        protected override void Start()
        {
            base.Start();
            m_TimeActivated = -cooldown;
        }

        void Update()
        {
            readyBar = (Time.time - m_TimeActivated) / cooldown;
            readyBar = Mathf.Clamp(readyBar, 0, 1);
        }

        public override void CheckToUseAbility()
        {
            if (Time.time > m_TimeActivated + cooldown)
            {
                GameObject thrown_grenade = Instantiate(grenade, transform.position + Vector3.up * 1.4f, transform.rotation);
                Rigidbody rb = thrown_grenade.GetComponent<Rigidbody>();
                rb.AddForce(playerCamera.transform.forward * throwForce);
                m_TimeActivated = Time.time;
            }
        }
    }
}
