﻿using System.Collections.Generic;
using UnityEngine;

public static class CUT_CuttingHelper
{
	/* CUT_CuttingHelper :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	[TO DO]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[DATE]
	 *	Author :		[NAME]
	 *
	 *	Changes :
	 *
	 *	[CHANGES]
	 *
	 *	-----------------------------------
	*/

	
	#region Methods

	#region Original Methods
    private static CUT_MeshTriangle GetTriangle(int _indexA, int _indexB, int _indexC, int _subMeshIndex, Mesh _originalMesh)
    {
        Vector3[] _vertices = new Vector3[3] { _originalMesh.vertices[_indexA], _originalMesh.vertices[_indexB], _originalMesh.vertices[_indexC] };
        Vector3[] _normals = new Vector3[3] { _originalMesh.normals[_indexA], _originalMesh.normals[_indexB], _originalMesh.normals[_indexC] };
        Vector2[] _uvs = new Vector2[3] { _originalMesh.uv[_indexA], _originalMesh.uv[_indexB], _originalMesh.uv[_indexC] };
        return new CUT_MeshTriangle(_vertices, _normals, _uvs, _subMeshIndex);
    }

    private static void CutTriangle(Plane _cuttingPlane, CUT_MeshTriangle _triangle, bool _triangleALeftSide, bool _triangleBLeftSide, bool _triangleCLeftSide, CUT_GeneratedMesh _leftSide, CUT_GeneratedMesh _rightSide, List<Vector3> _addedVertices)
    {
        List<bool> _leftSidesBool = new List<bool>();
        _leftSidesBool.Add(_triangleALeftSide);
        _leftSidesBool.Add(_triangleBLeftSide);
        _leftSidesBool.Add(_triangleCLeftSide);

        CUT_MeshTriangle _leftMeshTriangle = new CUT_MeshTriangle(new Vector3[2], new Vector3[2], new Vector2[2], _triangle.SubmeshIndex);
        CUT_MeshTriangle _rightMeshTriangle = new CUT_MeshTriangle(new Vector3[2], new Vector3[2], new Vector2[2], _triangle.SubmeshIndex);

        bool _left = false;
        bool _right = false;

        for (int i = 0; i < 3; i++)
        {
            if(_leftSidesBool[i])
            {
                if(!_left)
                {
                    _left = true;
                    _leftMeshTriangle.Vertices[0] = _triangle.Vertices[i];
                    _leftMeshTriangle.Vertices[1] = _leftMeshTriangle.Vertices[0];

                    _leftMeshTriangle.UVs[0] = _triangle.UVs[i];
                    _leftMeshTriangle.UVs[1] = _leftMeshTriangle.UVs[0];

                    _leftMeshTriangle.Normals[0] = _triangle.Normals[i];
                    _leftMeshTriangle.Normals[1] = _leftMeshTriangle.Normals[0];
                }
                else
                {
                    _leftMeshTriangle.Vertices[1] = _triangle.Vertices[i];
                    _leftMeshTriangle.Normals[1] = _triangle.Normals[i];
                    _leftMeshTriangle.UVs[1] = _triangle.UVs[i];
                }
            }
            else
            {
                if (!_right)
                {
                    _right = true;
                    _rightMeshTriangle.Vertices[0] = _triangle.Vertices[i];
                    _rightMeshTriangle.Vertices[1] = _rightMeshTriangle.Vertices[0];

                    _rightMeshTriangle.UVs[0] = _triangle.UVs[i];
                    _rightMeshTriangle.UVs[1] = _rightMeshTriangle.UVs[0];

                    _rightMeshTriangle.Normals[0] = _triangle.Normals[i];
                    _rightMeshTriangle.Normals[1] = _rightMeshTriangle.Normals[0];
                }
                else
                {
                    _rightMeshTriangle.Vertices[1] = _triangle.Vertices[i];
                    _rightMeshTriangle.Normals[1] = _triangle.Normals[i];
                    _rightMeshTriangle.UVs[1] = _triangle.UVs[i];
                }
            }
        }

        float _normalizedDistance = 0;
        float _distance = 0;

        _cuttingPlane.Raycast(new Ray(_leftMeshTriangle.Vertices[0], (_rightMeshTriangle.Vertices[0] - _leftMeshTriangle.Vertices[0]).normalized), out _distance);

        _normalizedDistance = _distance / (_rightMeshTriangle.Vertices[0] - _leftMeshTriangle.Vertices[0]).magnitude;
        Vector3 _vertexLeft = Vector3.Lerp(_leftMeshTriangle.Vertices[0], _rightMeshTriangle.Vertices[0], _normalizedDistance);
        _addedVertices.Add(_vertexLeft);
        Vector3 _normalLeft = Vector3.Lerp(_leftMeshTriangle.Normals[0], _rightMeshTriangle.Normals[0], _normalizedDistance);
        Vector2 _uvLeft = Vector2.Lerp(_leftMeshTriangle.UVs[0], _rightMeshTriangle.UVs[0], _normalizedDistance);

        _cuttingPlane.Raycast(new Ray(_leftMeshTriangle.Vertices[1], (_rightMeshTriangle.Vertices[1] - _leftMeshTriangle.Vertices[1]).normalized), out _distance);

        _normalizedDistance = _distance / (_rightMeshTriangle.Vertices[1] - _leftMeshTriangle.Vertices[1]).magnitude;
        Vector3 _vertexRight = Vector3.Lerp(_leftMeshTriangle.Vertices[1], _rightMeshTriangle.Vertices[1], _normalizedDistance);
        _addedVertices.Add(_vertexRight);
        Vector3 _normalRight = Vector3.Lerp(_leftMeshTriangle.Normals[1], _rightMeshTriangle.Normals[1], _normalizedDistance);
        Vector2 _uvRight = Vector2.Lerp(_leftMeshTriangle.UVs[1], _rightMeshTriangle.UVs[1], _normalizedDistance);

        #region LEFT
        Vector3[] updatedVertices = new Vector3[] { _leftMeshTriangle.Vertices[0], _vertexLeft, _vertexRight };
        Vector3[] updatedNormals = new Vector3[] { _leftMeshTriangle.Normals[0], _normalLeft, _normalRight };
        Vector2[] updatedUVs = new Vector2[] { _leftMeshTriangle.UVs[0], _uvLeft, _uvRight };
        CUT_MeshTriangle _currentTriangle = new CUT_MeshTriangle(updatedVertices, updatedNormals, updatedUVs, _triangle.SubmeshIndex);

        if(updatedVertices[0] != updatedVertices[1] && updatedVertices[0] != updatedVertices[2])
        {
            if(Vector3.Dot(Vector3.Cross(updatedVertices[1] - updatedVertices[0], updatedVertices[2] - updatedVertices[0]), updatedNormals[0]) < 0)
            {
                FlipTriangle(_currentTriangle); 
            }
            _leftSide.AddTriangle(_currentTriangle); 
        }

        updatedVertices = new Vector3[] { _leftMeshTriangle.Vertices[0], _leftMeshTriangle.Vertices[1], _vertexRight };
        updatedNormals = new Vector3[] { _leftMeshTriangle.Normals[0], _leftMeshTriangle.Normals[1], _normalRight };
        updatedUVs = new Vector2[] { _leftMeshTriangle.UVs[0], _leftMeshTriangle.UVs[1], _uvRight };
        _currentTriangle = new CUT_MeshTriangle(updatedVertices, updatedNormals, updatedUVs, _triangle.SubmeshIndex);

        if (updatedVertices[0] != updatedVertices[1] && updatedVertices[0] != updatedVertices[2])
        {
            if (Vector3.Dot(Vector3.Cross(updatedVertices[1] - updatedVertices[0], updatedVertices[2] - updatedVertices[0]), updatedNormals[0]) < 0)
            {
                FlipTriangle(_currentTriangle);
            }
            _leftSide.AddTriangle(_currentTriangle);
        }
        #endregion

        #region Right
        /// RIGHT
        updatedVertices = new Vector3[] { _rightMeshTriangle.Vertices[0], _vertexLeft, _vertexRight };
        updatedNormals = new Vector3[] { _rightMeshTriangle.Normals[0], _normalLeft, _normalRight };
        updatedUVs = new Vector2[] { _rightMeshTriangle.UVs[0], _uvLeft, _uvRight };
        _currentTriangle = new CUT_MeshTriangle(updatedVertices, updatedNormals, updatedUVs, _triangle.SubmeshIndex);

        if (updatedVertices[0] != updatedVertices[1] && updatedVertices[0] != updatedVertices[2])
        {
            if (Vector3.Dot(Vector3.Cross(updatedVertices[1] - updatedVertices[0], updatedVertices[2] - updatedVertices[0]), updatedNormals[0]) < 0)
            {
                FlipTriangle(_currentTriangle);
            }
            _rightSide.AddTriangle(_currentTriangle);
        }

        updatedVertices = new Vector3[] { _rightMeshTriangle.Vertices[0], _rightMeshTriangle.Vertices[1], _vertexRight };
        updatedNormals = new Vector3[] { _rightMeshTriangle.Normals[0], _rightMeshTriangle.Normals[1], _normalRight };
        updatedUVs = new Vector2[] { _rightMeshTriangle.UVs[0], _rightMeshTriangle.UVs[1], _uvRight };
        _currentTriangle = new CUT_MeshTriangle(updatedVertices, updatedNormals, updatedUVs, _triangle.SubmeshIndex);

        if (updatedVertices[0] != updatedVertices[1] && updatedVertices[0] != updatedVertices[2])
        {
            if (Vector3.Dot(Vector3.Cross(updatedVertices[1] - updatedVertices[0], updatedVertices[2] - updatedVertices[0]), updatedNormals[0]) < 0)
            {
                FlipTriangle(_currentTriangle);
            }
            _rightSide.AddTriangle(_currentTriangle);
        }
        #endregion 
    }

