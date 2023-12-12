using System.Collections;
using UnityEngine;

public class Move : MonoBehaviour
{
    [Header("Move")]
    [Space]
    [Header("Necessary")]
    [Space]
    [SerializeField] Look look;
    [SerializeField] Life life;
    [SerializeField] Rigidbody rb;
    [SerializeField] CapsuleCollider playerCollider;
    [SerializeField] Transform groundTransform;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask everythingLayer;
    [SerializeField] Transform orientation;
    [SerializeField] Transform headCheck;
    [SerializeField] Transform cam;
    [SerializeField] float gravity; //9f
    [SerializeField] float acceleration;
    [SerializeField] float slopeAcceleration;
    [HideInInspector] float currentSpeed;
    [HideInInspector] float horizontal;
    [HideInInspector] float vertical;
    [HideInInspector] float currentVelocity;
    [HideInInspector] float desiredMoveSpeed;
    [HideInInspector] float lastDesiredMoveSpeed;
    [HideInInspector] float speed;

    [Header("Customizable")]
    [Space] 
    [Header("Walking")]
    [Space]
    [SerializeField] float walkSpeed;
    [HideInInspector] public bool grounded;
    [HideInInspector] public Vector3 direction;
    [HideInInspector] public Vector3 velocity;

    [Space]
    [Header("Jumping")]
    [Space]
    [SerializeField] float jumpForce;
    [HideInInspector] bool jumping;
    
    [Space]
    [Header("Crouching")]
    [Space]
    [SerializeField] float crouchHeight;
    [SerializeField] float standHeight;
    [SerializeField] float headCheckLength;
    [HideInInspector] float initialHeadCheckLength;
    [HideInInspector] bool canStand;
    [HideInInspector] public bool crouching;
    
    [Space]
    [Header("Balancing Force")]
    [Space]
    [SerializeField] float balanceVelocity = 1f; // cant equal 0
    [HideInInspector] float balancingForce;
    [HideInInspector] Vector3 balanceForce;
    
    [Space]
    [Header("Stopping")]
    [Space]
    [SerializeField] float groundDrag;
    [SerializeField] float stopSmothness;
    [SerializeField] float airDrag;
    [HideInInspector] Vector3 velocityLimit;
    
    [Space]
    [Header("Slope Movement")]
    [Space]
    [SerializeField] float maxSlopeAngle;
    [HideInInspector] bool slope;
    [HideInInspector] float slopeAngle;
    [HideInInspector] RaycastHit slopeObject;
    [HideInInspector] RaycastHit slopeHit;
    
    [Space]
    [Header("Sliding")]
    [Space]
    [SerializeField] float maxSlideTime;
    [SerializeField] float slideForce;
    [SerializeField] float slideSpeed;
    [SerializeField] public bool sliding;
    [SerializeField] float slideTimer;
    [HideInInspector] Vector3 slideDirection;

    [Space]
    [Header("Vaulting")]
    [Space]
    [Header("Vaulting")]
    [SerializeField] Transform vaultOffset;
    [SerializeField] float vaultHeight = 1f;
    [SerializeField] float vaultDuration;
    [SerializeField, Range(0,1)] float ledgeDetectionThreshold;
    [HideInInspector] public bool vaulting = false;

    [Space]
    [Header("VFX")]
    [Space]
    [SerializeField] ParticleSystem landingEffect;
    [SerializeField] Animator playerAnimator;

    void Awake(){
        initialHeadCheckLength = headCheckLength;
    }
    void Update()
    {      
        print("Vaulting?: " + vaulting);
        variables();

        if(grounded && jumping && !vaulting){
            jump();
        }

        // handleSliding();

        handleCrouching();

        handleVaulting();

        handleMomentum();

        playerCollider.center = crouching || sliding ? new Vector3(0f, .1f, 0f) : new Vector3(0f, -0.25f, 0f);
        playerCollider.height = crouching || sliding ? crouchHeight : standHeight;

    //     if(grounded && horizontal != 0 || vertical != 0){
    //         playerAnimator.SetBool("Walking", true);
    //     } else{
    //         playerAnimator.SetBool("Walking", false);
    //     } 

    //     playerAnimator.SetBool("Grounded", grounded);            
    //     playerAnimator.SetBool("Crouching", crouching);            
    //     playerAnimator.SetBool("Sliding", sliding);         
    } 

