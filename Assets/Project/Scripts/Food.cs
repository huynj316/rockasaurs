using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Food : MonoBehaviour {
    
    public static event Action<Food> OnBagToss = delegate { };
    public static event Action<Food> OnBagStop = delegate { };
    public static event Action<Food> OnBagPushed = delegate { };
    public static event Action<Food> OnBagHit = delegate { };
    //public static event Action<Food> OnDestroyBag = delegate { };

    public PointType pointType = PointType.None;
    public int throwIndex = 0;

    public FoodConfig config;

    public Player player = Player.One;
    public Material player1Mat;
    public Material player2Mat;

    public bool TriggeredNextTurn = false; //Tracks whether we moved to the next turn as a result of tossing this bag yet, prevents turn skipping by double registering things

    public bool isActive { get; private set; }

    private bool isMoving;
    internal bool triggerEnter;
    internal bool triggerExit;
    public bool pushed = false;
    public bool isPushing = false;

    public bool isStopped { get; private set; }

    private void Awake()
    {
        isActive = true;
        isMoving = false;
        pushed = false;
    }

    private void Start()
    {
        //player = ScoreManager.instance.currentPlayer;
        //throwIndex = ScoreManager.instance.ThrowIndexForPlayer(player);
                                 
        //GetComponentInChildren<Renderer>().material = (player == Player.One) ? player1Mat : player2Mat;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player1Bag" && 
           isStopped
           //&& throwIndex != ScoreManager.instance.currentThrow
           ) {
//            Debug.Log("Pushed bag!!");
            pushed = true;
            OnBagPushed(this);
        }
    }

    private void FixedUpdate()
    {

        if (!isMoving)
            return;

        //bag stopped and entered trigger
        if (GetComponent<Rigidbody>().velocity.magnitude < 0.01f && !isActive && triggerEnter)
        {
            isMoving = false;
            isStopped = true;
            OnBagStop(this);
        }
    }

    public void Toss(Vector3 direction, float timeInterval)
    {         
        //Debug.Log("***TOSSING");
        GetComponent<Rigidbody>().isKinematic = false;

        direction = FixDirection(direction);

        Vector3 throwForce = -direction / timeInterval * config.throwForce;


        //if (Application.isEditor)
        //{
        //    throwForce = ScoreManager.instance.DEBUG_TOSS_VECTOR;
        //}

        throwForce = ClampThrowForce(throwForce);

        GetComponent<Rigidbody>().AddForce(throwForce);
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * config.loftForce);
        isActive = false;
        isMoving = true;
        isStopped = false;
        OnBagToss(this);

        //unparent bag
        transform.parent = null;
    }


    private Vector3 FixDirection(Vector3 direction)
    {   
        direction = Quaternion.Euler(-30, 0, 0) * direction; //All forces have been tweaked assuming the camera is angled 30 degrees on the X, so let's continue that thinking for now
        direction = Camera.main.transform.TransformDirection(direction); //Then rotate based on camera
     
        return direction;
    }

    private static Vector3 ClampThrowForce(Vector3 throwForce)
    {
        float maxForce = 600;
        float minForce = 50;

        if (throwForce.magnitude > maxForce)
        {
            Debug.Log("> clamping");
            throwForce = throwForce.normalized * maxForce;
        }
        else if (throwForce.magnitude < minForce)
        {
            Debug.Log("< normalizing");

            throwForce = throwForce.normalized * minForce;
        }

        return throwForce;
    }
}
