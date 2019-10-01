using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

[RequireComponent(typeof(MeshCollider), typeof(Rigidbody), typeof(MeshFilter))]
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
    [SerializeField] private Mesh m_originalMesh = null;

    [SerializeField] private MeshFilter m_meshFilter = null;
    [SerializeField] private MeshCollider m_meshCollider = null;

    private CUT_GeneratedMesh m_keptMesh = null;
    private CUT_GeneratedMesh m_removedMesh = null; 
    #endregion

    #region Methods

    #region Original Methods

    public void Cut()
    {
        //SET THE NEW MESH
        Mesh _newMesh = new Mesh();
        _newMesh.SetVertices(m_keptMesh.Vertices);
        _newMesh.SetNormals(m_keptMesh.Normals);
        _newMesh.SetUVs(0, m_keptMesh.UVs);
        for (int i = 0; i < m_keptMesh.SubmeshIndices.Count; i++)
        {
            _newMesh.SetTriangles(m_keptMesh.SubmeshIndices[i], i);
        }
        SetNewMesh(_newMesh);
        //INSTANTIATE A PARTICLE TO MAKE THE OTHER PART FALL
        _newMesh = new Mesh();
        _newMesh.SetVertices(m_removedMesh.Vertices);
        _newMesh.SetNormals(m_removedMesh.Normals);
        _newMesh.SetUVs(0, m_removedMesh.UVs);
        for (int i = 0; i < m_removedMesh.SubmeshIndices.Count; i++)
        {
            _newMesh.SetTriangles(m_removedMesh.SubmeshIndices[i], i);
        }
        GameObject _particle = Resources.Load("CuttingParticleSystem") as GameObject; 
        ParticleSystemRenderer _renderer = _particle.GetComponent<ParticleSystemRenderer>();
        Mesh[] _particleMeshes = new Mesh[] { _newMesh }; 
        _renderer.SetMeshes(_particleMeshes);
        _renderer.material = GetComponent<MeshRenderer>().material;
        Vector3 _pos = transform.position;
        Instantiate(_particle, _pos, Quaternion.identity); 
        // NULLIFY THE LEFT AND RIGHT MESH 
        m_keptMesh = null;
        m_removedMesh = null;

        m_isCurrentlyCut = false; 
    }

    public void SetNewMesh(Mesh _newMesh)
    {
        if(_newMesh.vertexCount == 0)
        {
            Destroy(gameObject);
            return; 
        }
        if (!GetComponent<MeshRenderer>()) gameObject.AddComponent<MeshRenderer>();
        if (!m_meshFilter) m_meshFilter = GetComponent<MeshFilter>(); 
        m_originalMesh = _newMesh;
        m_meshFilter.mesh = m_originalMesh;
        m_meshFilter.sharedMesh = m_originalMesh;
        if (!m_meshCollider) m_meshCollider = GetComponent<MeshCollider>();
        m_meshCollider.sharedMesh = m_originalMesh; 
    }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (!m_originalMesh) m_originalMesh = GetComponent<MeshFilter>().mesh;
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 _cuttingPos = transform.position;
        RaycastHit _hitInfo; 
        if(Physics.Raycast(new Ray(other.transform.position, transform.position - other.transform.position), out _hitInfo))
        {
            _cuttingPos = _hitInfo.point; 
        }
    }
    #endregion

    #endregion
}
