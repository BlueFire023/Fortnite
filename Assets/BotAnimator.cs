using UnityEngine;

public class BotAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private string _currentAnimation = "Idle";

    public enum AnimationState
    {
        Idle,
        Walking,
        WalkFire,
        Firing,
        Death
    }

    private void ResetAnimations()
    {
        animator.SetBool("Death", false);
        animator.SetBool("Firing", false);
        animator.SetBool("WalkFire", false);
        animator.SetBool("Walking", false);
        animator.SetBool("Idle", false);
    }

    public void SetAnimation(AnimationState identifier)
    {
        var identifierString = identifier.ToString();
        if (_currentAnimation.Equals(identifierString))
            return;
        ResetAnimations();
        _currentAnimation = identifierString;
        animator.SetBool(identifierString, true);
    }
}
