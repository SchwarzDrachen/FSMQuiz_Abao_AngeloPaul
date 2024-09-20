using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankController : AdvancedFSM
{
   
    // Movement variables
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rotateSpeed = 10.0f;
    // How "close" to the player is considered as chasing range
    [SerializeField] private float chaseDistance = 5.0f;
    [SerializeField] private float attackDistance = 3.0f;
    // How "close" from the target waypoint until we choose another waypoint
    [SerializeField] private float waypointDistance = 1.0f;
    // The array of transform points in the scene that the agent will move
    // towards to while patrolling
    [SerializeField] private Transform[] waypoints;
    // Reference to the player tank
    [SerializeField] private Transform player;
    [SerializeField] protected float maxHealth = 100.0f;
    [SerializeField] protected float curHealth = 100.0f;
    [SerializeField] public Healthbar healthbar;
    // Reference to attacks
    private float fireRate = 5.0f;
    private float fireCD = 0.0f;
    [SerializeField] public GameObject Explosion;
    [SerializeField]private Transform turret;
    [SerializeField]private Transform bulletSpawnPoint;

    public float MoveSpeed => moveSpeed;
    public float RotateSpeed => rotateSpeed;
    public float ChaseDistance => chaseDistance;
    public float AttackDistance => attackDistance;
    public float WaypointDistance => waypointDistance;
    public float MaxHealth => maxHealth;
    public float CurHealth => curHealth;
    public Healthbar Healthbar => healthbar;

    protected override void Initialize()
    {
        fsmStates = new();
        //Construct the FSM
        PatrolState patrol = new PatrolState(waypoints);
        patrol.AddTransition(TransitionID.SawPlayer, StateID.Chase);
        patrol.AddTransition(TransitionID.NoHealth, StateID.Dead);
        ChaseState chase = new ChaseState();
        chase.AddTransition(TransitionID.LostPlayer, StateID.Patrol);
        chase.AddTransition(TransitionID.ReachPlayer, StateID.Attack);
        chase.AddTransition(TransitionID.NoHealth, StateID.Dead);
        AttackState attack = new AttackState();
        attack.AddTransition(TransitionID.LostPlayer, StateID.Patrol);
        attack.AddTransition(TransitionID.NoHealth, StateID.Dead);
        DeadState dead = new DeadState();

        // First element is the default
        AddState(patrol);
        AddState(chase);
        AddState(attack);
        AddState(dead);
    }

    protected override void FSMUpdate()
    {
       CurrentState.RunState(player, this.transform);
       CurrentState.CheckTransitionRules(player, this.transform);
    }

    public void MoveToTarget(Transform currentTarget){
        // Get the vector pointing towards the direction of the target
        Vector3 targetDirection = currentTarget.position - transform.position;
        // Get the roatation that faces the targetDirection
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        // Rotate the tank to face the targetRotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 
            Time.deltaTime * rotateSpeed);
        // Tank is already rotate, simply move forward
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
        // Rotate turret forward
        turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation,targetRotation,
        Time.deltaTime * rotateSpeed);
    }

    public void AttackTarget(Transform currentTarget){
        fireCD += 0.02f;
        // Get the vector pointing towards the direction of the target
        Vector3 targetDirection = currentTarget.position - transform.position;
        // Get the roatation that faces the targetDirection
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        // Rotate the tank to face the targetRotation
        turret.transform.rotation = Quaternion.Slerp(turret.transform.rotation, targetRotation, 
            Time.deltaTime * rotateSpeed);

            if(fireCD >= fireRate){
                fireCD = 0.0f;
                Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            }
    }
    public void Death(){
        Instantiate(Explosion, gameObject.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Bullet(Clone)")
        {
            curHealth -= 25.0f;
            healthbar.UpdateHealth(curHealth/maxHealth);
        }
    }
}
