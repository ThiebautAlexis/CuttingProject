using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CUT_GeneratedMesh 
{
    /* CUT_GeneratedMesh :
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
    public List<Vector3> Vertices           { get; private set; } = new List<Vector3>(); 
    public List<Vector3> Normals            { get; private set; } = new List<Vector3>(); 
    public List<Vector2> UVs                { get; private set; } = new List<Vector2>(); 
    public List<List<int>> SubmeshIndices   { get; private set; } = new List<List<int>>();
    #endregion

    #region Methods

    #region Original Methods
    public void AddTriangle(CUT_MeshTriangle _triangle)
    {
        int currentVerticesCount = Vertices.Count;

        // Add the vertices, the normals and the UVs to the global lists
        Vertices.AddRange(_triangle.Vertices);
        Normals.AddRange(_triangle.Normals);
        UVs.AddRange(_triangle.UVs); 

        // If the Submesh Indices count if less than the submesh index of the triangle add a new list of submesh index for each indexes that aren't in the list
        if(SubmeshIndices.Count < _triangle.SubmeshIndex + 1)
        {
            for (int i = SubmeshIndices.Count; i < _triangle.SubmeshIndex + 1; i++)
            {
                SubmeshIndices.Add(new List<int>()); 
            }
        }

        // For each point of the triangle, add the submesh index according to the inital count of the vertices
        for (int i = 0; i < 3; i++)
        {
            SubmeshIndices[_triangle.SubmeshIndex].Add(currentVerticesCount + i); 
        }
    }
    #endregion

	#endregion
}
