using UnityEngine;
using UnityEngine.UI;
using XRAccelerator.Enums;

namespace XRAccelerator.Gameplay
{
    public abstract class ModeController : MonoBehaviour
    {
        private static readonly int ActiveHash = Animator.StringToHash("Active");

        [SerializeField]
        [Tooltip("Reference to this mode tab button")]
        public Button TabButton;

        protected ModesController ModesController;
        private Animator tabAnimator;

        private bool HasTabEntry => TabButton != null;

        public abstract Mode Mode { get; }

        public virtual void EnableMode()
        {
            gameObject.SetActive(true);

            if (HasTabEntry)
            {
                TabButton.interactable = false;
                tabAnimator.SetBool(ActiveHash, true);
            }
        }

        public virtual void DisableMode()
        {
            gameObject.SetActive(false);

            if (HasTabEntry)
            {
                TabButton.interactable = true;
                tabAnimator.SetBool(ActiveHash, false);
            }
        }

        public virtual void Setup(ModesController controller)
        {
            ModesController = controller;

            if (HasTabEntry)
            {
                TabButton.onClick.AddListener(() => ModesController.ChangeMode(Mode));
                tabAnimator = TabButton.GetComponent<Animator>();
            }
        }
    }
}