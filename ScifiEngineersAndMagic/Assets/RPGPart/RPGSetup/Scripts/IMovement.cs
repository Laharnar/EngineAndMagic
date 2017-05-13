

using UnityEngine;

interface ICharacter2D {
    SpriteRenderer sprite { get; set; }
}

interface ITriggerCollisions2D {
    void OnTriggerEnter2D(Collider2D other);
}

interface INormalCollisions2D {
    void OnCollisionEnter2D(Collision2D other);
}

interface IMovement2D {

    Transform jumpTarget { get; set; }
    Transform movementTarget { get; set; }

    bool jumping { get; set; }
    Vector3 jumpPos { get; set; }
    float jumpStrength { get; set; }
    void Jump(float y);


    float speed { get; set; }
    void Move2D(float x, float z, float speed);
}
interface IAttacking {

    bool attacking { get; set; }

    void Attack();
    void Block();
    void Magic();
    void Range();
    void Strong();
}