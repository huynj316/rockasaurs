using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ThrowScore : MonoBehaviour {

    private Sprite emptySprite;

    public Sprite fullSprite;
    public TMP_Text scoreText;

    public int score {
        set {
            scoreText.text = value.ToString();
            GetComponent<Image>().sprite = fullSprite;
        }
    }

    public void Clear()
    {
        scoreText.text = "";
        GetComponent<Image>().sprite = emptySprite;
    }

    private void Awake()
    {
        scoreText = (scoreText) ?? GetComponentInChildren<TMP_Text>();
        emptySprite = GetComponent<Image>().sprite;
    }

    private void Start()
    {
        Clear();
    }

}