    private static void FlipTriangle(CUT_MeshTriangle _triangle)
    {
        Vector3 _invertedVertex = _triangle.Vertices[0];
        Vector3 _invertedNormal = _triangle.Normals[0];
        Vector2 _invertedUv = _triangle.UVs[0];

        _triangle.Vertices[0] = _triangle.Vertices[2];
        _triangle.Normals[0] = _triangle.Normals[2];
        _triangle.UVs[0] = _triangle.UVs[2];

        _triangle.Vertices[2] = _invertedVertex;
        _triangle.Normals[2] = _invertedNormal;
        _triangle.UVs[2] = _invertedUv;
    }

    private static void FillCut(List<Vector3> _addedVertices, Plane _cuttingPlane, CUT_GeneratedMesh _leftMesh, CUT_GeneratedMesh _rightMesh)
    {
        List<Vector3> _vertices = new List<Vector3>();
        List<Vector3> _polygone = new List<Vector3>();
        for (int i = 0; i < _addedVertices.Count ; i++)
        {
            if (!_vertices.Contains(_addedVertices[i]))
            {
                _polygone.Clear();
                _polygone.Add(_addedVertices[i]);
                _polygone.Add(_addedVertices[i + 1]);

                _vertices.Add(_addedVertices[i]);
                _vertices.Add(_addedVertices[i + 1]);


                EvaluatePairs(_addedVertices, _vertices, _polygone);
                Fill(_polygone, _cuttingPlane, _leftMesh, _rightMesh);
            }
        }
    }

