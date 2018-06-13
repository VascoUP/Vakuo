using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class InvalidBezierPointsException : Exception
{
    public string message;
    
    public InvalidBezierPointsException(String message)
    {
        this.message = message;
    }
}


[ExecuteInEditMode]
public class BezierGenerator : MonoBehaviour {
    private class BezierCalc
    {
        private BezierGenerator _generator;
        private int _planeArea;
        private int _trianglesArea;
        private float _step;
        private double[,] _bernsteinStepCalc;

        private Vector3[] _vertices;
        private Vector2[] _uvs;
        private int[] _triangles;

        public BezierCalc(BezierGenerator generator)
        {
            _generator = generator;
        }

        private void CalculateVariables()
        {
            _trianglesArea = (int)Mathf.Pow(_generator.resolution, 2);
            _planeArea = (int)Mathf.Pow(_generator.resolution + 1, 2);
            _step = 1f / (float)_generator.resolution;

            int size = (_generator.uOrder > _generator.vOrder ? _generator.uOrder : _generator.vOrder) + 1;
            if(_bernsteinStepCalc == null || _bernsteinStepCalc.Length != size)
            {
                _bernsteinStepCalc = new double[size, size];
                for(int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        _bernsteinStepCalc[i, j] = BernsteinStepCalc(i, j);
                    }
                }
            }
        }
        
        // Bernstein Algorithm:
        // Sum(v,i)( Sum(u,j)( Point(uPosition,vPosition) * BezierPolynomial(u,i,uPosition) * BezierPolynomial(v,j,vPosition) ) )
        private Vector3 BernsteinPolynomialSum(int v, int u, float vPosition, float uPosition)
        {
            Vector3 result = Vector3.zero;
            int index = 0;
            // Iterate through control points
            for (int i = 0; i <= v; i++)
            {
                for (int j = 0; j <= u; j++)
                {
                    result +=
                        // Position of control point
                        _generator.controlPoints[index++] *
                        // Necessary bernstein calculations
                        (float)(BernsteinPolynomial(u, j, uPosition) * BernsteinPolynomial(v, i, vPosition));
                }
            }
            return result;
        }

        private double BernsteinStepCalc(int order, int step)
        {
            double ans = 1f;
            if (order == 0 || step == 0)
                return ans;

            int j = order;

            for (var i = 1; i <= step; j--, i++)
            {
                ans *= j / (float)i;
            }

            return ans;
        }

        private double BernsteinPolynomial(int order, int step, float position)
        {
            return _bernsteinStepCalc[order, step] * Mathf.Pow(position, step) * Mathf.Pow(1f - position, order - step);
        }


        private void GenerateMeshPoints()
        {
            // Initialize only if needed
            if(_vertices == null || _vertices.Length != _planeArea)
            {
                _vertices = new Vector3[_planeArea];
            }
            if(_uvs == null || _uvs.Length != _planeArea)
            {
                _uvs = new Vector2[_planeArea];
            }

            // Step over each vertex
            float vFac = 0, uFac = 0;
            for (int i = 0; i < _planeArea; i++)
            {
                _vertices[i] = BernsteinPolynomialSum(_generator.vOrder, _generator.uOrder, vFac, uFac);
                _uvs[i] = new Vector2(uFac, vFac);

                if ((i + 1) % (_generator.resolution + 1) == 0)
                {
                    uFac = 0;
                    vFac += _step;
                }
                else
                {
                    uFac += _step;
                }
            }
        }

        private void GenerateTriangles()
        {
            int resolution = _generator.resolution + 1;
            int nTriangles = _trianglesArea * 2 * 3;
            if (_triangles == null || _triangles.Length != nTriangles)
            {
                _triangles = new int[nTriangles];
            }

            nTriangles = 0;
            for (int v = 0; v < _generator.resolution; v++)
            {
                for (int u = 0; u < _generator.resolution; u++)
                {
                    //TODO: Double sided not working
                    if (_generator.drawMode == DrawMode.COUNTER_CLOCK)
                    {
                        _triangles[nTriangles++] = DoubleCoordinatesToIndex(u, v, resolution);
                        _triangles[nTriangles++] = DoubleCoordinatesToIndex(u + 1, v, resolution);
                        _triangles[nTriangles++] = DoubleCoordinatesToIndex(u, v + 1, resolution);

                        _triangles[nTriangles++] = DoubleCoordinatesToIndex(u + 1, v, resolution);
                        _triangles[nTriangles++] = DoubleCoordinatesToIndex(u + 1, v + 1, resolution);
                        _triangles[nTriangles++] = DoubleCoordinatesToIndex(u, v + 1, resolution);
                    }
                    else if (_generator.drawMode == DrawMode.CLOCK)
                    {
                        _triangles[nTriangles++] = DoubleCoordinatesToIndex(u, v + 1, resolution);
                        _triangles[nTriangles++] = DoubleCoordinatesToIndex(u + 1, v, resolution);
                        _triangles[nTriangles++] = DoubleCoordinatesToIndex(u, v, resolution);

                        _triangles[nTriangles++] = DoubleCoordinatesToIndex(u, v + 1, resolution);
                        _triangles[nTriangles++] = DoubleCoordinatesToIndex(u + 1, v + 1, resolution);
                        _triangles[nTriangles++] = DoubleCoordinatesToIndex(u + 1, v, resolution);
                    }
                }
            }
        }

        public Mesh GenerateMesh()
        {
            CalculateVariables();
            GenerateMeshPoints();
            GenerateTriangles();

            Mesh mesh = new Mesh
            {
                vertices = _vertices,
                triangles = _triangles,
                uv = _uvs
            };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            mesh.MarkDynamic();

            return mesh;
        }
    }

    private BezierCalc _bezierCalc;

    private Mesh _mesh;
    private MeshFilter _meshFilter;
    private MeshCollider _meshCollider;

    public enum DrawMode { CLOCK, COUNTER_CLOCK };
    public DrawMode drawMode;
    [HideInInspector]
    public bool reDraw = false;

    [HideInInspector]
    public int resolution = 20;
    public int uOrder = 3;
    public int vOrder = 3;
    public List<Vector3> controlPoints;
        
    private void OnEnable()
    {
        _bezierCalc = new BezierCalc(this);
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();
        _mesh = null;
        
        ApplyMesh();
    }

    public bool IsValid()
    {
        return controlPoints != null && controlPoints.Count == (uOrder + 1) * (vOrder + 1);
    }

    public static int DoubleCoordinatesToIndex(int u, int v, int uv_Width)
    {
        return u + v * uv_Width;
    }

    public static Vector2 IndexToDoubleCoordinates(int i, int uv_Width)
    {
        return new Vector2(i % uv_Width, Mathf.Floor(i / uv_Width));
    }

    private void ApplyMesh()
    {
        _mesh = _bezierCalc.GenerateMesh();
        _meshFilter.sharedMesh = _mesh;
        if(_meshCollider != null)
        {
            _meshCollider.sharedMesh = _mesh;
        }
    }

    private void Update()
    {
        if (reDraw && IsValid())
        {
            ApplyMesh();
        }
    }
}
