using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class BattleUnit : MonoBehaviour
{
    public PokemonBasic _base;
    public int _level;
    [SerializeField] private bool isPlayer;
    public bool IsPlayer
    {
        get => IsPlayer;
    }
    [SerializeField] private BattleHUD hud;
    public BattleHUD Hud
    {
        get => hud;
    }

    public Porkemon Porkemon { get; set; }

    private Image porkemonImage;
    private Vector3 initialPosition;
    private Color initialColor;
    private float capturedTimeAnim = .4f;

    [SerializeField] private float startTimeAnim = 1f, attackTimeAnim = .3f, 
        dieTimeAnim = 1f, hitTimeAnim = .1f;

    private void Awake()
    {
        porkemonImage = GetComponent<Image>();
        initialPosition = porkemonImage.transform.localPosition;
        initialColor = porkemonImage.color;
    }


    public void SetUpPorkemon(Porkemon porkemon)
    {
        Porkemon = porkemon;

        porkemonImage.sprite = (isPlayer ? Porkemon.Base.BackSprite : Porkemon.Base.FrontSprite);
        porkemonImage.color = initialColor;
        porkemonImage.transform.position = initialPosition;
        hud.SetPorkemonData(porkemon);
        transform.localScale = new Vector3(1, 1, 1);
        PlayStartAnimation();
    }

    public void PlayStartAnimation()
    {

        porkemonImage.transform.localPosition = new Vector3(initialPosition.x + (isPlayer ? -1 : 1) * 500,
            initialPosition.y);

        porkemonImage.transform.DOLocalMoveX(initialPosition.x, startTimeAnim);
    }

    public void PlayAttackAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(porkemonImage.transform.DOLocalMoveX(initialPosition.x + (isPlayer ? 1 : -1) * 60,attackTimeAnim));
        seq.Append(porkemonImage.transform.DOLocalMoveX(initialPosition.x, attackTimeAnim));
    }

    public void PlayReceiveAttackAnimation()
    {
        var seq = DOTween.Sequence();
        for (int i = 0; i < 3; i++)
        {
            seq.Append(porkemonImage.DOColor(Color.red, hitTimeAnim));
            seq.Append(porkemonImage.DOColor(initialColor, hitTimeAnim));
        }
    }

    public void PlayFaintAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(porkemonImage.transform.DOLocalMoveY(initialPosition.y-100,dieTimeAnim));
        seq.Join(porkemonImage.DOFade(0, dieTimeAnim));
    }

    public IEnumerator PlayCapturedAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(porkemonImage.DOFade(0, capturedTimeAnim));
        seq.Join(transform.DOScale(new Vector3(.2f, .2f,.2f),capturedTimeAnim));
        seq.Join(transform.DOLocalMoveY(initialPosition.y + 50, capturedTimeAnim));

        yield return seq.WaitForCompletion();
    }

    public IEnumerator PlayBreackOutAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(porkemonImage.DOFade(1, capturedTimeAnim));
        seq.Join(transform.DOScale(new Vector3(1, 1, 1), capturedTimeAnim));
        seq.Join(transform.DOLocalMoveY(initialPosition.y, capturedTimeAnim));

        yield return seq.WaitForCompletion();
    }
}