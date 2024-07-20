package messagingtoolkit.p2pclient;

import com.sun.lwuit.Button;
import com.sun.lwuit.ButtonGroup;
import com.sun.lwuit.Container;
import com.sun.lwuit.Form;
import com.sun.lwuit.Image;
import com.sun.lwuit.Label;
import com.sun.lwuit.RadioButton;
import com.sun.lwuit.TabbedPane;
import com.sun.lwuit.events.ActionEvent;
import com.sun.lwuit.events.ActionListener;
import com.sun.lwuit.layouts.BorderLayout;
import com.sun.lwuit.layouts.BoxLayout;
import com.sun.lwuit.util.Resources;

/**
 * Main Menu
 *
 */
public class MainMenu extends Form {

	private StartApp midlet;
	
	private TabbedPane tp = null;
	 
	/**
	 * Constructor
	 * 
	 * @param midlet Parent midlet
	 * @param title Form title
	 */
	public MainMenu(StartApp parent, String title) {
		this.midlet = parent;
		setTitle(title);
	}
	
	protected void execute() {
        setLayout(new BorderLayout());
        setScrollable(false);
        tp = new TabbedPane();

        tp.addTab("Tab 1", new Label("Welcome to TabbedPane demo!"));

        Container radioButtonsPanel = new Container(new BoxLayout(BoxLayout.Y_AXIS));

        RadioButton topRB = new RadioButton("Top");
        RadioButton LeftRB = new RadioButton("Left");
        RadioButton BottomRB = new RadioButton("Bottom");
        RadioButton RightRB = new RadioButton("Right");

        RadioListener myListener = new RadioListener();
        topRB.addActionListener(myListener);
        LeftRB.addActionListener(myListener);
        BottomRB.addActionListener(myListener);
        RightRB.addActionListener(myListener);

        ButtonGroup group1 = new ButtonGroup();
        group1.add(topRB);
        group1.add(LeftRB);
        group1.add(BottomRB);
        group1.add(RightRB);

        radioButtonsPanel.addComponent(new Label("Please choose a tab placement direction:"));
        radioButtonsPanel.addComponent(topRB);
        radioButtonsPanel.addComponent(LeftRB);
        radioButtonsPanel.addComponent(BottomRB);
        radioButtonsPanel.addComponent(RightRB);

        tp.addTab("Tab 2", radioButtonsPanel);
        try {
            Container panel = new Container(new BorderLayout());
            /*
            Image img1 = UIDemoMIDlet.getResource("duke").getAnimation("duke3_1.gif");
            panel.addComponent("North", new Button("North Button", img1));
            Image img2 = UIDemoMIDlet.getResource("duke").getAnimation("duke3_1.gif");
            panel.addComponent("Center", new Button("Center Button", img2));
            */
            tp.addTab("Tab 3", panel);
            //Resources images = UIDemoMIDlet.getResource("images");
            //tp.addTab("Tab 4", new Label(images.getImage("tabImage.jpg")));
        } catch (Exception e) {
            e.printStackTrace();
        }

        addComponent("Center", tp);
    }

	public void cleanup() {
        tp = null;
    }
	
    /** Listens to the radio buttons. */
    class RadioListener implements ActionListener {

        public void actionPerformed(ActionEvent e) {

            String title = ((RadioButton) e.getSource()).getText();
            if ("Top".equals(title)) {
                tp.setTabPlacement(TabbedPane.TOP);
            } else if ("Left".equals(title)) {
                tp.setTabPlacement(TabbedPane.LEFT);
            } else if ("Bottom".equals(title)) {
                tp.setTabPlacement(TabbedPane.BOTTOM);
            } else {//right
                tp.setTabPlacement(TabbedPane.RIGHT);
            }
        }
    }
}
