using UnityEngine;

public struct ActionCollection
{
    public enum Option
    {
        Idle,
        Move,
        Build,
        EnterInteractable,
        Attach,
        Harvest,
        Attack,
        Follow

    }
    public delegate void Action();

    static public Action Execute;

    static public Action GetActionWith(Option actionDescription)
    {

        switch (actionDescription)
        {
            case Option.Idle:
                Execute = Idle;
                return Idle;
            case Option.Move:
                Execute = Move;
                return Move;
            case Option.Build:
                Execute = Build;
                return Build;
            case Option.EnterInteractable:
                Execute = EnterInteractable;
                return EnterInteractable;
            case Option.Attack:
                Execute = Attack;
                return Attack;
            default:
                break;
        }
        return Idle;
    }

    static Action GetActionWith(int optionNumber) {
        return GetActionWith((Option)optionNumber);
    }

    static public void Idle()
    {
        Debug.Log("execute Idle");
    }

    static public void Move()
    {
        Debug.Log("execute Move");
    }

    static public void Build()
    {
        Debug.Log("execute Build");
    }

    static public void EnterInteractable()
    {
        Debug.Log("execute EnterInteractable");
    }

    static public void Harvest()
    {
        Debug.Log("execute Harvest");
    }

    static public void Attach()
    {
        Debug.Log("execute Attach");
    }
    static public void Attack() {
        Debug.Log("execute Attack");
    }
}
