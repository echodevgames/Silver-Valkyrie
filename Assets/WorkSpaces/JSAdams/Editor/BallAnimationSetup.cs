using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// One-shot editor tool that slices pinball_SV-Sheet.png, builds three looping
/// animation clips (BallUpLeft, BallUp, BallUpRight), and creates the Ball
/// AnimatorController wired with a Direction int parameter.
/// Open via Silver Valkyrie > Setup Ball Animation.
/// </summary>
public static class BallAnimationSetup
{
    private const string SpritesheetPath = "Assets/WorkSpaces/JCDock/pngs/pinball_SV-Sheet.png";
    private const string OutputFolder    = "Assets/WorkSpaces/JSAdams/Animations";
    private const string ControllerPath  = OutputFolder + "/Ball.controller";

    private const int FramesPerClip  = 4;
    private const int AnimFrameRate  = 12; // fps — raise for faster ball feel

    // Rows are ordered top-to-bottom as Unity slices them: UpLeft, Up, UpRight
    private static readonly (string name, int startIndex)[] Clips =
    {
        ("BallUpLeft",  0),
        ("BallUp",      4),
        ("BallUpRight", 8),
    };

    [MenuItem("Silver Valkyrie/Setup Ball Animation")]
    public static void Setup()
    {
        // ── Slice the spritesheet ────────────────────────────────────────────
        var importer = AssetImporter.GetAtPath(SpritesheetPath) as TextureImporter;
        if (importer == null)
        {
            Debug.LogError($"[BallAnimationSetup] Could not find TextureImporter at {SpritesheetPath}");
            return;
        }

        importer.spriteImportMode = SpriteImportMode.Multiple;

        // Build the grid slice metadata (4 cols × 3 rows, 48×48 each)
        var sheet = new SpriteMetaData[12];
        int col = 0, row = 0;
        for (int i = 0; i < 12; i++)
        {
            col = i % 4;
            row = i / 4;  // row 0 = top in Unity texture space means y=96
            sheet[i] = new SpriteMetaData
            {
                name      = $"pinball_SV-Sheet_{i}",
                rect      = new Rect(col * 48, (2 - row) * 48, 48, 48), // flip Y: Unity tex origin is bottom-left
                pivot     = new Vector2(0.5f, 0.5f),
                alignment = 9 // custom pivot
            };
        }

        importer.spritesheet = sheet;
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();

        AssetDatabase.Refresh();

        // ── Load sprites ─────────────────────────────────────────────────────
        Sprite[] sprites = AssetDatabase.LoadAllAssetsAtPath(SpritesheetPath)
            .OfType<Sprite>()
            .OrderBy(s =>
            {
                // Sort by embedded index: "pinball_SV-Sheet_7" → 7
                string[] parts = s.name.Split('_');
                return int.TryParse(parts[parts.Length - 1], out int idx) ? idx : 0;
            })
            .ToArray();

        if (sprites.Length < 12)
        {
            Debug.LogError($"[BallAnimationSetup] Expected 12 sprites, found {sprites.Length}. Aborting.");
            return;
        }

        // ── Ensure output folder exists ───────────────────────────────────────
        if (!AssetDatabase.IsValidFolder(OutputFolder))
            AssetDatabase.CreateFolder("Assets/WorkSpaces/JSAdams", "Animations");

        // ── Build animation clips ─────────────────────────────────────────────
        AnimationClip[] animClips = new AnimationClip[Clips.Length];
        for (int i = 0; i < Clips.Length; i++)
            animClips[i] = CreateClip(Clips[i].name, sprites, Clips[i].startIndex);

        // ── Build animator controller ─────────────────────────────────────────
        CreateController(animClips);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[BallAnimationSetup] Ball.controller and animation clips created in Assets/WorkSpaces/JSAdams/Animations/");
    }

    private static AnimationClip CreateClip(string clipName, Sprite[] sprites, int startIndex)
    {
        string path = $"{OutputFolder}/{clipName}.anim";
        AssetDatabase.DeleteAsset(path);

        var clip = new AnimationClip { frameRate = AnimFrameRate };

        var binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");

        // FramesPerClip keyframes + 1 closing keyframe for a seamless loop
        var keyframes = new ObjectReferenceKeyframe[FramesPerClip + 1];
        for (int i = 0; i < FramesPerClip; i++)
        {
            keyframes[i] = new ObjectReferenceKeyframe
            {
                time  = i / (float)AnimFrameRate,
                value = sprites[startIndex + i]
            };
        }
        keyframes[FramesPerClip] = new ObjectReferenceKeyframe
        {
            time  = FramesPerClip / (float)AnimFrameRate,
            value = sprites[startIndex] // loops back to first frame
        };

        AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);

        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        AssetDatabase.CreateAsset(clip, path);
        return clip;
    }

    private static void CreateController(AnimationClip[] clips)
    {
        AssetDatabase.DeleteAsset(ControllerPath);
        var controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);

        // Int parameter: 0 = UpLeft, 1 = Up, 2 = UpRight
        controller.AddParameter("Direction", AnimatorControllerParameterType.Int);

        var sm = controller.layers[0].stateMachine;

        // Create states
        var states = new AnimatorState[clips.Length];
        for (int i = 0; i < clips.Length; i++)
        {
            states[i]        = sm.AddState(Clips[i].name, new Vector3(260, i * 80 - 80, 0));
            states[i].motion = clips[i];
            states[i].writeDefaultValues = false;
        }

        sm.defaultState     = states[1]; // BallUp
        sm.anyStatePosition = new Vector3(-20, 0, 0);

        // Any State → each direction, instant and no exit time
        for (int i = 0; i < states.Length; i++)
        {
            var t = sm.AddAnyStateTransition(states[i]);
            t.AddCondition(AnimatorConditionMode.Equals, i, "Direction");
            t.duration            = 0f;
            t.hasExitTime         = false;
            t.canTransitionToSelf = false;
        }
    }
}
