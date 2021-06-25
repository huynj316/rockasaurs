using UnityEngine;

public class Singleton<Instance> : MonoBehaviour where Instance : Singleton<Instance>
{
    public static Instance instance;
    public bool dontDestroyOnLoad;

    public virtual void Awake()
    {
        if (dontDestroyOnLoad)
        {
            if (!instance)
            {
                instance = this as Instance;
            }
            else
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            instance = this as Instance;
        }
    }

}

