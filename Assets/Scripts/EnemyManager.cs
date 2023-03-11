using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script which manages spawning, activation and deactivation of enemies
/// </summary>
public class EnemyManager
{
    private List<GameObject> deactiveEnemyList; //list to store deactive enemy gameobjects
    private Vector3[] enemySpawnPos = new Vector3[4]; //array of possible spawn position
    private GameObject enemyHolder; //parent object for all enemy objects
    private float moveSpeed; //moving speed
    public int currentCars = 0;

    public EnemyManager(Vector3 spawnPos, float moveSpeed) //constructor of script
    {
        this.moveSpeed = moveSpeed; //set the speed
        deactiveEnemyList = new List<GameObject>(); //initialize list

        enemySpawnPos[0] = spawnPos - Vector3.right * 3;
        enemySpawnPos[1] = spawnPos - Vector3.right * 6;
        enemySpawnPos[2] = spawnPos;
        enemySpawnPos[3] = spawnPos + Vector3.right * 3;
        //create gameobject of name EnemyHolder as assign to enemyHolder
        enemyHolder = new GameObject("EnemyHolder");
    }

    public void SpawnEnemies(GameObject[] vehiclePrefabs) //method to spawn enemies
    {
        for (int i = 0; i < vehiclePrefabs.Length; i++) //loop through all the vehicles in the list
        {
            //spawn the enemy
            GameObject enemy = Object.Instantiate(vehiclePrefabs[i], enemySpawnPos[1], Quaternion.identity);
            enemy.SetActive(false); //deactivate the enemy
            enemy.transform.SetParent(enemyHolder.transform); //set its parent
            enemy.name = "Enemy"; //set the name
            enemy.AddComponent<EnemyController>(); //attach EnemyController componenet to it
            float speed = Random.Range(0, moveSpeed);
            enemy.GetComponent<EnemyController>().SetDefault(speed, this);
            deactiveEnemyList.Add(enemy); //add to deactiveEnemyList
        }
    }

    public void ActivateEnemy() //method to activate the enemy
    {
        if (deactiveEnemyList.Count >= 4)
            SpawnCountEnemy(Random.Range(1, 3));
        else
            Spawn();
    }

    private void SpawnCountEnemy(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (deactiveEnemyList.Count > 0) //check if the deactiveEnemyList have elements
            {
                Spawn();
                currentCars++;
            }
        }
    }

    private void Spawn()
    {
        int index = Random.Range(0, deactiveEnemyList.Count - 1);
        if (index >= deactiveEnemyList.Count) index = Random.Range(0, deactiveEnemyList.Count - 1);
        if (index >= deactiveEnemyList.Count) index = 0;
        if (index < deactiveEnemyList.Count)
        {
            GameObject enemy = deactiveEnemyList[index];
            SpawnEnemy(enemy);
        }
    }

    private void SpawnEnemy(GameObject enemy)
    {
        deactiveEnemyList.Remove(enemy); //remove the element from the list
        enemy.transform.position = enemySpawnPos[Random.Range(0, enemySpawnPos.Length)]; //set spawn position
        enemy.SetActive(true); //activate the enemy
        EnemyController controller = enemy.GetComponent<EnemyController>();
        if (enemy.transform.position.x > 0)
        {
            controller.moveSpeed = controller.initialMoveSpeed;
            enemy.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            controller.moveSpeed = -3f * controller.initialMoveSpeed;
            enemy.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    public void DeactivateEnemy(GameObject enemy) //method to deactivate the enemy
    {
        enemy.SetActive(false); //deactivate the enemy
        deactiveEnemyList.Add(enemy); //add to deactiveEnemyList
        currentCars--;
    }
}