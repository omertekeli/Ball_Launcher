using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class BallHandler : MonoBehaviour
{
    [SerializeField] GameObject ballPrefab;
    [SerializeField] Rigidbody2D pivot; 
    [SerializeField] float respawnDelay;
    [SerializeField] float delayTime;

    private Rigidbody2D currentBallRigidbody2D;
    private SpringJoint2D currentBallSprintJoint2D;
    private Camera mainCamera;
    private bool isDragging;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        SpawnNewBall();
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBallRigidbody2D==null)
        {
            return;
        }

        //To avoid get value constantly, use below code. No return if there is no press in the screen
        //After type " Touch" hit control period and select using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
        if (Touch.activeTouches.Count == 0)
        {
            if (isDragging)
            {
                LaunchBall();
            }

            isDragging = false;
            return;
        }

        isDragging = true;
        //Physics rules are not working there is touch
        currentBallRigidbody2D.isKinematic = true;

        //primaryTouch gives firt touch and that is what you want because in this game there is no multiple touches
        //position.ReadValue() ensures to translate Vector2 value.
       
        Vector2 touchPosition = new Vector2();

        foreach (Touch touch in Touch.activeTouches)
        {
            touchPosition += touch.screenPosition;
        }

        touchPosition /= Touch.activeTouches.Count;

        //We use camera class because it has ScreenWorldPoint method which provides to convert screen coordinates to world coordinates
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);

        //Ball position will be where you touch in the screen
        currentBallRigidbody2D.position = worldPosition;
    }

    //Throw ball
    private void LaunchBall()
    {
        currentBallRigidbody2D.isKinematic = false;//Physics rules are working there is no touch
        currentBallRigidbody2D = null; //clear the option

        //Give a second between Launchball method and DetacBall method
        Invoke(nameof(DetachBall), delayTime);
    }

    private void DetachBall()
    {
        currentBallSprintJoint2D.enabled = false;//Think that the rope is broken
        currentBallSprintJoint2D = null;//clear the option

        //After throwing, spawn new ball
        Invoke(nameof(SpawnNewBall), respawnDelay);
    }

    //To spawn balls
    private void SpawnNewBall()
    {
        //Create the ball as ball instance to use rigidbody and sprinjoint2D
        GameObject ballInstance = Instantiate(ballPrefab, pivot.position, Quaternion.identity);

        currentBallRigidbody2D = ballInstance.GetComponent<Rigidbody2D>();
        currentBallSprintJoint2D = ballInstance.GetComponent<SpringJoint2D>();

        //Connect the joint and ball
        currentBallSprintJoint2D.connectedBody = pivot;

    }
}
