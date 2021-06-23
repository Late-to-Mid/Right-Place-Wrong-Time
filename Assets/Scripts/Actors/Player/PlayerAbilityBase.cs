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
        protected AbilityState m_State = AbilityState.Ready;
        protected float m_TimeActivated;
        protected float m_TimeEnded;
        protected Actor m_Actor;
        protected ActorsManager actorsManager;
        protected PlayerInputHandler m_PlayerInputHandler;

        public UnityAction onAbilityUsed;
        public UnityAction onAbilityOver;

        void Start()
        {
            m_Actor = GetComponent<Actor>();
            actorsManager = FindObjectOfType<ActorsManager>();

            m_PlayerInputHandler = GetComponent<PlayerInputHandler>();

            readyBar = 1f;
        }

        public virtual void CheckToUseAbility() 
        {
            Debug.Log("Ability");
        }
    }
}