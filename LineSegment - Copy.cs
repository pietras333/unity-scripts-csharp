using System.Collections.Generic;
using UnityEngine;

public class LineSegment : MonoBehaviour
{
    #region Inspector Fields

    [Header("Line Segment")]
    [Tooltip("Index of this line segment in the overall line.")]
    [SerializeField]
    private int lineSegmentIndex;

    [Tooltip("List of all line segments that this segment is connected to.")]
    [SerializeField]
    private List<GameObject> thisLineSegments = new List<GameObject>();

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets or sets the index of this line segment in the overall line.
    /// </summary>
    public int LineSegmentIndex
    {
        get => lineSegmentIndex;
        set => lineSegmentIndex = value;
    }

    /// <summary>
    /// Gets or sets the list of all line segments that this segment is connected to.
    /// </summary>
    public List<GameObject> ThisLineSegments
    {
        get => thisLineSegments;
        set => thisLineSegments = value;
    }

    #endregion
}
