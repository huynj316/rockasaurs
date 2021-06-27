using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    private GameObject shakeObject;
    private GameObject shakingObject;
    private Canvas myCanvas;
    private GameObject breakObject;
    private GameObject feelingsObject;

    public GameObject faceParticle;

    public float ShakeDetectionThreshold;
    public float MinShakeInterval;

    private float sqrShakeDetectionThreshold;
    private float timeSinceLastShake;
    private AudioSource audioSource;


    // called zero
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        myCanvas = FindObjectOfType<Canvas>();
        breakObject = myCanvas.transform.GetChild(0).gameObject;
        breakObject.SetActive(true);
        shakeObject = myCanvas.transform.GetChild(1).gameObject;
        shakeObject.SetActive(false);
        shakingObject = myCanvas.transform.GetChild(2).gameObject;
        shakingObject.SetActive(false);
        faceParticle.SetActive(false);
        feelingsObject = myCanvas.transform.GetChild(3).gameObject;
        feelingsObject.SetActive(false);
        //Debug.Log("Awake");
    }

    // called first
    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);
    }

    // called third
    void Start()
    {
        sqrShakeDetectionThreshold = Mathf.Pow(ShakeDetectionThreshold, 2);
        //Debug.Log("Start");
    }

    // called when the game is terminated
    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //public void LoadPortalScene()
    //{

    //    SceneManager.LoadScene("PortalVR", LoadSceneMode.Single);
    //}

    public void YesBreak()
    {
        breakObject.SetActive(false);
        shakeObject.SetActive(true);
        //audioSource.Play();
    }

    public void NoBreak()
    {
        breakObject.SetActive(false);

    }

    public void Happy()
    {
        breakObject.SetActive(false);
        shakeObject.SetActive(false);
        shakingObject.SetActive(false);
        faceParticle.SetActive(false);
        audioSource.Stop();
        feelingsObject.SetActive(true);
    }

    private IEnumerator Feelings(float delay)
    {
        yield return new WaitForSeconds(delay);
        feelingsObject.SetActive(true);
    }

    public void ShakeDetect()
    {
        breakObject.SetActive(false);
        shakeObject.SetActive(false);
        shakingObject.SetActive(true);
        faceParticle.SetActive(true);
        audioSource.Play();
        StartCoroutine("Feelings", 10f);
    }


    void Update()
    {
        if (Input.acceleration.sqrMagnitude >= sqrShakeDetectionThreshold
            && Time.unscaledTime >= timeSinceLastShake + MinShakeInterval)
        {
            //physicsController.ShakeRigidbodies(Input.acceleration);
            ShakeDetect();
            timeSinceLastShake = Time.unscaledTime;
        }
    }
}