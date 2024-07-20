package com.mbit.update;

import com.mbit.base.BaseMidlet;
import com.mbit.common.Constants;
import com.mbit.common.ErrorCode;
import com.mbit.common.Tag;
import com.mbit.screen.ProgressScreen;
import com.mbit.util.DownloadUtils;
import com.mbit.valueobject.channel.ChannelRecord;
import com.mbit.valueobject.channel.ChannelStore;
import com.mbit.valueobject.parameter.ParameterRecord;
import com.mbit.valueobject.parameter.ParameterStore;
import com.mbit.valueobject.station.StationRecord;
import com.nextel.net.HttpRequest;
import de.enough.polish.util.Locale;
import org.kxml2.io.KXmlParser;

import javax.microedition.lcdui.Display;
import java.io.InputStream;
import java.util.Vector;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jul 4, 2006 2:22:26 PM
 * @version : $Id:
 */
public class ChannelUpdate extends AbstractUpdate {

    private ProgressScreen progressScreen;

    private StationRecord station;

    private Vector channels;

    /**
     * Constructor
     */
    public ChannelUpdate(BaseMidlet midlet, StationRecord station) {
        super();
        progressScreen = new ProgressScreen(Locale.get("progress.channelupdate"));
        this.station = station;
        Display display = Display.getDisplay(midlet);
        display.setCurrent(progressScreen);
        channels = new Vector();
    }

    /**
     * Retrieve the channels
     */
    public void run() {
        String url = "";
        String baseUrl = "";
        ParameterRecord p = ParameterStore.getParameterRecord(Constants.NEWS_SERVER_BASE_URL_PARAMETER);
        if (p != null) {
            baseUrl = p.getValue();
            url = p.getValue();
        }
        url += station.getChannel();

        //#debug
        System.out.println("Retrieve channel from " + url);
        HttpRequest httpRequest = new HttpRequest(url, Constants.HTTP_TIME_OUT, Constants.HTTP_TIME_OUT);
        try {

            // create DataInputStream on top of the socket connection
            InputStream inputStream = httpRequest.get();
            KXmlParser parser = new KXmlParser();
            parser.setInput(inputStream, Constants.PAGE_ENCODING);

            ChannelStore channelStore = ChannelStore.getInstance();
            channels.removeAllElements();

            progressScreen.setProgress(ProgressScreen.HALF_PROGRESS_VALUE);

            while (parser.getEventType() != KXmlParser.END_DOCUMENT) {
                if (parser.getEventType() == KXmlParser.START_TAG) {
                    //#debug
                    System.out.println("start tag: " + parser.getName());
                    if (Tag.OUTLINE_TAG.equals(parser.getName())) {
                        do {
                            /* For every outline */
                            ChannelRecord channel = new ChannelRecord();
                            channel.setId(Integer.parseInt(parser.getAttributeValue(0)));
                            channel.setName(parser.getAttributeValue(1));
                            channel.setUrl(parser.getAttributeValue(2));
                            channel.setAdUrl(parser.getAttributeValue(3));
                            channel.setImage(parser.getAttributeValue(4));
                            channel.setStationName(station.getName());
                            channels.addElement(channel);
                            //#debug
                            System.out.println("Channel: " + channel.getName() + " retrieved.");
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
            httpRequest.cleanup();

            progressScreen.setProgress(ProgressScreen.THREE_QUARTER_PROGRESS_VALUE);

            for (int i = 0; i < channels.size(); i++) {
                ChannelRecord channel = (ChannelRecord) channels.elementAt(i);
                if (!"".equals(channel.getImage())) {
                    //#debug
                    System.out.println("Retrieving image from " + baseUrl + channel.getImage());
                    channel.setImageData(DownloadUtils.getImage(baseUrl + channel.getImage()));
                }
                channelStore.addRecord(channel);
            }

            progressScreen.setProgress(ProgressScreen.COMPLETE_PROGRESS_VALUE);
        } catch (Exception ex) {
            //#debug
            System.out.println(ex.getMessage());
            setErrorCode(ErrorCode.CHANNEL_UPPDATE_ERROR);
            setErrorMsg(ex.getMessage());
        } finally {
            try {
                httpRequest.cleanup();
            } catch (Exception ex) {
            }
        }

    }
}