    void FixedUpdate()
    {
        if(sliding){
            slidingMovement();
        }

        if(grounded){
            if(onSlope(groundTransform.transform.position)){
                slopeMovement();
            }else{
                movement();
            }
        }else{
            inAirMovement();
        }

        handleVelocity();
    }

    // void OnCollisionEnter(Collision collider){
    //     Instantiate(landingEffect, groundTransform.position, groundTransform.rotation);
    // }

    //
    //  Executive functions
    //

    void variables(){
        if(!sliding){
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }

        canStand = !Physics.Raycast(headCheck.position, orientation.up, headCheckLength, everythingLayer);
        headCheckLength = canStand ? initialHeadCheckLength : initialHeadCheckLength + 1;

        crouching = Input.GetKey(KeyCode.LeftControl);
        jumping = Input.GetKey(KeyCode.Space);
        grounded = Physics.CheckSphere(groundTransform.position, .45f, groundLayer);

        velocity = rb.velocity;
        currentVelocity = rb.velocity.magnitude;

        direction = horizontal * orientation.right + vertical * orientation.forward;
        direction.Normalize();

        rb.drag = grounded ? groundDrag : airDrag;
        rb.useGravity = !onSlope(groundTransform.transform.position);
        
        speed = walkSpeed;
    }

    void handleMomentum(){
        if(onSlope(groundTransform.transform.position) && rb.velocity.y < .1f && sliding){
            desiredMoveSpeed = slideSpeed;
        }else{
            desiredMoveSpeed = speed;
        }

        if(Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 1f && currentSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(momentum());
        }
        else{
            currentSpeed = desiredMoveSpeed;
        }
        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    void slopeMovement(){
        if(rb.velocity.y > 0f){
            rb.AddForce(-orientation.up * gravity, ForceMode.Force);
        }
        rb.AddForce(getSlopeDirection(direction) * currentSpeed * 10f + getSlopeBalancingForce(), ForceMode.Force); 
    }

    void movement(){
        rb.AddForce(direction * currentSpeed * 10f + balanceForce * Time.deltaTime, ForceMode.Force);
        
    }

    void inAirMovement(){
        rb.AddForce(-orientation.up * gravity * 2f, ForceMode.Acceleration);
        rb.AddForce(direction * currentSpeed * 5f + balanceForce, ForceMode.Force);
    }
    
    void jump(){
        rb.AddForce(orientation.up * jumpForce, ForceMode.Impulse);
    }

    void handleVelocity(){
        if(direction == Vector3.zero && grounded){
            rb.velocity = Vector3.Slerp(rb.velocity, -new Vector3(0, 0, 0), 10 * Time.fixedDeltaTime);
        }
        if (onSlope(groundTransform.transform.position))
        {
            if (currentVelocity > currentSpeed)
                rb.velocity = rb.velocity.normalized * currentSpeed;
        }else{
            Vector3 planeVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (planeVelocity.magnitude > currentSpeed)
            {
                Vector3 limitedVel = planeVelocity.normalized * currentSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    void handleCrouching(){
        if(Input.GetKeyDown(KeyCode.LeftControl) && grounded && rb.velocity.magnitude > 2f){
            slideStart();
        }
        if(grounded && currentVelocity < 2f || Input.GetKeyUp(KeyCode.LeftControl) || life.currentStamina < 15f || slideTimer < .1f){
            slideStop();
        }
        if(crouching || !canStand){
            crouchStart();
        }
    }

    void crouchStart(){
        // playerCollider.height = crouchHeight;
        rb.AddForce(-orientation.up * 2f, ForceMode.Force);
    }

    void crouchEnd(){
        // playerCollider.height = standHeight;
        rb.AddForce(orientation.up * 2f, ForceMode.Force);
    }

    void slideStart()
    {
        sliding = true;
        // playerCollider.height = crouchHeight;
        rb.AddForce(-orientation.up * 2f, ForceMode.Force);
        slideTimer = maxSlideTime;
    }

    void slidingMovement()
    {
        
        Vector3 inputDirection = orientation.forward * vertical + orientation.right * horizontal;
        if(!onSlope(groundTransform.transform.position))
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);
            slideTimer -= Time.deltaTime;
        }else{
            rb.AddForce(getSlopeDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        if (slideTimer <= 0){
            slideStop();
        }
    }

    void slideStop()
    {
        sliding = false;
        playerCollider.height = standHeight;
    }

    IEnumerator momentum(){
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - currentSpeed);
        float startValue = currentSpeed;

        while (time < difference)
        {
            currentSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, 2 * time / difference);

            if (onSlope(groundTransform.transform.position))
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * slopeAcceleration * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * acceleration;

            yield return null;
        }

        currentSpeed = desiredMoveSpeed;
    }

    void handleVaulting()
    {
        Vector3 velocity = rb.velocity;
        if (!grounded && !sliding && !crouching)
        {
            Vector3 maxVaultPos = this.transform.position + Vector3.up * 1.5f;
            UnityEngine.Debug.DrawLine(maxVaultPos, maxVaultPos + direction, Color.red, 2f);
            if(Physics.Raycast(maxVaultPos, maxVaultPos + direction * 1.25f, 3f, groundLayer)){
                return;
            }

            Vector3 hoverPos = maxVaultPos + direction * 2;

            RaycastHit hit;
            if(!Physics.Raycast(hoverPos, Vector3.down, out hit, 3f, groundLayer)){
                return;
            }

            Vector3 landingPos = hit.point + (Vector3.up * playerCollider.height * 0.5f);
            UnityEngine.Debug.DrawLine(hit.point, hit.point + (Vector3.up * playerCollider.height * 0.5f), Color.blue, 2f);
        }
    }
    
    //
    // Variables
    //

    bool onSlope(Vector3 from){
        if(Physics.Raycast(from, Vector3.down, out slopeHit, 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    Vector3 getSlopeDirection(Vector3 customDirection){
        return Vector3.ProjectOnPlane(customDirection, slopeHit.normal).normalized;
    }
    
    Vector3 getSlopeBalancingForce(){
        return new Vector3(-getSlopeDirection(direction).x * balancingForce, rb.velocity.y, -getSlopeDirection(direction).z * balancingForce);
    }
    Vector3 getBalancingForce(){
        balancingForce = Mathf.Abs(((currentVelocity * balanceVelocity / (currentVelocity + 1))) / 2-currentVelocity);
        return new Vector3(-rb.velocity.x * balancingForce, rb.velocity.y, -rb.velocity.z * balancingForce);
    }

    bool nearTopEdge(Collider obstacleCollider, Vector3 vaultPosition)
    {
        // Vector3 obstacleTop = obstacleCollider.bounds.center + Vector3.up * obstacleCollider.bounds.extents.y;
        // return Vector3.Distance(vaultPosition, obstacleTop) < ledgeDetectionThreshold;
        Vector3 obstacleTop = obstacleCollider.bounds.center + Vector3.up * obstacleCollider.bounds.extents.y;

        // Calculate a point on the top edge of the obstacle collider
        Vector3 edgePoint = obstacleTop + Vector3.up * (obstacleCollider.bounds.size.y * 0.5f);

        // Check if your vault position is close enough to the edge point
        bool isNearEdge = Vector3.Distance(vaultPosition, edgePoint) < ledgeDetectionThreshold;

        return isNearEdge;
    }

    bool checkObstacles(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        if (Physics.Raycast(start, direction, direction.magnitude, groundLayer))
        {
            return true; 
        }
        return false;
    }
}
