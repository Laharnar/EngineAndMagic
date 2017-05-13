using UnityEngine;
using System.Collections;

public class PlayerUi : MonoBehaviour {

    public Hp hp;
    public Stamina stamina;
    public Stamina pills;
    Vector3 originalHpScale;
//    public Move stamina;
 //   public Energy energy;

    /// <summary>
    /// Assigned slots:
    /// 0: hp bar
    /// 1: stamina bar
    /// 2: energy
    /// </summary>
    public Transform[] ui;
    public Transform hpBar;
    public Transform staminaBar;
    public Transform pillsBar;
    private Vector3 originalPillsScale;
    private Vector3 originalStaminaScale;

    // Use this for initialization
    void Start () {
        // TODo: save starting scales of ui
        originalHpScale = hpBar.transform.localScale;
        originalStaminaScale = staminaBar.transform.localScale;
        originalPillsScale = pillsBar.transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
        // todo: apply new scales from the hp, stamina itd. Hp should derive from ui property
        Vector3 s = hpBar.transform.localScale;
        s.x = hp.hp / hp.maxHp * originalHpScale.x;
        hpBar.transform.ScaleToLeft(s);


        Vector3 s2 = staminaBar.transform.localScale;
        s2.x = stamina.stamina / stamina.maxStamina * originalStaminaScale.x;
        staminaBar.transform.ScaleToLeft(s2);

        Vector3 s3 = pillsBar.transform.localScale;
        s3.x = pills.stamina / pills.maxStamina * originalPillsScale.x;
        staminaBar.transform.ScaleToLeft(s3);

    }


}

public static class ScalingHelper {
    public static void ScaleToLeft(this Transform transform, Vector3 newlocalScale) {
        Vector3 diff = transform.localScale-newlocalScale;
        transform.localScale = newlocalScale;
        transform.Translate(Vector3.left*diff.x/2);
    }
}