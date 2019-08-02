using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.VectorGraphics;
using System.IO;

//[ExecuteInEditMode]
public class Spline1 : MonoBehaviour
{
    public Transform[] controlPoints;

    private Scene m_Scene;
    private Shape m_Path;
    private VectorUtils.TessellationOptions m_Options;
    private Mesh m_Mesh;

    Scene display_scene;
    public float pen_width = 10f;

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
                Stroke = new Stroke() {
                    Color = Color.white,
                    HalfThickness = 0.1f
                }
            }
        };

        m_Scene = new Scene() {
            Root = new SceneNode() {
                Shapes = new List<Shape> {
                    //m_Path
                }
            }
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


        // =======================================================================
        string path = string.Format(@"D:\WriteByHand\svgs\{0}.svg", 20986);
        string svg = SVGHelper.readSVG(path);
        SVGParser.SceneInfo scene_info = SVGParser.ImportSVG(new StringReader(svg));
        Scene scene = scene_info.Scene;
        SceneNode word = scene.Root.Children[1];

        // 前半為背景(無 Clipper)，後半為寫字筆劃(有 Clipper)
        List<SceneNode> bg_and_stroke = word.Children;
        int double_stroke_number = bg_and_stroke.Count;
        int stroke_number = double_stroke_number / 2;

        // 筆劃第一筆
        SceneNode test_node = bg_and_stroke[stroke_number];
        List<Shape> test_shapes = test_node.Shapes;
        SceneNode test_clipper_node = (test_node.Clipper == null) ? null : test_node.Clipper;
        List<Shape> test_clippers = new List<Shape>();
        if (test_clipper_node != null)
        {
            test_clippers = test_clipper_node.Children[0].Shapes;
            if (test_clippers != null)
            {
                print("test_clippers len:" + test_clippers.Count);
                Shape test_clipper_shape = test_clippers[0];
            }
            else
            {
                print("test_clippers is null");
            }
        }
        else
        {
            print("test_clipper_node is null");
        }

        Shape test_stroke = test_shapes[0];
        BezierContour[] bezierContours = test_stroke.Contours;
        BezierPathSegment[] bezierPathSegments = bezierContours[0].Segments;
        BezierPathSegment point1 = bezierPathSegments[0];
        BezierPathSegment point2 = bezierPathSegments[bezierPathSegments.Length - 1];

        #region Word scene
        // 遮罩嘗試
        display_scene = new Scene()
        {
            Root = new SceneNode()
            {
                Children = new List<SceneNode>()
                {
                    #region One stroke
                    new SceneNode()
                    {
                        Shapes = new List<Shape>()
                        {
                            #region Piece of stroke
                            new Shape()
                            {
                                Contours = new BezierContour[]{
                                    new BezierContour() {
                                        Segments = new BezierPathSegment[]
                                        {
                                            point1,
                                            point2
                                        }
                                    },
                                    //new BezierContour() {
                                    //    Segments = new BezierPathSegment[2]
                                    //}
                                },
                                PathProps = new PathProperties() {
                                    Stroke = new Stroke() {
                                        Color = Color.white,
                                        HalfThickness = 10f
                                    }
                                }
                            }
                            #endregion Piece of stroke end
                        },
                        Clipper = new SceneNode()
                        {
                            Shapes = new List<Shape>()
                            {
                                #region Piece of clipper
                                test_clippers[0]
                                #endregion Piece of clipper end
                            }
                        }

                    }
                    #endregion One stroke end
                }
            }
        };
        #endregion Word scene end

        StartCoroutine(nextStroke(bg_and_stroke));
    }

    void Update()
    {
        if (m_Scene == null)
        {
            Start();
        }

        // Update the control points of the spline.
        //// line 1
        //m_Path.Contours[0].Segments[0].P0 = (Vector2)controlPoints[0].localPosition;
        //m_Path.Contours[0].Segments[0].P1 = (Vector2)controlPoints[1].localPosition;
        //m_Path.Contours[0].Segments[0].P2 = (Vector2)controlPoints[2].localPosition;
        //m_Path.Contours[0].Segments[1].P0 = (Vector2)controlPoints[3].localPosition;

        //// line 2
        //m_Path.Contours[1].Segments[0].P0 = (Vector2)controlPoints[4].localPosition;
        //m_Path.Contours[1].Segments[0].P1 = (Vector2)controlPoints[4].localPosition;
        //m_Path.Contours[1].Segments[0].P2 = (Vector2)controlPoints[4].localPosition;
        //m_Path.Contours[1].Segments[1].P0 = (Vector2)controlPoints[5].localPosition;

        // Tessellate the vector scene, and fill the mesh with the resulting geometry.
        var geoms = VectorUtils.TessellateScene(m_Scene, m_Options);
        VectorUtils.FillMesh(m_Mesh, geoms, 1.0f);
    }

    IEnumerator nextStroke(List<SceneNode> bg_and_stroke)
    {
        int stroke_number = bg_and_stroke.Count;
        int half_number = stroke_number / 2;
        int curr_stroke;
        for(curr_stroke = 0; curr_stroke < half_number; curr_stroke++)
        {
            print("curr_stroke:" + curr_stroke);
            //m_Scene.Root.Shapes.Add(bg_and_stroke[curr_stroke].Shapes[0]);
        }
        yield return new WaitForSeconds(Time.deltaTime);

        for (; curr_stroke < stroke_number; curr_stroke++)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            while (!Input.GetKeyDown(KeyCode.N))
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }
            print("curr_stroke:" + curr_stroke);
            
            m_Scene.Root.Shapes.Add(newShape(bg_and_stroke[curr_stroke].Shapes[0].Contours));
        }
    }

    Shape newShape(BezierContour[] contours)
    {
        return new Shape()
        {
            Contours = contours,
            PathProps = new PathProperties()
            {
                Stroke = new Stroke()
                {
                    Color = Color.white,
                    HalfThickness = 10f
                }
            }
        };
    }
}
