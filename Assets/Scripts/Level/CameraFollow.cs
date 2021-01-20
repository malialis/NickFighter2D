
using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{

    private Transform[] playerTransforms;

    public float yOffset = 1.0f;
    public float minDistance = 2.5f;

    private float xMin, xMax, yMin, yMax;


    private void Start()
    {
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        playerTransforms = new Transform[allPlayers.Length];
        for(int i = 0; i < allPlayers.Length; i++)
        {
            playerTransforms[i] = allPlayers[i].transform;
        }
    }

    private void LateUpdate()
    {
        if(playerTransforms.Length == 0)
        {
            Debug.Log("have not found player, please tag them Player");
            return;
        }

        xMin = xMax = playerTransforms[0].position.x;
        yMin = yMax = playerTransforms[0].position.y;
        
        for(int i = 1; i < playerTransforms.Length; i++)
        {
            //for xMin and xMax
            if (playerTransforms[i].position.x < xMin)
                xMin = playerTransforms[i].position.x;
            if (playerTransforms[i].position.x > xMax)
                xMax = playerTransforms[i].position.x;
            //for yMin and yMax
            if (playerTransforms[i].position.y < yMin)
                yMin = playerTransforms[i].position.y;
            if (playerTransforms[i].position.x > yMax)
                yMax = playerTransforms[i].position.y;
        }

        float xMiddle = (xMin + xMax) / 2;
        float yMiddle = (yMin + yMax) / 2;
        float distance = xMax - xMin;

        if (distance < minDistance)
            distance = minDistance;

        xMiddle = Mathf.Clamp(xMiddle, -2, 1);

        transform.position = new Vector3(xMiddle, yMiddle + yOffset, -distance);
    }


}