using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;

public interface ICoordConverter 
{
	/*
	 * [-1,1]의 일정 간격의 값을 적절한 vertex 좌표용 값으로 변환하는 함수
	 */
	float Convert (float val);
}

public class UniformCoordConverter : ICoordConverter 
{
	public float Convert (float val) 
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

public class NonUniformCoordConverter : ICoordConverter 
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

	public float Convert (float val) 
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

public class BarrelDistortion
{
	// Parameters from the Oculus Rift DK1
	float hResolution = 1280;
	float vResolution = 800;
	float hScreenSize = 0.14976f;
	float vScreenSize = 0.0936f;
	float interpupillaryDistance = 0.064f;
	float lensSeparationDistance = 0.064f;
	float eyeToScreenDistance = 0.041f;
	//hmdWarpParam == distortionK
	float[] distortionK = { 1.0f, 0.22f, 0.24f, 0.0f };

	Vector2 scale = new Vector2(1.0f, 1.0f);
	Vector2 scaleIn = new Vector2(1.0f, 1.0f);
	Vector2 lensCenter = new Vector2(0.0f, 0.0f);

	public BarrelDistortion()
	{
		// set screen info
		hResolution = Screen.width;
		vResolution = Screen.height;

		// Compute aspect ratio and FOV
		float aspect = hResolution / (2.0f * vResolution);

		// Fov is normally computed with:
		//   2*atan2(HMD.vScreenSize,2*HMD.eyeToScreenDistance)
		// But with lens distortion it is increased (see Oculus SDK Documentation)
		float r = -1.0f - (4.0f * (hScreenSize/4.0f - lensSeparationDistance/2.0f) / hScreenSize);
		float distScale = (distortionK[0] + distortionK[1] * Mathf.Pow(r,2) + distortionK[2] * Mathf.Pow(r,4) + distortionK[3] * Mathf.Pow(r,6));
		float fov = 2.0f*Mathf.Atan2(vScreenSize*distScale, 2.0f*eyeToScreenDistance);
		
		// Distortion shader parameters
		float lensShift = 4.0f * (hScreenSize/4.0f - lensSeparationDistance/2.0f) / hScreenSize;
		//for left
		lensCenter.x = lensShift;

		//for right
		//lensCenter.x = -lensShift;


		scale.x = 1.0f/distScale;
		scale.y = 1.0f*aspect/distScale;
		
		scaleIn.x = 1.0f;
		scaleIn.y = 1.0f/aspect;

		//_renderTexture = _driver->addRenderTargetTexture(dimension2d<u32>(HMD.hResolution*distScale/2.0f, HMD.vResolution*distScale));

	}

	public Vector2 Distort(Vector2 p)
	{

		const float BarrelPower = 0.7f;
		//http://github.prideout.net/barrel-distortion/
		float theta  = Mathf.Atan2(p.y, p.x);
		float radius = p.magnitude;
		radius = Mathf.Pow(radius, BarrelPower);
		p.x = radius * Mathf.Cos(theta);
		p.y = radius * Mathf.Sin(theta);
		//return 0.5f * (p + 1.0f);
		return p;

		/*
		//vec2 theta = (uv-lensCenter)*scaleIn;
		Vector2 theta = new Vector2 ();
		{
			Vector2 tmp = p - lensCenter;
			theta.x = tmp.x * scaleIn.x;
			theta.y = tmp.y * scaleIn.y;
		}

		//float rSq = theta.x*theta.x + theta.y*theta.y;
		float rSq = theta.x*theta.x + theta.y*theta.y;

		//vec2 rvector = theta*(hmdWarpParam.x + hmdWarpParam.y*rSq + hmdWarpParam.z*rSq*rSq + hmdWarpParam.w*rSq*rSq*rSq);
		Vector2 rvector = theta*(distortionK[0] + distortionK[1]*rSq + distortionK[2]*rSq*rSq + distortionK[3]*rSq*rSq*rSq);

		//vec2 tc = (lensCenter + scale * rvector);
		Vector2 tc = lensCenter + new Vector2(scale.x * rvector.x, scale.y * rvector.y);

		//tc = (tc+1.0)/2.0; // range from [-1,1] to [0,1]
		//vertex 기반으로 변환하기 떄문에 텍스쳐 좌표로 바꿀 필요 

		//if (any(bvec2(clamp(tc, vec2(0.0,0.0), vec2(1.0,1.0))-tc)))
		Vector2 tmptc = new Vector2 (tc.x, tc.y);
		if (tmptc.x < 0)
		{
			tmptc.x = 0;
		}
		else if (tmptc.x > 1.0f)
		{
			tmptc.x = 1.0f;
		}

		if (tmptc.y < 0)
		{
			tmptc.y = 0;
		}
		else if (tmptc.y > 1.0f)
		{
			tmptc.y = 1.0f;
		}
		return tmptc - tc;
*/

	}
}

public class GridGenerator 
{
	ICoordConverter coordConverter;

	public GridGenerator(ICoordConverter converter) {
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

        // set vertices / texcoord
        float gridVertexWidth = 2.0f / splitX;
        float gridVertexHeight = 2.0f / splitY;

        Vector3[] vertices = new Vector3[vertexCount];
		Vector2[] uv = new Vector2[vertexCount];
		BarrelDistortion distortion = new BarrelDistortion ();

        for (int j = 0; j <= splitY; ++j)
        {
            for (int i = 0; i <= splitX; ++i)
            {
                int idx = i + j * (splitX + 1);
                float x = (gridVertexWidth * i) - 1.0f;
                float y = (gridVertexHeight * j) - 1.0f;

				float posX = coordConverter.Convert(x);
				float posY = coordConverter.Convert(y);

				float u = (posX * 0.5f) + 0.5f;
				float v = (posY * 0.5f) + 0.5f;
				Vector2 texcoord = new Vector2(u, v);
				uv[idx] = texcoord;


				Vector2 pos = distortion.Distort(new Vector2(posX, posY));
                Vector3 vert = new Vector3(pos.x, pos.y, 0);
				//Vector3 vert = new Vector3(posX, posY, 0);
                vertices[idx] = vert;


            }
        }

        mesh.vertices = vertices;
		mesh.uv = uv;

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
                tris[baseIndex + 2] = rightBottom;
                tris[baseIndex + 1] = leftTop;

                tris[baseIndex + 3] = rightBottom;
                tris[baseIndex + 5] = rightTop;
                tris[baseIndex + 4] = leftTop;
            }
		}
        mesh.triangles = tris;


        // set normal
        Vector3[] normals = new Vector3[vertexCount];
        for(int i = 0 ; i < vertexCount ; ++i) {
            normals[i] = -Vector3.forward;
        }
        mesh.normals = normals;

        return mesh;
    }
}