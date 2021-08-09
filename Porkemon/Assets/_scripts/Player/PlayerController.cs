using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private bool isMoving;

    [SerializeField] private float speed;
    private Vector2 input;

    private Animator _animator;

    [SerializeField] private LayerMask solidObjectsLayer, pokemonLayer;

    public event Action OnPorkemonEncounter;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0)
                input.y = 0;

            if (input != Vector2.zero)
            {
                _animator.SetFloat("Move X", input.x);
                _animator.SetFloat("Move Y", input.y);

                var targetPosition = transform.position;
                targetPosition.x += input.x;
                targetPosition.y += input.y;

                if (IsAvailable(targetPosition))
                    StartCoroutine(MoveTowards(targetPosition));
            }
        }
    }

    void LateUpdate()
    {
        _animator.SetBool("IsMoving", isMoving);
    }

    IEnumerator MoveTowards(Vector3 destination)
    {
        isMoving = true;

        while (Vector3.Distance(transform.position,destination) > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = destination;
        isMoving = false;

        CheckForPokemon();
    }

    /// <summary>
    /// Comprueba que la zona a la que queremos acceder esté disponible
    /// </summary>
    /// <param name="target">Zona deseada a acceder</param>
    /// <returns><i>True</i> si el target está disponible, <i>false</i> en caso contrario</returns>
    private bool IsAvailable(Vector3 target)
    {
        if (Physics2D.OverlapCircle(target, .2f, solidObjectsLayer) != null)
            return false;

        return true;
    }

    [SerializeField] private float verticalOffset = 0.2f;

    void CheckForPokemon()
    {
        if (Physics2D.OverlapCircle(transform.position - new Vector3(0, verticalOffset), 0.2f, pokemonLayer) != null)
        {
            if (Random.Range(0,100) < 17)
            {
                OnPorkemonEncounter();
            }
        }
    }

    /// <summary>
    /// Método mío para elegir la hierba y porkémon salvajes correspondientes
    /// </summary>
    /// <param name="porkemonAreas"></param>
    /// <returns>Devuelve el objeto que tiene el componente PorkemonMapArea</returns>
    public GameObject CheckNearestPorkemonArea(GameObject[] porkemonAreas)
    {
        GameObject nearest = null, player = FindObjectOfType<PlayerController>().gameObject;
        float nearestDistance = 100000f, distance;

        foreach (var area in porkemonAreas)
        {
            distance = Vector3.Distance(area.gameObject.transform.position, player.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = area.GetComponentInParent<PorkemonMapArea>().gameObject;
            }
        }

        if (nearest != null)
        {
            return nearest;
        }
        else
        {
            return null;
        }
    }
}