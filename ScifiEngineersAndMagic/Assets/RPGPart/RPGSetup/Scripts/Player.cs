using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour , IMovement2D, IAttacking, ICharacter2D {

    public bool jumping {
        get;
        set;
    }

    public Vector3 jumpPos {
        get;
        set;
    }

    public CommandCode[] _attacks;
    public CommandCode[] attacks {
        get {
            return _attacks;
        }

        set {
            _attacks = value;
        }
    }

    public bool attacking {
        get;
        set;
    }

    public float _jumpStrength=20;
    public float jumpStrength {
        get {
            return _jumpStrength;
                }

        set {
            _jumpStrength = value;
        }
    }

    public float _speed = 1;
    public float speed {
        get {
            return _speed;
        }

        set {
            _speed = value;
        }
    }

    public Transform _jumpTarget;
    public Transform jumpTarget {
        get {
            return _jumpTarget;
        }

        set {
            _jumpTarget = value;
        }
    }

    public Transform _movementTarget;
    public Transform movementTarget {
        get {
            return _movementTarget;
        }

        set {
            _movementTarget = value;
        }
    }

    public SpriteRenderer _sprite;
    public SpriteRenderer sprite {
        get {
            return _sprite;
        }

        set {
            _sprite = value;
        }
    }

    /// <summary>
    /// Mirroring is applied as the unit moves left and right.
    /// </summary>
    public Transform mirroring;

    Rigidbody2D rbody;

    public Transform target;

    public Weapon weapon;

    float dashTime;
    public float dashRate = 0.75f;
    public float dashSpeed = 12;
    public float dashLength = 0.4f;
    float dashLengthTime;

    void Awake() {
        tempAttackType = sprite.GetComponent<SpriteRenderer>().color;
        rbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update () {
        
        // use unity's axis raw input for player
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time > dashTime) {
            dashTime = Time.time + dashRate;
            Dash2D(horizontal, vertical);
            dashLengthTime = Time.time + dashLength;
        } else {
            if (Time.time > dashLengthTime) {
                Move2D(horizontal, vertical, speed);
            }
        }

        if (Input.GetKeyDown(KeyCode.J)) {
            Attack();
        }
        if (Input.GetKeyDown(KeyCode.K)) {
            Magic();
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            Block();
        }
        if (Input.GetKeyDown(KeyCode.I)) {
            Range();
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            Strong();
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            Debug.Log("test");
            if (GetComponent<WorldInteractions>().CanInteract("move into building")) {
            Debug.Log("moving in");
                GetComponent<WorldInteractions>().InteractiWithWorld(WorldInteractions.WorldInteractionCommands.MoveIntoBuilding);
            } else if (GetComponent<WorldInteractions>().CanInteract("talk to npc")) {
                Debug.Log("player is trying to talk");
            }
        }

    }

    private void Dash2D(float x, float y) {
        Move2D(x, y, dashSpeed);
    }

    void FixedUpdate() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Jump(jumpStrength);
        }
    }

    public void Move2D(float x, float y, float speed) {
        //movementTarget.Translate(x*speed * Time.deltaTime, y*speed*Time.deltaTime, 0);
        if (CanMoveInDir(rbody.position + new Vector2(x * speed, y * speed) * Time.deltaTime)) {
            rbody.MovePosition(rbody.position + new Vector2(x * speed, y * speed) * Time.deltaTime);
            mirroring.transform.rotation = x < 0 ? Quaternion.Euler(0, -180, 0) : x > 0 ? Quaternion.Euler(0, 0, 0) : mirroring.transform.rotation;
        }
    }

    private bool CanMoveInDir(Vector2 vector2) {
        // TO-DO: handle movement prediction with collision, by using raycasts 
        // or checking range, or combination of both, raycast then check range/imaginary collision with it.
        return true;
    }

    public void Jump(float height) {
        Debug.LogWarning("Jump is disabled.");
        /*if (jumping == false) {
            Debug.Log("jump");
            jumpPos = transform.position;

            jumping = true;
            jumpTarget.GetComponent<Rigidbody2D>().AddForce(Vector2.up * height*Time.fixedDeltaTime, ForceMode2D.Impulse);
        }*/
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.name == "Ground") {
            Debug.Log("check 2 " + other.transform);
            Vector3 point = other.transform.position;
            transform.GetComponent<Rigidbody2D>().MovePosition((transform.position - point).normalized * 2);
        }

        if (jumping && other.gameObject.name == "JumpArea") {
            jumping = false;
            Debug.Log("jump done");
        }
    }

    Color tempAttackType;
    public void Attack() {
        if (!attacking) {
            weapon.Attack();
            SimulateAttackWithColor(Color.red);
        }
    }

    private void SimulateAttackWithColor(Color col) {
        sprite.color = col;
        StartCoroutine(_WaitForAttackStop(0.5f));
    }

    private IEnumerator _WaitForAttackStop(float v) {
        attacking = true;
        yield return new WaitForSeconds(v);
        sprite.color = tempAttackType;
        attacking = false;
    }

    public void Block() {
        if (!attacking) {
            SimulateAttackWithColor(Color.grey);
        }
    }

    public void Magic() {
        if (!attacking) {
            SimulateAttackWithColor(Color.blue);
        }
    }

    public void Range() {
        if (!attacking) {
            SimulateAttackWithColor(Color.green);
        }
    }

    public void Strong() {
        if (!attacking) {
            SimulateAttackWithColor(Color.magenta);
        }
    }
}


