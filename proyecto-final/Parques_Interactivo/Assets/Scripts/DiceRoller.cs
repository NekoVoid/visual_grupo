using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DiceRoller : MonoBehaviour
{
    public GameObject dicePrefab;

    private GameObject dice1, dice2;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RollDice();
        }
    }

    public void RollDice()
    {
        if (dice1 != null) Destroy(dice1);
        if (dice2 != null) Destroy(dice2);

        Vector3 basePos = transform.position;

        Vector3 offset1 = new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(0.5f, 1.5f),
            Random.Range(-0.5f, 0.5f)
        );

        Vector3 offset2 = new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(0.5f, 1.5f),
            Random.Range(-0.5f, 0.5f)
        );

        Vector3 pos1 = basePos + offset1;
        Vector3 pos2 = basePos + offset2;

        dice1 = Instantiate(dicePrefab, pos1, Random.rotation);
        dice2 = Instantiate(dicePrefab, pos2, Random.rotation);

        Rigidbody rb1 = dice1.GetComponent<Rigidbody>();
        Rigidbody rb2 = dice2.GetComponent<Rigidbody>();

        rb1.AddForce(Random.onUnitSphere * 5f, ForceMode.Impulse);
        rb1.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);

        rb2.AddForce(Random.onUnitSphere * 5f, ForceMode.Impulse);
        rb2.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);

        StartCoroutine(WaitForDiceToStop(rb1, rb2));
    }

    IEnumerator WaitForDiceToStop(Rigidbody rb1, Rigidbody rb2)
    {
        float threshold = 0.1f;
        float timeStill = 0f;
        float requiredStillTime = 0.5f;

        while (true)
        {
            bool dice1Still = rb1.linearVelocity.magnitude < threshold && rb1.angularVelocity.magnitude < threshold;
            bool dice2Still = rb2.linearVelocity.magnitude < threshold && rb2.angularVelocity.magnitude < threshold;

            if (dice1Still && dice2Still)
            {
                timeStill += Time.deltaTime;
                if (timeStill >= requiredStillTime)
                    break;
            }
            else
            {
                timeStill = 0f;
            }

            yield return null;
        }

        int r1 = GetTopFaceNumber(dice1);
        int r2 = GetTopFaceNumber(dice2);

        Debug.Log("Resultado dado 1: " + r1);
        Debug.Log("Resultado dado 2: " + r2);
    }


    int GetTopFaceNumber(GameObject dice)
    {
        Transform t = dice.transform;
        Vector3 up = Vector3.up;

        float maxDot = -1f;
        int result = -1;

        Dictionary<int, Vector3> faceDirs = new Dictionary<int, Vector3>
        {
            { 2, -Vector3.up },
            { 6, -Vector3.forward },
            { 4, -Vector3.right },
            { 3, Vector3.right },
            { 1, Vector3.forward },
            { 5, Vector3.up }
        };

        foreach (var pair in faceDirs)
        {
            Vector3 worldDir = t.TransformDirection(pair.Value);
            float dot = Vector3.Dot(worldDir.normalized, up);

            if (dot > maxDot)
            {
                maxDot = dot;
                result = pair.Key;
            }
        }
        return result;
    }
}
