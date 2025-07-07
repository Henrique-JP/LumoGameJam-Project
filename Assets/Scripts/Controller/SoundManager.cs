using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Sons de Ambiente")]
    public AudioSource musicSource; // Fonte de m��sica
    [Header("Sons de Jogador")]
    public AudioSource PlayerWalkSource; // Fonte de passos do jogador
    [Header("Sons de Fantasma")]
    public AudioSource GhostWalk_Terror; // Fonte de passos do fantasma
}
