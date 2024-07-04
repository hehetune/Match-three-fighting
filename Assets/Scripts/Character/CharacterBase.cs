﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
    public class CharacterBase : MonoBehaviour
    {
        public CharacterAnimator characterAnimator;
        private Color tintColor;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = transform.Find("Model").GetComponent<SpriteRenderer>();
        }
        
        private void Update() {
            if (tintColor.a > 0) {
                float tintFadeSpeed = 6f;
                tintColor.a -= tintFadeSpeed * Time.deltaTime;
                spriteRenderer.color = tintColor;
            }
        }
        
        public void SetColorTint(Color color) {
            tintColor = color;
        }

        public void PlayAnimAttack(Vector3 direction, Action onHit, Action onComplete)
        {
            characterAnimator.PlayNormalAttackAnimation(onHit, onComplete);
        }
    }
}