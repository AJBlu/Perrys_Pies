using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Patrol : State
{
    public bool isDeaf = false;
    public float TimeDeafened = 10f;
    private Search _search;
    private Pursuit _pursuit;


    private void Awake()
    {
        _search = gameObject.GetComponent<Search>();
        _pursuit = gameObject.GetComponent<Pursuit>();
        _statemachine = gameObject.GetComponent<State_Machine>();

        

    }
    public override void InitializeState()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.green;
        
        //implement deafness
        StartCoroutine("deafenPerry");
        //create room nodes
        //events
        //events
        EnteredState.AddListener(gameObject.GetComponent<PerryNav>().OnPatrol);
        EnteredState.AddListener(gameObject.GetComponent<PerrySensor>().OnPatrol);
        ExitedState.AddListener(gameObject.GetComponent<PerryNav>().OnPatrolExit);
        ExitedState.AddListener(gameObject.GetComponent<PerrySensor>().OnPatrolExit);
        EnteredState.Invoke();
        isActive = true;


    }

    public override void UpdateState()
    {
        if (!isDeaf)
        {
            //audio cue code goes here
        }

        //idle pathing code goes here
            //go to closest random unexplored node


        //eyesight code goes here
            //if distant eyesight range sees player, change state to pursue
            
            //if close eyesight range sees player, change state to pursue

    }

    public override void ExitState()
    {


        ExitedState.Invoke();
        isActive = false;

    }

    private IEnumerator deafenPerry()
    {
        isDeaf = true;
        yield return new WaitForSeconds(TimeDeafened);
        isDeaf = false;
    }

    public void OnDistantPlayerSeen()
    {
        if (isActive)
        {
            //change state to search
            _statemachine.ChangeState(_search);
        }

    }

    public void OnClosePlayerSeen()
    {
        if (isActive)
        {
            Debug.Log("Seeing player from patrol state.");
            //change state to pursuit
            _statemachine.ChangeState(_pursuit);
        }
    }
    public void OnAudioCueHeard()
    {
        Debug.Log("Event invoked!");
        if (isActive)
        {
            if(!isDeaf)
            {
                _statemachine.ChangeState(_search);
            }
        }
    }

}
