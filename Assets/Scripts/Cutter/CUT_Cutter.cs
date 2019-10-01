using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUT_Cutter : MonoBehaviour 
{
    /* CUT_Cutter :
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
    private CUT_GeneratedMesh m_meshOne = null;
    private CUT_GeneratedMesh m_meshTwo = null;
    private Collider m_currentlyCut = null;

    private bool m_canApplyCut = false; 
    #endregion

    #region Methods

    #region Original Methods
    private void StartCut(Collider _cutCollider)
    {
        RaycastHit _hitInfo;
        if (Physics.Raycast(new Ray(transform.position, transform.up), out _hitInfo))
        {
            m_canApplyCut = false;
            m_currentlyCut = _cutCollider;
            m_canApplyCut = CUT_CuttingHelper.PrepareCut(-transform.right, _hitInfo.point, _cutCollider.gameObject.transform, _cutCollider.GetComponent<MeshFilter>().mesh, out m_meshOne, out m_meshTwo);
            Debug.Log(m_canApplyCut);
        }
    }

    private IEnumerator WaitingForCutPreparation()
    {
        while (!m_canApplyCut && m_currentlyCut != null)
        {
            yield return null; 
        }
        CUT_CuttingHelper.ApplyCut(m_currentlyCut.GetComponent<MeshRenderer>(), m_meshOne, m_meshTwo);
        m_meshTwo = null;
        m_meshTwo = null;
        m_currentlyCut = null;
    }
    #endregion

    #region Unity Methods
    private void OnTriggerEnter(Collider other)
    {
        if (m_currentlyCut != null) return; 
        if(other.GetComponent<MeshRenderer>() && other.GetComponent<MeshFilter>())
        {
            StartCut(other); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other == m_currentlyCut)
        {
            StartCoroutine(WaitingForCutPreparation()); 
        }
    }
    #endregion

    #endregion
}
