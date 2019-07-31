using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class SVGHelper : MonoBehaviour
{
    private void Start()
    {
        string path = string.Format(@"D:\WriteByHand\svgs\{0}.svg", 20986);
        string svg = readSVG(path);
        print(svg);
    }

    private void Update()
    {
        
    }

    public static string matchCSS(string svg)
    {
        string pattern = @"<style type=""text/css"">([.\s\S]*)</style>";
        Regex regex = new Regex(pattern);
        Match match = regex.Match(svg);
        string css = match.Value;

        return css;
    }

    public static string readSVG(string path, bool without_css = true)
    {
        StreamReader file = new StreamReader(path);
        string svg = file.ReadToEnd();
        svg = svg.Trim();
        file.Close();

        if (without_css)
        {
            string pattern = @"<style type=""text/css"">([.\s\S]*)</style>";
            svg = Regex.Replace(svg, pattern, "");
        }

        return svg;
    }
}
