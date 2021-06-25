using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Food))]
public class SwipeGesture : MonoBehaviour 
{

    Vector3 startPos, endPos, direction;
    float touchTimeStart, touchTimeFinish, timeInterval;
    float distance;

    private Food food;

    public bool swipe;

    private GraphicRaycaster uiRaycaster;
    private bool skippedToss = false; //Did we skip starting a toss because we were touching specific UI elements
    private bool isSwiping;

    public static event Action OnEndSwipe = delegate { };

    private void Awake()
    {
        food = GetComponent<Food>();
        swipe = true;

        //uiRaycaster = GameObject.Find("GameScreenCanvas").GetComponent<GraphicRaycaster>();
    }

    private void Start()
    {
        //RulesPanel.OnRulesOpened += RulesPanel_OnRulesOpened;
        //RulesPanel.OnRulesClosed += RulesPanel_OnRulesClosed;
    }

    private void OnDestroy()
    {
        //RulesPanel.OnRulesOpened -= RulesPanel_OnRulesOpened;
        //RulesPanel.OnRulesClosed -= RulesPanel_OnRulesClosed;
    }

    void RulesPanel_OnRulesOpened()
    {
        swipe = false;

    }

    void RulesPanel_OnRulesClosed()
    {
        swipe = true;
    }


    private bool HitTest()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit)) { 
            if (hit.collider != null) {
                if (hit.collider.name == "food")
                    Debug.Log(hit.collider.name + " hit!");
                    return true;
            }
        }
        return false;
    }
    private void Update()
    {
        if (!food.isActive)
        return;

        if (swipe) // still catching if bool is false
        {
            
            if (Input.GetMouseButtonDown(0) && HitTest())
            {
                isSwiping = true;

                //if (!IsOverInfo())
                //{
                Debug.Log("swipe starting");
                    StartSwipe(Input.mousePosition);
                //}
                //else
                //{
                //    skippedToss = true;
                //}
            }
            else if (Input.GetMouseButtonUp(0) && isSwiping)
            {
                //if (!skippedToss)
                //{
                Debug.Log("ending toss");
                    EndSwipe(Input.mousePosition);
                //}
                isSwiping = false;

                skippedToss = false; //Always clear on up release
            }

        }
    }

    private bool IsOverInfo()
    {
        PointerEventData pointerEvent = new PointerEventData(null);
        pointerEvent.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        uiRaycaster.Raycast(pointerEvent, results);
		
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.name.Equals("Open Rules"))
            {
                return true;
            }
        }

        return false;
    }
    
    private void StartSwipe(Vector3 pos)
    {
        
        touchTimeStart = Time.time;
        startPos = new Vector3(pos.x, 0, pos.y);
    }

    private void EndSwipe(Vector3 pos)
    {
        touchTimeFinish = Time.time;
        endPos = new Vector3(pos.x, 0, pos.y);
        direction = startPos - endPos;
        timeInterval = touchTimeFinish - touchTimeStart;

        OnEndSwipe();
        Debug.Log("endSwipe");

        GetComponent<Food>().Toss(direction, timeInterval);
    }

    }
