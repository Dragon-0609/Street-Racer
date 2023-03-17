using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Script which controls the player
/// </summary>
[RequireComponent(typeof(Rigidbody))] //make sure the gameobject which has this script has rigidbody on it
public class PlayerController : MonoBehaviour
{
    private Collider colliderComponent; //ref to collider component
    private float endXPos = 0; //variable to change player x position
    private Rigidbody myBody;
    private Vector3 maxPosition;

    private Tween _tween;
    [SerializeField] private int _turnSpeed = 10;

    private void OnDisable()
    {
        InputManager.instance.horizontalMove -= ActionOnHorizontalMove; //unsubscribe to the event
    }

    private void Start()
    {
        myBody = gameObject.GetComponent<Rigidbody>(); //get reference to Rigidbody
        myBody.isKinematic = true; //set isKinematic to false
        myBody.useGravity = false; //set useGravity ture
        SpawnVehicle(GameManager.singeton.currentCarIndex); //spawn the selected car
        maxPosition = LevelManager.Instance.playerMaxPosition;
    }

    public void GameStarted()
    {
        InputManager.instance.horizontalMove += ActionOnHorizontalMove; //subscribe to the event
    }

    public void SpawnVehicle(int index) //method alled to spawn the selected car
    {
        if (transform.childCount > 0) //check for the children
        {
            Destroy(transform.GetChild(0).gameObject); //destroy the 1st child
        }

        //spawn he selected car as child
        GameObject child = Instantiate(LevelManager.Instance.VehiclePrefabs[index], transform);


        CarInfo carInfo = child.GetComponent<CarInfo>();
        LevelManager.Instance.speed = carInfo.speed;
        LevelManager.Instance.breakSpeed = carInfo.breakSpeed;
        LevelManager.Instance.breakDecelSpeed = carInfo.breakSpeed / 32;
        carInfo.cameraPosition.x += LevelManager.Instance.playerInitialPosition.x;
        LevelManager.Instance.cameraToAttach[LevelManager.Instance.fpsCameraIndex].position = carInfo.cameraPosition;
        LevelManager.Instance.cameraToAttach[LevelManager.Instance.fpsCameraIndex].rotation =
            Quaternion.Euler(carInfo.cameraRotation);

        colliderComponent = child.GetComponent<Collider>(); //get reference to collider
        colliderComponent.isTrigger = true; //set isTrigger to true
    }

    void ActionOnHorizontalMove(SwipeType swipeType) //method alled on swipe action of InputManager
    {
        if (GameManager.singeton.gameStatus == GameStatus.PLAYING) //is gamestatus is playing
        {
            float turnSpeed = _turnSpeed + LevelManager.Instance.moveSpeed / 3f;

            switch (swipeType)
            {
                case SwipeType.LEFT: //if we left swipe
                    endXPos = transform.position.x - turnSpeed * Time.deltaTime; //change endXPos by 3 to left
                    break;
                case SwipeType.RIGHT: //if we right swipe
                    endXPos = transform.position.x + turnSpeed * Time.deltaTime; //change endXPos by 3 to right
                    break;
            }

            endXPos = Mathf.Clamp(endXPos, maxPosition.y, maxPosition.x); //clamp endXPos between -3 and 3
            _tween = transform.DOMoveX(endXPos, 0.15f).OnComplete(() => _tween = null); //move the car
        }
    }

    private void OnTriggerEnter(Collider other) //Unity default mthod to detect collision
    {
        if (other.GetComponent<EnemyController>()) //check if the collided object has EnemyController on it
        {
            if (GameManager.singeton.gameStatus == GameStatus.PLAYING) //check if gameStatus is PLAYING
            {
                LevelManager.Instance.GameOver(); //call GameOver
                colliderComponent.isTrigger = false; //set isTrigger to false
                StopController();
            }
        }
    }

    public void StopController()
    {
        if (_tween != null)
        {
            _tween.Kill();
        }

        myBody.isKinematic = false; //set isKinematic to false
        myBody.useGravity = true; //set useGravity ture
        //add a random force
        gameObject.GetComponent<Rigidbody>().AddForce(Random.insideUnitCircle.normalized * 100f);
    }
}