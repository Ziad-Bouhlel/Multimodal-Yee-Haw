using UnityEngine;


public enum State
{
    Setup,
    Exploration
}

public class SystemState : MonoBehaviour
{
    private State currentState;

    public State GetState()
    {
        return currentState;
    }

    public void SetState(State newState)
    {
        currentState = newState;
    }
}
