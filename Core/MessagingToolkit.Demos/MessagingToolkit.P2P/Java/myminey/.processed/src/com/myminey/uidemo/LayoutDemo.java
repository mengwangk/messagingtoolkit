/*
 * Copyright © 2008 Sun Microsystems, Inc. All rights reserved.
 * Use is subject to license terms.
 *
 */
package com.myminey.uidemo;

import com.sun.lwuit.Button;
import com.sun.lwuit.Form;
import com.sun.lwuit.events.ActionEvent;
import com.sun.lwuit.events.ActionListener;
import com.sun.lwuit.layouts.BorderLayout;
import com.sun.lwuit.layouts.BoxLayout;
import com.sun.lwuit.layouts.FlowLayout;
import com.sun.lwuit.layouts.GridLayout;

/**
 *
 * @author cf130546
 */
public class LayoutDemo extends Demo {

    private Button border;
    
    private Button boxY;
    
    private Button boxX;
    
    private Button flow;
    
    private Button grid;
    
     public void cleanup() {
         border = null;
         boxY = null;
         flow = null;
         grid = null;
         boxX = null;
     }

    public String getName() {
        return "Layouts";
    }

    protected String getHelp() {
        return "The toolkit supports 5 different Layouts: FlowLayout, BorderLayout, BoxLayout, GridLayout and GroupLayout." +
                "In this Demo we added 5 Components to the Form and we changes the Layout and rearrange the Components" +
                "A developer can add his own Layout by extending the Layout class";
    }

    protected void execute(final Form f) {
        f.setLayout(new BoxLayout(BoxLayout.Y_AXIS));
        border = new Button("BorderLayout");
        
        border.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent evt) {
                f.setLayout(new BorderLayout());
                f.removeAll();
                f.setScrollable(false);
                f.addComponent(BorderLayout.NORTH, border);
                f.addComponent(BorderLayout.EAST, boxY);
                f.addComponent(BorderLayout.CENTER, grid);
                f.addComponent(BorderLayout.WEST, flow);
                f.addComponent(BorderLayout.SOUTH, boxX);
                f.show();
            }
        });
        boxY = new Button("BoxLayout-Y");
        boxY.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent evt) {
                f.setLayout(new BoxLayout(BoxLayout.Y_AXIS));
                f.setScrollable(false);
                addComponents(f);
                f.show();
            }
        });
        flow = new Button("FlowLayout");
        flow.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent evt) {
                f.setLayout(new FlowLayout());
                f.setScrollable(false);
                addComponents(f);
                f.show();
            }
        });
        
        grid = new Button("GridLayout");
        grid.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent evt) {
                f.setLayout(new GridLayout(3, 2));
                f.setScrollable(false);
                addComponents(f);
                f.show();
            }
        });
        
        boxX = new Button("BoxLayout-X");
        boxX.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent evt) {
                f.setLayout(new BoxLayout(BoxLayout.X_AXIS));
                f.setScrollable(true);
                addComponents(f);
                f.show();
            }
        });
        
        
        addComponents(f);
        f.show();
    }
    
    private void addComponents(final Form f){
        f.removeAll();
        f.addComponent(boxY);
        f.addComponent(boxX);
        f.addComponent(border);
        f.addComponent(flow);
        f.addComponent(grid);
    }
}
