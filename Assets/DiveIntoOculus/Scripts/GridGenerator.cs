using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;

public abstract class BaseCoordConverter 
{
	/*
	 * [-1,1]의 일정 간격의 값을 적절한 vertex 좌표용 값으로 변환하는 함수
	 */
	public abstract float ConvertVertexPos (float val);
	/*
	 * [0,1]의 일정한 간격의 값을 적절한 texcoord coord 값으로 변환하는 함수
	 */
	public float ConvertTexcoordCoord(float val)
	{
		float t = (val * 2.0f) - 1.0f;
		return ConvertVertexPos (t);
	}
}

public class UniformCoordConverter : BaseCoordConverter 
{
	public override float ConvertVertexPos (float val) 
	{
		return val;
	}
}

public enum NonUniformEaseType {
	Expo,
	Quartic,
	Cubic,
	Quad,
	Circular
}

public class NonUniformCoordConverter : BaseCoordConverter 
{
	/*
	 * https://raw.githubusercontent.com/prideout/distortion/master/media/NonuniformGrid.png
	 * 와 같이 가운데로 갈수록 밀집되는 grid를 생성하는데 이용한다
	 * out과 in을 동시에 사용하는 이유는 가운데 구간만 밀집 시키기 위해서이다.
	 * 
	 * http://easings.net/
	 * http://www.robertpenner.com/easing/
	 */
	public NonUniformEaseType easeType;
	public NonUniformCoordConverter(NonUniformEaseType easeType)
	{
		this.easeType = easeType;
	}

	public override float ConvertVertexPos (float val) 
	{
		if (val < 0) {
			float curr = val + 1.0f;
			float startValue = -1.0f;
			float length = 1.0f;

			switch(easeType) {
			case NonUniformEaseType.Expo:
				return Ease.ExpoEaseOut(curr, startValue, length);
			case NonUniformEaseType.Quartic:
				return Ease.QuartEaseOut(curr, startValue, length);
			case NonUniformEaseType.Cubic:
				return Ease.CubicEaseOut(curr, startValue, length);
			case NonUniformEaseType.Quad:
				return Ease.QuadEaseOut(curr, startValue, length);
			case NonUniformEaseType.Circular:
				return Ease.CircEaseOut(curr, startValue, length);
			default:
				return -1;
			}

		} else {
			float curr = val;
			float startValue = 0.0f;
			float length = 1.0f;

			switch(easeType) {
			case NonUniformEaseType.Expo:
				return Ease.ExpoEaseIn(curr, startValue, length);
			case NonUniformEaseType.Quartic:
				return Ease.QuartEaseIn(curr, startValue, length);
			case NonUniformEaseType.Cubic:
				return Ease.CubicEaseIn(curr, startValue, length);
			case NonUniformEaseType.Quad:
				return Ease.QuadEaseIn(curr, startValue, length);
			case NonUniformEaseType.Circular:
				return Ease.CircEaseIn(curr, startValue, length);
			default:
				return -1;
			}
		}
	}
}

public class GridGenerator 
{
	BaseCoordConverter coordConverter;

	public GridGenerator(BaseCoordConverter converter) {
		this.coordConverter = converter;
	}

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
    public Mesh CreateBase(int splitX, int splitY)
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

				float vertX = coordConverter.ConvertVertexPos(x);
				float vertY = coordConverter.ConvertVertexPos(y);
                Vector3 vert = new Vector3(vertX, vertY, 0);
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
				float u = coordConverter.ConvertTexcoordCoord(s);
				float v = coordConverter.ConvertTexcoordCoord(t);
                Vector2 texcoord = new Vector2(u, v);
                uv[idx] = texcoord;
            }
        }
        mesh.uv = uv;


        return mesh;
    }
}