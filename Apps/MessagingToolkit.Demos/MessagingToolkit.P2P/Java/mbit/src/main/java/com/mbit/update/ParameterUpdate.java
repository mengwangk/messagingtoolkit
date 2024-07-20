package com.mbit.update;

import com.mbit.base.BaseMidlet;
import com.mbit.common.Constants;
import com.mbit.common.ErrorCode;
import com.mbit.common.Tag;
import com.mbit.screen.ProgressScreen;
import com.mbit.valueobject.parameter.ParameterRecord;
import com.mbit.valueobject.parameter.ParameterStore;
import com.nextel.net.HttpRequest;
import de.enough.polish.util.Locale;
import org.kxml2.io.KXmlParser;

import javax.microedition.lcdui.Display;
import java.io.InputStream;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 22, 2006 10:25:08 PM
 * @version : $Id:
 */
public class ParameterUpdate extends AbstractUpdate {

    private ProgressScreen progressScreen;


    /**
     * Constructor
     */
    public ParameterUpdate(BaseMidlet midlet) {
        super();
        progressScreen = new ProgressScreen(Locale.get("progress.parameterupdate"));
        Display display = Display.getDisplay(midlet);
        display.setCurrent(progressScreen);
    }

    /**
     * Retrieve the parameters
     */
    public void run() {
        HttpRequest httpRequest = new HttpRequest(Constants.UPDATE_URL, Constants.HTTP_TIME_OUT, Constants.HTTP_TIME_OUT);
        try {
            //StringBuffer results = new StringBuffer();

            // create DataInputStream on top of the socket connection
            InputStream inputStream = httpRequest.get();

            // retrieve the contents of the requested page from Web server
            /*
            int inputChar;
            while ((inputChar = dataInputStream.read()) != -1) {
                results.append((char) inputChar);
            }
            */
            KXmlParser parser = new KXmlParser();
            parser.setInput(inputStream, Constants.PAGE_ENCODING);

            ParameterStore parameterStore = ParameterStore.getInstance();
            parameterStore.deleteAll();  // to clean up

            progressScreen.setProgress(ProgressScreen.HALF_PROGRESS_VALUE);

            while (parser.getEventType() != KXmlParser.END_DOCUMENT) {
                if (parser.getEventType() == KXmlParser.START_TAG) {
                    if (Tag.PARAMETER_TAG.equals(parser.getName())) {
                        do {
                            ParameterRecord parameter = new ParameterRecord();
                            parameter.setId(Integer.parseInt(parser.getAttributeValue(0)));
                            parameter.setName(parser.getAttributeValue(1));
                            parameter.setValue(parser.getAttributeValue(2));
                            parameterStore.addRecord(parameter);
                        } while (parser.nextTag() != KXmlParser.END_TAG &&
                                Tag.PARAMETER_TAG.equals(parser.getName()));
                    }
                }
                try {
                    parser.next();
                } catch (Exception ex) {

                }
            }

            inputStream.close();
            progressScreen.setProgress(ProgressScreen.COMPLETE_PROGRESS_VALUE);
        } catch (Exception ex) {
            //#debug
            System.out.println(ex.getMessage());
            setErrorCode(ErrorCode.INITIALIZATION_ERROR);
            setErrorMsg(ex.getMessage());
        } finally {
            httpRequest.cleanup();
        }

    }
}
