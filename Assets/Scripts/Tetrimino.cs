using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tetrimino : MonoBehaviour {

    //Falling variables
    float fall = 0;
    private float fallspeed;
    private float contVertSpeed = .05f;                 //Speed at which piece moves when down is pressed
    private float contSpeed = .1f;                      //Speed at which piece moves when left or right is pressed
    private float vertTimer = 0;                       
    private float horizTimer = 0;
    private float buttonDownWaitMax = 0.2f;
    private float buttonDownHorizTime = 0;
    private float buttonDownVertTime = 0;

    //Checks whether piece is moved vertically or horizontally
    private bool movedHoriz = false;
    private bool movedVert = false;

    //Variables for rotation
    public bool allowRotation = true;
    public bool limitRotation = false;

    //Counts the amount of rows hard dropped
    public int downPress = 0;

    //Touch variables
    private int touchSensHoriz = 8;
    private int touchSensVert = 4;

    Vector2 prevUnitPos = Vector2.zero;
    Vector2 dircetion = Vector2.zero;
    bool moved = false;

	// Use this for initialization
	void Start () {
        fallspeed = GameObject.Find("grid").GetComponent<Game>().fallSpeed;
	}
	
	// Update is called once per frame
	void Update () {
        checkInput();
	}

    //Checks user input
    void checkInput() {
        if(Input.touchCount > 0) {
            Touch t = Input.GetTouch(0);
            if(t.phase == TouchPhase.Began) {

            }
            else if (t.phase == TouchPhase.Moved) {

            }
            else if (t.phase == TouchPhase.Ended) {

            }
        }
        if(Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow)) {
            horizTimer = 0;
            buttonDownHorizTime = 0;
            movedHoriz = false;
            
        }
        if(Input.GetKeyUp(KeyCode.DownArrow)) {
            vertTimer = 0;
            movedVert = false;
            buttonDownVertTime = 0;
        }

        //Right
        if (Input.GetKey(KeyCode.RightArrow)) {
            moveRight();
        }

        //Left
        if (Input.GetKey(KeyCode.LeftArrow)) {
            moveLeft();
        }

        //Rotate
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            rotatePiece();
                
        }

        //Down
        if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallspeed) {
            moveDown();
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("Start");
        }
    }

    void moveLeft() {
        //Left
        if (Input.GetKey(KeyCode.LeftArrow)) {
            if (movedHoriz) {
                if (buttonDownHorizTime < buttonDownWaitMax) {
                    buttonDownHorizTime += Time.deltaTime;
                    return;
                }
                if (horizTimer < contSpeed) {
                    horizTimer += Time.deltaTime;
                    return;
                }
            }
            if (!movedHoriz) {
                movedHoriz = true;
            }
            horizTimer = 0;
            transform.position += Vector3.left;
            if (!checkIsValidPos()) {
                transform.position += Vector3.right;
            }
            else {
                FindObjectOfType<Game>().updateGrid(this);
            }
        }
    }

    void moveRight() {
        //Right
        if (Input.GetKey(KeyCode.RightArrow)) {
            if (movedHoriz) {
                if (buttonDownHorizTime < buttonDownWaitMax) {
                    buttonDownHorizTime += Time.deltaTime;
                    return;
                }
                if (horizTimer < contSpeed) {
                    horizTimer += Time.deltaTime;
                    return;
                }
            }
            if (!movedHoriz) {
                movedHoriz = true;
            }
            horizTimer = 0;
            transform.position += Vector3.right;
            if (!checkIsValidPos()) {
                transform.position += Vector3.left;
            }
            else {
                FindObjectOfType<Game>().updateGrid(this);
            }
        }
    }

    void moveDown() {
        //Down
        if (Input.GetKey(KeyCode.DownArrow) || Time.time - fall >= fallspeed) {
            if (movedVert) {
                if (buttonDownVertTime < buttonDownWaitMax) {
                    buttonDownVertTime += Time.deltaTime;
                    return;
                }
                if (vertTimer < contVertSpeed) {
                    vertTimer += Time.deltaTime;
                    return;
                }
            }
            if (!movedVert) {
                movedVert = true;
            }
            vertTimer = 0;
            transform.position += Vector3.down;
            if (Input.GetKey(KeyCode.DownArrow)) {
                downPress++;
            }

            if (!checkIsValidPos()) {
                transform.position += Vector3.up;
                enabled = false;
                Game.currentScore += downPress;
                FindObjectOfType<Game>().deleteRow();
                if (FindObjectOfType<Game>().topOut(this)) {
                    FindObjectOfType<Game>().gameOver();
                }
                FindObjectOfType<Game>().spawnPiece();
            }
            else {
                FindObjectOfType<Game>().updateGrid(this);
            }
            fall = Time.time;
        }
    }

    void rotatePiece() {
        //Rotate
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            if (allowRotation) {                                            //Checks to see if piece is allowed to rotate
                if (limitRotation) {                                        //Checks if rotation is limited
                    if (transform.rotation.eulerAngles.z >= 90) {           //If limited, do not allow rotations more than 90 deg
                        transform.Rotate(0, 0, -90);
                    }
                    else
                        transform.Rotate(0, 0, 90);
                }
                else {
                    transform.Rotate(0, 0, 90);
                }

                if (!checkIsValidPos()) {                                  //A check for pieces that are limited, makes sure that piece will stay in play
                    if (limitRotation) {
                        if (transform.rotation.eulerAngles.z >= 90) {
                            transform.Rotate(0, 0, -90);
                        }
                        else
                            transform.Rotate(0, 0, 90);
                    }
                    else {
                        transform.Rotate(0, 0, -90);
                    }
                }
                else {
                    FindObjectOfType<Game>().updateGrid(this);
                }

            }

        }
    }

    //Checks if piece position is valid
    bool checkIsValidPos() {
        foreach(Transform mino in transform) {
            Vector2 pos = FindObjectOfType<Game>().Round(mino.position);
            if (!FindObjectOfType<Game>().ifInGrid(pos)) {
                return false;
            }
            if (FindObjectOfType<Game>().getTransformAtGridPos(pos) != null && FindObjectOfType<Game>().getTransformAtGridPos(pos).parent != transform) {
                return false;
            }
        }

        return true;
    }
    
}
