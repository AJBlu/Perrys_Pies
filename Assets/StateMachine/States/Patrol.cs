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
        _perry = GetComponent<PerryNav>();
        _perrySensor = gameObject.GetComponent<PerrySensor>();
        _statemachine = gameObject.GetComponent<State_Machine>();
        _search = gameObject.GetComponent<Search>();
        _pursuit = gameObject.GetComponent<Pursuit>();


    }
    public override void InitializeState()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.green;
        
        //implement deafness
        StartCoroutine("deafenPerry");
        //create room nodes

        isActive = true;
        EnteredState.Invoke();


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
        //clear explored room nodes
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
            //change state to pursuit
            _statemachine.ChangeState(_pursuit);
        }
    }
    public void OnAudioCueHeard()
    {
        if (isActive)
        {
            if(!isDeaf)
            {
                _statemachine.ChangeState(_search);
            }
        }
    }

}
