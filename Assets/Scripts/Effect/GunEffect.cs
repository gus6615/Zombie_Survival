using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunEffect : MonoBehaviour {

    private SpriteRenderer sprite;

    public bool isBloodCtrlOn;
    public float time;

	// Use this for initialization
	void Start () {
        sprite = GetComponent<SpriteRenderer>();

        if(this.gameObject.tag == "Bullet")
        {
            Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
            Vector2 randVec = new Vector2(Random.Range(-5f, 10f), Random.Range(10f, 20f));
            rigidbody.AddForce(randVec);
            rigidbody.AddTorque(Random.Range(5f, 15f));
            time = 1.2f;
        } else
            time = 5f;
    }
	
	// Update is called once per frame
	void Update () {

        this.transform.position += Vector3.right * PlayerScript.moveSpeed * Time.deltaTime;

        if (sprite.color.a >= 0.01f)
        {
            float data = sprite.color.a;
            data -= Time.deltaTime * time;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, data);
        }
        else
        {
            Destroy(this.gameObject);
        }
	}
}
