﻿using UnityEngine;

namespace DotsClone {
    public class TouchSystem : Singleton<TouchSystem> {
        public delegate void OnTouchHit(Dot dot);
        public static event OnTouchHit TouchHit;

        public delegate void OnDragEnd();
        public static event OnDragEnd DragEnd;

        bool isDragging;
        bool isInsideDot;

        public Vector2 pointerWorldPosition {
            get {
                if(Application.isEditor) {
                    return Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
                else if(Input.touchCount > 0) {
                    return Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
                }
                else {
                    return Vector2.zero;
                }
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void InitSingleton() {
            DummyCreate();
        }

        private void Update() {
            if(isDragging && ((Application.isEditor && Input.GetMouseButtonUp(0)) || (!Application.isEditor && Input.touchCount == 0))) {
                isDragging = false;
                isInsideDot = false;
                if(DragEnd != null) {
                    DragEnd();
                }
            }
            if(Application.isEditor && (Input.GetMouseButtonDown(0) || isDragging)) {
                CheckInputDown(Input.mousePosition);
            }
            else if(!Application.isEditor && Input.touchCount > 0) {
                CheckInputDown(Input.GetTouch(0).position);
            }
        }

        private void CheckInputDown(Vector2 screenPoint) {
            var collider = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(screenPoint), LayerMask.GetMask("Dot"));
            if(collider != null && !isInsideDot) {
                isInsideDot = true;
                isDragging = true;
                if(TouchHit != null) {
                    TouchHit(collider.GetComponent<Dot>());
                }
            }
            else if(collider == null && isInsideDot) {
                isInsideDot = false;
            }
        }
    }
}