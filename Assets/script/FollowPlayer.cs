using UnityEngine;
using UnityEngine.AI; // Required for NavMeshAgent

// Ensure this script is attached to your Emma GameObject
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EmmaFollowTarget : MonoBehaviour
{
    // Public variable to assign the target (e.g., PlayerCapsule) in the Inspector
    // This can be set by a spawner script, or it will try to find "Player" tag in Start.
    public Transform targetToFollow;

    // Threshold for how much the target needs to move before we recalculate the path
    public float targetMoveThreshold = 0.1f; // Adjust as needed

    // Private references to components on this GameObject (Emma)
    private NavMeshAgent agent;
    private Animator animator;

    // To store the target's position from the last frame we set a destination
    private Vector3 lastTargetPosition = Vector3.positiveInfinity; // Initialize to a value that ensures first check passes

    // Name of the boolean parameter in your Animator Controller
    private readonly int isWalkingHash = Animator.StringToHash("IsWalking");

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component not found on " + gameObject.name + ". Disabling script.");
            enabled = false;
            return;
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name + ". Disabling script.");
            enabled = false;
            return;
        }

        // If targetToFollow was not assigned by a spawner or in the Inspector for a scene instance
        if (targetToFollow == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player"); // Make sure your player has the "Player" tag
            if (playerObject != null)
            {
                targetToFollow = playerObject.transform;
                Debug.Log(gameObject.name + " automatically found and is targeting: " + playerObject.name);
            }
            else
            {
                Debug.LogWarning("TargetToFollow is not assigned and GameObject with tag 'Player' not found for " + gameObject.name + ". Emma will not move.");
            }
        }

        // Initialize lastTargetPosition after targetToFollow is confirmed
        if (targetToFollow != null)
        {
            // Set lastTargetPosition to something that will guarantee the first SetDestination call
            // if the target hasn't moved relative to this initial offset.
            lastTargetPosition = targetToFollow.position + (transform.forward * -1 * (targetMoveThreshold + 0.01f));
        }
        else
        {
            // If still no target, disable the script to prevent errors in Update
            enabled = false;
        }
    }

    void Update()
    {
        // The 'enabled = false' in Start should prevent Update from running if no target.
        // But as an extra safe guard:
        if (targetToFollow == null || !agent.enabled || !agent.isOnNavMesh)
        {
            // Ensure walking animation is off if there's no target or agent is invalid
            if (animator != null && animator.GetBool(isWalkingHash))
            {
                animator.SetBool(isWalkingHash, false);
            }
            // Attempt to re-validate agent's NavMesh status if it was lost
            if (targetToFollow != null && agent.enabled && !agent.isOnNavMesh)
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
                {
                    agent.Warp(hit.position);
                }
            }
            return;
        }


        // --- Optimized Set Destination ---
        // Only set a new destination if the target has moved significantly
        // or if the agent doesn't have a path (e.g., first update, or path was completed/invalidated)
        if (!agent.hasPath || Vector3.Distance(targetToFollow.position, lastTargetPosition) > targetMoveThreshold)
        {
            if (agent.isOnNavMesh) // Double check before setting destination
            {
                agent.SetDestination(targetToFollow.position);
                lastTargetPosition = targetToFollow.position; // Update the last known target position
            }
        }

        // --- Animation Control ---
        bool shouldBeWalking = false;

        if (agent.pathPending)
        {
            shouldBeWalking = false;
        }
        else
        {
            if (agent.hasPath)
            {
                if (agent.remainingDistance > agent.stoppingDistance)
                {
                    shouldBeWalking = true;
                }
                else
                {
                    shouldBeWalking = false;
                }
            }
            else
            {
                shouldBeWalking = false;
            }
        }

        if (animator.GetBool(isWalkingHash) != shouldBeWalking)
        {
            animator.SetBool(isWalkingHash, shouldBeWalking);
        }
    }
}

// ---------------------------------------------------------------------------
// EXAMPLE: How a separate Spawner script would set Emma's target
// ---------------------------------------------------------------------------
/*
public class EmmaSpawner : MonoBehaviour
{
    public GameObject emmaPrefab;       // Assign your Emma prefab in the Inspector
    public Transform spawnPoint;        // Assign a spawn point transform

    void Start()
    {
        SpawnNewEmma();
    }

    public void SpawnNewEmma()
    {
        if (emmaPrefab == null)
        {
            Debug.LogError("Emma Prefab not assigned to spawner!");
            return;
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player"); // Ensure player has "Player" tag
        if (playerObject == null)
        {
            Debug.LogError("Player object with tag 'Player' not found in scene!");
            return;
        }

        Vector3 positionToSpawn = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion rotationToSpawn = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        GameObject spawnedEmmaGO = Instantiate(emmaPrefab, positionToSpawn, rotationToSpawn);
        EmmaFollowTarget emmaScript = spawnedEmmaGO.GetComponent<EmmaFollowTarget>();

        if (emmaScript != null)
        {
            emmaScript.targetToFollow = playerObject.transform; // Crucial step!
            Debug.Log("Spawned Emma and assigned target: " + playerObject.name);
        }
        else
        {
            Debug.LogError("Spawned Emma prefab does not have the EmmaFollowTarget script attached!");
        }
    }
}
*/
