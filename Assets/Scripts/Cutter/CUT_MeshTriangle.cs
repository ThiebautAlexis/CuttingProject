using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

[Serializable]
public class CUT_MeshTriangle 
{
    /* CUT_MeshTriangle :
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
    public List<Vector3> Vertices { get; private set; } = new List<Vector3>();
    public List<Vector3> Normals { get; private set; } = new List<Vector3>();
    public List<Vector2> UVs { get; private set; } = new List<Vector2>();
    public int SubmeshIndex { get; private set; }
    #endregion

    #region Constructor
    public CUT_MeshTriangle(Vector3[] _vertices, Vector3[] _normals, Vector2[] _uvs, int _submeshIndex)
    {
        Vertices = _vertices.ToList();
        Normals = _normals.ToList();
        UVs = _uvs.ToList();
        SubmeshIndex = _submeshIndex; 
    }
    #endregion  

    #region Methods

    #region Original Methods

    #endregion

    #endregion
}
