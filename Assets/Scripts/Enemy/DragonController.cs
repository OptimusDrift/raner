using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private Transform target;
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private float timeSpawn = 1f;
    [SerializeField]
    private float loadAtackMin = 15f;
    [SerializeField]
    private float loadAtack = 15f;
    [SerializeField]
    private float loadAtackMax = 30f;
    [SerializeField]
    private float timeAttack = .6f;
    [SerializeField]
    private Transform endLevel;
    [SerializeField]
    private Animator dragon;
    [SerializeField]
    private GameObject wall;
    [SerializeField]
    private Animator shadow;

    private bool isAttack = false;
    void Start()
    {
        Physics2D.IgnoreCollision(endLevel.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }

    IEnumerator Spawn(float time)
    {
        shadow.SetBool("spawn", true);
        gameObject.transform.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
        gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        gameObject.transform.position = new Vector3(1000, 1000, 1000);
        yield return new WaitForSeconds(time);
        ChangeState(0);
        shadow.SetBool("spawn", false);
        
        gameObject.transform.position = spawnPoint.position;
        isAttack = false;
        ChangeSpeedAnimation(1f);
    }

    public void Play()
    {
        StartCoroutine(Spawn(timeSpawn));
    }

    void Update()
    {
        if (isAttack)
        {
            dragon.speed += Time.deltaTime;
        }
    }

    private void ChangeSpeedAnimation(float speed)
    {
        dragon.speed = speed;
    }

    public void ChangeState(int state){
        dragon.SetInteger("dragonState", state);
    }
    public void LoadAttack()
    {
        ChangeSpeedAnimation(.5f);
        isAttack = true;
        ChangeState(1);
        gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(timeAttack);
        gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 1) * speed;
    }

    private int wait = 0;
    IEnumerator Wait()
    {
        if ((target.transform.position.x - gameObject.transform.position.x) > 1.5f)
        {
            gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(1, 0) * speed/3;
            transform.GetComponent<SpriteRenderer>().flipX = false;

        }
        else if ((target.transform.position.x - gameObject.transform.position.x) < -1.5f)
        {
            gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(-1, 0) * speed/3;
            transform.GetComponent<SpriteRenderer>().flipX = true;
        } else {
            gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
        wait++;
        yield return new WaitForSeconds(.3f);
        if(wait > loadAtack){
            LoadAttack();
            loadAtack = Random.Range(loadAtackMin, loadAtackMax);
            wait = 0;
        }else
        {
            StartCoroutine(Wait());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Rock"))
        {
            if (!other.gameObject.GetComponent<Rock>().IsDestroyed())
            {
                if (dragon.GetInteger("dragonState") >= 1)
                {
                    Vibration.Vibrate(50);
                    wall.GetComponent<IWiggle>().Wiggles();
                }
            }
            other.gameObject.GetComponent<Rock>().Destroyed();
        }
        if (other.gameObject.CompareTag("DeathZone"))
        {
            if (wait <= 0)
            {
                wait = 1;
                gameObject.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                StartCoroutine(Wait());
            }
        }
        if (other.gameObject.CompareTag("Respawn"))
        {
            StartCoroutine(Spawn(timeSpawn));
        }
    }
}
