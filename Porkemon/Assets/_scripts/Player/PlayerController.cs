using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private LayerMask solidObjectsLayer, pokemonLayer;

    private bool isMoving;

    [SerializeField]
    private float speed;

    public event Action OnPorkemonEncounter;

    private Vector2 input;

    private Animator _animator;

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

    void CheckForPokemon()
    {
        if (Physics2D.OverlapCircle(transform.position,0.2f,pokemonLayer) != null)
        {
            if (Random.Range(0,100) < 17)
            {
                isMoving = false;
                Debug.Log("Hay pokemon. Jaja cagaste!!");
                OnPorkemonEncounter();
            }
        }
    }

    /// <summary>
    /// Comprueba que la zona a la que queremos acceder esté disponible
    /// </summary>
    /// <param name="target">Zona deseada a acceder</param>
    /// <returns><i>True</i> si el target está disponible, <i>false</i> en caso contrario</returns>
    private bool IsAvailable(Vector3 target)
    {
        if (Physics2D.OverlapCircle(target, .1f, solidObjectsLayer) != null)
            return false;

        return true;
    }
}