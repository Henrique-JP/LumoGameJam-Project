using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneController : MonoBehaviour
{
    float SceneNumber = 0f; // Número da cena atual
    public Sprite[] cutSceneSprites; // Sprites para a cena
    public Image cutSceneImage; // Referência à imagem que exibirá o sprite da cena

    void OnEnable()
    {
        //cutSceneImage.preserveAspect = true; // Preserva a proporção do sprite
        // Inicia a coroutine para controlar a cena
        StartCoroutine(SceneController(5f)); // Espera 5 segundos antes de mudar a cena
    }


    IEnumerator SceneController(float IntervalTime)
    {
        if (SceneNumber <= cutSceneSprites.Length)
        {
            cutSceneImage.sprite = cutSceneSprites[(int)SceneNumber]; // Define o sprite de fundo da cena
            yield return new WaitForSeconds(IntervalTime); // Espera o tempo do intervalo
            SceneNumber++;
            StartCoroutine(SceneController(IntervalTime)); // Chama a coroutine novamente para a próxima cena
        }
    }
}
