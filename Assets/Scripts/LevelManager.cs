using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// This script controls the game, from spawning enemy, road, player to setting game status
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] public float moveSpeed = 0; //speed with which game moves
    [SerializeField] public float speed = 8; //speed with which game moves
    [SerializeField] public float breakSpeed = 4; //speed with which game moves
    [SerializeField] public float breakDecelSpeed = 4; //speed with which game moves

    [FormerlySerializedAs("minSpeed")] [SerializeField]
    private float minimumSpeed = 4; //speed with which game moves

    [SerializeField] private float enemyMoveSpeed = 12; //speed with which game moves
    [SerializeField] private RoadTemplate[] roadPrefabs; //ref to raod prefab
    [SerializeField] private int[] notDuplicatablePrefabs; //ref to raod prefab

    [SerializeField] private GameObject[] vehiclePrefabs; //ref to all vehicle prefabs
    [SerializeField] private GameObject[] trafficCarPrefabs; //ref to enemy car prefabs

    private List<RoadTemplate> _roads; //list to store the roads spawned in the game
    private Vector3 _nextRoadPos = new Vector3(1.5f, 0, 0); //position for next road
    private GameObject _tempRoad, _roadHolder; //variables
    private EnemyManager _enemyManager; //variable to store EnemyManager
    private PlayerController _playerController; //variable to store PlayerController
    private float _distanceTravelled = 0; //track distance travelled
    private int _trackRoadAtIndex = 0, _lastTrack = 0; //this variable are used to Reuse the roads in the game
    public Transform[] cameraToAttach;
    public int fpsCameraIndex = 0;
    public Vector3 playerInitialPosition;
    public Vector3 playerMaxPosition;

    private Vector3[] oldCameraPosition;

    public GameObject[] VehiclePrefabs
    {
        get { return vehiclePrefabs; }
    } //getter

    public PlayerController PlayerController
    {
        get { return _playerController; }
    } //getter

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _roadHolder = new GameObject("RoadHolder"); //create gameobject with name RoadHolder
        _roads = new List<RoadTemplate>(); //initialize the roads list
        Vector3 enemySpawnPos = _nextRoadPos;
        int index = 0;
        int previndex = 0;
        for (int i = 0; i < 5; i++) //Spawn 5 road prefabs
        {
            Vector3 roadPos = _nextRoadPos;
            roadPos.x = 0;
            index = Random.Range(0, roadPrefabs.Length);
            if (previndex == index)
                index = CheckDuplicity(index);
            Debug.Log($"{index}, {roadPrefabs.Length}");
            GameObject road = Instantiate(roadPrefabs[index].gameObject, roadPos, Quaternion.identity); //spawn the road
            road.transform.SetParent(_roadHolder.transform); //set its parent
            RoadTemplate template = road.GetComponent<RoadTemplate>();
            _nextRoadPos += Vector3.forward * template.removeAfter; //set the nextRoadPos
            _roads.Add(template); //add road to roads list
            if (i == 3)
                enemySpawnPos = _nextRoadPos;
            previndex = index;
        }

        _enemyManager = new EnemyManager(enemySpawnPos, enemyMoveSpeed); //create EnemyManager
        _enemyManager.SpawnEnemies(trafficCarPrefabs); //spawn enemies
        SpawnPlayer();
    }

    private int CheckDuplicity(int index)
    {
        if (notDuplicatablePrefabs.Contains(index))
        {
            for (int j = 0; j < roadPrefabs.Length; j++)
            {
                if (!notDuplicatablePrefabs.Contains(j))
                {
                    index = j;
                    break;
                }
            }
        }

        return index;
    }

    void SpawnPlayer() //method used to spawn player
    {
        GameObject player = new GameObject("Player"); //create Player gameobject
        player.transform.position = playerInitialPosition; //set its position to zero
        player.AddComponent<PlayerController>(); //attach PlayerController script to it
        _playerController = player.GetComponent<PlayerController>(); //get reference to PlayerController
    }

    private void MoveRoad() //method responsible for moving roads
    {
        for (int i = 0; i < _roads.Count; i++) //loop through roads list
        {
            //move each road with moveSpeed
            _roads[i].transform.Translate(-transform.forward * moveSpeed * Time.deltaTime);
        }

        RoadTemplate road = _roads[_trackRoadAtIndex];
        if (road.transform.position.z <=
            -road.removeAfter) //if the road at 0th element is at z distance below -10
        {
            //lastTrack = trackRoadAtIndex - 1;                       //set lastTrack
            //if (lastTrack < 0)                                      //check if lastTrack is less than 0
            //{
            //    lastTrack = roads.Count - 1;                        //set lastTrack to last index of road
            //}

            //set lastTrack, check if lastTrack is less than 0, set lastTrack to last index of road
            _lastTrack = (_trackRoadAtIndex - 1) < 0 ? _roads.Count - 1 : _trackRoadAtIndex - 1;
            //change its position
            road.transform.position =
                _roads[_lastTrack].transform.position + Vector3.forward * road.removeAfter;

            //increase trackRoadAtIndex by 1, check if trackRoadAtIndex more or equal to roads elments, set trackRoadAtIndex to zero
            _trackRoadAtIndex = ++_trackRoadAtIndex >= _roads.Count ? 0 : _trackRoadAtIndex;

            //trackRoadAtIndex++;                                     //increase trackRoadAtIndex by 1
            //if (trackRoadAtIndex >= roads.Count)                    //check if trackRoadAtIndex more or equal to roads elments
            //{
            //    trackRoadAtIndex = 0;                               //set trackRoadAtIndex to zero
            //}
        }
    }

    public void GameStarted() //method called when Play button is clicked
    {
        GameManager.singeton.gameStatus = GameStatus.PLAYING; //set the game status to Playing
        _enemyManager.ActivateEnemy(); //activate the enemy
        _playerController.GameStarted(); //inform player about game starting\
        foreach (Transform camera1 in cameraToAttach)
        {
            camera1.SetParent(_playerController.transform);
        }

        InputManager.instance.acceleration += Acceleration;
    }

    private void Acceleration(AccelerationType accelerationType)
    {
        if (accelerationType == AccelerationType.Accelerate)
        {
            moveSpeed += (speed * Time.deltaTime) / (moveSpeed / 15f);
        }
        else
        {
            if (moveSpeed >= minimumSpeed)
            {
                Decelerate(accelerationType);
                if (moveSpeed < minimumSpeed) moveSpeed = minimumSpeed;
            }
        }
    }

    private void Decelerate(AccelerationType accelerationType)
    {
        if (accelerationType == AccelerationType.Decelerate)
        {
            moveSpeed -= breakSpeed * Time.deltaTime;
        }
        else
        {
            moveSpeed -= breakDecelSpeed * Time.deltaTime;
        }
    }

    private void Update()
    {
        if (GameManager.singeton.gameStatus != GameStatus.FAILED) //check if gamestatus is not Failed
        {
            MoveRoad(); //move the roads
        }

        if (GameManager.singeton.gameStatus == GameStatus.PLAYING) //check if gamestatus is not PLAYING
        {
            _distanceTravelled += moveSpeed * Time.deltaTime; //update the distanceTravelled
            //update DistanceText
            UIManager.instance.DistanceText.text = string.Format("{0:0}", _distanceTravelled);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (moveSpeed > 10 &&
            other.GetComponent<EnemyController>()) //check if the collided object has EnemyController on it        
        {
            _enemyManager.ActivateEnemy(); //ActivateEnemy
        }
    }

    public void GameOver()
    {
        GameManager.singeton.gameStatus = GameStatus.FAILED; //set gameStatus to FAILED
        //do camera shake adn after 1sec call UIManager GameOver method
        // cameraToAttach.position = oldCameraPosition;
        InputManager.instance.acceleration -= Acceleration;
        Camera.main.transform.DOShakePosition(1f, Random.insideUnitCircle.normalized, 5, 10f, false, true)
            .OnComplete
            (
                () => UIManager.instance.GameOver()
            );
    }
}