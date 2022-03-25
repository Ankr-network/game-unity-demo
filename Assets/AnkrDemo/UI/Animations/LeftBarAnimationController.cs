using UnityEngine;

namespace Demo.UI.Animations
{
    public class LeftBarAnimationController : MonoBehaviour
    {

        private Animator animator;

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();    
        }

        public void LeftBarSlideIn() {
            animator.Play("LeftBar_SlideIn");
        }

        public void LeftBarSlideOut() {
            animator.Play("LeftBar_SlideOut");
        }
    }
}
