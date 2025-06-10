using UnityEngine;
using Mirror;
using TMPro;

public class PlayerMovements : NetworkBehaviour
{
    public float speed = 5f;
    public GameObject Camera;
    public GameObject hitParticles;
    private GameObject healthBar;
    private TMP_Text Score;
    private int ScoreVal = 0;
    public int health = 100;
    void Start()
    {
        if (!isLocalPlayer)
        {
            Camera.SetActive(false);
            return;

        }
       //Cursor.lockState = CursorLockMode.Locked;

        //Cursor.visible = false;

        healthBar = GameObject.Find("HealthBar");
        Score = GameObject.Find("Score").GetComponent<TMP_Text>();
        if (healthBar == null)
        {
            Debug.LogError("HealthBar GameObject not found in the scene.");
        }
        else
        {
            healthBar.transform.localScale = new Vector3((float)health / 100, 1, 1);
        }

        UpdateScore(ScoreVal);
        
    }

    void UpdateScore(int score)
    {
        if (Score != null)
        {
            Score.text = "Score: " + score;
        }
        else
        {
            Debug.LogError("Score Text component not found in the scene.");
        }
    }

    void PlayParticules(Vector3 pos, Vector3 normal)
    {
        if (hitParticles != null)
        {
            GameObject particles = Instantiate(hitParticles, pos, Quaternion.identity);
            particles.transform.rotation = Quaternion.LookRotation(normal);
            particles.transform.position += normal * 0.1f;
            Destroy(particles, 2f);
        }
    }

    void Die()
    {
        Debug.Log("Player has died.");
        health = 100;
        healthBar.transform.localScale = new Vector3(1, 1, 1);
        transform.position = Vector3.zero; // Reset position
        transform.rotation = Quaternion.identity; // Reset rotation
    }

    void DoDamage(int clientID)
    {
        health -= 10;
        if (health <= 0)
        {
            Debug.Log("Player " + clientID + " has died.");
        }
        else
        {
            Debug.Log("Player " + clientID + " took damage. Health: " + health);
        }
        healthBar.transform.localScale = new Vector3((float)health / 100, 1, 1);
    }

    [ClientRpc]
    void RpcShootPlayer(int clientID)
    {
        if (isLocalPlayer)
        {
            Debug.Log("Client " + clientID + " is shot.");
            DoDamage(clientID);
        }
    }
    
    [ClientRpc]
    void RpcShootParticles(Vector3 pos, Vector3 normal)
    {
        PlayParticules(pos, normal);
    }

    [Command(requiresAuthority = false)]
    void CmdShootParticles(Vector3 pos, Vector3 normal)
    {
        RpcShootParticles(pos, normal);
    }

    [Command(requiresAuthority = false)]
    void CmdShootPlayer(int clientID)
    {
        RpcShootPlayer(clientID);
    }

    void Shoot()
    {
        if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out RaycastHit hit, 1000f))
        {
            CmdShootParticles(hit.point, hit.normal);
            Debug.Log("Hit: " + hit.collider.name);
            if (hit.collider.CompareTag("Player"))
            {
                int clientID = hit.collider.GetComponent<NetworkIdentity>().connectionToClient.connectionId;
                Debug.Log("Shooting player with ID: " + clientID);
                CmdShootPlayer(clientID);
                ScoreVal += 10; // Increment score for hitting a player
                UpdateScore(ScoreVal);
            }
            else
            {
                Debug.Log("Hit something else: " + hit.collider.name);
            }
        }
    }

    float lastShootTime = 0f;
    float shootInterval = 2f;

    void Update()
    {
        if (!isLocalPlayer)
            return;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontal, 0, vertical);

        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        transform.Translate(movement * Time.deltaTime * speed);
        //GetComponent<Rigidbody>().linearVelocity = movement * speed * Time.deltaTime;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (GetComponent<Rigidbody>() != null && transform.position.y < 0.2f)
            {
                GetComponent<Rigidbody>().AddForce(Vector3.up * 5f, ForceMode.Impulse);
            }
        }

        transform.Rotate(0, mouseX * 5f, 0);

        Camera.transform.Rotate(-mouseY * 5f, 0, 0);

        Vector3 cameraRotation = Camera.transform.localEulerAngles;
        if (cameraRotation.x > 180)
        {
            cameraRotation.x -= 360;
        }
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -60, 60);
        Camera.transform.localEulerAngles = cameraRotation;

        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastShootTime < shootInterval)
                return;
            lastShootTime = Time.time;
            Shoot();
        }
    }
}