    private static void EvaluatePairs(List<Vector3> _addedVertices, List<Vector3> _vertices, List<Vector3> _polygone)
    {
        for (int i = 0; i < _addedVertices.Count; i += 2)
        {
            if (_addedVertices[i] == _polygone[_polygone.Count - 1] && !_vertices.Contains(_addedVertices[i + 1]))
            {
                _polygone.Add(_addedVertices[i + 1]);
                _vertices.Add(_addedVertices[i + 1]);
                i = 0; 
            }
            else if (_addedVertices[i + 1] == _polygone[_polygone.Count - 1] && !_vertices.Contains(_addedVertices[i]))
            {
                _polygone.Add(_addedVertices[i]);
                _vertices.Add(_addedVertices[i]);
                i = 0; 
            }
        }
    }

    private static void Fill(List<Vector3> _vertices, Plane _cuttingPlane, CUT_GeneratedMesh _leftMesh, CUT_GeneratedMesh _rightMesh)
    {
        Vector3 _centerPosition = Vector3.zero;
        for (int i = 0; i < _vertices.Count; i++)
        {
            _centerPosition += _vertices[i]; 
        }
        _centerPosition = _centerPosition/_vertices.Count;

        Vector3 _up = new Vector3(_cuttingPlane.normal.x, _cuttingPlane.normal.y, _cuttingPlane.normal.z);
        Vector3 _left = Vector3.Cross(_cuttingPlane.normal, _cuttingPlane.normal);

        Vector3 _displacement = Vector3.zero;
        Vector2 _uv1 = Vector2.zero;
        Vector2 _uv2 = Vector2.zero;
        Vector3[] _newVertices = null; 
        Vector3[] _normals = null;
        Vector2[] _uvs = null;

        CUT_MeshTriangle _currentTriangle = null; 

        for (int i = 0; i < _vertices.Count; i++)
        {
            _displacement = _vertices[i] - _centerPosition;
            _uv1 = new Vector2(.5f + Vector3.Dot(_displacement, _left), .5f + Vector3.Dot(_displacement, _up));

            _displacement = _vertices[(i + 1) % _vertices.Count] - _centerPosition;
            _uv2 = new Vector2(.5f + Vector3.Dot(_displacement, _left), .5f + Vector3.Dot(_displacement, _up));

            _newVertices = new Vector3[] { _vertices[i], _vertices[(i + 1) % _vertices.Count], _centerPosition };
            _normals = new Vector3[] { -_cuttingPlane.normal, -_cuttingPlane.normal, -_cuttingPlane.normal };
            _uvs = new Vector2[] { _uv1, _uv2, Vector2.one / 2 };

            _currentTriangle = new CUT_MeshTriangle(_newVertices, _normals, _uvs, _leftMesh.SubmeshIndices.Count - 1); 

            if(Vector3.Dot(Vector3.Cross(_newVertices[1] - _newVertices[0], _newVertices[2] - _newVertices[0]), _normals[0]) <0)
            {
                FlipTriangle(_currentTriangle); 
            }
            _leftMesh.AddTriangle(_currentTriangle);

            _normals = new Vector3[] { _cuttingPlane.normal, _cuttingPlane.normal, _cuttingPlane.normal };
            _currentTriangle = new CUT_MeshTriangle(_newVertices, _normals, _uvs, _rightMesh.SubmeshIndices.Count - 1);

            if (Vector3.Dot(Vector3.Cross(_newVertices[1] - _newVertices[0], _newVertices[2] - _newVertices[0]), _normals[0]) < 0)
            {
                FlipTriangle(_currentTriangle);
            }
            _rightMesh.AddTriangle(_currentTriangle);
        }
    }

