package com.mbit.valueobject.parameter;

import com.nextel.rms.OAbstractRecord;
import com.nextel.rms.OAbstractStore;


/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 22, 2006 10:09:01 PM
 * @version : $Id:
 */
public class ParameterStore extends OAbstractStore {

    private static ParameterStore parameterStore;

    /**
     * Constructor
     */
    private ParameterStore() {
        super("parameter", true /* use the default cache */);
    }


    /**
     * Returns an instance of a <code>ParameterStore</code>.
     *
     * @return An instance of a ParameterStore
     */
    public static ParameterStore getInstance() {
        if (parameterStore == null)
            parameterStore = new ParameterStore();

        return parameterStore;
    }

    /**
     * Get a particular parameter by name
     *
     * @param name
     * @return ParameterRecord
     */
    public static ParameterRecord getParameterRecord(String name) {
        ParameterStore parameterStore = ParameterStore.getInstance();
        try {
            OAbstractRecord[] records = parameterStore.getAll(null, new ParameterFilter(name));
            return (ParameterRecord)records[0];
        } catch (Exception ex) {
            //#debug
            System.out.println(ex.getMessage());
            return null;
        }
    }

    /**
     * Called by {@link com.nextel.rms.OAbstractStore} to create
     * a new record.
     *
     * @return A new SalaryRecord.
     */
    protected OAbstractRecord createRecord() {
        return new ParameterRecord();
    }

}