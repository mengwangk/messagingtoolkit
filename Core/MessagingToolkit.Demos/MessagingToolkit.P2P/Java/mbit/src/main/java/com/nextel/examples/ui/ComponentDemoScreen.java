/* Copyright (c) 2001 Nextel Communications, Inc.
 * All rights reserved.
 *  
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met: 
 *  
 *   - Redistributions of source code must retain the above copyright
 *     notice, this list of conditions and the following disclaimer. 
 *   - Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimer in the
 *     documentation and/or other materials provided with the distribution. 
 *   - Neither the name of Nextel Communications, Inc. nor the names of its
 *     contributors may be used to endorse or promote products derived from
 *     this software without specific prior written permission. 
 *  
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS
 * IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
 * TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
 * PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL NEXTEL
 * COMMUNICATIONS, INC. OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
 * USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

package com.nextel.examples.ui;

import com.nextel.ui.*;
import javax.microedition.lcdui.Font;
import javax.microedition.lcdui.Graphics;
import javax.microedition.lcdui.Image;
import com.nextel.ui.*;

/**
 * The <b>ComponentDemoScreen</b> provides a demonstration of the UI components
 * found within the Nextel MIDP Toolkit. The following Componets are demonstrated:
 * <p>
 * <li>{@link com.nextel.ui.OAnimation}</li>
 * <li>{@link com.nextel.ui.OCheckBox}</li>
 * <li>{@link com.nextel.ui.ODateField}</li>
 * <li>{@link com.nextel.ui.ODayField}</li>
 * <li>{@link com.nextel.ui.ODropDownList}</li>
 * <li>{@link com.nextel.ui.OGrid}</li>
 * <li>{@link com.nextel.ui.OLabel}</li>
 * <li>{@link com.nextel.ui.OMoneyField}</li>
 * <li>{@link com.nextel.ui.OMonthField}</li>
 * <li>{@link com.nextel.ui.OPushButton}</li>
 * <li>{@link com.nextel.ui.ORadioButton}</li>
 * <li>{@link com.nextel.ui.OCompositeScreen}</li>
 * <li>{@link com.nextel.ui.OScrollList}</li>
 * <li>{@link com.nextel.ui.OTextScreen}</li>
 * <p>
 * The following event handling mechanisms are utilized:
 * <p>
 * <li>{@link com.nextel.ui.OCommandAction}</li>
 * <li>{@link com.nextel.ui.OSoftKey}</li>
 * @author Anthony Paper
 */

public class ComponentDemoScreen extends OCompositeScreen
{
  /**
   * Values used for the option menu
   */
  private static final String options[]={ "A", "B", "C","D", "E", "F", "G"};
  
  /**
   * Values used for the scrolled list, option menu, and grid components
   */
  private static final String  items[]={ "1) Grapes", "2) Oranges", "3) Pears",
                                 "4) Nuts", "5) Apples", "6) Beans", "7) Water",
                                 "8) Juice"};
  
 /**
  * {@link com.nextel.ui.OCompositeScreen} instance handle used for screen naviation.
  */
  private OCompositeScreen THIS;
  
  /**
   * Command action used for screen navigation.
   */
  private OCommandAction BACK;
  
  /**
   * Font used to display labels 
   */
  private Font labelFont = OUILook.PLAIN_SMALL;
  
  /**
   * Label used for check boxes
   */
  private OLabel     leftCheckBox = new OLabel( "CHECK ", labelFont);
  
  /**
   * The left check box contained on the screen
   */
  private OCheckBox centerCheckBox = new OCheckBox( "Left", OUILook.TEXT_FONT );
  
  /**
   * The right check box contained on the screen.
   */
  private OCheckBox rightCheckBox = new OCheckBox( "Right", OUILook.TEXT_FONT );
  
  /**
   * The SoftKey used for screen navigation (Back).
   */
  private OSoftKey backSoftKey = new OSoftKey( "BACK" );
  
  /**
   * The SoftKey used for screen navigation (Exit).
   */
  private OSoftKey exitSoftKey = new OSoftKey( "EXIT" );
  
  /**
   * Radio Button (1) Displayed on the screen.
   */
  private ORadioButton radioButton1 = new ORadioButton( "1", 
    OUILook.TEXT_FONT );
  
  /**
   * Radio Button (2) Displayed on the screen.
   */  
  private ORadioButton radioButton2 = new ORadioButton( "W", 
    OUILook.TEXT_FONT );
  
  /**
   * Radio Button (3) Displayed on the screen.
   */
  private ORadioButton radioButton3 = new ORadioButton( "g", 
    OUILook.TEXT_FONT );
  
  /**
   * Label used for radio buttons
   */
  private OLabel label = new OLabel( "RADIO ", labelFont);
  
  /**
   * Textbox input screen used to demonstrate soft key navigation
   * to another screen.
   */
  private OTextScreen textBox;
  
