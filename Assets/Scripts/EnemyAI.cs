using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent), typeof(Collider))]
public class EnemyAI : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float chaseSpeed = 5f;
    public float patrolSpeed = 2f;
    public float shoutDuration = 2f;
    public Vector2 preshoutDelay = new Vector2(0.1f, 0.5f);
    private int currentPatrolIndex;
    private NavMeshAgent agent;
    private Transform playerTransform;
    private Animator animator;
    private EnemyState _currentEnemyState;
    private Vector3 lastPosition;

    [SerializeField]
    private EnemyState startingState = EnemyState.Waiting;
    private bool isShouting = false;

    
    [Header("Sounds")]
    [SerializeField] private AudioClip idleSound;
    [SerializeField] private AudioClip alertSound;
    [SerializeField] private AudioClip patrolSound;
    [SerializeField] private AudioClip chaseSound;

    public float idleVolume = 0.5f;
    public float alertVolume = .8f;
    public float patrolVolume = .4f;
    public float chaseVolume = .5f;
    
    public Vector2 pitchRange = new Vector2(0.8f, 1.2f);

    private AudioSource audioSource;
    private AudioClip currentPlayingSound;

    private enum EnemyState
    {
        Waiting,
        Patrolling,
        Chasing
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        switch (startingState)
        {
            case EnemyState.Waiting:
                Idle();
                break;
            case EnemyState.Patrolling:
                StartPatrolling();
                break;
            case EnemyState.Chasing:
                OnAlert();
                break;
        }
        
        lastPosition = transform.position;
    }

    void Update()
    {
        if(isShouting)
            return;
     
        switch (_currentEnemyState)
        {
            case EnemyState.Waiting:
                Idle();
                break;
            case EnemyState.Patrolling:
                Patrol();
                goto default;
            case EnemyState.Chasing:
                Chase();
                goto default;
            default:
                UpdateAnimation();
                break;
        }
    }
    
    void UpdateAnimation()
    {
        Vector3 currentPosition = transform.position;
        Vector3 velocity = (currentPosition - lastPosition) / Time.deltaTime;
        lastPosition = currentPosition;

        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        float speedPercent = agent.velocity.magnitude / patrolSpeed;
        speedPercent++;
    
        animator.SetBool("isRunning", true);
        animator.SetFloat("MoveSpeed", speedPercent);
        animator.SetBool("isRunningLeft", localVelocity.x < -0.1f);
        animator.SetBool("isRunningRight", localVelocity.x > 0.1f);
    }


    
    void Idle()
    {
        agent.SetDestination(this.transform.position);
        animator.SetBool("isRunning", false);
        if(audioSource.clip != idleSound || !audioSource.isPlaying)
            PlaySound(idleSound, idleVolume);
    }

    void Patrol()
    {
        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolIndex].position) < .8f)
        {
            // Move to the next patrol point, looping back to the first one if at the end of the array
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    void Chase()
    {
        agent.SetDestination(playerTransform.position);
    }

    [ContextMenu("Alert")]
    public void OnAlert()
    {
        if(isShouting || _currentEnemyState == EnemyState.Chasing)
            return;
        
        StartCoroutine(StartChaseAfterShout());
    }
    
    private IEnumerator StartChaseAfterShout()
    {
        isShouting = true;
        
        float myDelay = Random.Range(preshoutDelay.x, preshoutDelay.y);
        yield return new WaitForSeconds(myDelay);

        agent.isStopped = true;
        // Trigger the shout animation
        animator.SetTrigger("Shout");
        PlaySound(alertSound, alertVolume, false); // Play alert sound

        // Wait for the shout duration
        yield return new WaitForSeconds(shoutDuration);

        // After shouting, start chasing
        isShouting = false;
        _currentEnemyState = EnemyState.Chasing;
        agent.speed = chaseSpeed;
        agent.isStopped = false;
        PlaySound(chaseSound, chaseVolume);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EndGame();
        }
    }

    void StartPatrolling()
    {
        // Debug.Log(gameObject.name + " is patrolling");
        _currentEnemyState = EnemyState.Patrolling;
        agent.speed = patrolSpeed;
        currentPatrolIndex = 0;
        
        PlaySound(patrolSound, patrolVolume);
    }
    
    private void PlaySound(AudioClip clip, float volume, bool loop = true, float delayMin = .05f, float delayMax = .2f)
    {
        
        audioSource.Stop();
        audioSource.volume = volume;
        audioSource.clip = clip;
        audioSource.loop = loop;

        StartCoroutine(ActivateClip(Random.Range(delayMin, delayMax)));
    }

    private IEnumerator ActivateClip(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.Play();
    }

    void EndGame()
    {
        // Implement game end logic here
        GameManager.Instance.Defeat();
        
        animator.speed = 0f;
        agent.isStopped = true;
        animator.SetBool("isRunning", false);
        animator.SetBool("isRunningLeft", false);
        animator.SetBool("isRunningRight", false);
    }
}