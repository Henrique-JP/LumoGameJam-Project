using UnityEngine;
using System.Collections.Generic;
using System.Collections; // Certifique-se de ter isso para usar List

public class FantasyGhost_AI : GhostAI_Base
{
    [Header("Habilidade Única: Teleporte")]
    [SerializeField] private float teleportInterval = 30f;
    // [SerializeField] private Vector2 teleportAreaCenter = Vector2.zero;
    // [SerializeField] private Vector2 teleportAreaSize = new (40f, 20f);

    private float teleportTimer;
    public float giggleTimer = 10f; // Timer para o som de risada

    [Header("Sons do Fantasma")]
    public AudioSource GhostGiggle_Fantasy; // Fonte de passos do fantasma
    public AudioSource GhostTeleport_Fantasy; // Fonte de passos do fantasma

    protected override void Awake()
    {
        base.Awake(); // Chama o Awake da base
        teleportTimer = teleportInterval;

        StartCoroutine(PlayGiggleSound(giggleTimer)); // Inicia o cronômetro de risada
    }

    protected override void HandleStateBehavior()
    {
        base.HandleStateBehavior(); // Executa o comportamento padrão da base (patrulha/fuga)

        teleportTimer -= Time.deltaTime;
        if (teleportTimer <= 0)
        {
            Teleport();
            teleportTimer = teleportInterval;
        }
    }

    private void Teleport()
    {
        // Verifica se há waypoints disponíveis para teleportar
        if (waypoints == null || waypoints.Count == 0)
        {
            Debug.LogWarning("FantasyGhost_AI: Não há waypoints configurados para o teleporte!");
            return; // Sai da função se não houver waypoints
        }

        // Escolhe um waypoint aleatório da lista de waypoints da classe base
        int randomIndex = Random.Range(0, waypoints.Count);
        Transform targetWaypoint = waypoints[randomIndex];

        // Teleporta o fantasma para a posição do waypoint escolhido
        GhostTeleport_Fantasy.Play(); // Toca o som de teleporte
        transform.position = targetWaypoint.position;
        GhostTeleport_Fantasy.Play(); // Toca o som de passos do fantasma

        Debug.Log("FantasyGhost_AI: Se teleportou para o waypoint: " + targetWaypoint.name + " em " + transform.position);
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected(); // Chama o OnDrawGizmosSelected da base para desenhar as gizmos padrão
        // Gizmos.color = new Color(0.5f, 0f, 1f, 0.5f);
        // Gizmos.DrawCube(teleportAreaCenter, teleportAreaSize);
    }

    IEnumerator PlayGiggleSound(float giggleTimer)
    {
        yield return new WaitForSeconds(giggleTimer);

        if (soundManager != null && GhostGiggle_Fantasy != null)
        {
            GhostGiggle_Fantasy.Play();
        }
        else
        {
            Debug.LogWarning("FantasyGhost_AI: SoundManager ou GhostGiggle_Fantasy não está configurado!");
        }

        StartCoroutine(PlayGiggleSound(giggleTimer)); // Reinicia o cronômetro de risada
    }
}