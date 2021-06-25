using System;
using System.Collections;
using UnityEngine;

public class PointTrigger : MonoBehaviour
{

    public static event Action<PointType, Food> OnPointTrigger = delegate { };
    public PointType pointType = PointType.None;

    public GameConfig gameConfig;

    public GameObject GameScreenCanvas;

    public ParticleSystem starParticles;

    [SerializeField] private Food food;
    [SerializeField] private Player player;

    private void Start()
    {
        //Food.OnBagStop += Food_OnBagStop;
        //Food.OnBagPushed += Food_OnBagPushed;
    }

    private void OnDestroy()
    {
        //Food.OnBagStop -= Food_OnBagStop;
        //Food.OnBagPushed -= Food_OnBagPushed;

    }


    private void OnTriggerEnter(Collider other)
    {
        food = other.gameObject.GetComponentInParent<Food>();
        food.triggerEnter = true;
        food.pointType = pointType;

        ScoreManager.instance.EvaluatePoints(food);

        // hole scored
        if (pointType == PointType.Hole)
        {  
            OnPointTrigger(pointType, food);
            food = null;
            Destroy(other.gameObject, 0.01f);

            StartCoroutine("ShowHoleIndicator", 0.5f);
            starParticles.Play();

            return;
        }
        
        
        if (
//            food.pushed == false && 
            ScoreManager.instance.OnRoundOver(food.player)
                && !food.TriggeredNextTurn)

        {
            food.TriggeredNextTurn = true;

            Debug.Log("**!!!abotu to start next round coroutine frompointtrigger");
            StartCoroutine(ScoreManager.instance.NextRound());
            return;
        }
        
        if (
//            food.pushed == false) 
            !food.TriggeredNextTurn)
        {
            food.TriggeredNextTurn = true;

            ScoreManager.instance.currentPlayer = (ScoreManager.instance.currentPlayer == Player.One) ? Player.Two : Player.One;
            ScoreManager.instance.NextThrow();
        }
        
        if (starParticles != null)
        {
            starParticles.Stop();
        }
   
    }

    private void OnTriggerExit(Collider other)
    {
        if(food != null){
            //food.triggerEnter = false; //Removed due to a bug with a single bag slowly falling off that edge that causes the game to not proceed
            food = null;
        }
     
    }

    void Food_OnBagPushed(Food obj)
    {

            if (obj.Equals(food)
               )
            {
            OnPointTrigger(pointType, food);
                //foul scored
                if (pointType == PointType.Foul)
                {
                obj.pointType = this.pointType;
                starParticles = null;
                StartCoroutine("ShowFoulIndicator", 0.5f);
                }

                //hole scored
                if (pointType == PointType.Hole)
                {
                obj.pointType = this.pointType;
                starParticles.Play();
                StartCoroutine("ShowHoleIndicator", 0.5f);
            }
                else if (starParticles != null)
                {

                    starParticles.Stop();
                }
            }
        }


    void Food_OnBagStop(Food obj)
    {
    
        //score classified as foul or woody after food stops
        if(obj.Equals(food)) {
            OnPointTrigger(pointType, food);

            //foul scored
            if (pointType == PointType.Foul)
            {
                obj.pointType = this.pointType;
                starParticles = null;
                StartCoroutine("ShowFoulIndicator", 0.5f);
            }

            //woody scored
            if (pointType == PointType.Woody)
            {
                obj.pointType = this.pointType;
                starParticles = null;
                StartCoroutine("ShowWoodyIndicator", 0.5f);

            }
        }
    }


    IEnumerator ShowFoulIndicator(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameScreenCanvas.transform.GetChild(0).gameObject.SetActive(true);


        yield return new WaitForSeconds(1f);
        GameScreenCanvas.transform.GetChild(0).gameObject.SetActive(false);



    }
    IEnumerator ShowWoodyIndicator(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameScreenCanvas.transform.GetChild(1).gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        GameScreenCanvas.transform.GetChild(1).gameObject.SetActive(false);


    }
    IEnumerator ShowHoleIndicator(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameScreenCanvas.transform.GetChild(3).gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        GameScreenCanvas.transform.GetChild(3).gameObject.SetActive(false);

    }

}
