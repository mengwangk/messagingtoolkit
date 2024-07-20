package com.mbit;

import com.mbit.base.BaseMidlet;
import com.mbit.update.StationUpdate;
import com.mbit.update.ParameterUpdate;
import com.mbit.update.FeedUpdate;
import com.mbit.update.AdUpdate;
import com.mbit.common.ErrorCode;
import com.mbit.valueobject.parameter.ParameterStore;
import com.mbit.valueobject.parameter.ParameterRecord;
import com.mbit.valueobject.parameter.ParameterComparator;
import com.mbit.valueobject.parameter.ParameterFilter;
import com.mbit.valueobject.station.StationStore;
import com.mbit.valueobject.station.StationRecord;
import com.mbit.valueobject.station.StationFilter;
import com.mbit.valueobject.station.StationComparator;
import com.mbit.valueobject.rss.RssFeedRecord;
import com.mbit.valueobject.channel.ChannelRecord;
import com.mbit.util.ImageUtils;
import com.mbit.custom.ImageTicker;
import com.nextel.rms.OAbstractRecord;

import javax.microedition.midlet.MIDletStateChangeException;
import javax.microedition.lcdui.*;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 18, 2006 9:10:43 PM
 * @version : $Id:
 */
public class TestMidlet extends BaseMidlet {


    private Form myForm;

    protected void startApp() throws MIDletStateChangeException {
        //#style displayScreen
        myForm = new Form("Testing");
        try {
            Image image = Image.createImage("/progress.png");
            System.out.println("ok");
            ImageTicker ticker = new ImageTicker(image, "testing testing", myForm.getWidth());
            ImageTicker ticker1 = new ImageTicker(image, "testing tesfsdfsdfsdfsdfsdfsting", myForm.getWidth());
            myForm.append(ticker);
            myForm.append(ticker1);
        } catch (Exception ex) {
            ex.printStackTrace();
            System.out.println(ex.getMessage());

        }
        Display.getDisplay(this).setCurrent(myForm);

        /* ChannelRecord channel = new ChannelRecord();
        channel.setAdUrl("ad.xml");
        AdUpdate adUpdate = new AdUpdate(this, channel);
        adUpdate.run();*/
        //RssFeedRecord feed = new RssFeedRecord();
        //feed.setUrl("http://lightspeedleader.com/mbit/sinchew_intl.xml");
        /*FeedUpdate feedUpdate = new FeedUpdate(this, feed);
        feedUpdate.run();*/
        /*
        StringBuffer results = new StringBuffer();
        ParameterStore parameterStore = ParameterStore.getInstance();
        StationStore stationStore = StationStore.getInstance();
        try {

            ParameterUpdate parameterUpdate = new ParameterUpdate(this);
            parameterUpdate.run();
            if (parameterUpdate.getErrorCode() == ErrorCode.NO_ERROR) {
                OAbstractRecord[] parameters = parameterStore.getAll(new ParameterComparator(), null);
                for (int i = 0; i < parameters.length; i++) {
                    ParameterRecord parameter = (ParameterRecord) parameters[i];
                    results.append("Id : " + parameter.getRecordId() + " Name: " + parameter.getName() + " Value: " + parameter.getValue());
                    results.append("\n");
                }
            } else {
                results.append(parameterUpdate.getErrorMsg());
            }
            OAbstractRecord[] parameters  = parameterStore.getAll(new ParameterComparator(), new ParameterFilter("last-updated"));
            ParameterRecord parameter = (ParameterRecord) parameters[0];
            results.append("Id : " + parameter.getRecordId() + " Name: " + parameter.getName() + " Value: " + parameter.getValue());
            

            StationUpdate stationUpdate = new StationUpdate(this);
            stationUpdate.run();
            if (stationUpdate.getErrorCode() == ErrorCode.NO_ERROR) {
                OAbstractRecord[] stations = stationStore.getAll(new StationComparator(), null);
                for (int i = 0; i < stations.length; i++) {
                    StationRecord station = (StationRecord) stations[i];
                    results.append("Id : " + station.getRecordId() + " Name: " + station.getName() + " Value: " + station.getName());
                    results.append("\n");

                    ImageItem img = new ImageItem("", ImageUtils.convertByteArrayToImage(station.getImageData()), 0,"",0);
                    myForm.append(img);

                }
            } else {
                results.append(stationUpdate.getErrorMsg());
            }
            OAbstractRecord[] stations  = stationStore.getAll(null, new StationFilter("The Sun"));
            StationRecord station = (StationRecord) stations[0];
            results.append("Id : " + station.getRecordId() + " Name: " + station.getName() + " Value: " + station.getName());

            parameterStore.deleteAll();
            stationStore.deleteAll();

        } catch (Exception ex) {
            //#debug
            System.out.println("Error " + ex.getMessage());
        }


        // display the page contents on the phone screen
        StringItem resultField = new StringItem(null, results.toString());
        myForm.append(resultField);
        Display display = Display.getDisplay(this);
        display.setCurrent(myForm);
        */

        /*
        HttpRequest httpRequest = new HttpRequest("http://lightspeedleader.com/mbit/update.xml", 20, 3);

        try {

            // create DataInputStream on top of the socket connection
            InputStream inputStream = httpRequest.get();
            DataInputStream dataInputStream = new DataInputStream(inputStream);

            // retrieve the contents of the requested page from Web server
            int inputChar;
            while ((inputChar = dataInputStream.read()) != -1) {
                results.append((char) inputChar);
            }
        } catch (Exception ex) {
            System.out.println(ex.getMessage());
        }


        // display the page contents on the phone screen
        StringItem resultField = new StringItem(null, results.toString());
        myForm.append(resultField);
        Display display = Display.getDisplay(this);
        display.setCurrent(myForm);
        */

    }

    protected void pauseApp() {

    }

    protected void destroyApp(boolean b) throws MIDletStateChangeException {

    }

    public void commandAction(Command command, Displayable displayable) {

    }
}
