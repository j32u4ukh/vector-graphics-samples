using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VectorGraphics;
using System.IO;

public class Test : MonoBehaviour
{
    VectorUtils.TessellationOptions tessellation;
    List<VectorUtils.Geometry> geometries;
    SpriteRenderer render;

    private void Awake()
    {
        tessellation = new VectorUtils.TessellationOptions()
        {
            StepDistance = 100.0f,
            MaxCordDeviation = 0.5f,
            MaxTanAngleDeviation = 0.1f,
            SamplingStepSize = 0.01f
        };

        render = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        string path = string.Format(@"D:\WriteByHand\svgs\{0}.svg", 20986);
        string svg = SVGHelper.readSVG(path);
        SVGParser.SceneInfo scene_info = SVGParser.ImportSVG(new StringReader(svg));
        Scene scene = scene_info.Scene;

        // svg 本體
        SceneNode node = scene.Root;
        List<SceneNode> layer1 = node.Children;

        // node0：背景米字；node1：字的筆劃
        SceneNode node0 = layer1[0], node1 = layer1[1];

        // 背景米字
        //List<SceneNode> layer2 = node0.Children;
        //print("layer2:" + layer2.Count);

        // 字的筆劃
        List<SceneNode> layer3 = node1.Children;
        //print("layer3:" + layer3.Count);
        int n = 0;
        foreach (SceneNode scene_node in layer3)
        {
            print(string.Format("=== Node {0} ===", ++n));
            List<Shape> shapes = scene_node.Shapes; // length = 1
            Shape shape = shapes[0]; // length = 1
            BezierContour[] contours = shape.Contours; // length = 1
            BezierPathSegment[] segments = contours[0].Segments; // 每一筆劃的區段數量不同
            int num = 0;
            foreach(BezierPathSegment bezierPathSegment in segments)
            {
                print(string.Format("= segment {0} =", ++num));
                print(bezierPathSegment.P0);
                print(bezierPathSegment.P1);
                print(bezierPathSegment.P2);
                break;
            }

            //Matrix2D matrix2 = shape.FillTransform;
            //PathProperties path_properties = shape.PathProps;
            //Stroke stroke = path_properties.Stroke;

            SceneNode clipper = (scene_node.Clipper == null) ? null : scene_node.Clipper;
            if(clipper != null)
            {
                List<SceneNode> clippers = clipper.Children; // length = 1
                SceneNode mask = clippers[0];
                List<Shape> m_shapes = mask.Shapes; // length = 1
                Shape m_shape = m_shapes[0];
                BezierContour[] m_contours = m_shape.Contours; // length = 1
                //print("m_contours len:" + m_contours.Length);
                BezierPathSegment[] m_segments = m_contours[0].Segments; // 每一筆劃的區段數量不同
                //print("m_segments len:" + m_segments.Length);
                int number = 0;
                foreach (BezierPathSegment m_bezierPathSegment in m_segments)
                {
                    print(string.Format("= clipper segment {0} =", ++number));
                    print(m_bezierPathSegment.P0);
                    print(m_bezierPathSegment.P1);
                    print(m_bezierPathSegment.P2);
                    break;
                }
            }
            else
            {
                print("clipper is null");
            }
        }

        sceneDisplay(scene, render);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void sceneDisplay(Scene scene, SpriteRenderer render)
    {
        List<VectorUtils.Geometry> geoms = VectorUtils.TessellateScene(scene, tessellation);

        // Build a sprite with the tessellated geometry.
        Sprite sprite = VectorUtils.BuildSprite(
            geoms, 
            10.0f, 
            VectorUtils.Alignment.Center, 
            Vector2.zero, 
            128, 
            true);

        render.sprite = sprite;
    }
}
