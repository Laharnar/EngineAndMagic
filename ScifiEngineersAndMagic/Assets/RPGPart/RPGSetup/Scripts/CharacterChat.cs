using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

/// <summary>
/// Allows single character to say things with a bubble.
/// </summary>
public class CharacterChat : MonoBehaviour {

    bool talking = false;

    public Text display;

    public void SayLine(string line, float pauseAfterDone, float talkSpeed) {
        if (talking == false)
            //StartCoroutine(Talk(line, talkSpeed, pauseAfterDone));
            StartCoroutine(TalkRandomly(line, talkSpeed, pauseAfterDone));
    }

    private IEnumerator Talk(string line, float talkSpeed, float pauseAfterDone) {
        talking = true;
        if (line == "") {
            Debug.Log("Talk warning: Nothing to say, empty line.");
            display.text = "...";
        } else {
            display.text = "";
            for (int i = 0; i < line.Length; i++) {
                display.text += line[i];
                yield return new WaitForSeconds(talkSpeed);
            }
        }
        yield return new WaitForSeconds(pauseAfterDone);
        talking = false;
    }

    /// <summary>
    /// Puts random characters instead of given line
    /// </summary>
    /// <param name="line"></param>
    /// <param name="talkSpeed"></param>
    /// <param name="pauseAfterDone"></param>
    /// <returns></returns>
    private IEnumerator TalkRandomly(string line, float talkSpeed, float pauseAfterDone) {
        talking = true;
        if (line == "") {
            Debug.Log("Talk warning - random: Nothing to say, empty line.");
            display.text = "...?";
        } else {
            display.text = ""; 
            for (int i = 0; i < line.Length; i++) {

                // example for custom speech pattern. you can make your own formating language with this.
                display.text += (char)UnityEngine.Random.Range(65, 120); ;
                yield return new WaitForSeconds(talkSpeed);
            }
        }
        yield return new WaitForSeconds(pauseAfterDone);
        talking = false;
    }

    internal static void GetBubble(Transform bubble, Transform target, Vector3 offset) {
        bubble.position = target.position + offset;
    }
}
