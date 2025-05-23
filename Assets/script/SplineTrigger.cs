using UnityEngine;
using UnityEngine.Splines;

public class CharacterSplineController : MonoBehaviour
{
    [Header("Component References")]
    [Tooltip("Assign the GameObject that has your SplineAnimate component.")]

    public SplineAnimate splineAnimateComponent;

    [Tooltip("Assign your character's Animator component.")]
    public Animator characterAnimator;

    [Header("Animation Settings")]
    [Tooltip("Name of the boolean parameter in your Animator for the walking animation.")]
    public string walkingParameterName = "IsWalking"; 

    [Header("Spline End Behavior")]
    [Tooltip("Should the SplineAnimate component be disabled when the end is reached?")]
    public bool disableSplineAnimateOnEnd = true;
    [Tooltip("Should this controller script disable itself after the spline is completed?")]
    public bool disableThisScriptOnEnd = true;

    private bool hasReachedEnd = false;

    void Start()
    {
        
        if (splineAnimateComponent == null)
        {
            Debug.LogError("SplineAnimate component is NOT ASSIGNED in the Inspector on " + gameObject.name + ". This script will be disabled.", this);
            enabled = false; 
            return;
        }

        if (characterAnimator == null)
        {
            
            characterAnimator = splineAnimateComponent.GetComponent<Animator>();
            if (characterAnimator == null)
            {
                
                characterAnimator = GetComponent<Animator>();
            }

            if (characterAnimator == null)
            {
                Debug.LogWarning("Animator component not found on " + gameObject.name + " or on SplineAnimate's GameObject. Walking animation cannot be controlled.", this);
            }
        }

        splineAnimateComponent.Loop = SplineAnimate.LoopMode.Once;

        splineAnimateComponent.ElapsedTime = 0f;

        splineAnimateComponent.enabled = true;

        if (characterAnimator != null)
        {
            characterAnimator.SetBool(walkingParameterName, true);
        }

        hasReachedEnd = false;
    }

    void Update()
    {
        
        if (hasReachedEnd || splineAnimateComponent == null || !splineAnimateComponent.isActiveAndEnabled)
        {
            return;
        }

       
        if (splineAnimateComponent.NormalizedTime >= 1.0f)
        {
            HandleSplineEnd();
        }
    }

    void HandleSplineEnd()
    {
        
        if (hasReachedEnd) return;
        hasReachedEnd = true;

        Debug.Log("Character has reached the end of the spline.", this);

        if (characterAnimator != null)
        {
            characterAnimator.SetBool(walkingParameterName, false);
        }

        if (disableSplineAnimateOnEnd)
        {
            splineAnimateComponent.enabled = false;
        }

        if (disableThisScriptOnEnd)
        {
            enabled = false; 
        }
    }
}