using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FairyControl : MonoBehaviour
{
    public GameObject effect_join;
    public Transform tf_fairy;
    public AudioSource aud_start;//欢迎来到康复训练中心，请目视前方朝前站立，点击“开始”按钮后，保持静止姿势，等待3秒即可。
    public AudioClip[] clips;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        effect_join.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        effect_join.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(2);
        tf_fairy.DOScale(Vector3.one * 0.25f, 2f).SetEase(Ease.OutExpo);
        yield return new WaitForSeconds(2);
        //aud_start.Play();
    }

    //0：教学  1：非常好 2：五星  
    public void OnPlayAudAtIndex(int ind)
    {
        aud_start.clip= clips[ind];
        aud_start.Play();
    }



}
