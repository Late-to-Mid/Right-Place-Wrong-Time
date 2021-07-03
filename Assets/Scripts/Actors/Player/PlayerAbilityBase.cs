using UnityEngine;
using UnityEngine.Events;

namespace PlayerScripts
{
    [RequireComponent(typeof(Actor), typeof(PlayerCharacterController), typeof(PlayerInputHandler))]
    public class PlayerAbilityBase : MonoBehaviour
    {
        public enum AbilityState
        {
            Cooldown,
            Ready,
            Active
        }

        public float readyBar { get; protected set; }
        // public float cooldown = 5f;
        protected AbilityState m_State = AbilityState.Ready;
        protected float m_TimeActivated;
        protected float m_TimeEnded;
        protected Actor m_Actor;
        protected ActorsManager actorsManager;
        protected PlayerInputHandler m_PlayerInputHandler;
        protected PlayerWeaponsManager m_PlayerWeaponsManager;
        protected Animator m_Animator;

        public UnityAction onAbilityUsed;
        public UnityAction onAbilityOver;

        protected virtual void Start()
        {
            m_Actor = GetComponent<Actor>();
            actorsManager = FindObjectOfType<ActorsManager>();

            m_PlayerInputHandler = GetComponent<PlayerInputHandler>();

            m_PlayerWeaponsManager = GetComponent<PlayerWeaponsManager>();

            readyBar = 1f;
        }

        public virtual void CheckToUseAbility() 
        {
            Debug.Log("Used Ability");
        }
    }
}