using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStatemachine : MonoBehaviour
{
    public enum aiStates
    {
        Idle,
        Walking,
        Attacking
    }

    // Current state of the player
    private aiStates currentState;

    void Start()
    {
        // Set the initial state when the script starts
        SetState(aiStates.Idle);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SetState(aiStates.Attacking);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            SetState(aiStates.Walking);
        }
        else
        {
            SetState(aiStates.Idle);
        }

        // Perform state-specific behavior
        PerformStateBehavior();
    }

    // Method to set the current state
    private void SetState(aiStates newState)
    {
        currentState = newState;
        // Perform any additional actions when entering a new state
        EnterState();
    }

    // Method to handle state-specific behavior
    private void PerformStateBehavior()
    {
        // Implement behavior specific to each state
        switch (currentState)
        {
            case aiStates.Idle:
                // Code for idle state
                break;
            case aiStates.Walking:
                // Code for walking state
                break;
            case aiStates.Attacking:
                // Code for attacking state
                break;
                // Add more cases for additional states
        }
    }

    // Method to handle actions when entering a new state
    private void EnterState()
    {
        // Implement any actions needed when entering a new state
        Debug.Log("Entering State: " + currentState);
    }
}
