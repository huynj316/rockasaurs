using UnityEngine;
namespace AA.Foodfight
{
    public delegate void OnStateChangeHandler();
    public class GameManager<Instance> : MonoBehaviour where Instance: GameManager<Instance>
    {
        public static Instance instance;
        public bool dontDestroyOnLoad;
        public event OnStateChangeHandler OnStateChange;
        public GameState gameState { get; private set; }

        public virtual void Awake()
        {
            if(dontDestroyOnLoad)
            {
                if(!instance)
                {
                    instance = this as Instance;
                } else {
                    Object.Destroy(gameObject);
                }
                DontDestroyOnLoad(gameObject);
            } else {
                instance = this as Instance;
            }
        }
        public void SetGameState(GameState state)
        {
            this.gameState = state;
            OnStateChange();
        }

        public void OnApplicationQuit()
        {
            instance = null;
        }

    }
  

}
