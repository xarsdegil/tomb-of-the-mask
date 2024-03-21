using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Rigidbody2D rb;
    Animator anim;
    private Vector2 startTouchPosition, endTouchPosition, swipeDelta;
    [SerializeField] GameObject fallDustPrefab;

    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    private Direction direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    break;

                case TouchPhase.Ended:
                    endTouchPosition = touch.position;
                    swipeDelta = endTouchPosition - startTouchPosition;
                    ProcessMovement();
                    break;
            }
        }
    }

    private void ProcessMovement()
    {
        float x = swipeDelta.x;
        float y = swipeDelta.y;

        anim.SetTrigger("Fall");

        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            rb.velocity = new Vector2(Mathf.Sign(x) * speed, 0);
        }
        else
        {
            rb.velocity = new Vector2(0, Mathf.Sign(y) * speed);
        }

        if (rb.velocity.x > 0)
        {
            direction = Direction.Right;
        }
        else if (rb.velocity.x < 0)
        {
            direction = Direction.Left;
        }
        else if (rb.velocity.y > 0)
        {
            direction = Direction.Up;
        }
        else if (rb.velocity.y < 0)
        {
            direction = Direction.Down;
        }

        SetRotation();
    }

    private void SetRotation()
    {
        switch (direction)
        {
            case Direction.Up:
                transform.rotation = Quaternion.Euler(0, 0, 180);
                break;
            case Direction.Down:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case Direction.Left:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
            case Direction.Right:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        rb.velocity = Vector2.zero;
        anim.SetTrigger("Idle");

        var dust = Instantiate(fallDustPrefab, transform.position, Quaternion.identity);
        dust.transform.rotation = transform.rotation;
        Destroy(dust, 1f);

        switch (direction)
        {
            case Direction.Up:
                transform.position = new Vector2(transform.position.x, transform.position.y - 0.025f);
                dust.transform.position = new Vector2(dust.transform.position.x, dust.transform.position.y + 0.25f);
                break;
            case Direction.Down:
                transform.position = new Vector2(transform.position.x, transform.position.y + 0.025f);
                dust.transform.position = new Vector2(dust.transform.position.x, dust.transform.position.y - 0.25f);
                break;
            case Direction.Left:
                transform.position = new Vector2(transform.position.x + 0.025f, transform.position.y);
                dust.transform.position = new Vector2(dust.transform.position.x - 0.25f, dust.transform.position.y);
                break;
            case Direction.Right:
                transform.position = new Vector2(transform.position.x - 0.025f, transform.position.y);
                dust.transform.position = new Vector2(dust.transform.position.x + 0.25f, dust.transform.position.y);
                break;
        }
    }
}
