using UnityEngine;
using System.Collections;

public abstract class BaseGridGenerator 
{
    public Mesh CreateMesh(float w, float h, int splitX, int splitY)
    {
        Mesh mesh = CreateBase(splitX, splitY);

        float halfWidth = w * 0.5f;
        float halfHeight = h * 0.5f;

        int vertexCount = mesh.vertexCount;
        for (int i = 0; i < vertexCount; ++i) 
        {
            Vector3 prev = mesh.vertices[i];
            mesh.vertices[i] = new Vector3(prev.x * halfWidth, prev.y * halfHeight, prev.z);
        }
        return mesh;
    }

    /*
     * [-1,1] 영역의 grid mesh 만들어내는 함수
     */
    public abstract Mesh CreateBase(int splitX, int splitY);
}

public class UniformGridGenerator : BaseGridGenerator
{
    public override Mesh CreateBase(int splitX, int splitY)
    {
        /*
         * vertex position
         * 
         * 2(splitX+1) ...
         * splitX+1 splitX+2 ...
         * 0    1   2   3   4 ..
         */

        Mesh mesh = new Mesh();
        
        int vertexCount = (splitX + 1) * (splitY + 1);

        // set vertices
        float gridVertexWidth = 2.0f / splitX;
        float gridVertexHeight = 2.0f / splitY;

        Vector3[] vertices = new Vector3[vertexCount];
        for (int j = 0; j <= splitY; ++j)
        {
            for (int i = 0; i <= splitX; ++i)
            {
                int idx = i + j * (splitX + 1);
                float x = (gridVertexWidth * i) - 1.0f;
                float y = (gridVertexHeight * j) - 1.0f;
                Vector3 vert = new Vector3(x, y, 0);
                vertices[idx] = vert;
            }
        }
        mesh.vertices = vertices;

        // set triangles
        int[] tris = new int[splitX * splitY * 6];
		for (int j = 0; j < splitY; ++j)
		{
            for (int i = 0; i < splitX; ++i)
            {
                int baseIndex = (i + j * splitX) * 6;

                int leftBottom = i + j * (splitX + 1);
                int rightBottom = leftBottom + 1;
                int leftTop = i + (j + 1) * (splitX + 1);
                int rightTop = leftTop + 1;

                tris[baseIndex + 0] = leftBottom;
                tris[baseIndex + 1] = rightBottom;
                tris[baseIndex + 2] = leftTop;

                tris[baseIndex + 3] = rightBottom;
                tris[baseIndex + 4] = rightTop;
                tris[baseIndex + 5] = leftTop;
            }
		}
        mesh.triangles = tris;


        // set normal
        Vector3[] normals = new Vector3[vertexCount];
        for(int i = 0 ; i < vertexCount ; ++i) {
            normals[i] = -Vector3.forward;
        }
        mesh.normals = normals;

        // set uv
        float gridTexcoordWidth = 1.0f / splitX;
        float gridTexcoordHeight = 1.0f / splitY;

        Vector2[] uv = new Vector2[vertexCount];
        for (int j = 0 ; j <= splitY; ++j)
        {
            for (int i = 0; i <= splitX; ++i)
            {
                int idx = i + j * (splitX + 1);
                float s = i * gridTexcoordWidth;
                float t = j * gridTexcoordHeight;
                Vector2 texcoord = new Vector2(s, t);
                uv[idx] = texcoord;
            }
        }
        mesh.uv = uv;


        return mesh;
    }
}
/*
public class NonUniformGridGenerator : BaseGridGenerator
{
    public Mesh CreateBase(int splitX, int splitY)
    {
        Mesh mesh = new Mesh();
        return mesh;
    }
}
*/