/*==============================================================================
            Copyright (c) 2010-2011 QUALCOMM Austria Research Center GmbH.
            All Rights Reserved.
            Qualcomm Confidential and Proprietary
==============================================================================*/

// An interface for handling QCAR errors.
public interface IQCARErrorHandler
{
    // Called when a QCAR initialization error has occured.
    void SetErrorCode(TrackerBehaviour.InitError errorCode);
    void SetErrorOccurred(bool errorOccurred);
}
