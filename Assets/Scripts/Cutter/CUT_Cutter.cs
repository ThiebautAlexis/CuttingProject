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
    private CUT_CutElement m_cutElementCurrent = null; 
    #endregion

    #region Methods

    #region Original Methods

    #endregion

    #region Unity Methods
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<CUT_CutElement>())
        {
            m_cutElementCurrent = other.GetComponent<CUT_CutElement>();
            RaycastHit _hitInfo; 
            if(Physics.Raycast(new Ray(transform.position, transform.up), out _hitInfo))
            {
                m_cutElementCurrent.PrepareCut(-transform.right, _hitInfo.point); 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(m_cutElementCurrent != null)
        {
            m_cutElementCurrent.Cut();
            m_cutElementCurrent = null; 
        }
    }
    #endregion

    #endregion
}
