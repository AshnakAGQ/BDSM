using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    GameObject[] players;
    LineRenderer lineRenderer;
    [SerializeField] float length = 5f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        lineRenderer.positionCount = 2 * players.Length + 1;
        PlayerController.PitEvent.AddListener(shrinkRope);
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        PlayerController.PitEvent.RemoveAllListeners();
    }

    private void Update()
    {
        lineRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y * 100f) * -1;

        lineRenderer.SetPosition(0, transform.position);

        for (int i = 0; i < players.Length; i++)
        {
            lineRenderer.SetPosition(2 * i + 1, players[i].transform.position);
            lineRenderer.SetPosition(2 * i + 2, transform.position);
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (players.Length != 0)
        {
            Vector2 center = Vector2.zero;
            foreach (GameObject player in players)
            {
                center += (Vector2) player.transform.position;
            }
            center /= players.Length;

            transform.position = center;

            float centerDistance = Vector2.Distance(center, players[0].transform.position);
            float distance = centerDistance * players.Length;

            if (distance > length)
            {
                float halfPoint = (centerDistance - length / players.Length) / (players.Length * centerDistance);
                foreach (GameObject player in players)
                {
                    PlayerController controller = player.GetComponent<PlayerController>();
                    if (controller != null && !controller.falling) // Players only get affected by the rope if they are not falling
                    {
                        player.transform.position = Vector2.Lerp(player.transform.position, center, halfPoint);
                    }
                }
            }
        }
    }

    private void shrinkRope(GameObject caller)
    {
        StartCoroutine(shrinkRopeCoroutine(caller));
    }

    // After making sure that the caller is indeed an object tied to this rope, shrink the rope until it is size 0
    // This will pull the other players in to have them fall as well
    private IEnumerator shrinkRopeCoroutine(GameObject caller)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == caller)
            {
                while (length > 0)
                {
                    length -= 0.1f;
                    yield return null;
                }
                length = 0.0f;
                lineRenderer.enabled = false;
            }
        }
    }
}
