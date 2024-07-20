package com.mbit.screen;

import com.mbit.MainMidlet;
import com.mbit.common.Constants;
import com.mbit.util.ImageUtils;
import com.mbit.custom.NewsItem;
import com.mbit.valueobject.rss.RssFeedRecord;
import com.mbit.valueobject.rss.RssItemRecord;
import com.mbit.valueobject.advertisement.AdStore;
import com.mbit.valueobject.advertisement.AdComparator;
import com.mbit.valueobject.advertisement.AdFilter;
import com.mbit.valueobject.advertisement.AdRecord;
import com.mbit.valueobject.channel.ChannelRecord;
import com.mbit.valueobject.parameter.ParameterRecord;
import com.mbit.valueobject.parameter.ParameterStore;
import com.nextel.rms.OAbstractRecord;
import de.enough.polish.util.Locale;

import javax.microedition.lcdui.*;
import java.util.Enumeration;
import java.util.Vector;
import java.util.Random;

/**
 * @author : <a href="mailto:meng-wang.koh@hp.com">Koh Meng Wang</a>
 *         Created : Jun 11, 2006 11:58:57 PM
 * @version : $Id:
 */
public class NewsScreen extends Form
        implements CommandListener, ItemCommandListener {

    private MainMidlet midlet;
    private ChannelScreen channelScreen;
    private Command viewCmd = new Command(Locale.get("cmd.view"), Command.ITEM, 10);
    private RssFeedRecord rssFeedRecord;
    private NewsDetailsScreen newsDetailsScreen;
    private ChannelRecord channel;

    /**
     * Constructor
     *
     * @param midlet
     */
    public NewsScreen(MainMidlet midlet, ChannelScreen channelScreen) {
        //#style newsScreen
        super(Locale.get("news.title"));
        this.midlet = midlet;
        this.channelScreen = channelScreen;

        addCommand(midlet.backCmd);
        addCommand(midlet.saveCmd);
        setCommandListener(this);
        newsDetailsScreen = new NewsDetailsScreen(midlet, this);
    }

    public void setup(ChannelRecord channel) {
        this.channel = channel;
    }

    public void display(RssFeedRecord rssFeed) {
        int index = 0;
        if (rssFeed != null) {

            this.rssFeedRecord = rssFeed;
            deleteAll();

            // Display any image
            if (rssFeed.getImageData() != null) {
                Image image = ImageUtils.convertByteArrayToImage(rssFeed.getImageData());
                //#style imageBanner
                append(image);
                index++;
            }

            // Check if any advertisement
            AdStore adStore = AdStore.getInstance();
            String url = "";
            ParameterRecord p = ParameterStore.getParameterRecord(Constants.NEWS_SERVER_BASE_URL_PARAMETER);
            if (p != null) {
                url = p.getValue();
            }
            url += channel.getAdUrl();
            try {
                OAbstractRecord[] ads = adStore.getAll(new AdComparator(), new AdFilter(url));
                if (ads != null && ads.length > 0) {
                    Random random = new Random();
                    int idx = Math.abs(random.nextInt() % ads.length);
                    AdRecord ad = (AdRecord) ads[idx];
                    random = null;
                    if (ad.getImageData() != null) {
                        Image image = ImageUtils.convertByteArrayToImage(ad.getImageData());
                        //#style imageBanner
                        append(image);
                    } else {
                        //#style imageBanner
                        append(ad.getText());
                    }
                    index++;
                }
            } catch (Exception e) {
                //#debug
                System.out.println(e.getMessage());
            }

            // Display the feed
            Vector items = rssFeed.getItems();
            Enumeration enum = items.elements();
            while (enum.hasMoreElements()) {
                RssItemRecord rssItem = (RssItemRecord) enum.nextElement();
                NewsItem item = new NewsItem(rssItem, getWidth());

                item.setDefaultCommand(viewCmd);
                item.setItemCommandListener(this);

                //#style newsItem
                append(item);
            }
        } else {
            //#style newsItem
            append(Locale.get("news.nofeed"));
        }

        // Show the current screen
        Display.getDisplay(midlet).setCurrent(this);
        // Default to the first item
        Display.getDisplay(midlet).setCurrentItem(this.get(index));
    }

    public void commandAction(Command command, Displayable displayable) {
        if (command == midlet.backCmd) {
            Display.getDisplay(midlet).setCurrent(channelScreen);
        } else if (command == midlet.saveCmd) {
            // Save the current feed

        }
    }

    public void commandAction(Command command, Item item) {
        if (command == viewCmd) {
            if (item instanceof NewsItem) {
                NewsItem newsItem = (NewsItem) item;
                String key = newsItem.getKey();
                Vector items = rssFeedRecord.getItems();
                Enumeration enum = items.elements();
                while (enum.hasMoreElements()) {
                    RssItemRecord rssItem = (RssItemRecord) enum.nextElement();
                    if (rssItem.getTitle().equals(key)) {
                        newsDetailsScreen.setup();
                        newsDetailsScreen.display(rssItem);
                        break;
                    }
                }
            }
        }
    }
}
