package com.mbit.update;

import com.mbit.MainMidlet;
import com.mbit.TestMidlet;
import com.mbit.common.Constants;
import com.mbit.common.ErrorCode;
import com.mbit.common.Tag;
import com.mbit.screen.ProgressScreen;
import com.mbit.util.DownloadUtils;
import com.mbit.valueobject.advertisement.AdRecord;
import com.mbit.valueobject.advertisement.AdStore;
import com.mbit.valueobject.advertisement.AdComparator;
import com.mbit.valueobject.advertisement.AdFilter;
import com.mbit.valueobject.channel.ChannelRecord;
import com.mbit.valueobject.parameter.ParameterRecord;
import com.mbit.valueobject.parameter.ParameterStore;
import com.nextel.net.HttpRequest;
import com.nextel.rms.OAbstractRecord;
import de.enough.polish.util.Locale;
import org.kxml2.io.KXmlParser;

import javax.microedition.lcdui.Display;
import java.io.InputStream;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 10, 2006 6:59:33 PM
 * @version : $Id:
 */
public class AdUpdate extends AbstractUpdate {

    private ProgressScreen progressScreen;

    private ChannelRecord channel;

    private MainMidlet midlet;


    /**
     * Constructor
     */
    public AdUpdate(MainMidlet midlet, ChannelRecord channel) {
        super();
        progressScreen = new ProgressScreen(Locale.get("progress.adupdate"));
        Display display = Display.getDisplay(midlet);
        display.setCurrent(progressScreen);
        this.midlet = midlet;
        this.channel = channel;
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
        url += channel.getAdUrl();

        HttpRequest httpRequest = new HttpRequest(url, Constants.HTTP_TIME_OUT, Constants.HTTP_TIME_OUT);
        try {

            // create DataInputStream on top of the socket connection
            InputStream inputStream = httpRequest.get();
            KXmlParser parser = new KXmlParser();
            parser.setInput(inputStream, Constants.PAGE_ENCODING);

            AdStore adStore = AdStore.getInstance();
            // Clean up
            try {
                OAbstractRecord[] ads = adStore.getAll(new AdComparator(), new AdFilter(url));
                if (ads != null && ads.length > 0) {
                    for (int i = 0; i < ads.length; i++) {
                        AdRecord r = (AdRecord) ads[i];
                        if (r.getUrl().equals(url) && r.getSessionId() == midlet.getSessionId()) {
                            // Same ads that have been retrieved before
                            return ;
                        }
                        adStore.deleteRecord(r);
                    }
                }
            } catch (Exception ex) {

            }
            progressScreen.setProgress(ProgressScreen.HALF_PROGRESS_VALUE);

            while (parser.getEventType() != KXmlParser.END_DOCUMENT) {
                if (parser.getEventType() == KXmlParser.START_TAG) {
                    //#debug
                    System.out.println("start tag: " + parser.getName());
                    if (Tag.ADVERSTISEMENT_TAG.equals(parser.getName())) {
                        do {
                            // For every advertisement
                            AdRecord ad = new AdRecord();
                            ad.setId(Integer.parseInt(parser.getAttributeValue(0)));
                            ad.setUrl(url);
                            ad.setText(parser.getAttributeValue(1));
                            ad.setImage(parser.getAttributeValue(2));
                            ad.setActionId(parser.getAttributeValue(3));
                            if (ad.getImage() != null && !"".equals(ad.getImage())) {
                                //#debug
                                System.out.println("Retrieving image from " + ad.getImage());
                                ad.setImageData(DownloadUtils.getImage(ad.getImage()));
                            }
                            ad.setSessionId(midlet.getSessionId());
                            adStore.addRecord(ad);
                        } while (parser.nextTag() != KXmlParser.END_TAG &&
                                Tag.ADVERSTISEMENT_TAG.equals(parser.getName()));
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
            setErrorCode(ErrorCode.AD_UPPDATE_ERROR);
            setErrorMsg(ex.getMessage());
        } finally {
            httpRequest.cleanup();
        }

    }
}