  /**
   * Button Group used to manage and control the radio buttons
   */
  private OButtonGroup buttonGroup = new OButtonGroup( label, Graphics.LEFT );
  
  /**
   * Drop Down List (1) displayed on the screen.
   */
  private ODropDownList dropDownList1 = new ODropDownList(options,OUILook.PLAIN_SMALL);
  
  /**
   * Drop Down List (2) displayed on the screen.
   */  
  private ODropDownList dropDownList2 = new ODropDownList(items,OUILook.PLAIN_SMALL);
  

 /** 
  * Anonymous Class used to exit the Midlet.  This command action is invoked
  * when the Left Soft Key is pressed from the <b>Component Demo</b> screen.
  */
  private OCommandAction exitAction = new OCommandAction()
 {
    public void performAction()
    {
      OHandset.getMIDlet().notifyDestroyed();
    }
  };
  
 /** 
  * Anonymous Class used to navigate between screens.  This command action is invoked
  * when the Soft Key is pressed from the <b>Text Box</b> screen.  This will force
  * the midlet to navigate back to the <b>Component Demo</b> screen.
  */  
  public OCommandAction backAction = new OCommandAction()
  {
    public void performAction()
    {
        System.out.println("BACK");
        OHandset.getDisplay().setCurrent(THIS);
    }
  };
  
 /** 
  * Anonymous Class used to navigate between screens.  This command action is invoked
  * when the Soft Key is pressed from the <b>Component Demo</b> screen.  This will force
  * the midlet to navigate to the <b>Text Box</b> screen.
  */  
  private OCommandAction leftAction = new OCommandAction()
  {
    public void performAction()
    {
        System.out.println("LEFT");
        textBox = new OTextScreen("LEFT TEXTBOX", labelFont,
		       "LEFT pushbutton was pressed!!!", OUILook.TEXT_FONT, 
                       100,
		       OTextScreen.ANY);
        OSoftKey backSoftKey = new OSoftKey("BACK");
        backSoftKey.setAction(backAction);
        textBox.addSoftKey(backSoftKey, Graphics.LEFT );
        
        OHandset.getDisplay().setCurrent(textBox);
    }
  };
 /** 
  * Anonymous Class used to navigate between screens.  This command action is invoked
  * when the Soft Key is pressed from the <b>Component Demo</b> screen.  This will force
  * the midlet to navigate to the <b>Text Box</b> screen.
  */  
   private OCommandAction rightAction = new OCommandAction()
  {
    public void performAction()
    {
        System.out.println("RIGHT");
        textBox = new OTextScreen("RIGHT TEXTBOX", labelFont,
		       "RIGHT pushbutton was pressed!!!", OUILook.TEXT_FONT, 
                       100,
		       OTextScreen.ANY);
        OSoftKey backSoftKey = new OSoftKey("BACK");
        backSoftKey.setAction(backAction);
        textBox.addSoftKey( backSoftKey, Graphics.RIGHT );
        OHandset.getDisplay().setCurrent(textBox);
    }
  };
  
  /**
   * Constructor used to allocate the <b>Component Demo</b> screen.
   */
  public ComponentDemoScreen()
  {
    super( "COMPONENT DEMO", OUILook.PLAIN_SMALL, 3);
    init();
  }
  
