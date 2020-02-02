﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerControllerAdventure : PlayerController
{
    [SerializeField]
    private SpriteRenderer FrontRenderer = null;
    [SerializeField]
    private SpriteRenderer BackRenderer = null;
    [SerializeField]
    private SpriteRenderer SideRenderer = null;


    private Animator FrontAnimator;
    private Animator SideAnimator;

    [Range(0, 1)]
    [SerializeField]
    private float FeetColliderRadius = 0.5f;
    [Range(0, 1)]
    [SerializeField]
    private float HeadColliderRadius = 0.5f;

    [SerializeField]
    private Collider2D FullSizeCollider = null;

    // small size controller for future use
    //[SerializeField]
    //private Collider2D SmallSizeCollider = null;

    private float ground_collide_radius;
    private float head_collide_radius;

    private float ground_collide_pos;
    private float head_collide_pos;

    private float footstep_time;

    new private void Awake()
    {
        base.Awake();
        ground_collide_radius = transform.localScale.x * FeetColliderRadius;
        head_collide_radius = transform.localScale.x * HeadColliderRadius;
        ground_collide_pos = (transform.localScale.y * 0.5f) - FeetColliderRadius;
        head_collide_pos = (transform.localScale.y * 0.5f) - HeadColliderRadius;

        is_facing = Faces.Streight;
        FrontRenderer.enabled = true;
        SideRenderer.enabled = false;

        // on ground always true
        on_ground = true;
        footstep_time = 0;
    }

    //private void FixedUpdate()
    //{
    //    Vector3 gcc = transform.position;
    //    gcc.Set(gcc.x, gcc.y - ground_collide_pos, gcc.z);
    //    Collider2D[] colliders = Physics2D.OverlapCircleAll(gcc, ground_collide_radius, GroundMask);
    //    for (int i = 0; i < colliders.Length; i++)
    //    {
    //        on_ground = colliders[i].gameObject != gameObject;
    //    }
    //}

    public override void Move(float move_left_right, float move_up_down, bool crouch, bool jump)
    {
        if (!crouch)
        {
            Vector3 gcc = transform.position;
            gcc.Set(gcc.x, gcc.y + head_collide_pos, gcc.z);
            if (Physics2D.OverlapCircle(gcc, head_collide_radius, GroundMask))
            {
                crouch = true;
            }
        }

        if (on_ground)
        {
            if (crouch)
            {
                move_left_right *= CrouchSpeedModifier;
                FullSizeCollider.enabled = false;
                //SmallSizeCollider.enabled = true;

                // play footstep sound
                FootstepSoundLogic(crouch);
            }
            else
            {
                FullSizeCollider.enabled = true;
                //SmallSizeCollider.enabled = false;
            }

            // movement in any case
            if (move_left_right != 0 || move_up_down != 0)
            {
                // play footstep sound
                FootstepSoundLogic(crouch);
            }

            // here is the movement
            Vector3 targetVelocity = new Vector2(move_left_right * MovementSpeed, move_up_down * MovementSpeed);
            player_physics_body.velocity = Vector3.SmoothDamp(player_physics_body.velocity, targetVelocity, ref previous_velocity, MovementSmoothing);

            // // no left right
            // if (move_left_right == 0.0f && is_facing != Faces.Streight)
            // {
            //     FrontRenderer.enabled = true;
            //     SideRenderer.enabled = false;
            // }

            // movement renderer
            // else
            // {
            //     FrontRenderer.enabled = false;
            //     SideRenderer.enabled = true;
            
            Vector3 theScale = transform.localScale;

            // movement right
            if (move_left_right > 0 && is_facing != Faces.Right)
            {
                is_facing = Faces.Right;
                SideRenderer.enabled = true;
                FrontRenderer.enabled = false;
                BackRenderer.enabled = false;
                theScale.x *= -1;
            }

            // movement left
            else if (move_left_right < 0 && is_facing != Faces.Left)
            {
                is_facing = Faces.Left;
                SideRenderer.enabled = true;
                FrontRenderer.enabled = false;
                BackRenderer.enabled = false;
                theScale.x *= -1;
            }

            // movement up
            else if (move_up_down > 0 && is_facing != Faces.Up)
            {
                is_facing = Faces.Up;
                SideRenderer.enabled = false;
                FrontRenderer.enabled = false;
                BackRenderer.enabled = true;
            }

            // movement down
            else if (move_up_down < 0 && is_facing != Faces.Down)
            {
                is_facing = Faces.Down;
                SideRenderer.enabled = false;
                FrontRenderer.enabled = true;
                BackRenderer.enabled = false;
            }


            transform.localScale = theScale;
            //}
        }

        if (on_ground && jump)
        {
            // not implemented
        }
    }

    void FootstepSoundLogic(bool crouch)
    {
        // play footstep sound
        footstep_time += Time.deltaTime;

        if (!crouch)
        {
            if (footstep_time >= footstep_acti_time)
            {
                GameObject.Find("SoundManager").GetComponent<SoundManager>().PlayFootstepAdventure();
                footstep_time = 0;
            }
        }
        else
        {
            if (footstep_time >= footstep_acti_time * 2)
            {
                GameObject.Find("SoundManager").GetComponent<SoundManager>().PlayFootstepAdventure();
                footstep_time = 0;
            }
        }
    }

}
