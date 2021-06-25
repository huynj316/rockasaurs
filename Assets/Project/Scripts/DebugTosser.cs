using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTosser : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		if (Application.isEditor)
		{
			StartCoroutine(Tosser());
		}
	}

	public IEnumerator Tosser()
	{
		while (true)
		{
			int count = 0;

			while (count < 8)
			{
				while (GetComponentInChildren<Food>() == null)
				{
					yield return null;
				}

				count++;
				Food food = GetComponentInChildren<Food>();
//				yield return new WaitForSeconds(.7f); //wait 2 seconds between
//				yield return new WaitForSeconds(2f); //wait 2 seconds between
				yield return new WaitForSeconds(.2f);

				if (food != null)
				{
					food.Toss(new Vector3(), 1);
				}
				else
				{
					Debug.Log("****~~~~~ null food");
				}
				
			}

			yield return new WaitForSeconds(2f);
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
