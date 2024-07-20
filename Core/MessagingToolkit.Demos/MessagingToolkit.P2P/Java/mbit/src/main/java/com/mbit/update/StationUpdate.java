package com.mbit.update;

import com.mbit.base.BaseMidlet;
import com.mbit.common.Constants;
import com.mbit.common.ErrorCode;
import com.mbit.common.Tag;
import com.mbit.screen.ProgressScreen;
import com.mbit.util.DownloadUtils;
import com.mbit.valueobject.parameter.ParameterRecord;
import com.mbit.valueobject.parameter.ParameterStore;
import com.mbit.valueobject.station.StationRecord;
import com.mbit.valueobject.station.StationStore;
import com.nextel.net.HttpRequest;
import de.enough.polish.util.Locale;
import org.kxml2.io.KXmlParser;

import javax.microedition.lcdui.Display;
import java.io.InputStream;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 29, 2006 10:48:55 PM
 * @version : $Id:
 */
public class StationUpdate extends AbstractUpdate {

    private ProgressScreen progressScreen;


    /**
     * Constructor
     */
    public StationUpdate(BaseMidlet midlet) {
        super();
        progressScreen = new ProgressScreen(Locale.get("progress.stationupdate"));
        Display display = Display.getDisplay(midlet);
        display.setCurrent(progressScreen);
    }

    /**
     * Retrieve the stations
     */
    public void run() {
        String url = "";
        String baseUrl = "";
        ParameterRecord p = ParameterStore.getParameterRecord(Constants.NEWS_SERVER_BASE_URL_PARAMETER);
        if (p != null) {
            baseUrl = p.getValue();
            url = p.getValue();
        }

        p = ParameterStore.getParameterRecord(Constants.STATION_UPDATE_FILE_PARAMETER);
        if (p != null) {
            url += p.getValue();
        }

        HttpRequest httpRequest = new HttpRequest(url, Constants.HTTP_TIME_OUT, Constants.HTTP_TIME_OUT);
        try {

            // create DataInputStream on top of the socket connection
            InputStream inputStream = httpRequest.get();
            KXmlParser parser = new KXmlParser();
            parser.setInput(inputStream, Constants.PAGE_ENCODING);

            StationStore stationStore = StationStore.getInstance();
            stationStore.deleteAll();  // to clean up

            progressScreen.setProgress(ProgressScreen.HALF_PROGRESS_VALUE);

            while (parser.getEventType() != KXmlParser.END_DOCUMENT) {
                if (parser.getEventType() == KXmlParser.START_TAG) {
                    //#debug
                    System.out.println("start tag: " + parser.getName());
                    if (Tag.OUTLINE_TAG.equals(parser.getName())) {
                        do {
                            // For every outline
                            StationRecord station = new StationRecord();
                            station.setId(Integer.parseInt(parser.getAttributeValue(0)));
                            station.setName(parser.getAttributeValue(1));
                            station.setImage(parser.getAttributeValue(2));
                            station.setChannel(parser.getAttributeValue(3));
                            System.out.println("Retrieving image from " + baseUrl + station.getImage());
                            station.setImageData(DownloadUtils.getImage(baseUrl + station.getImage()));
                            stationStore.addRecord(station);
                        } while (parser.nextTag() != KXmlParser.END_TAG &&
                                Tag.OUTLINE_TAG.equals(parser.getName()));

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
            setErrorCode(ErrorCode.STATION_UPPDATE_ERROR);
            setErrorMsg(ex.getMessage());
        } finally {
            httpRequest.cleanup();
        }

    }
}

