// In English: BookPickup.cs
using UnityEngine;

public class BookPickup : MonoBehaviour
{
    [Header("Configuração do Livro")]
    public BookGenre bookGenre; // ATUALIZADO: usa o enum BookGenre
    [TextArea(3, 5)]
    public string bookHint;

    private Collider2D _collider;
    private SpriteRenderer _renderer;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _renderer = GetComponent<SpriteRenderer>();

        if (_collider == null || !_collider.isTrigger)
        {
            Debug.LogWarning($"O livro '{gameObject.name}' precisa de um Collider2D com 'Is Trigger' marcado.", this);
        }
    }
}