    public static bool PrepareCut(Vector3 _normal, Vector3 _contactPoint, Transform _cutTransform, Mesh _originalMesh, out CUT_GeneratedMesh _keptMesh, out CUT_GeneratedMesh _removedMesh)
    {
        _keptMesh = null;
        _removedMesh = null;
        Plane _cuttingPlane = new Plane(_cutTransform.InverseTransformDirection(_normal.normalized), _cutTransform.InverseTransformPoint(_contactPoint));
        Debug.DrawLine(Vector3.zero, _cuttingPlane.normal, Color.blue, 200);

        List<Vector3> _addedVertices = new List<Vector3>();

        CUT_GeneratedMesh _leftMesh = new CUT_GeneratedMesh();
        CUT_GeneratedMesh _rightMesh = new CUT_GeneratedMesh();
        int[] _submeshIndices;
        int _triangleIndexA, _triangleIndexB, _triangleIndexC;
        bool _triangleALeftSide, _triangleBLeftSide, _triangleCLeftSide;
        for (int i = 0; i < _originalMesh.subMeshCount; i++)
        {
            _submeshIndices = _originalMesh.GetTriangles(i);

            for (int j = 0; j < _submeshIndices.Length; j += 3)
            {
                _triangleIndexA = _submeshIndices[j];
                _triangleIndexB = _submeshIndices[j + 1];
                _triangleIndexC = _submeshIndices[j + 2];

                CUT_MeshTriangle _currentTriangle = CUT_CuttingHelper.GetTriangle(_triangleIndexA, _triangleIndexB, _triangleIndexC, i, _originalMesh);
                _triangleALeftSide = _cuttingPlane.GetSide(_originalMesh.vertices[_triangleIndexA]);
                _triangleBLeftSide = _cuttingPlane.GetSide(_originalMesh.vertices[_triangleIndexB]);
                _triangleCLeftSide = _cuttingPlane.GetSide(_originalMesh.vertices[_triangleIndexC]);

                if (_triangleALeftSide && _triangleBLeftSide && _triangleCLeftSide)
                {
                    _leftMesh.AddTriangle(_currentTriangle);
                }
                else if (!_triangleALeftSide && !_triangleBLeftSide && !_triangleCLeftSide)
                {
                    _rightMesh.AddTriangle(_currentTriangle);
                }
                else
                {
                    CUT_CuttingHelper.CutTriangle(_cuttingPlane, _currentTriangle, _triangleALeftSide, _triangleBLeftSide, _triangleCLeftSide, _leftMesh, _rightMesh, _addedVertices);
                }
            }
        }

        CUT_CuttingHelper.FillCut(_addedVertices, _cuttingPlane, _leftMesh, _rightMesh);
        _keptMesh = _leftMesh;
        _removedMesh = _rightMesh;
        return true; 
    }

