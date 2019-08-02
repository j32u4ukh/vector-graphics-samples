﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.VectorGraphics;

[ExecuteInEditMode]
public class Spline1 : MonoBehaviour
{
    public Transform[] controlPoints;

    private Scene m_Scene;
    private Shape m_Path;
    private VectorUtils.TessellationOptions m_Options;
    private Mesh m_Mesh;

    void Start()
    {
        // Prepare the vector path, add it to the vector scene.
        m_Path = new Shape() {
            Contours = new BezierContour[]{
                new BezierContour() {
                    Segments = new BezierPathSegment[2]
                },
                new BezierContour() {
                    Segments = new BezierPathSegment[2]
                }
            },
            PathProps = new PathProperties() {
                Stroke = new Stroke() { Color = Color.white, HalfThickness = 0.1f }
            }
        };

        m_Scene = new Scene() {
            Root = new SceneNode() { Shapes = new List<Shape> { m_Path } }
        };

        m_Options = new VectorUtils.TessellationOptions() {
            StepDistance = 1000.0f,
            MaxCordDeviation = 0.05f,
            MaxTanAngleDeviation = 0.05f,
            SamplingStepSize = 0.01f
        };

        // Instantiate a new mesh, it will be filled with data in Update()
        m_Mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = m_Mesh;
    }

    void Update()
    {
        if (m_Scene == null)
        {
            Start();
        }

        // Update the control points of the spline.
        // line 1
        m_Path.Contours[0].Segments[0].P0 = (Vector2)controlPoints[0].localPosition;
        m_Path.Contours[0].Segments[0].P1 = (Vector2)controlPoints[1].localPosition;
        m_Path.Contours[0].Segments[0].P2 = (Vector2)controlPoints[2].localPosition;
        m_Path.Contours[0].Segments[1].P0 = (Vector2)controlPoints[3].localPosition;

        // line 2
        m_Path.Contours[1].Segments[0].P0 = (Vector2)controlPoints[4].localPosition;
        m_Path.Contours[1].Segments[0].P1 = (Vector2)controlPoints[4].localPosition;
        m_Path.Contours[1].Segments[0].P2 = (Vector2)controlPoints[4].localPosition;
        m_Path.Contours[1].Segments[1].P0 = (Vector2)controlPoints[5].localPosition;

        // Tessellate the vector scene, and fill the mesh with the resulting geometry.
        var geoms = VectorUtils.TessellateScene(m_Scene, m_Options);
        VectorUtils.FillMesh(m_Mesh, geoms, 1.0f);
    }
}
