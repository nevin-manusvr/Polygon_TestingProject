using UnityEngine;
using UnityEditor.Animations;

public class RecordTransformHierarchy : MonoBehaviour
{
    public AnimationClip clip;

    private GameObjectRecorder m_Recorder;

    public bool record = false;

    void Start()
    {
        // Create recorder and record the script GameObject.
        m_Recorder = new GameObjectRecorder(gameObject);

        // Bind all the Transforms on the GameObject and all its children.
        m_Recorder.BindComponentsOfType<Transform>(gameObject, true);
    }

    void LateUpdate()
    {
        if (clip == null)
            return;

        if(record)
        {
            // Take a snapshot and record all the bindings values for this frame.
            m_Recorder.TakeSnapshot(Time.deltaTime);
        }
        else if (m_Recorder.isRecording)
        {
            m_Recorder.SaveToClip(clip);
            m_Recorder.ResetRecording();
        }
    }
}