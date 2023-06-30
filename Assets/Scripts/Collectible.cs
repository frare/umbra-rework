using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public static int layer = 12;

    [SerializeField] private float floatSpeed;
    [SerializeField] private float floatMagnitude;

    private Transform sprite;





    private void OnValidate()
    {
        sprite = GetComponentInChildren<SpriteRenderer>(true).transform;
    }

    private void FixedUpdate()
    {
        sprite.localPosition = new Vector3(
            0f,
            Mathf.Sin(Time.fixedTime * floatSpeed) * floatMagnitude,
            0f
        );
    }

    public void Collect()
    {
        LevelManager.PageCollected();
        gameObject.SetActive(false);
    }
}