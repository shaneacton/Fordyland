using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DebugImage
{

    public static void DrawLine(Texture2D tex, Vector2 p1, Vector2 p2, Color col)
    {
        //Debug.Log("drawing line from " + p1 + " to " + p2);
        Vector2 t = p1;
        float frac = 1 / Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));
        float ctr = 0;

        while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y)
        {
            t = Vector2.Lerp(p1, p2, ctr);
            ctr += frac;
            tex.SetPixel((int)t.x, (int)t.y, col);
        }
        tex.Apply();
    }

    public static void run()
    {
        Texture2D tex = new Texture2D(1000, 1000);
        DrawLine(tex, new Vector2(0, 0), new Vector2(512, 256), Color.red);
        byte[] image = tex.EncodeToJPG();

        File.WriteAllBytes(Application.dataPath + "/Debug/Errors/SavedScreen.png", image);
    }

    internal static void drawWalk(Walk walk, Face face, Shape shape, Texture2D image = null)
    {
        if (image == null)
        {
            image = new Texture2D(1000, 1000);
        }
        Vector u = face.getU();
        Vector v = face.getV(shape);

        for (int i = 0; i < walk.vertCount; i++)
        {
            int nextId = (i + 1) % walk.vertCount;
            //Debug.Log("drawing walk line: " + new IndexLine(walk[i], walk[nextId]));
            drawIndexLine(shape, image, u, v, new IndexLine(walk[i], walk[nextId]), Color.green);
        }
        saveTexture(image, "walk lines");
    }

    public static Texture2D drawFaceLines(Face face, Shape shape, Texture2D image=null)
    {
        if (image == null)
        {
            image = new Texture2D(1000, 1000);
        }
        Vector u = face.getU();
        Vector v = face.getV(shape);

        //Debug.Log("U: " + u + " v: " + v);

        foreach (IndexLine line in face.lines)
        {
            drawIndexLine(shape, image, u, v, line, Color.red);
        }
        saveTexture(image, "face lines");
        return image;
    }

    private static void saveTexture(Texture2D image, string fileName)
    {
        byte[] imageRaw = image.EncodeToJPG();

        File.WriteAllBytes(Application.dataPath + "/Debug/Errors/"+fileName + ".jpg", imageRaw);
    }

    private static void drawIndexLine(Shape shape, Texture2D image, Vector u, Vector v, IndexLine line, Color colour)
    {
        RealLine realLine = line.getRealLine(shape);
        Vector2[] points = new Vector2[2];
        for (int i = 0; i < 2; i++)
        {
            Vector point = realLine[i];
            float x = point.dot(u) * 15 + 500;
            float y = point.dot(v) * 15 + 500;
            //Debug.Log("point: " + point + " x: " + x + " y: " + y);
            points[i] = new Vector2(x, y);
        }
        DrawLine(image, points[0], points[1], colour);
    }
}
