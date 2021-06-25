using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodManager : Singleton<FoodManager>
{
    public GameObject prefab;
    private List<Food> foods = new List<Food>();
    public Transform spawnTransform;
    Transform LookAtCameraVector;
    private readonly float spawnDelay = 1f;

    public float nextSpawn { get; private set; }

    private void Start()
    {
        //ScoreManager.OnNextThrow += ScoreManager_OnNextThrow;
        //ScoreManager.OnNextRound += ScoreManager_OnNextRound;
        SwipeGesture.OnEndSwipe += SwipeGesture_OnEndSwipe;

        StartCoroutine(SpawnFood(1f));
        nextSpawn = Time.time;
    }

    private void OnDestroy()
    {
        //ScoreManager.OnNextThrow -= ScoreManager_OnNextThrow;
        //ScoreManager.OnNextRound -= ScoreManager_OnNextRound;
        SwipeGesture.OnEndSwipe -= SwipeGesture_OnEndSwipe;
    }

    void SwipeGesture_OnEndSwipe()
    {
        Quaternion SpawnFoodRotation = transform.rotation;
        Vector3 SpawnFoodEulerAngles = SpawnFoodRotation.eulerAngles;

        LookAtCameraVector = Camera.main.transform;
        //Debug.Log("camPos"+ LookAtCameraVector);

        Vector3 relativePos = LookAtCameraVector.position - transform.position;
        Quaternion LookAtRotation = Quaternion.LookRotation(relativePos);
        //Debug.Log("foodManager lookRot: "+ LookAtRotation);

        Vector3 LookAtEulerAngles = LookAtRotation.eulerAngles;
        Vector3 ContentEulerAngles = new Vector3(SpawnFoodEulerAngles.x, LookAtEulerAngles.y, SpawnFoodEulerAngles.z);

        transform.rotation = Quaternion.Euler(ContentEulerAngles);
    }


    public void Reset()
    {
        foreach (var food in foods)
        {
            if (food)
            {
                Destroy(food.gameObject);
            }
        }

        foods.Clear();
    }


    public IEnumerator SpawnFood(float delay)
    {
        //if (GameController.instance == null)
        //    Debug.Log("SpawnFood gamecontroller is null");
        yield return new WaitForSeconds(delay);

        if (gameObject.transform.childCount <= 0)
        {
            Food food = Instantiate(prefab, spawnTransform).GetComponent<Food>();
            //spawnTransform = food.transform;
            foods.Add(food);
            Debug.Log(food + " added!");

            nextSpawn = Time.time + spawnDelay;
        }
     
    }

    void ScoreManager_OnNextThrow()
    {
        //Debug.Log("*MDOnNextThrow foodmanager.onnextthrow");
        StartCoroutine(SpawnFood(1f));
    }

    void ScoreManager_OnNextRound()
    {
        //Debug.Log("*MDOnNextThrow foodmanager.nextround");

        Reset();
        StartCoroutine(SpawnFood(1f));
    }

}