using DG.Tweening;
using UnityEngine;

public class TwoWay : MonoBehaviour
{
    public Transform p0, p1, p2;
    [Min(0)] public float y = 1f;
    [Min(0)] public float z = 1f;
    public Transform ent0, ent1;
    private Vector3 prsEntScl0, prsEntScl1;
    [SerializeField] bool isDebugging = false;

    public Transform startVecVal, endVecVal;
    [Min(0)] public float positioningTime = 1f, durationStartTime = 1f, durationEndTime = 1f;
    public Ease easeType = Ease.Linear;

    private Sequence _wobbleSeq;
    private Tween _t0, _t1, _t2;

    private void Start()
    {
        prsEntScl0 = ent0.localScale;
        prsEntScl1 = ent1.localScale * 0.99f; //Eger Olursa Z-Fighting Onlemek Amacli Scale Azaltildi.
        Wobble();
    }

    private void OnDestroy()
    {
        KillSequences();
    }

    private void OnDisable()
    {
        KillSequences();
    }

    public void Wobble()
    {
        //Debug Yazarak Tween OnUpdate Testi Yapildi.
        //Ilk Tween OnUpdate On Complete Ardindan Sonlanmistir. "() => { CheckAfterDo(); Debug.LogWarning("1."); }"
        _t0 = p1.DOLocalMove(startVecVal.localPosition, positioningTime)
            .SetEase(easeType)
            .OnComplete(WobbleOnComplete)
            .OnUpdate(CheckAfterDo);
    }

    public void WobbleOnComplete()
    {
        _wobbleSeq = DOTween.Sequence();
        _t1 = p1.DOLocalMove(endVecVal.localPosition, durationStartTime).SetEase(easeType);
        _t2 = p1.DOLocalMove(startVecVal.localPosition, durationEndTime).SetEase(easeType);
        _wobbleSeq.Append(_t1).Append(_t2).SetLoops(int.MaxValue).OnUpdate(CheckAfterDo);
    }

    public void KillSequences()
    {
        _wobbleSeq.Kill();
    }

    public void Interpolation()
    {
        Vector3 p1p0 = p1.localPosition - p0.localPosition;
        Vector3 p2p1 = p2.localPosition - p1.localPosition;
        Vector3 mid0 = p1p0 * 0.5f + p0.localPosition;
        Vector3 mid1 = p2p1 * 0.5f + p1.localPosition;

        Vector3 entity0 = prsEntScl0;
        entity0.x = p1p0.magnitude;
        ent0.localScale = entity0;
        ent0.localPosition = mid0;

        Vector3 entity1 = prsEntScl1;
        entity1.x = p2p1.magnitude;
        ent1.localScale = entity1;
        ent1.localPosition = mid1;

        Quaternion qt01 = Quaternion.LookRotation(p1p0);
        Quaternion qt02 = Quaternion.Euler(0, -90, 0);
        ent0.rotation =  qt01 * qt02;

        Quaternion qt11 = Quaternion.LookRotation(p2p1);
        Quaternion qt12 = Quaternion.Euler(0, -90, 0);
        ent1.rotation = qt11 * qt12;

    }

    public void CheckAfterDo()
    {
        if (p0.hasChanged || p1.hasChanged || p2.hasChanged)
        {
            //InterpolateBlock(t);
            Interpolation();
            if (isDebugging) Debug.LogWarning("Paremetreler Degisti!");
            p0.hasChanged = false;
            p1.hasChanged = false;
            p2.hasChanged = false;
        }
    }

}
