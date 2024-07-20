package messagingtoolkit.p2pclient;

import javax.microedition.midlet.*;

import com.sun.lwuit.*;
import com.sun.lwuit.events.*;
import javax.wireless.messaging.*;

/**
 * This is the main midlet to start up the P2P client
 * 
 */
public class StartApp extends MIDlet implements ActionListener {

	private static final int EXIT_COMMAND = 100;
    private static final int RUN_COMMAND = 200;
    private static final int BACK_COMMAND = 300;
    private static final int ABOUT_COMMAND = 400;
	   
	private static final Command runCommand = new Command("Run", RUN_COMMAND);
	private static final Command exitCommand = new Command("Exit", EXIT_COMMAND);
	private static final Command backCommand = new Command("Back", BACK_COMMAND);
	private static final Command aboutCommand = new Command("About", ABOUT_COMMAND);
	    
	private static MainMenu mainMenu;
	
    public void startApp() {
        Display.init(this);  
        setup();
    }

    public void pauseApp() {
    }

    public void destroyApp(boolean unconditional) {
    }

    public void actionPerformed(ActionEvent evt) {
        Command cmd = evt.getCommand();
        switch (cmd.getId()) {
            case RUN_COMMAND:
            	MessagePart messagePart ;
            	break;
            	/*
        	   SmsSender smsSender = new SmsSender(this);
               smsSender.setDestinationAddress("0192292309");
               smsSender.setMessage("test message");
               new Thread(smsSender).start();
               break;
               */
            case EXIT_COMMAND:
                notifyDestroyed();
                break;
        }
    }
    
    private void setup(){
    	mainMenu = new MainMenu(this, "Main");
		mainMenu.addCommand(exitCommand);
	    mainMenu.addCommand(aboutCommand);	
	    mainMenu.addCommand(runCommand);
		
	    mainMenu.setCommandListener(this);
	    mainMenu.execute();
	    mainMenu.show();    
    }
}