    public static void ApplyCut(MeshRenderer _originalMesh, CUT_GeneratedMesh _keptMesh, CUT_GeneratedMesh _removedMesh)
    {
        if (_keptMesh == null || _removedMesh == null)
        {
            Debug.Log("NULL"); 
            return;
        }
        //SET THE NEW MESH
        Mesh _newMesh = new Mesh();
        _newMesh.SetVertices(_keptMesh.Vertices);
        _newMesh.SetNormals(_keptMesh.Normals);
        _newMesh.SetUVs(0, _keptMesh.UVs);
        for (int i = 0; i < _keptMesh.SubmeshIndices.Count; i++)
        {
            _newMesh.SetTriangles(_keptMesh.SubmeshIndices[i], i);
        }
        _originalMesh.gameObject.GetComponent<MeshFilter>().mesh = _newMesh;
        return; 
        //INSTANTIATE A PARTICLE TO MAKE THE OTHER PART FALL
        _newMesh = new Mesh();
        _newMesh.SetVertices(_removedMesh.Vertices);
        _newMesh.SetNormals(_removedMesh.Normals);
        _newMesh.SetUVs(0, _removedMesh.UVs);
        for (int i = 0; i < _removedMesh.SubmeshIndices.Count; i++)
        {
            _newMesh.SetTriangles(_removedMesh.SubmeshIndices[i], i);
        }
        GameObject _particle = Resources.Load("CuttingParticleSystem") as GameObject;
        ParticleSystemRenderer _renderer = _particle.GetComponent<ParticleSystemRenderer>();
        Mesh[] _particleMeshes = new Mesh[] { _newMesh };
        _renderer.SetMeshes(_particleMeshes);
        _renderer.material = _originalMesh.material;
        Vector3 _pos = _originalMesh.gameObject.transform.position;
        Object.Instantiate(_particle, _pos, Quaternion.identity);
    }
    #endregion


    #endregion
}
