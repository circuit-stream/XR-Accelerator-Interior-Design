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

        public abstract Mode Mode { get; }

        public virtual void EnableMode()
        {
            gameObject.SetActive(true);
            TabButton.interactable = false;
            tabAnimator.SetBool(ActiveHash, true);
        }

        public virtual void DisableMode()
        {
            gameObject.SetActive(false);
            TabButton.interactable = true;
            tabAnimator.SetBool(ActiveHash, false);
        }

        public virtual void Setup(ModesController controller)
        {
            ModesController = controller;

            TabButton.onClick.AddListener(() => ModesController.ChangeMode(this));
            tabAnimator = TabButton.GetComponent<Animator>();
        }
    }
}