using System;
using System.Collections;
using System.Collections.Generic;
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
    public static CUT_MeshTriangle GetTriangle(int _indexA, int _indexB, int _indexC, int _subMeshIndex, Mesh _originalMesh)
    {
        Vector3[] _vertices = new Vector3[3] { _originalMesh.vertices[_indexA], _originalMesh.vertices[_indexB], _originalMesh.vertices[_indexC] };
        Vector3[] _normals = new Vector3[3] { _originalMesh.normals[_indexA], _originalMesh.normals[_indexB], _originalMesh.normals[_indexC] };
        Vector2[] _uvs = new Vector2[3] { _originalMesh.uv[_indexA], _originalMesh.uv[_indexB], _originalMesh.uv[_indexC] };
        return new CUT_MeshTriangle(_vertices, _normals, _uvs, _subMeshIndex);
    }

    public static void CutTriangle(Plane _cuttingPlane, CUT_MeshTriangle _triangle, bool _triangleALeftSide, bool _triangleBLeftSide, bool _triangleCLeftSide, CUT_GeneratedMesh _leftSide, CUT_GeneratedMesh _rightSide, List<Vector3> _addedVertices)
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
        _addedVertices.Add(_vertexLeft);
        Vector3 _normalRight = Vector3.Lerp(_leftMeshTriangle.Normals[1], _rightMeshTriangle.Normals[1], _normalizedDistance);
        Vector2 _uvRight = Vector2.Lerp(_leftMeshTriangle.UVs[1], _rightMeshTriangle.UVs[1], _normalizedDistance);

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
    #endregion


    #endregion
}