  /**
   * Method used to perform initialization.  All components are allocated
   * and added to the screen for proper layout.  This routine also sets up
   * the softkey navigation mechanisms.
   */
  private void init()
  {
      THIS = this;
      BACK = backAction;
      exitSoftKey.setAction( exitAction );
      addSoftKey( exitSoftKey, Graphics.LEFT );

      //
      // Add the Check boxes
      //
      int y = 0;
      add( leftCheckBox,   0, y, Graphics.LEFT );
      add( centerCheckBox, 1, y, Graphics.HCENTER );
      add( rightCheckBox,  2, y, Graphics.RIGHT );
      y++;
      
      //
      // Add Radio Button Group
      //
      buttonGroup.add( radioButton1 );
      buttonGroup.add( radioButton2 );
      buttonGroup.add( radioButton3 );
      add( buttonGroup, 0, y, Graphics.LEFT );
      y++;
      

      add(new OLabel("DATE", labelFont),0,y, Graphics.LEFT);
      add(new ODateField(OUILook.TEXT_FONT),2,y, Graphics.RIGHT);
      y++;
      add(new OLabel("DAY", labelFont),0,y, Graphics.LEFT);
      add(new ODayField(OUILook.TEXT_FONT),1,y, Graphics.HCENTER);
      y++;
      
      add(new OLabel("MONEY", labelFont),0,y, Graphics.LEFT);
      add(new OMoneyField( 4, OUILook.TEXT_FONT ),2,y, Graphics.RIGHT);
      y++;
      add(new OLabel("MONTH", labelFont),0,y, Graphics.LEFT);
      add(new OMonthField(OUILook.TEXT_FONT ),1,y, Graphics.RIGHT);
      y++;
      
      add(new OLabel("TEXT/ANY", labelFont),0,y, Graphics.LEFT);
      OTextField t = new OTextField(6,labelFont,OTextField.ANY);
      t.allowSpaces( true );
      try {
          t.setText("A12");
      } catch (Exception e){e.printStackTrace();}
      add(t,2,y, Graphics.RIGHT);
      y++;

      add(new OLabel("OPTIONS",labelFont ),0,y, Graphics.LEFT);
      add(dropDownList2,2,y, Graphics.RIGHT);        
      y++;

      
      add(new OLabel("TEXT/NUMERIC", labelFont),0,y, Graphics.LEFT);
      t = new OTextField(3,labelFont,OTextField.NUMERIC);
      try {
          t.setText("123");
      } catch (Exception e){e.printStackTrace();}
      add(t,2,y, Graphics.RIGHT);
      y++;
      
      add(new OLabel("TEXT/LOWER", labelFont),0,y, Graphics.LEFT);
      t = new OTextField(3,labelFont,OTextField.LOWERCASE);
      try {
          t.setText("abc");
      } catch (Exception e){e.printStackTrace();}
      add(t,2,y, Graphics.RIGHT);
      y++;
      
      try {
        String imageNames[] = {
           "/Phone1.png",
           "/Phone2.png",
           "/Phone3.png",
           "/Phone4.png"
        };
        OAnimation animation = new OAnimation(imageNames, 500);
        add(new OLabel("ANIMATION",labelFont), 0,y,Graphics.LEFT);
        add(animation,2,y,Graphics.HCENTER);
        y++;   
      } catch (Exception e){ e.printStackTrace();}
      
      add(new OLabel("TEXT/UPPER", labelFont),0,y, Graphics.LEFT);
      t = new OTextField(3,labelFont,OTextField.UPPERCASE);
      try {
          t.setText("ABC");
      } catch (Exception e){e.printStackTrace();}
      add(t,2,y, Graphics.RIGHT);
      y++;
      

      // Array of items for the scroll list
      String[] itemVals = { "1", "22", "333", "4444" };
      Font selectedFont = OUILook.PLAIN_SMALL;
      Font unselectedFont = OUILook.PLAIN_SMALL;
      OScrollList scrollList = new OScrollList(
                                           getWidth()/2,
                                           50, 
                                           selectedFont,
                                           unselectedFont);
      add(new OLabel("SCROLLED LIST",labelFont), 1,y,Graphics.HCENTER);
      y++;
      scrollList.populate(items);
      add(scrollList,1,y,Graphics.HCENTER);
      y++;

      
      OGridObjectRow items[] = new OGridObjectRow[itemVals.length];
      Image image = null;
      try {
        image = Image.createImage("/9x9.png");
       } catch (Exception e){ e.printStackTrace();}
      
      // Component list
      int numberOfLines = 3;
      int numberOfColumns = 3;
      String columnNames[] = {"Left","Cntr","Right"};
      int columnWidths[] = {getWidth()/3, (getWidth()/3) - 10,(getWidth()/3) + 10};
      int columnAlignments[] = {Graphics.LEFT, Graphics.HCENTER,Graphics.RIGHT};
      int maxWidth  = getWidth();
      int maxHeight = 40;
      Font columnHeadingFont = OUILook.PLAIN_SMALL;
      OGridInfo gridInfo = new OGridInfo(OGridInfo.GRID_BORDER_TYPE_BOTH,numberOfLines,
                               numberOfColumns, 
                         //      columnNames,
                               null,
                               columnAlignments,columnWidths,maxWidth,
                         //      maxHeight,
                               columnHeadingFont,
                               selectedFont, unselectedFont);

      OGrid grid = new OGrid(gridInfo);
      //
      // Build the item grid rows
      //

      for (int index=0; index < itemVals.length; index++)
      {
          OGridObjectRow row = new OGridObjectRow(3);
          row.addCell(0,itemVals[index]);
          row.addCell(1,image);
          row.addCell(2,itemVals[index]);
          items[index] = row;
      }
      grid.populate( items );
      grid.setColumnHeadings(columnNames,columnHeadingFont);
      add(new OLabel("GRID CELLS",labelFont), 1,y,Graphics.HCENTER);
      y++;
      add(grid, 1,y, Graphics.HCENTER );
      y++;
      add(new OLabel(" ",labelFont), 1,y,Graphics.HCENTER);
      y++;
      OPushButton pb = new OPushButton("LEFT", OUILook.PLAIN_SMALL, "GOTO 1");
      pb.setAction(leftAction);
      add(pb, 0,y,Graphics.LEFT);
      pb = new OPushButton("RIGHT",OUILook.PLAIN_SMALL, "GOTO 2");
      pb.setAction(rightAction);
      add(pb, 2,y,Graphics.RIGHT);      
      y++;
     
  }


}
