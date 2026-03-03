using System.Collections.Generic;
using UnityEngine;

public class ContrabandTableTrigger : MonoBehaviour
{
    public Dictionary<int, bool> luggagesOnTable;

    private void OnEnable()
    {
        luggagesOnTable = new Dictionary<int, bool>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // If it is a contraband security luggage not being held by the player, add it to the table and increase the score
        CheckBag(other);
    }

    private void OnTriggerStay(Collider other)
    {
        // Perform the same check in OnTriggerStay to account for the player dropping a bag on the table and it not registering in OnTriggerEnter
        CheckBag(other);
    }

    private void CheckBag(Collider collider)
    {
        if (collider.TryGetComponent<SecurityLuggage>(out SecurityLuggage luggage) && luggage.markedAsContraband && luggage.transform.parent == null)
        {
            if (luggagesOnTable.ContainsKey(luggage.luggageID))
            {
                return;
            }

            luggagesOnTable.Add(luggage.luggageID, true);
            SecurityScoring.Instance.luggagesCleared++;
            Debug.Log(SecurityScoring.Instance.luggagesCleared);
        }

        // End round if it's the last bag
        if (SecurityScoring.Instance.luggagesCleared >= SecurityScoring.Instance.luggageInRound)
        {
            SecurityScoring.Instance.RoundOver();
        }
    }
}
