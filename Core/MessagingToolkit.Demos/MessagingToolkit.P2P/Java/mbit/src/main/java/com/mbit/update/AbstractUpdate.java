package com.mbit.update;

import com.mbit.common.ErrorCode;
import com.mbit.common.Constants;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 28, 2006 11:01:31 PM
 * @version : $Id:
 */
public abstract class AbstractUpdate {

    protected int errorCode ;
    protected String errorMsg;

    public abstract void run();

    /**
     * Constructor
     */
    public AbstractUpdate(){
        errorCode = ErrorCode.NO_ERROR;
        errorMsg = Constants.STRING_EMPTY;

    }
    public int getErrorCode() {
        return errorCode;
    }

    public void setErrorCode(int errorCode){
        this.errorCode = errorCode;
    }

    public String getErrorMsg() {
        return errorMsg;
    }

    public void setErrorMsg(String errorMsg) {
        this.errorMsg = errorMsg;
    }


}
