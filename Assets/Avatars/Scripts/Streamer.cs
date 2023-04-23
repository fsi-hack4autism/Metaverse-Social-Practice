using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class Streamer : MonoBehaviour
{
    [SerializeField]
    private AudioSource avatarAudioSource;
    [SerializeField]
    private Animator avatarAnimator;
    private AnimatorOverrideController overrideController;
    private AnimationClipOverrides clipOverides;
    public AnimationClip defaultIdleAnimation;
    public AnimationClip defaultTalkAnimation;
    public AnimationClip newTalkAnimation;
    public Dictionary<string, SkinnedMeshRenderer> skinnedMeshRends = new Dictionary<string, SkinnedMeshRenderer>();

    internal static Dictionary<int, Dictionary<string, Dictionary<string, float>>> VisemeMap = new Dictionary<int, Dictionary<string, Dictionary<string, float>>>()
    {
       {0,  new Dictionary<string, Dictionary<string, float>>() },
        {1, new Dictionary<string, Dictionary<string, float>>()
            { // "uh" sound in the/us
                { "body", new Dictionary<string, float>() { { "V_Open", 25}, { "V_Wide", 52 }, { "V_Lip_Open", 38 }, { "A02_Brow_Down_Left", 9 }, { "A03_Brow_Down_Right", 9 }, { "A10_Eye_Look_Out_Left", 17 }, { "A12_Eye_Look_In_Right", 17 }, { "A14_Eye_Blink_Left", 14 }, { "A15_Eye_Blink_Right", 14 }, { "A20_Cheek_Puff", 3}, {"A25_Jaw_Open", 2 }, {"A28_Jaw_Right", 4 }, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { { "V_None", 40} } }
            }
        },
        {2,  new Dictionary<string, Dictionary<string, float>>()
            { // a sound in bat
                { "body", new Dictionary<string, float>() { {"V_Open", 73 }, {"A02_Brow_Down_Left", 9 }, {"A03_Brow_Down_Right", 9 }, {"A08_Eye_Look_Down_Left", 12 }, {"A09_Eye_Look_Down_Right", 12 }, {"A10_Eye_Look_Out_Left", 17 }, {"A12_Eye_Look_In_Right", 17 }, {"A14_Eye_Blink_Left", 13 }, {"A15_Eye_Blink_Right", 13 }, {"A20_Cheek_Puff", 3 }, {"A25_Jaw_Open", 2 }, {"A28_Jaw_Right", 4 }, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { {"V_None", 40} } }
            }
        },
        {3,  new Dictionary<string, Dictionary<string, float>>()
            { // o sound in for
                { "body", new Dictionary<string, float>() { {"V_Open", 17}, {"V_Explosive", 11}, {"V_Tight", 30}, {"V_Lip_Open", 2}, {"A02_Brow_Down_Left", 9 }, {"A03_Brow_Down_Right", 9 }, {"A08_Eye_Look_Down_Left", 12 }, {"A09_Eye_Look_Down_Right", 12 }, {"A10_Eye_Look_Out_Left", 17 }, {"A12_Eye_Look_In_Right", 17 }, {"A14_Eye_Blink_Left", 13 }, {"A15_Eye_Blink_Right", 13 }, {"A20_Cheek_Puff", 3 }, {"A25_Jaw_Open", 2 }, {"A28_Jaw_Right", 4 }, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { { "V_None", 20} } }
            }
        },
        {4,  new Dictionary<string, Dictionary<string, float>>()
            { // e sound in bed
                { "body", new Dictionary<string, float>() { {"V_Tight_O", 14}, {"V_Wide", 86}, {"V_Lip_Open", 78}, {"A02_Brow_Down_Left", 9 }, {"A03_Brow_Down_Right", 9 }, {"A08_Eye_Look_Down_Left", 12 }, {"A09_Eye_Look_Down_Right", 12 }, {"A10_Eye_Look_Out_Left", 17 }, {"A12_Eye_Look_In_Right", 17 }, {"A14_Eye_Blink_Left", 13 }, {"A15_Eye_Blink_Right", 13 }, {"A20_Cheek_Puff", 3 }, {"A25_Jaw_Open", 2 }, {"A28_Jaw_Right", 4 }, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { { "V_None", 15} } }
            }
        },
        {5,  new Dictionary<string, Dictionary<string, float>>()
            { // er ("er") sound in mirth/dinner
                { "body", new Dictionary<string, float>() { {"V_Open", 15}, {"V_Tight_O ", 6}, {"V_Tight", 18}, {"Mouth_Blow", 50}, {"V_Affricate", 21}, {"V_Lip_Open", 17}, {"A02_Brow_Down_Left", 9 }, {"A03_Brow_Down_Right", 9 }, {"A08_Eye_Look_Down_Left", 12 }, {"A09_Eye_Look_Down_Right", 12 }, {"A10_Eye_Look_Out_Left", 17 }, {"A12_Eye_Look_In_Right", 17 }, {"A14_Eye_Blink_Left", 13 }, {"A15_Eye_Blink_Right", 13 }, {"A20_Cheek_Puff", 3 }, {"A25_Jaw_Open", 2 }, {"A28_Jaw_Right", 4 }, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { { "V_None", 5} } }
            }
        },
        {6,  new Dictionary<string, Dictionary<string, float>>()
            { // i sound in tin/big/miss
                 { "body", new Dictionary<string, float>() { {"V_Open", 10}, {"V_Wide", 4}, {"V_Lip_Open", 60}, {"A02_Brow_Down_Left", 9 }, {"A03_Brow_Down_Right", 9 }, {"A08_Eye_Look_Down_Left", 12 }, {"A09_Eye_Look_Down_Right", 12 }, {"A10_Eye_Look_Out_Left", 17 }, {"A12_Eye_Look_In_Right", 17 }, {"A14_Eye_Blink_Left", 13 }, {"A15_Eye_Blink_Right", 13 }, {"A20_Cheek_Puff", 3 }, {"A25_Jaw_Open", 2 }, {"A28_Jaw_Right", 4 }, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { { "V_None", 10} } }
            }
        },
        {7,  new Dictionary<string, Dictionary<string, float>>()
            { // w sound in woman/wazzup
                { "body", new Dictionary<string, float>() { {"V_Open", 5}, {"V_Tight_O", 33}, {"V_Lip_Open", 33}, {"A02_Brow_Down_Left", 9 }, {"A03_Brow_Down_Right", 9 }, {"A08_Eye_Look_Down_Left", 12 }, {"A09_Eye_Look_Down_Right", 12 }, {"A10_Eye_Look_Out_Left", 17 }, {"A12_Eye_Look_In_Right", 17 }, {"A14_Eye_Blink_Left", 13 }, {"A15_Eye_Blink_Right", 13 }, {"A20_Cheek_Puff", 3 }, {"A25_Jaw_Open", 2 }, {"A28_Jaw_Right", 4 }, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { { "V_None", 15} } }
            }
        },
        {8,  new Dictionary<string, Dictionary<string, float>>()
            { // o sound in joke/photo
                { "body", new Dictionary<string, float>() { { "V_Open", 4}, {"V_Tight_O", 34}, {"A02_Brow_Down_Left", 9 }, {"A03_Brow_Down_Right", 9 }, {"A08_Eye_Look_Down_Left", 12 }, {"A09_Eye_Look_Down_Right", 12 }, {"A10_Eye_Look_Out_Left", 17 }, {"A12_Eye_Look_In_Right", 17 }, {"A14_Eye_Blink_Left", 13 }, {"A15_Eye_Blink_Right", 13 }, {"A20_Cheek_Puff", 3 }, {"A25_Jaw_Open", 2 }, {"A28_Jaw_Right", 4 }, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { { "V_None", 20} } }
            }
        },
        {9,  new Dictionary<string, Dictionary<string, float>>()
            { // ow sound in cow/about
             { "body", new Dictionary<string, float>() { {"V_Open", 4}, {"V_Tight_O", 34}, {"V_Lip_Open", 70}, {"A02_Brow_Down_Left", 9 }, {"A03_Brow_Down_Right", 9 }, {"A08_Eye_Look_Down_Left", 12 }, {"A09_Eye_Look_Down_Right", 12 }, {"A10_Eye_Look_Out_Left", 17 }, {"A12_Eye_Look_In_Right", 17 }, {"A14_Eye_Blink_Left", 13 }, {"A15_Eye_Blink_Right", 13 }, {"A20_Cheek_Puff", 3 }, {"A25_Jaw_Open", 2 }, {"A28_Jaw_Right", 4 }, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { { "V_None", 30} } }
            }
        },
        {10, new Dictionary<string, Dictionary<string, float>>()
            { // oy sound in boy/join
               { "body", new Dictionary<string, float>() { {"V_Open", 4}, {"V_Tight_O", 34}, {"V_Lip_Open", 70}, {"A02_Brow_Down_Left", 9 }, {"A03_Brow_Down_Right", 9 }, {"A08_Eye_Look_Down_Left", 12 }, {"A09_Eye_Look_Down_Right", 12 }, {"A10_Eye_Look_Out_Left", 17 }, {"A12_Eye_Look_In_Right", 17 }, {"A14_Eye_Blink_Left", 13 }, {"A15_Eye_Blink_Right", 13 }, {"A20_Cheek_Puff", 3 }, {"A25_Jaw_Open", 2 }, {"A28_Jaw_Right", 4 }, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { { "V_None", 20 } } }
            }
        },
        {11, new Dictionary<string, Dictionary<string, float>>()
            { // ai sound in try/lie
                { "body", new Dictionary<string, float>() { {"V_Open", 44}, {"V_Lip_Open", 28}, {"A02_Brow_Down_Left", 9 }, {"A03_Brow_Down_Right", 9 }, {"A08_Eye_Look_Down_Left", 12 }, {"A09_Eye_Look_Down_Right", 12 }, {"A10_Eye_Look_Out_Left", 17 }, {"A12_Eye_Look_In_Right", 17 }, {"A14_Eye_Blink_Left", 13 }, {"A15_Eye_Blink_Right", 13 }, {"A20_Cheek_Puff", 3 }, {"A25_Jaw_Open", 2 }, {"A28_Jaw_Right", 4 }, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { { "V_None", 30} } }
            }
        },
        {12, new Dictionary<string, Dictionary<string, float>>()
            {
                 { "body", new Dictionary<string, float>() { {"V_Open", 18}, {"V_Wide", 29}, {"V_Lip_Open", 51}, {"A02_Brow_Down_Left", 9 }, {"A03_Brow_Down_Right", 9 }, {"A08_Eye_Look_Down_Left", 12 }, {"A09_Eye_Look_Down_Right", 12 }, {"A10_Eye_Look_Out_Left", 17 }, {"A12_Eye_Look_In_Right", 17 }, {"A14_Eye_Blink_Left", 13 }, {"A15_Eye_Blink_Right", 13 }, {"A20_Cheek_Puff", 3 }, {"A25_Jaw_Open", 2 }, {"A28_Jaw_Right", 4 }, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { { "V_None", 30} } }
            }
        },
        {13, new Dictionary<string, Dictionary<string, float>>()
            { // the classic r sound in real/far
                 { "body", new Dictionary<string, float>() { { "V_Open", 9 }, { "V_Tight_O", 22 }, { "V_Tight", 14 }, { "V_Affricate", 35 }, { "V_Lip_Open", 8 }, {"A02_Brow_Down_Left", 9 }, {"A03_Brow_Down_Right", 9 }, {"A08_Eye_Look_Down_Left", 12 }, {"A09_Eye_Look_Down_Right", 12 }, {"A10_Eye_Look_Out_Left", 17 }, {"A12_Eye_Look_In_Right", 17 }, {"A14_Eye_Blink_Left", 13 }, {"A15_Eye_Blink_Right", 13 }, {"A20_Cheek_Puff", 3 }, {"A25_Jaw_Open", 2 }, {"A28_Jaw_Right", 4 }, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { { "V_None", 10 } } }
            }
        },
        {14, new Dictionary<string, Dictionary<string, float>>()
            { // l sound in love/liberty
               { "body", new Dictionary<string, float>() { { "V_Open", 19 }, { "V_Lip_Open", 48 }, {"A02_Brow_Down_Left", 9 }, {"A03_Brow_Down_Right", 9 }, {"A08_Eye_Look_Down_Left", 12 }, {"A09_Eye_Look_Down_Right", 12 }, {"A10_Eye_Look_Out_Left", 17 }, {"A12_Eye_Look_In_Right", 17 }, {"A14_Eye_Blink_Left", 13 }, {"A15_Eye_Blink_Right", 13 }, {"A20_Cheek_Puff", 3 }, {"A25_Jaw_Open", 2 }, {"A28_Jaw_Right", 4 }, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { { "V_None", 5 } } }
                // TODO: add tongue
            }
        },
        {15, new Dictionary<string, Dictionary<string, float>>()
            { // z/s sound in sound/zone/nose
              { "body", new Dictionary<string, float>() { { "V_Open", 2 }, { "V_Wide", 12 }, { "V_Lip_Open", 71 }, {"A02_Brow_Down_Left", 9 }, {"A03_Brow_Down_Right", 9 }, {"A08_Eye_Look_Down_Left", 12 }, {"A09_Eye_Look_Down_Right", 12 }, {"A10_Eye_Look_Out_Left", 17 }, {"A12_Eye_Look_In_Right", 17 }, {"A14_Eye_Blink_Left", 13 }, {"A15_Eye_Blink_Right", 13 }, {"A20_Cheek_Puff", 3 }, {"A25_Jaw_Open", 2 }, {"A28_Jaw_Right", 4 }, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { { "V_None", 0 } } }
            }
        },
        {16, new Dictionary<string, Dictionary<string, float>>()
            { // sh sound in sugar/shop/motion
                { "body", new Dictionary<string, float>() { { "V_Explosive", 1 }, { "V_Wide", 3 }, { "V_Affricate", 45 }, {"A02_Brow_Down_Left", 9 }, {"A03_Brow_Down_Right", 9 }, {"A08_Eye_Look_Down_Left", 12 }, {"A09_Eye_Look_Down_Right", 12 }, {"A10_Eye_Look_Out_Left", 17 }, {"A12_Eye_Look_In_Right", 17 }, {"A14_Eye_Blink_Left", 13 }, {"A15_Eye_Blink_Right", 13 }, {"A20_Cheek_Puff", 3 }, {"A25_Jaw_Open", 2 }, {"A28_Jaw_Right", 4 }, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { { "V_None", 10 } } }
            }
        },
        {17, new Dictionary<string, Dictionary<string, float>>()
            { // th sound in this/either
                { "body", new Dictionary<string, float>() { { "V_Open", 17 }, { "V_Tight_O", 8 }, { "V_Lip_Open", 45 }, {"A02_Brow_Down_Left", 9 }, {"A03_Brow_Down_Right", 9 }, {"A08_Eye_Look_Down_Left", 12 }, {"A09_Eye_Look_Down_Right", 12 }, {"A10_Eye_Look_Out_Left", 17 }, {"A12_Eye_Look_In_Right", 17 }, {"A14_Eye_Blink_Left", 13 }, {"A15_Eye_Blink_Right", 13 }, {"A20_Cheek_Puff", 3 }, {"A25_Jaw_Open", 2 }, {"A28_Jaw_Right", 4 }, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { { "V_None", 5 } } }
                // TODO: add tongue
            }
        },
        {18, new Dictionary<string, Dictionary<string, float>>()
            { // f/v sound in very/furry/finance/fiends
              { "body", new Dictionary<string, float>() { { "V_Open", 2 }, { "V_Dental_Lip", 34 }, { "V_Lip_Open", 10 }, {"A02_Brow_Down_Left", 9 }, {"A03_Brow_Down_Right", 9 }, {"A08_Eye_Look_Down_Left", 12 }, {"A09_Eye_Look_Down_Right", 12 }, {"A10_Eye_Look_Out_Left", 17 }, {"A12_Eye_Look_In_Right", 17 }, {"A14_Eye_Blink_Left", 13 }, {"A15_Eye_Blink_Right", 13 }, {"A20_Cheek_Puff", 3 }, {"A25_Jaw_Open", 2 }, {"A28_Jaw_Right", 4 }, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { { "V_None", 10 } } }
            }
        },
        {19, new Dictionary<string, Dictionary<string, float>>()
            { // d/t sound in date/mate/deliberate
               { "body", new Dictionary<string, float>() { { "V_Open", 6 }, { "V_Tight_O", 14 }, { "V_Lip_Open", 33 }, {"A02_Brow_Down_Left", 9 }, {"A03_Brow_Down_Right", 9 }, {"A08_Eye_Look_Down_Left", 12 }, {"A09_Eye_Look_Down_Right", 12 }, {"A10_Eye_Look_Out_Left", 17 }, {"A12_Eye_Look_In_Right", 17 }, {"A14_Eye_Blink_Left", 13 }, {"A15_Eye_Blink_Right", 13 }, {"A20_Cheek_Puff", 3 }, {"A25_Jaw_Open", 2 }, {"A28_Jaw_Right", 4 }, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { { "V_None", 15 } } }
            }
        },
        {20, new Dictionary<string, Dictionary<string, float>>()
            { // k/g sound in kick/cake/gag
               { "body", new Dictionary<string, float>() { { "V_Open", 21 }, { "V_Lip_Open", 54 }, {"A02_Brow_Down_Left", 9 }, {"A03_Brow_Down_Right", 9 }, {"A08_Eye_Look_Down_Left", 12 }, {"A09_Eye_Look_Down_Right", 12 }, {"A10_Eye_Look_Out_Left", 17 }, {"A12_Eye_Look_In_Right", 17 }, {"A14_Eye_Blink_Left", 13 }, {"A15_Eye_Blink_Right", 13 }, {"A20_Cheek_Puff", 3 }, {"A25_Jaw_Open", 2 }, {"A28_Jaw_Right", 4 }, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { { "V_None", 15 } } }
            }
        },
        {21, new Dictionary<string, Dictionary<string, float>>()
            {
                { "body", new Dictionary<string, float>() { { "V_Explosive", 25 }, { "V_Wide", 3}, { "V_Lip_Open", 13}, { "A03_Brow_Down_Right", 9}, { "A08_Eye_Look_Down_Left", 12}, { "A09_Eye_Look_Down_Right", 12}, { "A10_Eye_Look_Out_Left", 17}, { "A12_Eye_Look_In_Right", 17}, { "A14_Eye_Blink_Left", 13}, { "A15_Eye_Blink_Right", 13}, { "A20_Cheek_Puff", 3}, { "A25_Jaw_Open", 2}, { "A28_Jaw_Right", 4}, {"A30_Mouth_Pucker", 7 }, {"A37_Mouth_Close", 2 }, {"A46_Mouth_Lower_Down_Left", 1 }, {"A47_Mouth_Lower_Down_Right", 1 } } },
                { "teeth", new Dictionary<string, float>() { } }
            }
        },
    };
    public static Dictionary<string, int> BlendShapeMap = new Dictionary<string, int>();

    private void Start()
    {
        //// save default clips
        //var clips = this.avatarAnimator.runtimeAnimatorController.animationClips;
        //this.defaultIdleAnimation = clips[0];
        //this.defaultTalkAnimation = clips[1];

        //// override avatar animation so clips can be swapped out
        //this.overrideController = new AnimatorOverrideController(this.avatarAnimator.runtimeAnimatorController);
        //this.avatarAnimator.runtimeAnimatorController = this.overrideController;

        // Get skinned mesh renderers
        Debug.Log(avatarAnimator.transform.Find("CC_Base_Body").name);
        Debug.Log(avatarAnimator.transform.Find("CC_Base_Body").GetComponent<SkinnedMeshRenderer>());
        // Debug.Log(this.skinnedMeshRends["body"]);
        this.skinnedMeshRends.Add("body", avatarAnimator.transform.Find("CC_Base_Body").GetComponent<SkinnedMeshRenderer>());
        this.skinnedMeshRends.Add("teeth", avatarAnimator.transform.Find("CC_Base_Teeth").GetComponent<SkinnedMeshRenderer>());
        this.skinnedMeshRends.Add("tongue", avatarAnimator.transform.Find("CC_Base_Tongue").GetComponent<SkinnedMeshRenderer>());

        // Create blendshape mapping
        for (int i = 0; i < this.skinnedMeshRends["body"].sharedMesh.blendShapeCount; i++)
        {
            if (BlendShapeMap.ContainsKey(this.skinnedMeshRends["body"].sharedMesh.GetBlendShapeName(i)))
            {
                BlendShapeMap[this.skinnedMeshRends["body"].sharedMesh.GetBlendShapeName(i)] = i;
                Debug.LogWarning($"[Streamer] Duplicate blendshape name: {this.skinnedMeshRends["body"].sharedMesh.GetBlendShapeName(i)}. Overwriting...");
            }
            else
                BlendShapeMap.Add(this.skinnedMeshRends["body"].sharedMesh.GetBlendShapeName(i), i);
        }
    }

    public void PlayVisemes(Viseme[] visemes)
    {
        StartCoroutine(impl_PlayVisemes(visemes));
    }

    private IEnumerator impl_PlayVisemes(Viseme[] visemes)
    {
        // blend to first viseme
        // TODO: wait until audio is downloaded
        yield return BlendToViseme(visemes[0].viseme, visemes[0].time);

        // blend to each viseme over time
        for (int i = 1; i < visemes.Length; i++)
        {
            yield return BlendToViseme(visemes[i].viseme, visemes[i].time - visemes[i - 1].time);
        }
    }

    private IEnumerator BlendToViseme(int viseme, float duration)
    {
        // Linear Interpolation between blendkey dicts
        var startTime = Time.time;
        // get start condition
        var startKeys = new Dictionary<string, Dictionary<string, float>>();
        foreach (string key in this.skinnedMeshRends.Keys)
            startKeys.Add(key, GetBlendkeyFrom(this.skinnedMeshRends[key]));

        // interpolate
        while (Time.time - startTime <= duration)
        {
            // t for lerp
            float t = (Time.time - startTime) / duration;
            t = Mathf.Min(t, 1);

            foreach (string key in this.skinnedMeshRends.Keys)
            {
                // check if viseme uses this SkinnedMeshRenderer
                if (VisemeMap[viseme].ContainsKey(key))
                {
                    // Lerp and apply blendshape values
                    var T = LerpBlendkey(startKeys[key], VisemeMap[viseme][key], t);
                    ApplyBlendkey(T, this.skinnedMeshRends[key]);

                    // TODO: handle jaw movement - blendshapes don't move teeth. Using rig.
                    // FIXME: must be done in animation loop
                    //if (key.Equals("teeth"))
                    //{
                    //    avatarAnimator.SetBoneLocalRotation(HumanBodyBones.Jaw, Quaternion.Euler(-180, 0, VisemeMap[viseme][key]["V_None"]));
                    //}
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Applies a dictionary of blendshapes to a SkinnedMeshRenderer
    /// </summary>
    /// <param name="blendkey">the blendshapes and their values to apply</param>
    /// <param name="smr">the SkinnedMeshRenderer to apply the blendshape values to</param>
    private void ApplyBlendkey(Dictionary<string, float> blendkey, SkinnedMeshRenderer smr)
    {
        // Go through each blendshape and set its value as needed
        for (int i = 0; i < smr.sharedMesh.blendShapeCount; i++)
        {
            string name = smr.sharedMesh.GetBlendShapeName(i);
            // set blendshape if it exists in the key
            if (blendkey.ContainsKey(name))
            {
                smr.SetBlendShapeWeight(i, blendkey[name]);
            }
        }
    }

    /// <summary>
    /// Linear Interpolation between two dicts of blendkeys
    /// If either dictionary is missing a key, will lerp to 0
    /// </summary>
    /// <param name="A">0 or start value</param>
    /// <param name="B">1 or end value</param>
    /// <param name="t">[0, 1] percentage to interpolate to</param>
    /// <returns></returns>
    private Dictionary<string, float> LerpBlendkey(Dictionary<string, float> A, Dictionary<string, float> B, float t)
    {
        // bound t to [0, 1] in case of bad input
        t = Mathf.Max(0, Mathf.Min(t, 1));

        // Add missing keys to dictionaries
        foreach (string key in A.Keys)
            if (!B.ContainsKey(key)) B.Add(key, 0);
        foreach (string key in B.Keys)
            if (!A.ContainsKey(key)) A.Add(key, 0);

        // Lerp through each key
        Dictionary<string, float> ret = new Dictionary<string, float>();
        foreach (string key in A.Keys)
        {
            ret.Add(key, Mathf.Lerp(A[key], B[key], t));
        }

        return ret;
    }

    /// <summary>
    /// Gets all of the blendshapes in a SkinnedMeshRenderer
    /// Puts them in a dictionary with their name as the key,
    /// and their blendshape value as the dict value
    /// </summary>
    /// <param name="smr">The SkinnedMeshRenderer to parse</param>
    /// <returns></returns>
    private Dictionary<string, float> GetBlendkeyFrom(SkinnedMeshRenderer smr)
    {
        Dictionary<string, float> blendkey = new Dictionary<string, float>();
        for (int i = 0; i < smr.sharedMesh.blendShapeCount; i++)
        {
            //// ignore 0 values
            //if (smr.GetBlendShapeWeight(i) != 0)
            blendkey.Add(smr.sharedMesh.GetBlendShapeName(i), smr.GetBlendShapeWeight(i));
        }
        return blendkey;
    }

    [System.Serializable]
    public struct Viseme
    {
        public float time;
        public int viseme;
    }

    internal struct BlendKey
    {
        //public float Basis;
        public float V_Open;
        public float V_Explosive;
        public float V_Dental_Lip;
        public float V_Tight_O;
        public float V_Tight;
        public float V_Wide;
        public float V_Affricate;
        public float V_Lip_Open;
        public float Brow_Raise_Inner_Left;
        public float Brow_Raise_Inner_Right;
        public float Brow_Raise_Outer_Left;
        public float Brow_Raise_Outer_Right;
        public float Brow_Drop_Left;
        public float Brow_Drop_Right;
        public float Brow_Raise_Left;
        public float Brow_Raise_Right;
        public float Eyes_Blink;
        public float Eye_Blink_L;
        public float Eye_Blink_R;
        public float Eye_Wide_L;
        public float Eye_Wide_R;
        public float Eye_Squint_L;
        public float Eye_Squint_R;
        public float Nose_Scrunch;
        public float Nose_Flanks_Raise;
        public float Nose_Flank_Raise_L;
        public float Nose_Flank_Raise_R;
        public float Nose_Nostrils_Flare;
        public float Cheek_Raise_L;
        public float Cheek_Raise_R;
        public float Cheeks_Suck;
        public float Cheek_Blow_L;
        public float Cheek_Blow_R;
        public float Mouth_Smile;
        public float Mouth_Smile_L;
        public float Mouth_Smile_R;
        public float Mouth_Frown;
        public float Mouth_Frown_L;
        public float Mouth_Frown_R;
        public float Mouth_Blow;
        public float Mouth_Pucker;
        public float Mouth_Pucker_Open;
        public float Mouth_Widen;
        public float Mouth_Widen_Sides;
        public float Mouth_Dimple_L;
        public float Mouth_Dimple_R;
        public float Mouth_Plosive;
        public float Mouth_Lips_Tight;
        public float Mouth_Lips_Tuck;
        public float Mouth_Lips_Open;
        public float Mouth_Lips_Part;
        public float Mouth_Bottom_Lip_Down;
        public float Mouth_Top_Lip_Up;
        public float Mouth_Top_Lip_Under;
        public float Mouth_Bottom_Lip_Under;
        public float Mouth_Snarl_Upper_L;
        public float Mouth_Snarl_Upper_R;
        public float Mouth_Snarl_Lower_L;
        public float Mouth_Snarl_Lower_R;
        public float Mouth_Bottom_Lip_Bite;
        public float Mouth_Down;
        public float Mouth_Up;
        public float Mouth_L;
        public float Mouth_R;
        public float Mouth_Lips_Jaw_Adjust;
        public float Mouth_Bottom_Lip_Trans;
        public float Mouth_Skewer;
        public float Mouth_Open;

        public float A01_Brow_Inner_Up;
        public float A02_Brow_Down_Left;
        public float A03_Brow_Down_Right;
        public float A04_Brow_Outer_Up_Left;
        public float A05_Brow_Outer_Up_Right;
        public float A06_Eye_Look_Up_Left;
        public float A07_Eye_Look_Up_Right;
        public float A08_Eye_Look_Down_Left;
        public float A09_Eye_Look_Down_Right;
        public float A10_Eye_Look_Out_Left;
        public float A11_Eye_Look_In_Left;
        public float A12_Eye_Look_In_Right;
        public float A13_Eye_Look_Out_Right;
        public float A14_Eye_Blink_Left;
        public float A15_Eye_Blink_Right;
        public float A16_Eye_Squint_Left;
        public float A17_Eye_Squint_Right;
        public float A18_Eye_Wide_Left;
        public float A19_Eye_Wide_Right;
        public float A20_Cheek_Puff;
        public float A21_Cheek_Squint_Left;
        public float A22_Cheek_Squint_Right;
        public float A23_Nose_Sneer_Left;
        public float A24_Nose_Sneer_Right;
        public float A25_Jaw_Open;
        public float A26_Jaw_Forward;
        public float A27_Jaw_Left;
        public float A28_Jaw_Right;
        public float A29_Mouth_Funnel;
        public float A30_Mouth_Pucker;
        public float A31_Mouth_Left;
        public float A32_Mouth_Right;
        public float A33_Mouth_Roll_Upper;
        public float A34_Mouth_Roll_Lower;
        public float A35_Mouth_Shrug_Upper;
        public float A36_Mouth_Shrug_Lower;
        public float A37_Mouth_Close;
        public float A38_Mouth_Smile_Left;
        public float A39_Mouth_Smile_Right;
        public float A40_Mouth_Frown_Left;
        public float A41_Mouth_Frown_Right;
        public float A42_Mouth_Dimple_Left;
        public float A43_Mouth_Dimple_Right;
        public float A44_Mouth_Upper_Up_Left;
        public float A45_Mouth_Upper_Up_Right;
        public float A46_Mouth_Lower_Down_Left;
        public float A47_Mouth_Lower_Down_Right;
        public float A48_Mouth_Press_Left;
        public float A49_Mouth_Press_Right;
        public float A50_Mouth_Stretch_Left;
        public float A51_Mouth_Stretch_Right;

        public float T10_Tongue_Bulge_Left;
        public float T11_Tongue_Bulge_Right;


        public BlendKey(SkinnedMeshRenderer smr)
        {
            // Get the names of all of the blendshapes
            Dictionary<string, int> shapeMap = new Dictionary<string, int>();
            for (int i = 0; i < smr.sharedMesh.blendShapeCount; i++)
            {
                shapeMap.Add(smr.sharedMesh.GetBlendShapeName(i), i);
            }

            //this.Basis = smr.sharedMesh.GetBlendShapeFrameWeight(shapeMap["Basis"], 0);
            this.V_Open = smr.GetBlendShapeWeight(shapeMap["V_Open"]);
            this.V_Explosive = smr.GetBlendShapeWeight(shapeMap["V_Explosive"]);
            this.V_Dental_Lip = smr.GetBlendShapeWeight(shapeMap["V_Dental_Lip"]);
            this.V_Tight_O = smr.GetBlendShapeWeight(shapeMap["V_Tight_O"]);
            this.V_Tight = smr.GetBlendShapeWeight(shapeMap["V_Tight"]);
            this.V_Wide = smr.GetBlendShapeWeight(shapeMap["V_Wide"]);
            this.V_Affricate = smr.GetBlendShapeWeight(shapeMap["V_Affricate"]);
            this.V_Lip_Open = smr.GetBlendShapeWeight(shapeMap["V_Lip_Open"]);
            this.Brow_Raise_Inner_Left = smr.GetBlendShapeWeight(shapeMap["Brow_Raise_Inner_Left"]);
            this.Brow_Raise_Inner_Right = smr.GetBlendShapeWeight(shapeMap["Brow_Raise_Inner_Right"]);
            this.Brow_Raise_Outer_Left = smr.GetBlendShapeWeight(shapeMap["Brow_Raise_Outer_Left"]);
            this.Brow_Raise_Outer_Right = smr.GetBlendShapeWeight(shapeMap["Brow_Raise_Outer_Right"]);
            this.Brow_Drop_Left = smr.GetBlendShapeWeight(shapeMap["Brow_Drop_Left"]);
            this.Brow_Drop_Right = smr.GetBlendShapeWeight(shapeMap["Brow_Drop_Right"]);
            this.Brow_Raise_Left = smr.GetBlendShapeWeight(shapeMap["Brow_Raise_Left"]);
            this.Brow_Raise_Right = smr.GetBlendShapeWeight(shapeMap["Brow_Raise_Right"]);
            this.Eyes_Blink = smr.GetBlendShapeWeight(shapeMap["Eyes_Blink"]);
            this.Eye_Blink_L = smr.GetBlendShapeWeight(shapeMap["Eye_Blink_L"]);
            this.Eye_Blink_R = smr.GetBlendShapeWeight(shapeMap["Eye_Blink_R"]);
            this.Eye_Wide_L = smr.GetBlendShapeWeight(shapeMap["Eye_Wide_L"]);
            this.Eye_Wide_R = smr.GetBlendShapeWeight(shapeMap["Eye_Wide_R"]);
            this.Eye_Squint_L = smr.GetBlendShapeWeight(shapeMap["Eye_Squint_L"]);
            this.Eye_Squint_R = smr.GetBlendShapeWeight(shapeMap["Eye_Squint_R"]);
            this.Nose_Scrunch = smr.GetBlendShapeWeight(shapeMap["Nose_Scrunch"]);
            this.Nose_Flanks_Raise = smr.GetBlendShapeWeight(shapeMap["Nose_Flanks_Raise"]);
            this.Nose_Flank_Raise_L = smr.GetBlendShapeWeight(shapeMap["Nose_Flank_Raise_L"]);
            this.Nose_Flank_Raise_R = smr.GetBlendShapeWeight(shapeMap["Nose_Flank_Raise_R"]);
            this.Nose_Nostrils_Flare = smr.GetBlendShapeWeight(shapeMap["Nose_Nostrils_Flare"]);
            this.Cheek_Raise_L = smr.GetBlendShapeWeight(shapeMap["Cheek_Raise_L"]);
            this.Cheek_Raise_R = smr.GetBlendShapeWeight(shapeMap["Cheek_Raise_R"]);
            this.Cheeks_Suck = smr.GetBlendShapeWeight(shapeMap["Cheeks_Suck"]);
            this.Cheek_Blow_L = smr.GetBlendShapeWeight(shapeMap["Cheek_Blow_L"]);
            this.Cheek_Blow_R = smr.GetBlendShapeWeight(shapeMap["Cheek_Blow_R"]);
            this.Mouth_Smile = smr.GetBlendShapeWeight(shapeMap["Mouth_Smile"]);
            this.Mouth_Smile_L = smr.GetBlendShapeWeight(shapeMap["Mouth_Smile_L"]);
            this.Mouth_Smile_R = smr.GetBlendShapeWeight(shapeMap["Mouth_Smile_R"]);
            this.Mouth_Frown = smr.GetBlendShapeWeight(shapeMap["Mouth_Frown"]);
            this.Mouth_Frown_L = smr.GetBlendShapeWeight(shapeMap["Mouth_Frown_L"]);
            this.Mouth_Frown_R = smr.GetBlendShapeWeight(shapeMap["Mouth_Frown_R"]);
            this.Mouth_Blow = smr.GetBlendShapeWeight(shapeMap["Mouth_Blow"]);
            this.Mouth_Pucker = smr.GetBlendShapeWeight(shapeMap["Mouth_Pucker"]);
            this.Mouth_Pucker_Open = smr.GetBlendShapeWeight(shapeMap["Mouth_Pucker_Open"]);
            this.Mouth_Widen = smr.GetBlendShapeWeight(shapeMap["Mouth_Widen"]);
            this.Mouth_Widen_Sides = smr.GetBlendShapeWeight(shapeMap["Mouth_Widen_Sides"]);
            this.Mouth_Dimple_L = smr.GetBlendShapeWeight(shapeMap["Mouth_Dimple_L"]);
            this.Mouth_Dimple_R = smr.GetBlendShapeWeight(shapeMap["Mouth_Dimple_R"]);
            this.Mouth_Plosive = smr.GetBlendShapeWeight(shapeMap["Mouth_Plosive"]);
            this.Mouth_Lips_Tight = smr.GetBlendShapeWeight(shapeMap["Mouth_Lips_Tight"]);
            this.Mouth_Lips_Tuck = smr.GetBlendShapeWeight(shapeMap["Mouth_Lips_Tuck"]);
            this.Mouth_Lips_Open = smr.GetBlendShapeWeight(shapeMap["Mouth_Lips_Open"]);
            this.Mouth_Lips_Part = smr.GetBlendShapeWeight(shapeMap["Mouth_Lips_Part"]);
            this.Mouth_Bottom_Lip_Down = smr.GetBlendShapeWeight(shapeMap["Mouth_Bottom_Lip_Down"]);
            this.Mouth_Top_Lip_Up = smr.GetBlendShapeWeight(shapeMap["Mouth_Top_Lip_Up"]);
            this.Mouth_Top_Lip_Under = smr.GetBlendShapeWeight(shapeMap["Mouth_Top_Lip_Under"]);
            this.Mouth_Bottom_Lip_Under = smr.GetBlendShapeWeight(shapeMap["Mouth_Bottom_Lip_Under"]);
            this.Mouth_Snarl_Upper_L = smr.GetBlendShapeWeight(shapeMap["Mouth_Snarl_Upper_L"]);
            this.Mouth_Snarl_Upper_R = smr.GetBlendShapeWeight(shapeMap["Mouth_Snarl_Upper_R"]);
            this.Mouth_Snarl_Lower_L = smr.GetBlendShapeWeight(shapeMap["Mouth_Snarl_Lower_L"]);
            this.Mouth_Snarl_Lower_R = smr.GetBlendShapeWeight(shapeMap["Mouth_Snarl_Lower_R"]);
            this.Mouth_Bottom_Lip_Bite = smr.GetBlendShapeWeight(shapeMap["Mouth_Bottom_Lip_Bite"]);
            this.Mouth_Down = smr.GetBlendShapeWeight(shapeMap["Mouth_Down"]);
            this.Mouth_Up = smr.GetBlendShapeWeight(shapeMap["Mouth_Up"]);
            this.Mouth_L = smr.GetBlendShapeWeight(shapeMap["Mouth_L"]);
            this.Mouth_R = smr.GetBlendShapeWeight(shapeMap["Mouth_R"]);
            this.Mouth_Lips_Jaw_Adjust = smr.GetBlendShapeWeight(shapeMap["Mouth_Lips_Jaw_Adjust"]);
            this.Mouth_Bottom_Lip_Trans = smr.GetBlendShapeWeight(shapeMap["Mouth_Bottom_Lip_Trans"]);
            this.Mouth_Skewer = smr.GetBlendShapeWeight(shapeMap["Mouth_Skewer"]);
            this.Mouth_Open = smr.GetBlendShapeWeight(shapeMap["Mouth_Open"]);

            this.A01_Brow_Inner_Up = smr.GetBlendShapeWeight(shapeMap["A01_Brow_Inner_Up"]);
            this.A02_Brow_Down_Left = smr.GetBlendShapeWeight(shapeMap["A02_Brow_Down_Left"]);
            this.A03_Brow_Down_Right = smr.GetBlendShapeWeight(shapeMap["A03_Brow_Down_Right"]);
            this.A04_Brow_Outer_Up_Left = smr.GetBlendShapeWeight(shapeMap["A04_Brow_Outer_Up_Left"]);
            this.A05_Brow_Outer_Up_Right = smr.GetBlendShapeWeight(shapeMap["A05_Brow_Outer_Up_Right"]);
            this.A06_Eye_Look_Up_Left = smr.GetBlendShapeWeight(shapeMap["A06_Eye_Look_Up_Left"]);
            this.A07_Eye_Look_Up_Right = smr.GetBlendShapeWeight(shapeMap["A07_Eye_Look_Up_Right"]);
            this.A08_Eye_Look_Down_Left = smr.GetBlendShapeWeight(shapeMap["A08_Eye_Look_Down_Left"]);
            this.A09_Eye_Look_Down_Right = smr.GetBlendShapeWeight(shapeMap["A09_Eye_Look_Down_Right"]);
            this.A10_Eye_Look_Out_Left = smr.GetBlendShapeWeight(shapeMap["A10_Eye_Look_Out_Left"]);
            this.A11_Eye_Look_In_Left = smr.GetBlendShapeWeight(shapeMap["A11_Eye_Look_In_Left"]);
            this.A12_Eye_Look_In_Right = smr.GetBlendShapeWeight(shapeMap["A12_Eye_Look_In_Right"]);
            this.A13_Eye_Look_Out_Right = smr.GetBlendShapeWeight(shapeMap["A13_Eye_Look_Out_Right"]);
            this.A14_Eye_Blink_Left = smr.GetBlendShapeWeight(shapeMap["A14_Eye_Blink_Left"]);
            this.A15_Eye_Blink_Right = smr.GetBlendShapeWeight(shapeMap["A15_Eye_Blink_Right"]);
            this.A16_Eye_Squint_Left = smr.GetBlendShapeWeight(shapeMap["A16_Eye_Squint_Left"]);
            this.A17_Eye_Squint_Right = smr.GetBlendShapeWeight(shapeMap["A17_Eye_Squint_Right"]);
            this.A18_Eye_Wide_Left = smr.GetBlendShapeWeight(shapeMap["A18_Eye_Wide_Left"]);
            this.A19_Eye_Wide_Right = smr.GetBlendShapeWeight(shapeMap["A19_Eye_Wide_Right"]);
            this.A20_Cheek_Puff = smr.GetBlendShapeWeight(shapeMap["A20_Cheek_Puff"]);
            this.A21_Cheek_Squint_Left = smr.GetBlendShapeWeight(shapeMap["A21_Cheek_Squint_Left"]);
            this.A22_Cheek_Squint_Right = smr.GetBlendShapeWeight(shapeMap["A22_Cheek_Squint_Right"]);
            this.A23_Nose_Sneer_Left = smr.GetBlendShapeWeight(shapeMap["A23_Nose_Sneer_Left"]);
            this.A24_Nose_Sneer_Right = smr.GetBlendShapeWeight(shapeMap["A24_Nose_Sneer_Right"]);
            this.A25_Jaw_Open = smr.GetBlendShapeWeight(shapeMap["A25_Jaw_Open"]);
            this.A26_Jaw_Forward = smr.GetBlendShapeWeight(shapeMap["A26_Jaw_Forward"]);
            this.A27_Jaw_Left = smr.GetBlendShapeWeight(shapeMap["A27_Jaw_Left"]);
            this.A28_Jaw_Right = smr.GetBlendShapeWeight(shapeMap["A28_Jaw_Right"]);
            this.A29_Mouth_Funnel = smr.GetBlendShapeWeight(shapeMap["A29_Mouth_Funnel"]);
            this.A30_Mouth_Pucker = smr.GetBlendShapeWeight(shapeMap["A30_Mouth_Pucker"]);
            this.A31_Mouth_Left = smr.GetBlendShapeWeight(shapeMap["A31_Mouth_Left"]);
            this.A32_Mouth_Right = smr.GetBlendShapeWeight(shapeMap["A32_Mouth_Right"]);
            this.A33_Mouth_Roll_Upper = smr.GetBlendShapeWeight(shapeMap["A33_Mouth_Roll_Upper"]);
            this.A34_Mouth_Roll_Lower = smr.GetBlendShapeWeight(shapeMap["A34_Mouth_Roll_Lower"]);
            this.A35_Mouth_Shrug_Upper = smr.GetBlendShapeWeight(shapeMap["A35_Mouth_Shrug_Upper"]);
            this.A36_Mouth_Shrug_Lower = smr.GetBlendShapeWeight(shapeMap["A36_Mouth_Shrug_Lower"]);
            this.A37_Mouth_Close = smr.GetBlendShapeWeight(shapeMap["A37_Mouth_Close"]);
            this.A38_Mouth_Smile_Left = smr.GetBlendShapeWeight(shapeMap["A38_Mouth_Smile_Left"]);
            this.A39_Mouth_Smile_Right = smr.GetBlendShapeWeight(shapeMap["A39_Mouth_Smile_Right"]);
            this.A40_Mouth_Frown_Left = smr.GetBlendShapeWeight(shapeMap["A40_Mouth_Frown_Left"]);
            this.A41_Mouth_Frown_Right = smr.GetBlendShapeWeight(shapeMap["A41_Mouth_Frown_Right"]);
            this.A42_Mouth_Dimple_Left = smr.GetBlendShapeWeight(shapeMap["A42_Mouth_Dimple_Left"]);
            this.A43_Mouth_Dimple_Right = smr.GetBlendShapeWeight(shapeMap["A43_Mouth_Dimple_Right"]);
            this.A44_Mouth_Upper_Up_Left = smr.GetBlendShapeWeight(shapeMap["A44_Mouth_Upper_Up_Left"]);
            this.A45_Mouth_Upper_Up_Right = smr.GetBlendShapeWeight(shapeMap["A45_Mouth_Upper_Up_Right"]);
            this.A46_Mouth_Lower_Down_Left = smr.GetBlendShapeWeight(shapeMap["A46_Mouth_Lower_Down_Left"]);
            this.A47_Mouth_Lower_Down_Right = smr.GetBlendShapeWeight(shapeMap["A47_Mouth_Lower_Down_Right"]);
            this.A48_Mouth_Press_Left = smr.GetBlendShapeWeight(shapeMap["A48_Mouth_Press_Left"]);
            this.A49_Mouth_Press_Right = smr.GetBlendShapeWeight(shapeMap["A49_Mouth_Press_Right"]);
            this.A50_Mouth_Stretch_Left = smr.GetBlendShapeWeight(shapeMap["A50_Mouth_Stretch_Left"]);
            this.A51_Mouth_Stretch_Right = smr.GetBlendShapeWeight(shapeMap["A51_Mouth_Stretch_Right"]);

            this.T10_Tongue_Bulge_Left = smr.GetBlendShapeWeight(shapeMap["T10_Tongue_Bulge_Left"]);
            this.T11_Tongue_Bulge_Right = smr.GetBlendShapeWeight(shapeMap["T11_Tongue_Bulge_Right"]);
        }

        public static BlendKey Lerp(BlendKey A, BlendKey B, float t, SkinnedMeshRenderer smr = null)
        {
            BlendKey T = new BlendKey();
            var props = typeof(BlendKey).GetFields();
            for (int i = 0; i < props.Length; i++)
            {
                float a = (float)props[i].GetValue(A);
                float b = (float)props[i].GetValue(B);
                float value = Mathf.Lerp(a, b, t);
                props[i].SetValue(T, value);

                // Set blendshapes if given a renderer
                if (smr != null)
                {
                    smr.SetBlendShapeWeight(Streamer.BlendShapeMap[props[i].Name], Mathf.Max(0, Mathf.Min(value, 100)));
                }
            }
            return T;
        }

        public void ApplyTo(SkinnedMeshRenderer smr)
        {
            var props = typeof(BlendKey).GetFields();
            for (int i = 0; i < props.Length; i++)
            {
                float value = (float)props[i].GetValue(this);
                smr.SetBlendShapeWeight(Streamer.BlendShapeMap[props[i].Name], Mathf.Max(0, Mathf.Min(value, 100)));
            }
        }
    }
}

public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
{
    public AnimationClipOverrides(int capacity) : base(capacity) { }
    public AnimationClip this[string name]
    {
        get
        {
            return this.Find(x => x.Key.name.Equals(name)).Value;
        }
        set
        {
            int index = this.FindIndex(x => x.Key.name.Equals(name));
            if (index != -1)
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
        }
    }
}