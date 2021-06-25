using UnityEngine;

[CreateAssetMenu(fileName = "Assets", menuName = "Foodfight/FoodConfig", order = 1)]
public class FoodConfig : ScriptableObject {

    [Range(0.1f, 1.0f)]
    public float throwForce = 0.1f;

    [Range(0f, 100f)]
    public float loftForce = 0f;

}
