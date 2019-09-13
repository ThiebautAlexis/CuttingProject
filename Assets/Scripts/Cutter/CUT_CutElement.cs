using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody), typeof(MeshFilter))]
public class CUT_CutElement : MonoBehaviour 
{
    /* CUT_CutElement :
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

    #region Events

    #endregion

    #region Fields / Properties
    private bool m_isCurrentlyCut = false;
    [SerializeField] private BoxCollider m_collider = null;
    [SerializeField] Mesh m_originalMesh = null; 

    #endregion

    #region Methods

    #region Original Methods
    public void PrepareCut(Vector3 _normal, Vector3 _contactPoint)
    {
        if (m_isCurrentlyCut) return;
        m_isCurrentlyCut = true;
        Plane _cuttingPlane = new Plane(transform.InverseTransformDirection(_normal.normalized), transform.InverseTransformPoint(_contactPoint));
        Debug.DrawLine(Vector3.zero, _cuttingPlane.normal, Color.blue, 200);

        List<Vector3> _addedVertices = new List<Vector3>();

        CUT_GeneratedMesh _leftMesh = new CUT_GeneratedMesh();
        CUT_GeneratedMesh _rightMesh = new CUT_GeneratedMesh();
        int[] _submeshIndices;
        int _triangleIndexA, _triangleIndexB, _triangleIndexC;
        bool _triangleALeftSide, _triangleBLeftSide, _triangleCLeftSide; 
        for (int i = 0; i < m_originalMesh.subMeshCount; i++)
        {
            _submeshIndices = m_originalMesh.GetTriangles(i);

            for (int j = 0; j < _submeshIndices.Length; j+=3)
            {
                _triangleIndexA = _submeshIndices[j]; 
                _triangleIndexB = _submeshIndices[j + 1];
                _triangleIndexC = _submeshIndices[j + 2];

                CUT_MeshTriangle _currentTriangle =  CUT_CuttingHelper.GetTriangle(_triangleIndexA, _triangleIndexB, _triangleIndexC, i, m_originalMesh);
                _triangleALeftSide = _cuttingPlane.GetSide(m_originalMesh.vertices[_triangleIndexA]);
                _triangleBLeftSide = _cuttingPlane.GetSide(m_originalMesh.vertices[_triangleIndexB]);
                _triangleCLeftSide = _cuttingPlane.GetSide(m_originalMesh.vertices[_triangleIndexC]);

                if(_triangleALeftSide && _triangleBLeftSide && _triangleCLeftSide )
                {
                    _leftMesh.AddTriangle(_currentTriangle);
                }
                else if(!_triangleALeftSide && !_triangleBLeftSide && !_triangleCLeftSide)
                {
                    _rightMesh.AddTriangle(_currentTriangle);
                }
                else
                {
                    CUT_CuttingHelper.CutTriangle(_cuttingPlane, _currentTriangle, _triangleALeftSide, _triangleBLeftSide, _triangleCLeftSide, _leftMesh, _rightMesh, _addedVertices) ; 
                }
            }
        }

        CUT_CuttingHelper.FillCut(_addedVertices, _cuttingPlane, _leftMesh, _rightMesh); 
        Mesh _newMesh = new Mesh();
        _newMesh.SetVertices(_leftMesh.Vertices);
        _newMesh.SetNormals(_leftMesh.Normals);
        _newMesh.SetUVs(0, _leftMesh.UVs);
        for (int i = 0; i < _leftMesh.SubmeshIndices.Count; i++)
        {
            _newMesh.SetTriangles(_leftMesh.SubmeshIndices[i], i); 
        }

        SetNewMesh(_newMesh);
        _newMesh = new Mesh();
        _newMesh.SetVertices(_rightMesh.Vertices);
        _newMesh.SetNormals(_rightMesh.Normals);
        _newMesh.SetUVs(0, _rightMesh.UVs);
        for (int i = 0; i < _rightMesh.SubmeshIndices.Count; i++)
        {
            _newMesh.SetTriangles(_rightMesh.SubmeshIndices[i], i);
        }

        CUT_CutElement _elem = new GameObject().AddComponent<CUT_CutElement>();
        _elem.transform.position += new Vector3(0, 0, 1.5f); 
        _elem.SetNewMesh(_newMesh);
        _elem.SetMaterial(GetComponent<MeshRenderer>().material); 
        _elem.GetComponent<BoxCollider>().isTrigger = false;
    }

    public void SetNewMesh(Mesh _newMesh)
    {
        if (!GetComponent<MeshRenderer>()) gameObject.AddComponent<MeshRenderer>();
        m_originalMesh = _newMesh; 
        GetComponent<MeshFilter>().mesh = m_originalMesh;
        GetComponent<MeshFilter>().sharedMesh = m_originalMesh;
    }

    public void SetMaterial(Material _m)
    {
        if (!GetComponent<MeshRenderer>()) gameObject.AddComponent<MeshRenderer>();
        GetComponent<MeshRenderer>().material = _m; 
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (!m_collider) m_collider = GetComponent<BoxCollider>();
        if (!m_originalMesh) m_originalMesh = GetComponent<MeshFilter>().mesh;
    }

    private void OnTriggerEnter(Collider other)
    {
        PrepareCut(other.transform.right, transform.position + (Vector3.up * .5f)); 
    }

    private void OnDrawGizmos()
    {
        if (!m_originalMesh) return; 
        for (int i = 0; i < m_originalMesh.vertexCount; i++)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(m_originalMesh.vertices[i], .1f);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(m_originalMesh.normals[i], .1f);
        }
    }
    #endregion

    #endregion
}
